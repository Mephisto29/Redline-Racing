/*
 * This class represents the marker used to enter the shop.
 * 
 * colliding with the marker allows you to enter the shop
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

namespace RacingGame.Enviroment.Shop
{
    class ShopMarker
    {
        //protected data members
        protected GraphicsDeviceManager graphics;
        protected GraphicsDevice device;
        protected GameStateManager currentState;

        protected Model shopMarkerModel;
        protected Vector3 position;
        protected Effect effect;

        protected CarShop shop;

        protected BoundingSphere collisionSphere;

        //collision sphere
        public BoundingSphere CollisionSphere
        {
            get { return collisionSphere; }
        }

        //constructor
        public ShopMarker(ref GraphicsDeviceManager newGraphics, ref GameStateManager state, ref Model newModel, Vector3 newPosition, ref Effect newEffect)
        {
            graphics = newGraphics;
            device = graphics.GraphicsDevice;
            currentState = state;
            shopMarkerModel = newModel;

            position = newPosition;
            position.X -= 0.2f;
            position.Y -= 0.1f;
            position.Z -= 1.2f;
            effect = newEffect;

            collisionSphere = new BoundingSphere(position, 0.4f);
        }

        //load data
        public void LoadContent(ref CarShop newShop)
        {
            shop = newShop;
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
            Matrix[] transforms = new Matrix[shopMarkerModel.Bones.Count];
            shopMarkerModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in shopMarkerModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(0.22f) * Matrix.CreateTranslation(position);

                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                }

                mesh.Draw();
            }
        }
    }
}
