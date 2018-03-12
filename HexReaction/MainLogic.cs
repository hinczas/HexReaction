using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HexReaction
{
#pragma warning disable 0168
    class MainLogic
    {
        public Dictionary<Point, Node> nodesList;
        public Dictionary<int, Point> nodesHashes;
        public List<int> damageNodes;
        public List<int> removeNodes;
        public bool busy = false;
        public int score = 0, floor = 0;
        public LinkList linkList;
        private double difficulty;

        private int level = 0;
        private Random rand = new Random();
        int bonus = 0;
        static int workingHash;
        private int gridSize = 10;

        public MainLogic(int fl, int gs, double diff)
        {
            nodesList = new Dictionary<Point, Node>();
            nodesHashes = new Dictionary<int, Point>();
            damageNodes = new List<int>();
            removeNodes = new List<int>();
            rand = new Random();
            gridSize = gs;
            floor = fl;
            linkList = new HexReaction.LinkList();
            difficulty = diff;
            AddNodes();
            LinkNodes();
            HashNodes();
            linkList.InitialiseList(nodesList, nodesHashes);
        }

        /*
         * Initializes mesh/matrix of 55 nodesList
         * Adds nodesList to mainLogic NODES list
         */
        public void AddNodes()
        {
            int modInd = 2;
            for (int i = 1; i <= gridSize; i++)
            {
                int addition = i % modInd;
                for (int j = addition; j <= gridSize; j++)
                {
                    Point tmpPoint = new Point((j+1)*50,(i+1)*30);
                    Node tmpNode = new Node((j+1) * 50, (i+1) * 30, 1);
                    nodesList.Add(tmpPoint, tmpNode);
                    j++;
                }
            }
            RandomizeBangs();
        }

        public void RandomizeBangs()
        {
            int incr = 1;
            int min = 0;
            int low = 0;
            int maxLow = (int)((Math.Pow(gridSize, 2) / 2) * difficulty);
            int ranCnt = nodesList.Count;
            List<int> bangsList = new List<int>();
            for (int j = 0; j < ranCnt; j++)
            {
                int bangLow = rand.Next(1 + low, 4);
                int bangHigh = rand.Next(bangLow, 6);
                int bangs = rand.Next(bangLow, bangHigh + floor);
                if (bangs == incr) { min++; }
                if (min >= (maxLow + floor))
                {
                    low = low == 3 ? 3 : low + 1;
                    incr++;
                    min = 0;
                    maxLow = maxLow * 4;
                }
                bangsList.Add(bangs);
            }
            foreach(Node nod in nodesList.Values )
            {
                int tmpindex = rand.Next(bangsList.Count());
                nod.bangs = bangsList[tmpindex];
                bangsList.RemoveAt(tmpindex);
            }

        }
        /*
         * Translates mainLogic NODES list into hash, point dictionary for ID(hash) lookup
         */
        public void HashNodes()
        {
            int tmpIdx = nodesList.Count;
            for (int i = 0; i< tmpIdx; i++)
            {
                nodesHashes.Add(nodesList.ElementAt(i).Value.nodeID, nodesList.ElementAt(i).Key);
            }
        }
        public double DistanceTo(Point a, Point b)
        {
            int pointAx = a.X;
            int pointAy = a.Y;
            int pointBx = b.X;
            int pointBy = b.Y;
            var deltaX = Math.Pow((pointBx - pointAx), 2);
            var deltaY = Math.Pow((pointBy - pointAy), 2);
            //pythagoras theorem for distance
            var distance = Math.Sqrt(deltaY + deltaX);
            return distance;
        }
        
        /*
         * Links all close nodesList together
         * Populates each node's link list
         */
        public void LinkNodes()
        {
            int cnt = nodesList.Count;
            for (int j = 0; j < cnt; j++)
            {
                Node tmp = nodesList.ElementAt(j).Value;
                for (int i = 0; i < cnt; i++)
                {
                    Node comp = nodesList.ElementAt(i).Value;
                    if (tmp.nodeID != comp.nodeID)
                    {
                        double dist = DistanceTo(tmp.position, comp.position);
                        if (dist <= 60)
                        {
                            nodesList.ElementAt(j).Value.AddLink(comp.nodeID);
                        }
                    }
                }
            }
        }

        /**
         * Damages nodesList in batches
         * If node explodes and damages other nodesList 
         * they are added to the que to be processed all at once in the next run.
         */
        public int DamageQueuedNodes()
        {
            int tmpBonus = bonus;
            busy = true;
            level++;
            ResetSize();            
            if (damageNodes.Count > 0) // Check if there are any nodes to process queued
            {                
                busy = true; // there are nodes to precess, set to busy
                level++;
                int nodesCount = damageNodes.Count;
                List<int> tmpNodesDam = new List<int>();
                // Save number of nodes to process in temporary index
                // This needs to be done as the list will grow during processing
                for (int i = 0; i<nodesCount; i++)
                {
                    workingHash = damageNodes.ElementAt(i);
                    nodesList[nodesHashes[workingHash]].bangs--;                    
                    if (nodesList[nodesHashes[workingHash]].bangs <= 0) // Check if current node should be destroyed
                    {
                        score++;
                        bonus++;
                        tmpBonus = bonus;
                        List<int> tmpLinkIDs = nodesList[nodesHashes[workingHash]].linkIDs;                        
                        RemoveNodes(tmpLinkIDs, workingHash);
                        nodesList.Remove(nodesHashes[workingHash]);
                        nodesHashes.Remove(workingHash);
                        damageNodes.AddRange(tmpLinkIDs);
                        // Remove current node occurences from damageNodes to ensure it wont
                        // try to reprocess it again
                        while (damageNodes.Contains(workingHash))
                        {
                            int firstIndex = damageNodes.FindIndex(MatchesHashToRemove);                            
                            if (firstIndex < nodesCount) // adjust main iteration index after removing node
                            {
                                damageNodes.RemoveAt(firstIndex);
                                nodesCount--;
                                if (firstIndex <= i)
                                {
                                    i--;
                                }
                            } else
                            {
                                damageNodes.RemoveAt(firstIndex);
                            }
                         }
                    // node not to be destroyed so add into tmp nodes list
                    // to later remove from main damageNodes list
                    } else
                    {
                        tmpNodesDam.Add(workingHash);
                    }
                }
                // delete processed nodes from damageNodes queue list
                foreach (int nodeHash in tmpNodesDam)
                {
                    damageNodes.Remove(nodeHash);
                }
                Explode(damageNodes.ToArray());
            }
            // no nodes to damage, report not-busy back to main window
            else
            {
                bonus = 0;
                busy = false;
            }
            score = score + tmpBonus*(floor+1);
            return tmpBonus;
        }


        /*
         * Nodes marked as to damage are set to grow
         */
        private void Explode(int[] hashes)
        {
            foreach (int hash in hashes)
            {
                nodesList[nodesHashes[hash]].size = 3;
            }
        }

        /*
         * Match predicate to ensure destroyed node is removed from the damage queue (all instances)
         */
        private static bool MatchesHashToRemove(int hash)
        {
            return hash==workingHash;
        }
        
        /*
         * Restores original size of all nodesList
         */
        private void ResetSize()
        {
            //foreach(Node node in nodesList.Values)
            //{
            //    node.size = 0;
            //}
            foreach(int hash in damageNodes) 
            {
                nodesList[nodesHashes[hash]].size = 0;
            }
        }        
        
        /*
         * Removes parent / calling node from all linked nodesList' link lists
         */
        public void RemoveNodes(List<int> hashes, int parent)
        {
            linkList.RemoveLinks(parent);
            //linkList.AnimateLinks(parent);

            foreach (int hash in hashes)
            {
                nodesList[nodesHashes[hash]].linkIDs.Remove(parent);
            }
        }

        /*
         * Returns closes point to a given point
         */
        public Point Closest(Point currentPoint)
        {
            Point resultPoint = new Point();

            int cnt = nodesList.Count;
            for (int j = 0; j < cnt; j++)
            {
                Point tmpPoint = nodesList.ElementAt(j).Key;
                double distance = DistanceTo(tmpPoint, currentPoint);
                if (distance < 8.4)
                {
                    resultPoint = tmpPoint;
                    break;
                }
            }

            return resultPoint;
        }
    }
}
