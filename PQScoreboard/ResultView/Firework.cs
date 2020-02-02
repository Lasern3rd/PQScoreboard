namespace PQScoreboard
{
    public class Firework
    {
        public Firework(int brush, double maxAge, double radius, double gravityOff, double x, double y)
        {
            Brush = brush;
            MaxAge = maxAge;
            Radius = radius;
            GravityOff = gravityOff;
            Age = 0d;
            CenterX = x;
            CenterY = y;
        }

        public int Brush { get; private set; }

        public double MaxAge { get; private set; }

        public double Radius { get; private set; }

        public double GravityOff { get; private set; }

        public double Age { get; set; }

        public double CenterX { get; set; }

        public double CenterY { get; set; }
    }
}
