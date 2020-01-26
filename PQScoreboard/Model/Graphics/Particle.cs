using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PQScoreboard
{
    public class Particle : IGraphicsObject
    {
        public int PosX { get; set; }

        public int PosY { get; set; }

        public int OffX { get; set; }

        public int OffY { get; set; }

        public int A { get; set; }

        public int B { get; set; }

        public void Draw(Graphics graphics)
        {
            throw new System.NotImplementedException();
        }

        public bool Update(double elapsed)
        {
            throw new System.NotImplementedException();
        }
    }
}
