/*
 * This class represents the free roam menu
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
using RacingGame.Enviroment;
using RacingGame.PlayerData;

namespace RacingGame.GameMenus
{
    class FreeRoamMenu : GameScreen
    {
        //menu background
        private Texture2D background;

        //Game state
        private GameState nextState;
        private Race currentRace;
        private Player currentplayer;

        //Random variables
        private bool optionsDelay = true;

        //getters and setters
        public GameState NextState
        {
            get { return nextState; }
        }

        //constructor
        public FreeRoamMenu(ref GraphicsDeviceManager graphics, Game game, ref SpriteBatch spriteBatch, ref SpriteFont spriteFont, ref String[] menuItems, ref InputHandler newInput, ref Texture2D newBackground, ref GameStateManager currentState, String newMenuTitle, ref Race newRace, ref Player newcurrentplayer, ref GameTime activated)
            : base(ref graphics, game, ref spriteBatch, ref spriteFont, ref menuItems, ref newInput, ref newBackground, ref currentState, ref activated)
        {
            currentplayer = newcurrentplayer;
            background = newBackground;
            currentRace = newRace;
            menuTitle = newMenuTitle;
            menuActive = false;
        }

        //update used for input handling of the menu
        public override void Update(GameTime gameTime)
        {
            if (input.SelectInMenu)
            {
                if (gameTime.TotalGameTime - previousControllerButtonCooldown > (controllerButtonCooldown + transitionOnTime))
                {
                    // continue
                    if (selectedIndex == 0)
                    {
                        nextState = GameState.InGame;
                        gameStateChanged = true;
                        input.reset();
                    }
                    // options
                    else if (selectedIndex == 1)
                    {
                        if (optionsDelay)
                        {
                            nextState = GameState.InGameMenu;
                            gameStateChanged = true;
                            optionsDelay = false;
                            previousControllerButtonCooldown = gameTime.TotalGameTime;
                        }
                        else
                        {
                            optionsDelay = true;
                            previousControllerButtonCooldown = gameTime.TotalGameTime;
                        }
                    }
                    // main menu
                    else if (selectedIndex == 2)
                    {
                        nextState = GameState.MainMenu;
                        gameStateChanged = true;
                        input.reset();
                    }
                }
            }

            base.Update(gameTime);
        }

        //draw the menu
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(background, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 0.9f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(spriteFont, "Cash:", new Vector2(250, 400), Color.Red);
            spriteBatch.DrawString(spriteFont, currentplayer.CurrentCash.ToString(), new Vector2(400, 400), Color.Red);

            base.Draw(gameTime);
        }
    }
}
