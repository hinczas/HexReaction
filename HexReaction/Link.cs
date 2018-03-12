using System.Drawing;

namespace HexReaction
{
    class Link
    {
        public Point pointA;
        public Point pointB;
        public int hashA;
        public int hashB;
        public int linkID;
        public bool animate = false;

        public Link()
        {
            linkID = this.GetHashCode();
        }
        public Link(Point a , Point b, int ha, int hb)
        {
            linkID = this.GetHashCode();
            pointA = a;
            pointB = b;
            hashA = ha;
            hashB = hb;
        }
    }
}
