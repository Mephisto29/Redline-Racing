using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RacingGame.Engine.TerrainLoader
{
    class Node
    {
        //class used in the AI, the node contains the default information for a tile to deternine the A* shortest path

        //variables
        public int x;
        public int y;
        public Node parentNode;
        public int movementCost;
        public int functionScore;
        public bool closed = false;

        //contructor
        public Node(int x, int y, Node parentNode, int movementCost, int functionScore)
        {
            this.x = x;
            this.y = y;
            this.parentNode = parentNode;
            this.movementCost = movementCost;
            this.functionScore = functionScore;
        }
    }
}
