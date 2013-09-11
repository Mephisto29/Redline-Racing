/*
 * This class represents the in multiplayer menu
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
    class MultiPlayerMenu : GameScreen
    {
        //menu background
        protected Texture2D background;

        protected OptionsManager optionsM;

        //Game state
        protected GameState nextState;
        protected Race currentRace;
        protected Player currentplayer;

        //virtualkeyboard state
        protected IAsyncResult KeyboardResult;

        //protected data members
        protected string[] gears = new string[] { "Automatic", "Manual" };
        protected string[] maps = new string[] { "Alpine", "Mountain", "Plains" };
        protected string[] cars = new string[] { "Ford Mustang GT500", "Koenigsegg CCX", "Mercedes SLR Mclaren" };

        protected string car1;
        protected string car2;
        protected string player1Name;
        protected string player2Name;
        protected string map;

        protected int gearIndex1;
        protected int gearIndex2;
        protected int mapIndex;
        protected int carIndex1;
        protected int carIndex2;

        protected bool gearBox1;
        protected bool gearBox2;

        protected bool finsihedWithKeyboard = true;

        //select data
        protected TimeSpan previousSelectTime;
        protected TimeSpan selectCooldown = TimeSpan.FromSeconds(0.2f);

        //getters and setters
        public GameState NextState
        {
            get { return nextState; }
        }
        public string Map
        {
            get { return map; }
        }
        public string Car1
        {
            get { return car1; }
        }
        public string Car2
        {
            get { return car2; }
        }
        public string Player1
        {
            get { return player1Name; }
        }
        public string Player2
        {
            get { return player2Name; }
        }
        public bool GearBox1
        {
            get { return gearBox1; }
        }
        public bool GearBox2
        {
            get { return gearBox2; }
        }

        //constructor
        public MultiPlayerMenu(ref GraphicsDeviceManager graphics, Game game, ref SpriteBatch spriteBatch, ref SpriteFont spriteFont, ref String[] menuItems, ref InputHandler newInput, ref Texture2D newBackground, ref GameStateManager currentState, String newMenuTitle, ref Race newRace, ref Player newcurrentplayer, ref GameTime activated, ref OptionsManager optionsManager)
            : base(ref graphics, game, ref spriteBatch, ref spriteFont, ref menuItems, ref newInput, ref newBackground, ref currentState, ref activated)
        {

            optionsM = optionsManager;

            mapIndex = 0;
            carIndex1 = 0;
            carIndex2 = 0;

            car1 = cars[carIndex1];
            car2 = cars[carIndex2];

            player1Name = "unnamedPlayer1";
            player2Name = "unnamedPlayer2";

            map = maps[mapIndex];

            gearBox1 = optionsM.GearBox1;
            gearIndex1 = optionsM.GearIndex1;

            gearBox2 = optionsM.GearBox2;
            gearIndex2 = optionsM.GearIndex2;

            currentplayer = newcurrentplayer;
            background = newBackground;
            currentRace = newRace;
            menuTitle = newMenuTitle;
        }

        //update used for input handling and changing game states
        public override void Update(GameTime gameTime)
        {
            if (finsihedWithKeyboard)
            {
                if (selectedIndex == 3 && input.MoveDownInMenu)
                    selectedIndex = 4;
                if (selectedIndex == 7 && input.MoveDownInMenu)
                    selectedIndex = 8;
                if (selectedIndex == 9 && input.MoveDownInMenu)
                    selectedIndex = 10;
                if (selectedIndex == 11 && input.MoveDownInMenu)
                    selectedIndex = 12;

                if (selectedIndex == 3 && input.MoveUpInMenu)
                    selectedIndex = 2;
                if (selectedIndex == 7 && input.MoveUpInMenu)
                    selectedIndex = 6;
                if (selectedIndex == 9 && input.MoveUpInMenu)
                    selectedIndex = 8;
                if (selectedIndex == 11 && input.MoveUpInMenu)
                    selectedIndex = 10;

                if (selectedIndex == 0)
                {
                    if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                    {
                        previousSelectTime = gameTime.TotalGameTime;
                    }
                }

                if (selectedIndex == 0 && input.SelectInMenu)
                {
                    if (!Guide.IsVisible)
                        KeyboardResult = Guide.BeginShowKeyboardInput(0, "Player 1 Name", "Enter your name", player1Name, null, null);

                    finsihedWithKeyboard = false;
                }

                else if (selectedIndex == 4 && input.SelectInMenu)
                {
                    if (!Guide.IsVisible)
                        KeyboardResult = Guide.BeginShowKeyboardInput(0, "Player 1 Name", "Enter your name", player2Name, null, null);

                    finsihedWithKeyboard = false;
                }

                // Player 1
                // Car
                else if (selectedIndex == 1)
                {
                    if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                    {
                        previousSelectTime = gameTime.TotalGameTime;

                        if (input.RightInMenu)
                        {
                            carIndex1++;
                            if (carIndex1 > cars.Length - 1)
                            {
                                carIndex1 = 0;
                            }
                            car1 = cars[carIndex1];
                        }

                        if (input.LeftInMenu)
                        {
                            carIndex1--;
                            if (carIndex1 < 0)
                            {
                                carIndex1 = cars.Length - 1;
                            }
                            car1 = cars[carIndex1];
                        }
                    }
                }

                // gearBox Automatic = false, Manual = true
                else if (selectedIndex == 2)
                {
                    if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                    {
                        previousSelectTime = gameTime.TotalGameTime;

                        // automatic
                        if (input.RightInMenu)
                        {
                            gearBox1 = false;
                            gearIndex1 =0 ;
                            optionsM.GearBox1 = false;
                            optionsM.GearIndex1 = gearIndex1;
                        }

                        // manual
                        if (input.LeftInMenu)
                        {
                            gearBox1 = true;
                            gearIndex1 = 1;
                            optionsM.GearBox1 = true;
                            optionsM.GearIndex1 = gearIndex1;
                        }
                    }
                }

                // Player 2

                // Car
                else if (selectedIndex == 5)
                {
                    if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                    {
                        previousSelectTime = gameTime.TotalGameTime;

                        if (input.RightInMenu)
                        {
                            carIndex2++;
                            if (carIndex2 > cars.Length - 1)
                            {
                                carIndex2 = 0;
                            }
                            car2 = cars[carIndex2];
                        }

                        if (input.LeftInMenu)
                        {
                            carIndex2--;
                            if (carIndex2 < 0)
                            {
                                carIndex2 = cars.Length - 1;
                            }
                            car2 = cars[carIndex2];
                        }
                    }
                }

                // gearBox Automatic = false, Manual = true
                else if (selectedIndex == 6)
                {
                    if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                    {
                        previousSelectTime = gameTime.TotalGameTime;

                        // automatic
                        if (input.RightInMenu)
                        {
                            gearBox2 = false;
                            gearIndex2=0;
                            optionsM.GearBox2 = false;
                            optionsM.GearIndex2 = gearIndex2;
                        }

                        // manual
                        if (input.LeftInMenu)
                        {
                            gearBox2 = true;
                            gearIndex2= 1;
                            optionsM.GearBox2 = true;
                            optionsM.GearIndex2 = gearIndex2;
                        }
                    }
                }

                // Maps
                else if (selectedIndex == 8)
                {
                    if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                    {
                        previousSelectTime = gameTime.TotalGameTime;

                        // next map
                        if (input.RightInMenu)
                        {
                            mapIndex++;
                            if (mapIndex > maps.Length - 1)
                            {
                                mapIndex = 0;
                            }
                        }

                        // previous map
                        if (input.LeftInMenu)
                        {
                            mapIndex--;
                            if (mapIndex < 0)
                            {
                                mapIndex = maps.Length - 1;
                            }
                        }
                    }
                    map = maps[mapIndex];
                }

                else if (input.SelectInMenu)
                {
                    // start race
                    if (gameTime.TotalGameTime - previousControllerButtonCooldown > (controllerButtonCooldown + transitionOnTime))
                    {
                        if (selectedIndex == 10)
                        {
                            nextState = GameState.MultiPlayer;
                            gameStateChanged = true;
                            input.reset();
                        }
                        input.reset();
                    }

                    // Back
                    if (gameTime.TotalGameTime - previousControllerButtonCooldown > (controllerButtonCooldown + transitionOnTime))
                    {
                        if (selectedIndex == 12)
                        {
                            nextState = GameState.InGameMenu;
                            gameStateChanged = true;
                            optionsM.GearBox1 = false;
                            optionsM.GearIndex1 = 0;
                            optionsM.GearBox2 = false;
                            optionsM.GearIndex2 = 0;
                            input.reset();
                        }
                    }
                }

                base.Update(gameTime);
            }

            else
            {
                if (KeyboardResult.IsCompleted)
                {
                    if (KeyboardResult.IsCompleted)
                    {
                        if (selectedIndex == 0)
                        {
                            player1Name = Guide.EndShowKeyboardInput(KeyboardResult);
                            if (player1Name == null)
                                player1Name = "unnamedPlayer1";
                        }
                        else
                        {
                            player2Name = Guide.EndShowKeyboardInput(KeyboardResult);
                            if (player2Name == null)
                                player2Name = "unnamedPlayer1";
                        }
                        finsihedWithKeyboard = true;
                    }
                }
            }
        }

        //draw the menu
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            spriteBatch.DrawString(spriteFont, player1Name, new Vector2(600, 75), Color.Red);
            spriteBatch.DrawString(spriteFont, cars[carIndex1], new Vector2(600, 125), Color.Red);
            spriteBatch.DrawString(spriteFont, gears[gearIndex1], new Vector2(600, 175), Color.Red);

            spriteBatch.DrawString(spriteFont, player2Name, new Vector2(600, 275), Color.Red);
            spriteBatch.DrawString(spriteFont, cars[carIndex2], new Vector2(600, 325), Color.Red);
            spriteBatch.DrawString(spriteFont, gears[gearIndex2], new Vector2(600, 375), Color.Red);

            spriteBatch.DrawString(spriteFont, maps[mapIndex], new Vector2(600, 475), Color.Red);

            base.Draw(gameTime);
        }
    }
}
