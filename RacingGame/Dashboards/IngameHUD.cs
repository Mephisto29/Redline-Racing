/*
 * This class is used to draw the ingame HUD, i.e. the cash and current level
 * 
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

using RacingGame.Enviroment;
using RacingGame.PlayerData;

namespace RacingGame.Dashboards
{
    class IngameHUD
    {
        //private data members
        private Player currentPlayer;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D ingameHUD;
        private SpriteFont font;

        //constructor
        public IngameHUD(ref GraphicsDeviceManager newGraphics, ref SpriteBatch newSpriteBatch, ref Player player, ref Texture2D newIngameHUD, ref SpriteFont newFont)
        {
            currentPlayer = player;
            graphics = newGraphics;
            spriteBatch = newSpriteBatch;
            ingameHUD = newIngameHUD;
            font = newFont;
        }

        //draw method
        public void Draw()
        {
            spriteBatch.Draw(ingameHUD, new Vector2(graphics.GraphicsDevice.Viewport.Width - ingameHUD.Width, 0), Color.White);
            spriteBatch.DrawString(font, currentPlayer.CurrentCash.ToString(), new Vector2(graphics.GraphicsDevice.Viewport.Width - 330, 20), Color.Red, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(font, "Level: "+currentPlayer.CurrentLevel.ToString(), new Vector2(graphics.GraphicsDevice.Viewport.Width - 310, 50), Color.Red, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}
