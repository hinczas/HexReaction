using System.Collections.Generic;
using System.Drawing;

namespace HexReaction
{
    class LinkList : List<Link>
    {
        public void InitialiseList(Dictionary<Point, Node> nodes, Dictionary<int, Point> hashes)
        {
            foreach(Node node in nodes.Values)
            {
                foreach(int link in node.linkIDs) 
                {
                    if (!ContainsLink(node.nodeID, link))
                    {
                        Add(new Link(node.position, hashes[link], node.nodeID, link));
                    }
                }
            }
            
        }

        public bool ContainsLink(int nodeA, int nodeB)
        {
            bool result = false;

            foreach(Link link in this)
            {
                if ((link.hashA == nodeA && link.hashB == nodeB) || (link.hashA == nodeB && link.hashB == nodeA))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public void RemoveLinks(int hash)
        {
            List<Link> tempremove = new List<Link>();
            foreach(Link link in this)
            {
                if (link.hashA == hash || link.hashB == hash)
                {
                    tempremove.Add(link);
                }
            }
            foreach(Link link in tempremove)
            {
                Remove(link);
            }
        }

    }
}
