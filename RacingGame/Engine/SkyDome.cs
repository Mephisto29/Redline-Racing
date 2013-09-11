/*
 * This class is used to draw the enviroment skydome
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

namespace RacingGame.Engine
{
    class SkyDome
    {
        //current graphics device being used
        GraphicsDeviceManager grapics;
        GraphicsDevice device;

        //Dome model and the sky texture used
        Model skyDome;
        Texture2D skytexture;

        //current effect used
        BasicEffect effect;

        //constructor
        public SkyDome(ref Model newSkyDome, ref Texture2D newTexture, ref BasicEffect newEffect, ref GraphicsDeviceManager newDevice)
        {
            //initialize data memebers
            skyDome = newSkyDome;
            skytexture = newTexture;
            effect = newEffect;
            grapics = newDevice;
            device = grapics.GraphicsDevice;

            //set the effects of the model to the current effect being used
            skyDome.Meshes[0].MeshParts[0].Effect = effect.Clone(device);
        }

        //render the sky using a dome model and pasing sky texture on the model
        public void DrawSkyDome(Matrix view, Matrix projection, float rotation)
        {
            device.RenderState.DepthBufferWriteEnable = false;

            Matrix[] modelTransforms = new Matrix[skyDome.Bones.Count];
            skyDome.CopyAbsoluteBoneTransformsTo(modelTransforms);

            Matrix wMatrix = Matrix.CreateTranslation(0, -0.4f, 0) * Matrix.CreateScale(500) * Matrix.CreateRotationY(MathHelper.ToRadians(rotation));
            foreach (ModelMesh mesh in skyDome.Meshes)
            {
                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    currentEffect.FogEnabled = true;
                    currentEffect.FogColor = Color.White.ToVector3();
                    currentEffect.FogStart = 0.0f;
                    currentEffect.FogEnd = 400.0f;

                    currentEffect.World = wMatrix;
                    currentEffect.View = view;
                    currentEffect.Projection = projection;
                    currentEffect.TextureEnabled = true;
                    currentEffect.Texture = skytexture;
                }
                mesh.Draw();
            }

            device.RenderState.DepthBufferWriteEnable = true;
        }
    }
}
