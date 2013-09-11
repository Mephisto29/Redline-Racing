/*
 * This class represents the race marker menu
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
    class RaceMarkerMenu : GameScreen
    {
        //private data members
        private String MapInformation;
        private Texture2D currentMap;

        //Game state
        private GameState nextState;

        //select data
        private TimeSpan previousSelectTime;
        private TimeSpan selectCooldown = TimeSpan.FromSeconds(0.2f);

        //winning data
        float bet = 500f;
        float maxBet;

        //getters and setters
        public GameState NextState
        {
            get { return nextState; }
        }

        public Texture2D RaceMap
        {
            get { return currentMap; }
            set { currentMap = value; }
        }

        public float Bet
        {
            get { return bet; }
            set { bet = value; }
        }
        public float MaxBet
        {
            get { return maxBet; }
            set { maxBet = value; }
        }

        //constructor
        public RaceMarkerMenu(ref GraphicsDeviceManager graphics, Game game, ref SpriteBatch spriteBatch, ref SpriteFont spriteFont, ref String[] menuItems, ref InputHandler newInput, ref Texture2D newBackground, String mapInformation, ref GameStateManager currentState, String newMenuTitle, ref GameTime activated)
            : base(ref graphics, game, ref spriteBatch, ref spriteFont, ref menuItems, ref newInput, ref newBackground, ref currentState, ref activated)
        {
            MapInformation = mapInformation;

            menuTitle = newMenuTitle;
            menuActive = false;

            input.reset();
        }

        //update method, here it  makes it possitble for you to change the bet size and to change game state
        public override void Update(GameTime gameTime)
        {
            if (bet > maxBet)
                bet = maxBet;

            if (input.SelectInMenu)
            {
                if (gameTime.TotalGameTime - previousControllerButtonCooldown > (controllerButtonCooldown + transitionOnTime))
                {
                    if (selectedIndex == 0)
                    {
                        nextState = GameState.Racing;
                        gameStateChanged = true;
                        input.reset();
                    }

                    else if (selectedIndex == 2)
                    {
                        nextState = GameState.InGame;
                        gameStateChanged = true;
                        input.reset();
                    }
                }
            }
            
            if (selectedIndex == 1)
            {
                if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                {
                   previousSelectTime = gameTime.TotalGameTime;

                   if (input.RightInMenu)
                   {
                       if (bet + 100 <= maxBet)
                           bet += 100f;
                   }
                   if (input.LeftInMenu)
                   {
                       if (bet - 100 >= 0f)
                           bet -= 100f;
                   }
                }
            }

            base.Update(gameTime);
        }

        //draw the menu
        public override void Draw(GameTime gameTime)
        {
            Color white = new Color(1.0f,1.0f,1.0f, AlphaChange);
            Color red = new Color(1.0f, 0.0f, 0.0f, AlphaChange);
            Color black = new Color(0.0f, 0.0f, 0.0f, AlphaChange);

            spriteBatch.Draw(menuBackground, Vector2.Zero, null, white, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(currentMap, new Vector2(300, 50), null, white, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(spriteFont, MapInformation, new Vector2(300, 50 + (currentMap.Height / 2)), black);
            spriteBatch.DrawString(spriteFont, bet.ToString(), new Vector2(150, 120), white);

            spriteBatch.DrawString(spriteFont, "Current winnings: R " + (bet * 4).ToString(), new Vector2(300, 600), black);
            spriteBatch.DrawString(spriteFont, "*Note: Only first place receives winnings...", new Vector2(300, 640), red);
            base.Draw(gameTime);
        }
    }
}
