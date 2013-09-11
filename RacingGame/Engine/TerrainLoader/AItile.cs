using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
namespace RacingGame.Engine.TerrainLoader
{
    class AItile
    {
        //private data members
        private Vector3 position;
        private bool walkable;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public bool Walkable
        {
            get { return walkable; }
            set { walkable = value; }
        }

        //constructor
        public AItile(Vector3 newPosition, bool newState)
        {
            position = newPosition;
            walkable = newState;
        }
    }
}
