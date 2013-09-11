using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using RacingGame.Engine;
using RacingGame.Engine.TerrainLoader;
using RacingGame.Engine.HelperFunctions;
using RacingGame.Enviroment;

namespace RacingGame.Enviroment.Tracks
{
    class AlpineCollision
    {
        public List<BoundingBox> boundingBoxes;
        public bool collision = false;
        public BoundingBox[] boxes;

        public bool Collision
        {
            get { return collision;}
        }


        public AlpineCollision()
        {
            LoadContent();
        }

        public void LoadContent()
        {
            boundingBoxes = new List<BoundingBox>();
            Vector3[] boxPoints = new Vector3[2];
            boxPoints[0] = new Vector3(269f, 0f, -1828.3f);
            boxPoints[1] = new Vector3(199f,20f,-1935.7f);
            BoundingBox lol = BoundingBox.CreateFromPoints(boxPoints);
            boundingBoxes.Add(lol);



        }
        public void CollisionPoints(Vector3 position)
        {
            boxes = boundingBoxes.ToArray();
            for (int i = 0; i < 1; i++)
            {
                if (boxes[i].Contains(position) == ContainmentType.Contains)
                {
                    collision = true;
                }
            }
        }
    }
}
