//#define VISUALIZE_ANIMATION_SPEED

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace PQScoreboard
{
    public partial class ResultForm : Form
    {
        private const string PathResources = "Resources";

        private const int backBufferWidth = 1920;
        private const int backBufferHeight = 1080;

        private readonly float fontSize = 24f;

        // TODO: cleanup

        private Bitmap bitmap;
        private Graphics graphics;
        private StringFormat stringFormat;
        private Font font;
        private Font boldFont;
        private Pen pen;
        private Bitmap bitmapTotalScoreFrame;

        private Rectangle rectTotalScoreFrame;

        private double animationLength;
        private bool enableFireworks;

        private bool isRunning;
        private Thread renderThread;

        private string[] teams;
        private string[] categories;
        private decimal[,] scores;
        private decimal maxScore;
        private Color[] colors;
        private Brush[] brushes;
        private RectangleF[] categoryNamesPos;
        float[] animationSpeedSlowdownDomain;
        float[] animationSpeedSlowdownImage;

        public ResultForm()
        {
            InitializeComponent();

            bitmap = new Bitmap(backBufferWidth, backBufferHeight);
            graphics = Graphics.FromImage(bitmap);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            font = new Font("Yu Gothic UI Light", fontSize, FontStyle.Regular);
            boldFont = new Font("Yu Gothic UI Light", fontSize, FontStyle.Bold);
            pen = new Pen(Color.Black, 1f);

            bitmapTotalScoreFrame = LoadBitmap("TotalScoreFrame.png", out rectTotalScoreFrame);

            DoubleBuffered = true;
            Paint += Draw;
        }

        public void StopAnimationAndClose()
        {
            isRunning = false;

            if (renderThread != null)
            {
                renderThread.Join();
            }

            bitmap.Dispose();
            graphics.Dispose();
            stringFormat.Dispose();
            font.Dispose();
            boldFont.Dispose();
            pen.Dispose();
            bitmapTotalScoreFrame.Dispose();

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
            colors = new Color[expectedNumberOfCategories];
            brushes = new Brush[expectedNumberOfCategories--];
            for (int i = expectedNumberOfCategories; i >= 0; --i)
            {
                // TODO: beware of division by 0
                colors[i] = Color.FromArgb(255, 150 - (i * 100 / (expectedNumberOfCategories)), 0);
                brushes[i] = new SolidBrush(colors[i]);
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

        private void Draw(object sender, PaintEventArgs e)
        {
            var rect = e.Graphics.ClipBounds;
            e.Graphics.DrawImage(bitmap, 0f, 0f, rect.Width, rect.Height);
        }

        private void RenderScores(float animationPct)
        {
            // score on topmost grid line = gridLines * 10
            int gridLines = decimal.ToInt32((maxScore / 10m)) + 1;

            // grid
            for (int i = gridLines; i >= 0; --i)
            {
                float y = 90f + (i * 900f / gridLines);
                graphics.DrawLine(pen, 300f, y, 1900f, y);
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
                graphics.DrawString(teams[i], boldFont, Brushes.Black, rect, stringFormat);

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
                    graphics.FillRectangle(brushes[j], x + columnWidth, newY, columnWidth, previousY - newY);

                    previousY = newY;

                    if (maxScoreReached)
                    {
                        break;
                    }
                }

                rect.Y = newY - 80f;
                rect.X += columnWidth;
                rect.Width -= 2 * columnWidth;
                graphics.DrawImage(bitmapTotalScoreFrame, rect, rectTotalScoreFrame, GraphicsUnit.Pixel);
                string score = maxScoreReached ? decimal.Round(accScores[i]).ToString("0") : accScores[i].ToString("0.#");
                graphics.DrawString(score, boldFont, Brushes.Black, rect, stringFormat);
            }

            // category names
            for (int i = categories.Length - 1; i >= 0; --i)
            {
                float alpha = 255f * categoryProgress[i] / (teams.Length * 100f);
                Brush b = new SolidBrush(Color.FromArgb((int)alpha, colors[i]));
                graphics.DrawString(categories[i], font, b, categoryNamesPos[i], stringFormat);
            }
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
                    graphics.Clear(Color.Transparent);

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
        }

        private float CalculatedModifiedAnimationPct(float pct)
        {
            if (pct > 100f)
            {
                return 100f;
            }

            int index = 0;
            for (; index < animationSpeedSlowdownDomain.Length && pct > animationSpeedSlowdownDomain[index]; ++index) ;

            float x = LinearTransform(pct, animationSpeedSlowdownDomain[index - 1], animationSpeedSlowdownDomain[index], 0, 100);
            float y = (float)Math.Sqrt(x) * 10f;
            return LinearTransform(y, 0, 100, animationSpeedSlowdownImage[index - 1], animationSpeedSlowdownImage[index]);
        }

        private float LinearTransform(float x, float da, float db, float ra, float rb)
        {
            return ra + (x - da) * (rb - ra) / (db - da);
        }

        private Bitmap LoadBitmap(string filename, out Rectangle rect)
        {
            string path = Path.Combine(PathResources, filename);
            Bitmap bmp = (Bitmap)Image.FromFile(path);
            rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            return bmp;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                StopAnimationAndClose();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

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
