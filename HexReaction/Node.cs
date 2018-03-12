using System.Collections.Generic;
using System.Drawing;

namespace HexReaction
{
    class Node
    {
        public int nodeID;
        public List<int> linkIDs;
        public Point position;
        public int bangs, size;

        public Node(int posX, int posY, int bgs)
        {
            linkIDs = new List<int>();
            position = new Point(posX, posY);
            bangs   = bgs;
            nodeID = this.GetHashCode();
            size = 0;
        }
        
        public void AddLink(int linkID)
        {
            linkIDs.Add(linkID);
        }

        public void RemoveLink(int linkID)
        {
            linkIDs.Remove(linkID);
        }
        
        public Color Colour()
        {
            Color ret = Color.Black;

            switch(bangs)
            {
                case 0:
                    //ret = Color.FromArgb(37, 37, 38);
                    ret = Color.Black;
                    break;                   
                case 1: ret = Color.Red;
                    break;
                case 2:
                    ret = Color.Yellow;
                    break;
                case 3:
                    ret = Color.Green;
                    break;
                case 4:
                    ret = Color.Cyan;
                    break;
                case 5:
                    ret = Color.Blue;
                    break;
                case 6:
                    ret = Color.Violet;
                    break;
                case 7:
                    ret = Color.Pink;
                    break;
                case 8:
                    ret = Color.White;
                    break;
            }

            return ret;
        }
    }
}
