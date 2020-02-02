//#define VISUALIZE_ANIMATION_SPEED

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace PQScoreboard
{
    public partial class ResultForm : Form
    {
        private const int BackBufferWidth = 1920;
        private const int BackBufferHeight = 1080;
        private const int ParticlesPerFirework = 32;
        private const float FontSize = 24f;

        private readonly Random rng;

        // rendering

        private readonly Color colorBackground;
        private readonly Bitmap renderTarget;
        private readonly Graphics renderTargetGraphics;

        private readonly Bitmap renderTargetOverlay;
        private readonly Graphics renderTargetGraphicsOverlay;

        private readonly StringFormat stringFormat;
        private readonly Font fontCategoryNames;
        private readonly Font fontTeamNames;
        private readonly Font fontScores;
        private readonly Pen penGridLines;
        private readonly Brush brushTeamNames;
        private readonly Brush brushScores;
        private readonly Brush[] brushesFireworks;
        private Color[] colorsCategories;
        private Brush[] brushesCategories;

        private readonly Bitmap bitmapTotalScoreFrame;
        private readonly Rectangle rectTotalScoreFrame;

        private Thread renderThread;
        private bool isRunning;
        private double animationLength;
        private bool enableFireworks;

        private string[] teams;
        private string[] categories;
        private decimal[,] scores;
        private List<Firework> fireworks;

        // precomputed
        private decimal maxScore;
        private RectangleF[] categoryNamesPos;
        float[] animationSpeedSlowdownDomain;
        float[] animationSpeedSlowdownImage;

        #region ctor & cleanup

        public ResultForm(bool darkMode)
        {
            InitializeComponent();

            rng = new Random();

            colorBackground = darkMode ? Color.Black : Color.Transparent;

            renderTarget = new Bitmap(BackBufferWidth, BackBufferHeight);
            renderTargetGraphics = Graphics.FromImage(renderTarget);
            renderTargetGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            renderTargetOverlay = new Bitmap(BackBufferWidth, BackBufferHeight);
            renderTargetGraphicsOverlay = Graphics.FromImage(renderTargetOverlay);

            stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            fontCategoryNames = new Font("Yu Gothic UI Light", FontSize, FontStyle.Regular);
            fontTeamNames = new Font("Yu Gothic UI Light", FontSize, FontStyle.Bold);
            fontScores = new Font("Yu Gothic UI Light", FontSize, FontStyle.Bold);
            penGridLines = new Pen(darkMode ? Color.White : Color.Black, 1f);
            brushTeamNames = darkMode ? Brushes.White : Brushes.Black;
            brushScores = Brushes.Black;
            brushesFireworks = new Brush[] {
                new SolidBrush(Color.FromArgb(35, 0, 128, 255)),
                new SolidBrush(Color.FromArgb(35, 128, 0, 255)),
                new SolidBrush(Color.FromArgb(35, 128, 255, 0))
            };

            bitmapTotalScoreFrame = Properties.Resources.TotalScoreFrame;
            rectTotalScoreFrame = new Rectangle(0, 0, bitmapTotalScoreFrame.Width, bitmapTotalScoreFrame.Height);

            DoubleBuffered = true;
            Paint += Draw;

            fireworks = new List<Firework>();
        }

        private void Cleanup()
        {
            isRunning = false;

            if (renderThread != null)
            {
                renderThread.Join();
            }

            renderTarget.Dispose();
            renderTargetGraphics.Dispose();

            renderTargetOverlay.Dispose();
            renderTargetGraphicsOverlay.Dispose();

            stringFormat.Dispose();
            fontCategoryNames.Dispose();
            fontTeamNames.Dispose();
            fontScores.Dispose();
            penGridLines.Dispose();

            foreach (Brush brush in brushesFireworks)
            {
                brush.Dispose();
            }
            foreach (Brush brush in brushesCategories)
            {
                brush.Dispose();
            }
        }

        #endregion

        public void StopAnimationAndClose()
        {
            Cleanup();
            Close();
        }

        public void StartAnimation(Scoreboard scoreboard, double animationLength, bool enableFireworks)
        {
            isRunning = true;

            this.animationLength = animationLength * 1000d;
            this.enableFireworks = enableFireworks;

            teams = scoreboard.Teams;
            categories = scoreboard.Categories;
            scores = scoreboard.Scores;

            decimal[] sortedTotalScores = scoreboard.TotalScores;
            Array.Sort(sortedTotalScores);
            // TODO: eliminate duplicates
            maxScore = sortedTotalScores[sortedTotalScores.Length - 1];

            // devide total into points where "teams finish"
            animationSpeedSlowdownDomain = new float[teams.Length + 1];
            animationSpeedSlowdownDomain[0] = -0.01f;
            animationSpeedSlowdownDomain[teams.Length] = 100.01f;
            // get percentage of individual team scores w.r.t. to maxScore
            animationSpeedSlowdownImage = new float[teams.Length + 1];
            animationSpeedSlowdownImage[0] = 0f;
            animationSpeedSlowdownImage[teams.Length] = 100f;
            for (int i = 1; i < teams.Length; ++i)
            {
                animationSpeedSlowdownDomain[i] = i * (100f / teams.Length);
                animationSpeedSlowdownImage[i] = (float)(sortedTotalScores[i + teams.Length - animationSpeedSlowdownDomain.Length] * 100m / maxScore);
            }
            animationSpeedSlowdownDomain[teams.Length - 1] = 95f;

            int expectedNumberOfCategories = scoreboard.ExpectedNumberOfCategories;
            colorsCategories = new Color[expectedNumberOfCategories];
            brushesCategories = new Brush[expectedNumberOfCategories--];
            for (int i = expectedNumberOfCategories; i >= 0; --i)
            {
                // TODO: beware of division by 0
                colorsCategories[i] = Color.FromArgb(255, 150 - (i * 100 / (expectedNumberOfCategories)), 0);
                brushesCategories[i] = new SolidBrush(colorsCategories[i]);
            }

            // category name rects
            categoryNamesPos = new RectangleF[categories.Length];
            for (int i = 0; i < categories.Length; ++i)
            {
                categoryNamesPos[i] = new RectangleF(20f, 990f - (i + 1) * 900f / categories.Length, 260f, 900f / categories.Length);
            }

#if VISUALIZE_ANIMATION_SPEED
            RenderAnimationSpeed();
#else
            renderThread = new Thread(new ThreadStart(RenderThreadLoop));
            renderThread.Start();
#endif
        }

        #region rendering

        private void Draw(object sender, PaintEventArgs e)
        {
            var rect = e.Graphics.ClipBounds;
            e.Graphics.DrawImage(renderTarget, 0f, 0f, rect.Width, rect.Height);

            if (enableFireworks && fireworks.Count > 0)
            {
                e.Graphics.DrawImage(renderTargetOverlay, 0f, 0f, rect.Width, rect.Height);
            }
        }

        private void RenderScores(float animationPct)
        {
            renderTargetGraphics.Clear(colorBackground);

            // score on topmost grid line = gridLines * 10
            int gridLines = decimal.ToInt32((maxScore / 10m)) + 1;

            // grid
            for (int i = gridLines; i >= 0; --i)
            {
                float y = 90f + (i * 900f / gridLines);
                renderTargetGraphics.DrawLine(penGridLines, 300f, y, 1900f, y);
            }

            decimal maxScoreToDraw = new decimal(animationPct) * maxScore / 100m;

            float maxGridLineScore = gridLines * 10f;
            decimal[] accScores = new decimal[teams.Length];

            float width = 1600f / teams.Length;
            float previousY, newY = 0f;
            bool maxScoreReached;
            float columnWidth = width / 3f;
            float[] categoryProgress = new float[categories.Length];

            for (int i = teams.Length - 1; i >= 0; --i)
            {
                accScores[i] = 0m;

                // team name
                float x = 300f + i * width;
                RectangleF rect = new RectangleF(x, 980f, width, 80f);
                renderTargetGraphics.DrawString(teams[i], fontTeamNames, brushTeamNames, rect, stringFormat);

                // score
                maxScoreReached = false;
                previousY = 990f;

                for (int j = 0; j < categories.Length; ++j)
                {
                    decimal s = scores[i, j];

                    if (accScores[i] + s > maxScoreToDraw)
                    {
                        decimal o = maxScoreToDraw - accScores[i];
                        accScores[i] = maxScoreToDraw;
                        maxScoreReached = true;
                        categoryProgress[j] += (float)(o * 100m / s);
                    }
                    else
                    {
                        accScores[i] += s;
                        categoryProgress[j] += 100f;
                    }

                    newY = 990f - 900f * (float)accScores[i] / maxGridLineScore;
                    renderTargetGraphics.FillRectangle(brushesCategories[j], x + columnWidth, newY, columnWidth, previousY - newY);

                    previousY = newY;

                    if (maxScoreReached)
                    {
                        break;
                    }
                }

                rect.Y = newY - 80f;
                rect.X += columnWidth;
                rect.Width -= 2 * columnWidth;
                renderTargetGraphics.DrawImage(bitmapTotalScoreFrame, rect, rectTotalScoreFrame, GraphicsUnit.Pixel);
                string score = maxScoreReached ? decimal.Round(accScores[i]).ToString("0") : accScores[i].ToString("0.#");
                renderTargetGraphics.DrawString(score, fontScores, brushScores, rect, stringFormat);
            }

            // category names
            for (int i = categories.Length - 1; i >= 0; --i)
            {
                float alpha = 255f * categoryProgress[i] / (teams.Length * 100f);
                using (Brush b = new SolidBrush(Color.FromArgb((int)alpha, colorsCategories[i])))
                {
                    renderTargetGraphics.DrawString(categories[i], fontCategoryNames, b, categoryNamesPos[i], stringFormat);
                }
            }
        }

        private void RenderFireworks(double elapsed)
        {
            if (!enableFireworks || fireworks.Count == 0)
            {
                return;
            }

            renderTargetGraphicsOverlay.Clear(Color.Transparent);

            double a, x, y, r, g;
            foreach (Firework firework in fireworks)
            {
                // transform [0, MaxAge] -> [0, 17]
                // use x^(1/5) from [0, 17] -> [0, 1.5]
                // transform [0, 1.5] -> [0, Radius]
                r = LinearTransformD(firework.Age += elapsed, 0d, firework.MaxAge, 0d, 17d);
                r = Math.Pow(r, 1d / 5d);
                r = (int)LinearTransformD(r, 0d, 1.5d, 0d, firework.Radius);
                g = firework.GravityOff * firework.Age / firework.MaxAge;

                for (int i = ParticlesPerFirework - 1; i >= 0; --i)
                {
                    a = 2d * i * Math.PI / ParticlesPerFirework;
                    x = firework.CenterX + r * Math.Cos(a);
                    y = firework.CenterY + r * Math.Sin(a) + g;

                    for (float rr = 5f; rr > 0f; rr -= 0.5f)
                    {
                        renderTargetGraphicsOverlay.FillEllipse(brushesFireworks[firework.Brush], (float)x - rr, (float)y - rr, 2f * rr, 2f * rr);
                    }
                }
            }

            fireworks.RemoveAll(f => f.Age >= f.MaxAge);
        }

        private void RenderThreadLoop()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            double elapsed = 0d;

            while (isRunning)
            {
                if (stopwatch.Elapsed.TotalMilliseconds - elapsed >= 30)
                {
                    elapsed = stopwatch.Elapsed.TotalMilliseconds;

                    float pct = (float)(elapsed * 100f / animationLength);
                    float modifiedPct = CalculatedModifiedAnimationPct(pct);

                    if (modifiedPct >= 100f)
                    {
                        RenderScores(100f);
                        Invalidate();

                        break;
                    }

                    RenderScores(modifiedPct);
                    Invalidate();
                }
            }
            if (enableFireworks)
            {
                double prev = stopwatch.Elapsed.TotalMilliseconds;
                while (isRunning)
                {
                    if (fireworks.Count == 0)
                    {
                        // TODO: hardcoded -> config
                        double radius = 75d + 100d * rng.NextDouble();
                        fireworks.Add(new Firework(rng.Next(0, brushesFireworks.Length), 2000d + 2000d * rng.NextDouble(), radius, 7d + 15d * rng.NextDouble(),
                            300d + 1320d * rng.NextDouble(), 20d + radius + 200d * rng.NextDouble()));
                    }

                    if ((elapsed = stopwatch.Elapsed.TotalMilliseconds - prev) >= 30)
                    {
                        prev = stopwatch.Elapsed.TotalMilliseconds;

                        RenderFireworks(elapsed);
                        Invalidate();
                    }
                }
            }
        }

        #endregion

        private float CalculatedModifiedAnimationPct(float pct)
        {
            if (pct > 100f)
            {
                return 100f;
            }

            int index = 0;
            for (; index < animationSpeedSlowdownDomain.Length && pct > animationSpeedSlowdownDomain[index]; ++index) ;

            float x = LinearTransformF(pct, animationSpeedSlowdownDomain[index - 1], animationSpeedSlowdownDomain[index], 0, 100);
            float y = (float)Math.Sqrt(x) * 10f;
            return LinearTransformF(y, 0, 100, animationSpeedSlowdownImage[index - 1], animationSpeedSlowdownImage[index]);
        }

        private float LinearTransformF(float x, float da, float db, float ra, float rb)
        {
            return ra + (x - da) * (rb - ra) / (db - da);
        }

        private double LinearTransformD(double x, double da, double db, double ra, double rb)
        {
            return ra + (x - da) * (rb - ra) / (db - da);
        }

        #region events

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                StopAnimationAndClose();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ResultForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Cleanup();
        }

        #endregion

        #region test

#if VISUALIZE_ANIMATION_SPEED
        
        private void RenderAnimationSpeed()
        {
            int steps = 100;
            float x, pct, prevX = 0f, prevPct = 0f;
            for (int i = 0; i < steps; ++i)
            {
                x = i * 100f / steps;
                pct = CalculatedModifiedAnimationPct(x);

                graphics.DrawLine(pen, 100 + prevX * 10, 1000 - prevPct * 10, 100 + x * 10, 1000 - pct * 10);

                prevX = x;
                prevPct = pct;
            }

            Invalidate();
        }
#endif

        #endregion

    }
}
