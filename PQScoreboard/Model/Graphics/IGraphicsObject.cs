using System.Drawing;

namespace PQScoreboard
{
    interface IGraphicsObject
    {
        void Draw(Graphics graphics);

        bool Update(double elapsed);
    }
}
