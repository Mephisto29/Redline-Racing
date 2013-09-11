/*
 *  This class represents the Lamborghini Murcielago LP640 vehicle and set the data appropriately for this car 
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
using RacingGame.Engine.Physics;
using RacingGame.Engine.TerrainLoader;
using RacingGame.Engine.Audio;
using RacingGame.Engine.PaticleEngine;
using RacingGame.Dashboards;

namespace RacingGame.Vehicles
{
    class KoenigseggCCX : Vehicle
    {
        //gear ratios and power list for the engine of this car
        List<float> power = new List<float>(new float[] { 0.2f, 0.3f, 0.4f, 0.7f, 0.8f, 1.0f, 0.8f, 0.7f, 0.6f });
        List<float> ratios = new List<float>(new float[] { -2.215f, 0, 3.091f, 2.105f, 1.565f, 1.241f, 1.065f, 0.784f });

        //gauge textures
        private SpriteBatch spriteBatch;
        private SpriteFont currentFont;
        private Texture2D guageTexture;
        private Texture2D needleTexture;
        private Texture2D lightTexture;
        private Texture2D canister;

        //constructor
        public KoenigseggCCX(ref GraphicsDeviceManager graphics, ContentManager currentContentLoader, ref SpriteBatch spriteBatch, Vector3 start, InputHandler input, ref SpriteFont currentFont, ref Model carModel, Game game)
            : base(ref graphics , start, input) 
        {
            //smoke particle system
            smoke = new SmokePlumeParticleSystem(game, currentContentLoader, graphics.GraphicsDevice);
            smoke.DrawOrder = 100;
            game.Components.Add(smoke);

            //car model
            this.carModel = carModel;

            //gearbox and motor for this car
            changeGearBoxToAutomatic();

            //create Gauge
            guageTexture = currentContentLoader.Load<Texture2D>("Textures//Gauges//Speedo");
            needleTexture = currentContentLoader.Load<Texture2D>("Textures//Gauges//SpeedoNeedle");
            lightTexture = currentContentLoader.Load<Texture2D>("Textures//Gauges//SpeedoGearLight");
            canister = currentContentLoader.Load<Texture2D>("Textures//Gauges//NOScanister");
            this.spriteBatch = spriteBatch;
            this.currentFont = currentFont;
            createGauge(graphics.GraphicsDevice.Viewport);

            //collision data
            collisionSphere = new BoundingSphere(position / 20, 0.30f);
            collisionBox = CreateBoundingBox();
            collisionSphere = CreateBoundingSphere();

            //sound
            sound = new SoundEffectPlayer(currentContentLoader, this, "Sound//SoundEffects//Lamborghini Murcielago LP640");

            //apply rotation
            carGlobalRotation = MathHelper.ToRadians(90);

            //select car bones
            this.frontWheelRight = carModel.Bones["polySurface2925"];
            this.frontWheelLeft = carModel.Bones["polySurface2928"];
            this.backWheelRight = carModel.Bones["polySurface2926"];
            this.backWheelLeft = carModel.Bones["polySurface2927"];

            //car scale
            carScale = 0.05f;
        }

        protected override BoundingBox CreateBoundingBox()
        {
            //maintain vectors that are the max value
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            foreach (ModelMesh mesh in carModel.Meshes)
            {
                //curent model transforms
                Matrix[] transforms = new Matrix[carModel.Bones.Count];
                carModel.CopyAbsoluteBoneTransformsTo(transforms);

                //maximum position in the mesh
                Vector3 meshMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                //minimum position in the mesh
                Vector3 meshMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

                //because a mesh can consist of various other parts, have to loop over each part
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    //the stride is the size of the vertex in bytes
                    int stride = part.VertexStride;

                    //contains all vertices
                    byte[] vertexData = new byte[stride * part.NumVertices];
                    //add data to the vertexData array
                    mesh.VertexBuffer.GetData(part.BaseVertex * stride, vertexData, 0, part.NumVertices, 1);

                    Vector3 vertPosition = new Vector3();
                    for (int i = 0; i < vertexData.Length; i += stride)
                    {
                        //X,Y,Z positions of a vertex
                        vertPosition.X = BitConverter.ToSingle(vertexData, i);
                        vertPosition.Y = BitConverter.ToSingle(vertexData, i + sizeof(float));
                        vertPosition.Z = BitConverter.ToSingle(vertexData, i + sizeof(float) * 2);

                        // update our running values from this vertex
                        meshMin = Vector3.Min(meshMin, vertPosition);
                        meshMax = Vector3.Max(meshMax, vertPosition);
                    }
                }

                // transform by mesh bone transforms
                meshMin = Vector3.Transform(meshMin, transforms[mesh.ParentBone.Index] * Matrix.CreateRotationX(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(new Vector3(-0.3f, 2.3f, 0)));
                meshMax = Vector3.Transform(meshMax, transforms[mesh.ParentBone.Index] * Matrix.CreateRotationX(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(new Vector3(0.3f, 2.5f, 0)));

                meshMin.Z *= 0.5f;
                meshMax.Z *= 0.5f;
                meshMin.Y *= 0.18f;
                meshMax.Y *= 0.18f;

                //Expand model extents by the ones from this mesh
                min = Vector3.Min(min, meshMin);
                max = Vector3.Max(max, meshMax);
                min.Y += HeightOffset;
                max.Y += HeightOffset;

                UpdateBoundingBox();
            }

            return new BoundingBox(min, max);
        }

        //bounding sphere for collision of the car
        protected override BoundingSphere CreateBoundingSphere()
        {
            foreach (ModelMesh mesh in carModel.Meshes)
            {
                if (collisionSphere.Radius == 0)
                    collisionSphere = mesh.BoundingSphere;
                else
                    collisionSphere = BoundingSphere.CreateMerged(collisionSphere, mesh.BoundingSphere);
            }

            collisionSphere.Radius = collisionSphere.Radius / 400;

            return collisionSphere;
        }

        //car name
        public override string Name
        {
            get { return "Koenigsegg CCX"; }
        }
        //update car
        public override void update(GameTime gameTime, bool canRace, bool enableSound)
        {
            carGauge.Update(currentMotor.CurrentRPM, speed, currentMotor.GearBoxUsed.CurrentGear);
            base.update(gameTime, canRace, enableSound);
        }

        public override void createGauge(Viewport viewport)
        {
            //gauge data
            carGauge = new Gauges(ref guageTexture,
                                    ref needleTexture,
                                    ref lightTexture,
                                    ref canister,
                                    currentMotor.RedLineRPM,
                                    currentMotor.CurrentRPM,
                                    ref spriteBatch,
                                    new Vector2(viewport.Width - guageTexture.Width, viewport.Height - guageTexture.Height),
                                    ref currentFont);
        }

        //draw car
        public override void draw(ChaseCamera cameraPosition, Viewport currentViewport, GameTime gameTime)
        {
            base.draw(cameraPosition, currentViewport, gameTime);
        }

        //draw car gauge
        public override void drawGauge()
        {
            float percentage = nosAmmount / 300;
            carGauge.Draw(nos, percentage);
        }

        //change gearbox types
        public override void changeGearBoxToAutomatic()
        {
            GearBox gearbox = GearBox.Create(false, ratios, 0.2f);
            currentMotor = new CarMotor(power, 8, 8f, gearbox);
            resetGears();
        }

        public override void changeGearBoxToManual()
        {
            GearBox gearbox = GearBox.Create(true, ratios, 0.2f);
            currentMotor = new CarMotor(power, 8, 8f, gearbox);
            resetGears();
        }
    }
}
