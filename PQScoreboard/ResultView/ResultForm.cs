//#define VISUALIZE_ANIMATION_SPEED

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
        // divide into 4 regions:
        // table
        // category names
        // team names
        // dead region
        private const float dividerCategoryNames2Table = 0.15f * BackBufferWidth;
        private const float dividerTeamNames2Table = 0.85f * BackBufferHeight;
        private const float marginInner = 5f;
        private const float marginOuter = 10f;

        private const float categoryNamesLeft = marginOuter;
        private const float categoryNamesTop = marginOuter;
        private const float categoryNamesRight = dividerCategoryNames2Table - marginInner;
        private const float categoryNamesBottom = dividerTeamNames2Table - marginInner;
        private const float categoryNamesHeight = categoryNamesBottom - categoryNamesTop;
        private const float categoryNamesWidth = categoryNamesRight - categoryNamesLeft;

        private const float tableTotalScoresHeight = 50f;
        private const float tableTotalScoresWidth = 100f;
        private const float tableLeft = dividerCategoryNames2Table + marginInner;
        private const float tableTop = tableTotalScoresHeight + marginOuter;
        private const float tableRight = BackBufferWidth - marginOuter;
        private const float tableBottom = dividerTeamNames2Table - marginInner;
        private const float tableHeight = tableBottom - tableTop;
        private const float tableWidth = tableRight - tableLeft;

        private const float teamNamesLeft = dividerCategoryNames2Table + marginInner;
        private const float teamNamesTop = dividerTeamNames2Table + marginInner;
        private const float teamNamesRight = BackBufferWidth - marginOuter;
        private const float teamNamesBottom = BackBufferHeight - marginOuter;
        private const float teamNamesHeight = teamNamesBottom - teamNamesTop;


        private readonly Random rng;

        // rendering

        private readonly Color colorBackground;
        private readonly TripleBuffer tripleBuffer;

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
        private Bitmap bitmapScores; // paint fireworks on finished score board, don't need to render scoreboard everytime

        private Thread renderThread;
        private bool isRunning;
        private bool keepRendering;
        private double animationLength;
        private bool enableFireworks;
        private int numberOfFireworks; // -> config

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

        public ResultForm(bool darkMode, int numberOfFireworks)
        {
            InitializeComponent();

            rng = new Random();
            this.numberOfFireworks = numberOfFireworks;

            colorBackground = darkMode ? Color.Black : Color.Transparent;

            tripleBuffer = new TripleBuffer(BackBufferWidth, BackBufferHeight, true);

            stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            fontCategoryNames = new Font(Config.Values.FontCategoryNames, FontSize, FontStyle.Bold);
            fontTeamNames = new Font(Config.Values.FontTeamNames, FontSize, FontStyle.Bold);
            fontScores = new Font("Yu Gothic UI Light", FontSize, FontStyle.Bold);
            penGridLines = new Pen(darkMode ? Color.White : Color.Black, 1f);
            brushTeamNames = new SolidBrush(darkMode ? Color.White : Color.Black);
            brushScores = new SolidBrush(Color.Black);
            brushesFireworks = new Brush[] {
                new SolidBrush(Color.FromArgb(35, 0, 128, 255)),
                new SolidBrush(Color.FromArgb(35, 128, 0, 255)),
                new SolidBrush(Color.FromArgb(35, 128, 255, 0))
            };

            bitmapTotalScoreFrame = Properties.Resources.TotalScoreFrame;
            rectTotalScoreFrame = new Rectangle(0, 0, bitmapTotalScoreFrame.Width, bitmapTotalScoreFrame.Height);
            bitmapScores = null;

            DoubleBuffered = true;
            Paint += Draw;

            fireworks = new List<Firework>();
        }

        private void Cleanup()
        {
            isRunning = false;

            renderThread?.Join();

            tripleBuffer?.Dispose();

            stringFormat?.Dispose();
            fontCategoryNames?.Dispose();
            fontTeamNames?.Dispose();
            fontScores?.Dispose();
            penGridLines?.Dispose();
            brushTeamNames?.Dispose();
            brushScores?.Dispose();
            bitmapScores?.Dispose();

            if (brushesFireworks != null)
            {
                foreach (Brush brush in brushesFireworks)
                {
                    brush?.Dispose();
                }
            }
            if (brushesCategories != null)
            {
                foreach (Brush brush in brushesCategories)
                {
                    brush?.Dispose();
                }
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
            // precompute some values
            // and start render-thread/draw-loop

            isRunning = true;
            keepRendering = true;

            this.animationLength = animationLength * 1000d;
            this.enableFireworks = enableFireworks;

            if (scoreboard == null)
            {
                throw new ArgumentException("Failed to show results: Argument 'scoreboard' is null.");
            }

            teams = scoreboard.Teams;
            categories = scoreboard.Categories;
            scores = scoreboard.Scores;
            int expectedNumberOfCategories = scoreboard.ExpectedNumberOfCategories;

            if (teams == null || teams.Length == 0 || categories == null || categories.Length == 0 || expectedNumberOfCategories == 0)
            {
                throw new ArgumentException("Failed to show results: Invalid scoreboard.");
            }

            decimal[] sortedTotalScores = scoreboard.TotalScores;
            Array.Sort(sortedTotalScores);
            bool maxScoreTie = sortedTotalScores.Length >= 2 && sortedTotalScores[sortedTotalScores.Length - 1] == sortedTotalScores[sortedTotalScores.Length - 2];
            sortedTotalScores = sortedTotalScores.Distinct().ToArray();
            maxScore = sortedTotalScores[sortedTotalScores.Length - 1];

            // devide total into points where "teams finish"
            animationSpeedSlowdownDomain = new float[sortedTotalScores.Length + 1];
            animationSpeedSlowdownDomain[0] = -0.01f;
            animationSpeedSlowdownDomain[sortedTotalScores.Length] = 100.01f;
            // get percentage of individual team scores w.r.t. to maxScore
            animationSpeedSlowdownImage = new float[sortedTotalScores.Length + 1];
            animationSpeedSlowdownImage[0] = 0f;
            animationSpeedSlowdownImage[sortedTotalScores.Length] = 100f;
            for (int i = 1; i < sortedTotalScores.Length; ++i)
            {
                animationSpeedSlowdownDomain[i] = i * (100f / sortedTotalScores.Length);
                animationSpeedSlowdownImage[i] = (float)(sortedTotalScores[i + sortedTotalScores.Length - animationSpeedSlowdownDomain.Length] * 100m / maxScore);
            }
            if (!maxScoreTie)
            {
                animationSpeedSlowdownDomain[sortedTotalScores.Length - 1] = 98f;
            }

            colorsCategories = new Color[expectedNumberOfCategories];
            brushesCategories = new Brush[expectedNumberOfCategories--];
            for (int i = expectedNumberOfCategories; i >= 0; --i)
            {
                colorsCategories[i] = Color.FromArgb(255, 150 - (i * 100 / expectedNumberOfCategories), 0);
                brushesCategories[i] = new SolidBrush(colorsCategories[i]);
            }

            // category name rects
            categoryNamesPos = new RectangleF[categories.Length];
            float height = categoryNamesHeight / categories.Length;
            for (int i = 0; i < categories.Length; ++i)
            {
                categoryNamesPos[i] = new RectangleF(
                    categoryNamesLeft,
                    categoryNamesBottom - (i + 1) * height,
                    categoryNamesWidth,
                    height);
            }

#if VISUALIZE_ANIMATION_SPEED
            RenderAnimationSpeed();
#else
            renderThread = new Thread(new ThreadStart(RenderThreadLoop));
            renderThread.Start();
#endif

            Invalidate();
        }

        #region rendering

        private void Draw(object sender, PaintEventArgs e)
        {
            SingleBuffer buffer = null;
            try
            {
                while (null == (buffer = tripleBuffer.GetForDrawing()))
                {
                    if (!isRunning)
                    {
                        return;
                    }
                }

                var rect = e.Graphics.ClipBounds;

                if (bitmapScores != null)
                {
                    e.Graphics.DrawImage(bitmapScores, 0f, 0f, rect.Width, rect.Height);
                }

                e.Graphics.DrawImage(buffer.RenderTarget, 0f, 0f, rect.Width, rect.Height);
            }
            finally
            {
                if (buffer != null)
                {
                    tripleBuffer.ReleaseForDrawing(buffer);
                }
            }

            if (isRunning && keepRendering)
            {
                Invalidate();
            }
        }

        private void RenderScores(Graphics renderTargetGraphics, float animationPct)
        {
            renderTargetGraphics.Clear(colorBackground);

            // score on topmost grid line = gridLines * 10
            int gridLines = decimal.ToInt32((maxScore / 10m)) + 1;

            // grid
            for (int i = gridLines; i >= 0; --i)
            {
                float y = tableTop + (i * tableHeight / gridLines);
                renderTargetGraphics.DrawLine(penGridLines, tableLeft, y, tableRight, y);
            }

            decimal maxScoreToDraw = new decimal(animationPct) * maxScore / 100m;

            float maxGridLineScore = gridLines * 10f;
            decimal[] accScores = new decimal[teams.Length];

            float width = tableWidth / teams.Length;
            float previousY, newY = 0f;
            bool maxScoreReached;
            float columnWidth = width / 3f;
            float[] categoryProgress = new float[categories.Length];

            for (int i = teams.Length - 1; i >= 0; --i)
            {
                accScores[i] = 0m;

                // team name
                float x = tableLeft + i * width;
                RectangleF rect = new RectangleF(x, teamNamesTop, width, teamNamesHeight);
                renderTargetGraphics.DrawString(teams[i], fontTeamNames, brushTeamNames, rect, stringFormat);

                // score
                maxScoreReached = false;
                previousY = tableBottom;

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

                    newY = tableBottom - tableHeight * (float)accScores[i] / maxGridLineScore;
                    renderTargetGraphics.FillRectangle(brushesCategories[j], x + columnWidth, newY, columnWidth, previousY - newY);

                    previousY = newY;

                    if (maxScoreReached)
                    {
                        break;
                    }
                }

                rect.X += (width - tableTotalScoresWidth) / 2f;
                rect.Width = tableTotalScoresWidth;
                rect.Y = newY - tableTotalScoresHeight;
                rect.Height = tableTotalScoresHeight;
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

        private void RenderFireworks(Graphics renderTargetGraphics, double elapsed)
        {
            if (!enableFireworks || fireworks.Count == 0)
            {
                return;
            }

            renderTargetGraphics.Clear(Color.Transparent);

            double a, x, y, r, g;
            foreach (Firework firework in fireworks)
            {
                // transform [0, MaxAge] -> [0, 17]
                // use x^(1/5) from [0, 17] -> [0, 1.5]
                // transform [0, 1.5] -> [0, Radius]
                r = LinearTransformD(firework.Age += elapsed, 0d, firework.MaxAge, 0d, 17d);
                r = Math.Pow(r, 1d / 5d);
                r = LinearTransformD(r, 0d, 1.5d, 0d, firework.Radius);
                g = firework.GravityOff * firework.Age / firework.MaxAge;

                for (int i = ParticlesPerFirework - 1; i >= 0; --i)
                {
                    a = 2d * i * Math.PI / ParticlesPerFirework;
                    x = firework.CenterX + r * Math.Cos(a);
                    y = firework.CenterY + r * Math.Sin(a) + g;

                    for (float rr = 5f; rr > 0f; rr -= 0.5f)
                    {
                        renderTargetGraphics.FillEllipse(brushesFireworks[firework.Brush], (float)x - rr, (float)y - rr, 2f * rr, 2f * rr);
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

            SingleBuffer buffer = null;

            while (isRunning)
            {
                if (stopwatch.Elapsed.TotalMilliseconds - elapsed >= 30)
                {
                    elapsed = stopwatch.Elapsed.TotalMilliseconds;

                    try
                    {
                        while (null == (buffer = tripleBuffer.GetForRenderer())) ;

                        float pct = (float)(elapsed * 100f / animationLength);
                        float modifiedPct = CalculatedModifiedAnimationPct(pct);

                        if (modifiedPct >= 100f)
                        {
                            RenderScores(buffer.RenderTargetGraphics, 100f);
                            bitmapScores = new Bitmap(BackBufferWidth, BackBufferHeight);
                            using (Graphics graphics = Graphics.FromImage(bitmapScores))
                            {
                                graphics.DrawImage(buffer.RenderTarget, 0, 0);
                            }
                            break;
                        }

                        RenderScores(buffer.RenderTargetGraphics, modifiedPct);
                    }
                    finally
                    {
                        if (buffer != null)
                        {
                            tripleBuffer.ReleaseForRenderer(buffer);
                            buffer = null;
                        }
                    }
                }
            }

            if (isRunning && enableFireworks)
            {
                double prev = stopwatch.Elapsed.TotalMilliseconds;
                double[] fireworkRespawnTime = new double[numberOfFireworks];

                while (isRunning)
                {
                    for (int i = numberOfFireworks - 1; i >= 0; --i)
                    {
                        if (fireworkRespawnTime[i] < prev)
                        {
                            fireworkRespawnTime[i] = CreateFirework(prev);
                        }
                    }

                    if ((elapsed = stopwatch.Elapsed.TotalMilliseconds - prev) >= 30)
                    {
                        prev = stopwatch.Elapsed.TotalMilliseconds;
                        try
                        {

                            while (null == (buffer = tripleBuffer.GetForRenderer())) ;

                            RenderFireworks(buffer.RenderTargetGraphics, elapsed);
                        }
                        finally
                        {
                            if (buffer != null)
                            {
                                tripleBuffer.ReleaseForRenderer(buffer);

                            }
                        }
                    }
                }
            }

            keepRendering = false;
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

        private double CreateFirework(double time)
        {
            double radius = 75d + 100d * rng.NextDouble();
            double maxAge = 2000d + 2000d * rng.NextDouble();
            fireworks.Add(new Firework(rng.Next(0, brushesFireworks.Length), maxAge, radius, 7d + 15d * rng.NextDouble(),
                300d + 1320d * rng.NextDouble(), 20d + radius + 200d * rng.NextDouble()));

            return time + maxAge + rng.NextDouble() * 2500d;
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
