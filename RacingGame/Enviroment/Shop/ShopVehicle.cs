/*
 * This class represents a car in the shop, it is responsble for the required information as well as  the rotation of the models
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

namespace RacingGame.Enviroment.Shop
{
    class ShopVehicle
    {
        //private data members
        private Model currentModel;

        private Vector3 position;
        private Vector3 direction;
        private Vector3 normal;

        //cost and rotation angle
        private float angle;
        private float carCost;

        //car data
        private String carName;
        private String carTopSpeed;
        private String numberOfGears;   
        private Texture2D logo;         //car logo

        //getters and setters
        public Texture2D Logo
        {
            get { return logo; }
        }

        public String Name
        {
            get { return carName; }
        }

        public String Speed
        {
            get { return carTopSpeed; }
        }

        public String Gears
        {
            get { return numberOfGears; }
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public Vector3 Direction
        {
            get { return direction; }
        }

        public Vector3 Normal
        {
            get { return normal; }
        }

        public float Cost
        {
            get { return carCost; }
        }

        //set car information
        public void SetInformation(String name, String speed, String gears, Texture2D newLogo)
        {
            carName = name;
            carTopSpeed = speed;
            numberOfGears = gears;
            logo = newLogo;
        }

        //constructor
        public ShopVehicle(ref Model newModel, Vector3 newPosition, float cost)
        {
            currentModel = newModel;
            position = newPosition;
            carCost = cost;

            direction = Vector3.Forward;
            normal = Vector3.Up;
        }

        //rotate car periodically
        public void update(GameTime gameTime)
        {
            if (angle < -360)
                angle = 0;
            angle += 0.01f;
        }

        //draw car model
        public void Draw(GameTime gameTime, Matrix viewMatrix, Matrix projectionMatrix)
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[currentModel.Bones.Count];
            currentModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in currentModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(0.05f) * Matrix.CreateTranslation(position) * Matrix.CreateRotationY(angle);
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;
                }

                mesh.Draw();
            }
        }
    }
}
