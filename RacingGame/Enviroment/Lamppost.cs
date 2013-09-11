/*
 * This class represents a lamp post in the game
 * 
 * colliding with the lamp post causes the vehicle to stop
*/
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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using RacingGame.Engine;
using RacingGame.Engine.HelperFunctions;

namespace RacingGame.Enviroment
{
    class Lamppost
    {
        private Model lampPostModel;
        private Vector3 lampPosition;
        private BoundingSphere collisionSphere;

        public Vector3 Position
        {
            get { return lampPosition; }
            set { lampPosition = value; }
        }

        public BoundingSphere ColisionSphere
        {
            get { return collisionSphere; }
        }

        public Lamppost(ref Model lamp, Vector3 position)
        {
            lampPostModel = lamp;
            lampPosition = position;

            collisionSphere = new BoundingSphere(position, 0.04f);
            collisionSphere.Center.Y -= 0.6f;
        }

        //check if coision has occured
        public bool checkCollision(BoundingSphere carShere)
        {
            if (collisionSphere.Contains(carShere) != ContainmentType.Disjoint)
                return true;
            else
                return false;
        }

        //draw the marker
        public void Draw(ChaseCamera camera)
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[lampPostModel.Bones.Count];
            lampPostModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in lampPostModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(0.05f) * Matrix.CreateTranslation(lampPosition);

                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                }

                mesh.Draw();
            }
        }
    }
}
