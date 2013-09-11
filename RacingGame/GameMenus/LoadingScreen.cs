/*
 * This class represents the loading screen
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

using RacingGame.Engine;
using RacingGame.Engine.UI;

namespace RacingGame.GameMenus
{
    class LoadingScreen
    {
        //protected data members
        protected GraphicsDeviceManager graphics;
        protected SpriteFont gameFont;
        protected SpriteBatch spriteBatch;
        protected bool isActive;

        //is the screen active
        public bool Active
        {
            get { return isActive; }
            set { isActive = value; }
        }

        //constructor
        public LoadingScreen(ref GraphicsDeviceManager graphics, ref SpriteFont gameFont, ref SpriteBatch spriteBatch)
        {
            this.graphics = graphics;
            this.gameFont = gameFont;
            this.spriteBatch = spriteBatch;
        }

        //draw the loading screen
        public void Draw(GameTime gameTime)
        {
            //if the menu is active draw it
            if (isActive)
            {
                graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

                const string message = "Loading...";

                // Center the text in the viewport.
                Viewport viewport = graphics.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = gameFont.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize) / 2;

                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
                spriteBatch.DrawString(gameFont, message, textPosition, Color.White);
                spriteBatch.End();
            }
        }
    }
}
