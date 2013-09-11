/*
 * This class represents the multiplayer race finish menu
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

namespace RacingGame.GameMenus
{
    class MultiplayerRaceFinishMenu : GameScreen
    {
        //menu background
        private Texture2D background;

        //Game state
        private GameState nextState;
        private Race currentRace;

        //race finish time of player
        private string time;

        //getters and setters
        public GameState NextState
        {
            get { return nextState; }
        }

        public float GetBet
        {
            get { return currentRace.Winnings / 4; }
        }

        public string FinishTime
        {
            set { time = value; }
        }

        //constructor
        public MultiplayerRaceFinishMenu(ref GraphicsDeviceManager graphics, Game game, ref SpriteBatch spriteBatch, ref SpriteFont spriteFont, ref String[] menuItems, ref InputHandler newInput, ref Texture2D newBackground, ref GameStateManager currentState, String newMenuTitle, ref Race newRace, ref GameTime activated)
            : base(ref graphics, game, ref spriteBatch, ref spriteFont, ref menuItems, ref newInput, ref newBackground, ref currentState, ref activated)
        {

            background = newBackground;
            currentRace = newRace;
            menuTitle = newMenuTitle;
            menuActive = false;
        }

        //update method used for input handling and changinf game state
        public override void Update(GameTime gameTime)
        {
            if (input.SelectInMenu)
            {
                // Continue
                if (gameTime.TotalGameTime - previousControllerButtonCooldown > (controllerButtonCooldown + transitionOnTime))
                {
                    if (selectedIndex == 0)
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
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            spriteBatch.DrawString(spriteFont, "Winner:", new Vector2(350, 200), Color.Red);
            spriteBatch.DrawString(spriteFont, currentRace.Leader.Name.ToString(), new Vector2(350, 250), Color.White);
            spriteBatch.DrawString(spriteFont, "Time: " + time, new Vector2(200, 350), Color.White);

            base.Draw(gameTime);
        }
    }
}
