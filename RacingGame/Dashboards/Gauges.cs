/*
 * This class is used to draw the gauge of the car
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

using RacingGame.Engine.Physics;

namespace RacingGame.Dashboards
{
    class Gauges
    {
        //private datamembers used for drawing
        private SpriteBatch spriteBatch;
        private Texture2D gaugeTexture;
        private Texture2D gaugeNeedle;
        private Texture2D lightTexture;
        private Texture2D NOScanister;
        private Vector2 position;
        private Vector2 canisterPosition;
        private SpriteFont currentFont;

        private float rotation;
        private float currentSpeed;
        private float currentGear;
        private float totalRotation;
        private float currentRotation;

        //constructor
        public Gauges(ref Texture2D newGauge, ref Texture2D newNeedle, ref Texture2D shiftLight, ref Texture2D NOS,float totalToRotate, float currentRotate, ref SpriteBatch currentSpriteBatch, Vector2 newPosition, ref SpriteFont font)
        {
            spriteBatch = currentSpriteBatch;
            gaugeTexture = newGauge;
            gaugeNeedle = newNeedle;
            lightTexture = shiftLight;
            NOScanister = NOS;
            totalRotation = totalToRotate;
            currentRotation = currentRotate;
            position = newPosition;
            currentFont = font;
            canisterPosition = position;
        }

        //rotate the needle of the gauge appropriatly
        public void Update(float newCurrentRotation, float speed, int gear)
        {
            currentRotation = newCurrentRotation;
            currentSpeed = speed;
            currentGear = gear;

            rotation = (float)MathHelper.ToRadians((currentRotation / totalRotation) * 230);
            if (rotation > totalRotation - 4f)
                rotation = totalRotation - 4f;
        }

        //draw the gauge
        public void Draw(bool NOSenabled, float percentage)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            //draw gauge and needle and light
            spriteBatch.Draw(gaugeTexture, position, Color.White);
            spriteBatch.Draw(gaugeNeedle, new Vector2(position.X + gaugeTexture.Width / 2, position.Y + gaugeTexture.Height / 2), null, Color.White, rotation, new Vector2(gaugeNeedle.Width / 2, gaugeNeedle.Height / 2), 0.5f, SpriteEffects.None, 0);

            //draw shift light
            if (totalRotation - currentRotation < 1 && currentGear != -1)
                spriteBatch.Draw(lightTexture, position, Color.White);

            //draw nos canister
            if (NOSenabled)
            {
                Rectangle sourceRect = new Rectangle(0, (int)(NOScanister.Height - (NOScanister.Height * percentage)), (int)NOScanister.Width, (int)(NOScanister.Height));

                canisterPosition = position;
                canisterPosition.Y = position.Y + (NOScanister.Height - (NOScanister.Height * percentage));

                spriteBatch.Draw(NOScanister, canisterPosition, sourceRect, Color.White);
            }

            //draw current speed, current gear
            spriteBatch.DrawString(currentFont, Math.Round(currentSpeed).ToString(), new Vector2(position.X + 38, position.Y + gaugeTexture.Height - 70), Color.Red);

            if (currentGear == -1)
                spriteBatch.DrawString(currentFont, "R", new Vector2(position.X + gaugeTexture.Width - 70, position.Y + gaugeTexture.Height - 70), Color.Red);
            else if (currentGear == 0)
                spriteBatch.DrawString(currentFont, "N", new Vector2(position.X + gaugeTexture.Width - 70, position.Y + gaugeTexture.Height - 70), Color.Red);
            else
                spriteBatch.DrawString(currentFont, currentGear.ToString(), new Vector2(position.X + gaugeTexture.Width - 70, position.Y + gaugeTexture.Height - 70), Color.Red);

            spriteBatch.End();
        }
    }
}
