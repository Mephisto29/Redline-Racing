using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RacingGame.Engine.TerrainLoader
{
    class AImap
    {
        private AItile[,] aiMap;
        private List<Vector2> neighbours;
        private List<Node> openList;
        private List<Node> closedList;
        private bool[,] boolGrid;

        private Node parentNode;

        public AImap(AItile[,] ai)
        {
            aiMap = ai;
        }

        public void getNeighbours(int currentPositionX, int currentPositionY)
        {
            neighbours = new List<Vector2>();
            neighbours.Clear();

            //cell to the right
            if (currentPositionX + 1 < 512)
            {
                if(aiMap[currentPositionX, currentPositionY].Walkable)
                    neighbours.Add(new Vector2(currentPositionX, currentPositionY));
            }

            //cell to the left
            if (currentPositionX - 1 < 512)
            {
                if (aiMap[currentPositionX, currentPositionY].Walkable)
                    neighbours.Add(new Vector2(currentPositionX, currentPositionY));
            }

            //cell to the top
            if (currentPositionY - 1 < 512)
            {
                if (aiMap[currentPositionX, currentPositionY].Walkable)
                    neighbours.Add(new Vector2(currentPositionX, currentPositionY));
            }

            //cell to the bottom
            if (currentPositionY + 1 < 512)
            {
                if (aiMap[currentPositionX, currentPositionY].Walkable)
                    neighbours.Add(new Vector2(currentPositionX, currentPositionY));
            }
        }

        //gets the heuristic function (i.e. the guess of the distance to the player object) uses delta max approach
        public int getHeuristic(int currentPositionX, int currentPositionY, int destinationPositionX, int destinationPositionY)
        {
            return Math.Max(Math.Abs(currentPositionX - destinationPositionX), Math.Abs(currentPositionY - destinationPositionY));
        }

        public Vector2 getShortestPath(Vector2 position, Vector2 destination)
        {
            openList = new List<Node>();
            closedList = new List<Node>();
            boolGrid = new bool[512, 512];

            //setting the initial boolean grid to false, grid used for checking if tile has been visited
            for (int a = 0; a < 512; a++)
                for (int b = 0; b < 512; b++)
                    boolGrid[a, b] = false;

            int currentPositionInGridX = (int)position.X;
            int currentPositionInGridY = (int)position.Y;
            int destinationPositionInGridX = (int)destination.X;
            int destinationPositionInGridY = (int)destination.Y;

            //if position is at destination already stop search
            if (currentPositionInGridX == destinationPositionInGridX && currentPositionInGridY == destinationPositionInGridY)
                  return destination;

            else
            {
                //calculate function score for cell
                int functionScore = 1 + getHeuristic(currentPositionInGridX, currentPositionInGridY, destinationPositionInGridX, destinationPositionInGridY);

                //set the current parent node
                parentNode = new Node(currentPositionInGridX, currentPositionInGridY, null, 0, functionScore);
                openList.Add(parentNode);

                getNeighbours(parentNode.x, parentNode.y);

                //loop through all current neighbours and add to list
                foreach (Vector2 currentVector in neighbours)
                {
                    currentPositionInGridX = (int)currentVector.X ;
                    currentPositionInGridY = (int)currentVector.Y ;
                    functionScore = (parentNode.movementCost + 1) + getHeuristic(currentPositionInGridX, currentPositionInGridY, destinationPositionInGridX, destinationPositionInGridY);
                    openList.Add(new Node(currentPositionInGridX, currentPositionInGridY, parentNode, parentNode.movementCost + 1, functionScore));
                }

                //loop through openlist
                openList.Remove(parentNode);
                closedList.Add(parentNode);
                boolGrid[parentNode.x, parentNode.y] = true;

                for (int a = 0; a < openList.Count; a++)
                {
                    //remove starting position
                    a = 0;
                    parentNode = openList[getLowestScoreNode()];
                    currentPositionInGridX = parentNode.x;
                    currentPositionInGridY = parentNode.y;

                    if (currentPositionInGridX == destinationPositionInGridX && currentPositionInGridY == destinationPositionInGridY)
                    {
                        openList.Remove(parentNode);
                        closedList.Add(parentNode);
                        boolGrid[parentNode.x, parentNode.y] = true;
                        break;
                    }
                }
            }

            Node nextNode = getNextNode();
            Vector2 newPosition = new Vector2(nextNode.x, nextNode.y);

            return newPosition;
        }

        //get the lowest score node in the openlist
        public int getLowestScoreNode()
        {
            int counter = 0;
            int position = 0;
            int currentLowestScore = openList[position].functionScore;

            foreach (Node currentNode in openList)
            {
                if (currentNode.functionScore < currentLowestScore)
                {
                    position = counter;
                    currentLowestScore = currentNode.functionScore;
                }

                counter++;
            }

            return position;
        }

        //return the node that contains the next position enemy has to move to.
        public Node getNextNode()
        {
            Node lastNode = closedList[closedList.Count - 1];

            while (lastNode.parentNode.parentNode != null)
            {
                lastNode = lastNode.parentNode;
            }

            return lastNode;
        }
    }
}
