/*
 * This class is used for the main menu to animate and produce the car in the main menu
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

namespace RacingGame.Engine.UI
{
    class MenuCar
    {
        //private data memebers
        private Model currentModel;
        private Model modelWheels;

        private Vector3 position;
        private Vector3 direction;
        private Vector3 normal;

        private Vector3[] wheelPositions;
        private float scale;
        private float orientation;
        private float angle;

        //getters and setters
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

        //constructor
        public MenuCar(Model model, Model wheel, Vector3 position)
        {
            currentModel = model;
            modelWheels = wheel;
            wheelPositions = new Vector3[4];

            this.position = position;
            this.direction = Vector3.Forward;
            this.normal = Vector3.Up;
            this.scale = 0.15f;
            this.orientation = 130.0f;
            this.angle = 0.0f;

            this.wheelPositions[0] = new Vector3(0.15f, 0.03f, 0.155f);
            this.wheelPositions[1] = new Vector3(0.13f, 0.025f, -0.31f);
        }

        //update method that used to increase an angle periodically
        public void update(GameTime gameTime)
        {
            if (angle < -360)
                angle = 0;
            angle -= 0.03f;
        }

        //draw the car and rotated the wheels appropriately
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

                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(0.15f) * Matrix.CreateTranslation(position) * Matrix.CreateRotationY(MathHelper.ToRadians(orientation));
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;
                }

                mesh.Draw();
            }

            //draw back left wheel
            drawWheel(wheelPositions[0], viewMatrix, projectionMatrix, 0.0f, angle);
            drawWheel(wheelPositions[1], viewMatrix, projectionMatrix, -30.0f, 0.0f);
        }

        //draw the wheels
        private void drawWheel(Vector3 position, Matrix viewMatrix, Matrix projectionMatrix, float rotation, float angle)
        {
            Matrix[] transforms = new Matrix[modelWheels.Bones.Count];
            modelWheels.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in modelWheels.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale) * Matrix.CreateRotationX(angle) * Matrix.CreateRotationY(MathHelper.ToRadians(rotation)) * Matrix.CreateTranslation(position) * Matrix.CreateRotationY(MathHelper.ToRadians(orientation));
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;
                }

                mesh.Draw();
            }
        }
    }
}
