/*
 * This class represents the shop marker menu
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
    class ShopMenu : GameScreen
    {
        //Game state
        private GameState nextState;

        //getters and setters
        public GameState NextState
        {
            get { return nextState; }
        }

        //constructor
        public ShopMenu(ref GraphicsDeviceManager graphics, Game game, ref SpriteBatch spriteBatch, ref SpriteFont spriteFont, ref String[] menuItems, ref InputHandler newInput, ref Texture2D newBackground, ref GameStateManager currentState, String newMenuTitle, ref GameTime activated)
            : base(ref graphics, game, ref spriteBatch, ref spriteFont, ref menuItems, ref newInput, ref newBackground, ref currentState, ref activated)
        {
            menuActive = false;
            menuTitle = newMenuTitle;

            input.reset();
        }

        //handeling input and changing game state
        public override void Update(GameTime gameTime)
        {
            if (input.SelectInMenu)
                if (gameTime.TotalGameTime - previousControllerButtonCooldown > (controllerButtonCooldown + transitionOnTime))
                {
                    if (selectedIndex == 0)
                    {
                        nextState = GameState.InShop;
                        gameStateChanged = true;
                        input.reset();
                    }

                    else
                    {
                        nextState = GameState.InGame;
                        gameStateChanged = true;
                        input.reset();
                    }
                }

            base.Update(gameTime);
        }

        // draw the menu
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(menuBackground, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);

            base.Draw(gameTime);
        }
    }
}
