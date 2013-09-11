/*
 * This class is used to handle all game input
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

namespace RacingGame.Engine
{
    class InputHandler
    {
        //private data members
        private GameStateManager currentGameState;

        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        private GamePadState pad;

        private TimeSpan previousSelectTime;
        private TimeSpan selectCooldown;
        private TimeSpan musicSelectCooldown;

        //all booleans are set as keys are pressed and these booleans are used for key input
        private bool accelerate;
        private bool decelerate;
        private bool turnLeft;
        private bool turnRight;
        private bool handbrake;
        private bool gearUp;
        private bool gearDown;

        private bool nosActivate;

        private bool menuUp;
        private bool menuDown;
        private bool menuSelect;
        private bool menuLeft;
        private bool menuRight;
        private bool menuExit;

        private bool escape;

        private bool musicPrevious;
        private bool musicNext;

        private bool mustReset;

        private bool inMultiRace;

        private int player;
        private int tempPlayer;

        //getters and setters
        public bool Escape
        {
            get { return escape; }
        }

        public bool NosActivate
        {
            get { return nosActivate; }
        }

        public bool InMultiRace
        {
            get { return inMultiRace; }
            set { inMultiRace = value; }
        }

        public int Player
        {
            get { return player; }
            set { player = value; }
        }

        public bool IsAccelerating
        {
            get { return accelerate; }
            set { accelerate = value; }
        }
        public bool IsDecelerating
        {
            get { return decelerate; }
            set { decelerate = value; }
        }

        public bool IsTurningLeft
        {
            get { return turnLeft; }
            set { turnLeft = value; }
        }

        public bool IsTurningRight
        {
            get { return turnRight; }
            set { turnRight = value; }
        }

        public bool HandBrakeEngaged
        {
            get { return handbrake; }
            set { handbrake = value; }
        }

        public bool GearUp
        {
            get { return gearUp; }
        }

        public bool GearDown
        {
            get { return gearDown; }
        }

        public bool MoveUpInMenu
        {
            get { return menuUp; }
        }

        public bool MoveDownInMenu
        {
            get { return menuDown; }
        }

        public bool SelectInMenu
        {
            get { return menuSelect; }
        }

        public bool LeftInMenu
        {
            get { return menuLeft; }
        }

        public bool RightInMenu
        {
            get { return menuRight; }
        }

        public bool ExitMenu
        {
            get { return menuExit; }
        }
        public bool MustReset
        {
            get { return mustReset; }
            set { mustReset = value; }
        }

        public bool MusicNext
        {
            get { return musicNext; }
        }

        public bool MusicPrevious
        {
            get { return musicPrevious; }
        }

        //reset all booleans
        public void reset()
        {
            accelerate = false;
            decelerate = false;
            turnLeft = false;
            turnRight = false;
            handbrake = false;
            gearUp = false;
            gearDown = false;

            menuUp = false;
            menuDown = false;
            menuSelect = false;
            menuLeft = false;
            menuRight = false;
            menuExit = false;

            nosActivate = false;

            escape = false;
            mustReset = false;
        }
        //end of getters and setters

        //constructor
        public InputHandler(GameStateManager currentState)
        {
            currentGameState = currentState;
            selectCooldown = TimeSpan.FromSeconds(0.1f);
            musicSelectCooldown = TimeSpan.FromSeconds(0.1f);
        }

        //update
        public void update(KeyboardState currentState, GameTime gameTime)
        {
            //keyboard state
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = currentState;

            //current player
            if (player == 0)
                pad = GamePad.GetState(PlayerIndex.One);

            else if (player == 1)
                pad = GamePad.GetState(PlayerIndex.Two);


            //handle inmenu controls
            if (currentGameState.CurrentState == GameState.MainMenu || currentGameState.CurrentState == GameState.InGameMenu)
            {
                if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                {
                    tempPlayer = player;
                    player = 0;

                    if (player == 0)
                    {
                        if (currentKeyboardState.IsKeyDown(Keys.Up) || (pad.ThumbSticks.Left.Y >= 0.5f))
                        {
                            menuUp = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.Up) || (pad.ThumbSticks.Left.Y == 0))
                            menuUp = false;

                        if (currentKeyboardState.IsKeyDown(Keys.Down) || (pad.ThumbSticks.Left.Y < -0.5f))
                        {
                            menuDown = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.Down) || (pad.ThumbSticks.Left.Y == 0))
                            menuDown = false;

                        if (currentKeyboardState.IsKeyDown(Keys.Right) || (pad.ThumbSticks.Left.X > 0.5f))
                        {
                            menuRight = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.Right) || (pad.ThumbSticks.Left.X == 0))
                            menuRight = false;

                        if (currentKeyboardState.IsKeyDown(Keys.Left) || (pad.ThumbSticks.Left.X < -0.5f))
                        {
                            menuLeft = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.Left) || (pad.ThumbSticks.Left.X == 0))
                            menuLeft = false;

                        if (currentKeyboardState.IsKeyDown(Keys.Enter) || pad.IsButtonDown(Buttons.A))
                        {
                            menuSelect = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.Enter) || pad.IsButtonUp(Buttons.A))
                            menuSelect = false;

                        if (currentKeyboardState.IsKeyDown(Keys.Escape) || pad.IsButtonDown(Buttons.B) || pad.IsButtonDown(Buttons.Start))
                        {
                            escape = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.Escape) || pad.IsButtonUp(Buttons.B) || pad.IsButtonUp(Buttons.Start))
                            escape = false;
                    }
                    
                    player = tempPlayer;
                }
            }

            //handle ingame controls
            else if (currentGameState.CurrentState == GameState.InGame || currentGameState.CurrentState == GameState.Racing)
            {

                if (player == 0)
                {
                    if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                    {
                        //nos
                        if (currentKeyboardState.IsKeyDown(Keys.RightControl) || pad.IsButtonDown(Buttons.LeftShoulder))
                        {
                            nosActivate = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }
                        else if (currentKeyboardState.IsKeyUp(Keys.RightControl) || pad.IsButtonDown(Buttons.LeftShoulder))
                            nosActivate = false;

                        //accelerate
                        if (currentKeyboardState.IsKeyDown(Keys.W) || pad.Triggers.Right > 0.1f)
                        {
                            accelerate = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.W) || pad.Triggers.Right < 0.1f)
                            accelerate = false;

                        //decelerate
                        if (currentKeyboardState.IsKeyDown(Keys.S) || pad.Triggers.Left > 0.1f)
                        {
                            decelerate = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.S) || pad.Triggers.Left < 0.1f)
                            decelerate = false;

                        //turn left
                        if (currentKeyboardState.IsKeyDown(Keys.A) || (pad.ThumbSticks.Left.X < -0.1f))
                        {
                            turnLeft = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.A) || (pad.ThumbSticks.Left.X == 0f))
                            turnLeft = false;

                        //turn right
                        if (currentKeyboardState.IsKeyDown(Keys.D) || (pad.ThumbSticks.Left.X > 0.1f))
                        {
                            turnRight = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.D) || (pad.ThumbSticks.Left.X == 0f))
                            turnRight = false;

                        //handbrake
                        if (currentKeyboardState.IsKeyDown(Keys.Space) || pad.IsButtonDown(Buttons.B))
                        {
                            handbrake = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.Space) || pad.IsButtonDown(Buttons.B))
                            handbrake = false;

                        //gearUp
                        if (currentKeyboardState.IsKeyDown(Keys.LeftShift) || pad.IsButtonDown(Buttons.Y))
                        {
                            gearUp = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.LeftShift) || pad.IsButtonDown(Buttons.Y))
                            gearUp = false;

                        //gearDown
                        if (currentKeyboardState.IsKeyDown(Keys.RightShift) || pad.IsButtonDown(Buttons.X))
                        {
                            gearDown = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.RightShift) || pad.IsButtonDown(Buttons.X))
                            gearDown = false;

                        if (currentKeyboardState.IsKeyDown(Keys.R) || pad.IsButtonDown(Buttons.Back))
                        {
                            mustReset = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.R) || pad.IsButtonDown(Buttons.Back))
                            mustReset = false;

                        if (currentKeyboardState.IsKeyDown(Keys.Escape) || pad.IsButtonDown(Buttons.Start))
                        {
                            escape = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.Escape) || pad.IsButtonDown(Buttons.Start))
                            escape = false;
                    }

                    if (currentKeyboardState.IsKeyDown(Keys.NumPad1) || pad.IsButtonDown(Buttons.DPadLeft))
                    {
                        musicPrevious = true;
                        previousSelectTime = gameTime.TotalGameTime;
                    }

                    else if (currentKeyboardState.IsKeyUp(Keys.NumPad1) || pad.IsButtonDown(Buttons.DPadLeft))
                        musicPrevious = false;

                    if (currentKeyboardState.IsKeyDown(Keys.NumPad2) || pad.IsButtonDown(Buttons.DPadRight))
                    {
                        musicNext = true;
                        previousSelectTime = gameTime.TotalGameTime;
                    }

                    else if (currentKeyboardState.IsKeyUp(Keys.NumPad2) || pad.IsButtonDown(Buttons.DPadRight))
                        musicNext = false;
                }


                if (player == 1)
                {
                    if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                    {
                        //nos
                        if (currentKeyboardState.IsKeyDown(Keys.NumPad0) || pad.IsButtonDown(Buttons.LeftShoulder))
                        {
                            nosActivate = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }
                        else if (currentKeyboardState.IsKeyUp(Keys.NumPad0) || pad.IsButtonUp(Buttons.LeftShoulder))
                            nosActivate = false;

                        //accelerate
                        if (currentKeyboardState.IsKeyDown(Keys.Up) || pad.Triggers.Right > 0.1f)
                        {
                            accelerate = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.Up) || pad.Triggers.Right < 0.1f)
                            accelerate = false;

                        //decelerate
                        if (currentKeyboardState.IsKeyDown(Keys.Down) || pad.Triggers.Left > 0.1f)
                        {
                            decelerate = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.Down) || pad.Triggers.Left < 0.1f)
                            decelerate = false;

                        //turn left
                        if (currentKeyboardState.IsKeyDown(Keys.Right) || (pad.ThumbSticks.Left.X > 0.5f))
                        {
                            turnRight = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.Right) || (pad.ThumbSticks.Left.X == 0))
                            turnRight = false;

                        // turn right
                        if (currentKeyboardState.IsKeyDown(Keys.Left) || (pad.ThumbSticks.Left.X < -0.5f))
                        {
                            turnLeft = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.Left) || (pad.ThumbSticks.Left.X == 0))
                            turnLeft = false;

                        //handbrake
                        if (currentKeyboardState.IsKeyDown(Keys.RightShift) || pad.IsButtonDown(Buttons.B))
                        {
                            handbrake = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.RightShift) || pad.IsButtonDown(Buttons.B))
                            handbrake = false;

                        //gearUp
                        if (currentKeyboardState.IsKeyDown(Keys.NumPad0) || pad.IsButtonDown(Buttons.Y))
                        {
                            gearUp = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.NumPad0) || pad.IsButtonDown(Buttons.Y))
                            gearUp = false;

                        //gearDown
                        if (currentKeyboardState.IsKeyDown(Keys.NumPad1) || pad.IsButtonDown(Buttons.X))
                        {
                            gearDown = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.NumPad1) || pad.IsButtonDown(Buttons.X))
                            gearDown = false;

                        if (currentKeyboardState.IsKeyDown(Keys.R) || pad.IsButtonDown(Buttons.Back))
                        {
                            mustReset = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.R) || pad.IsButtonDown(Buttons.Back))
                            mustReset = false;

                        if (currentKeyboardState.IsKeyDown(Keys.Escape) || pad.IsButtonDown(Buttons.Start))
                        {
                            escape = true;
                            previousSelectTime = gameTime.TotalGameTime;
                        }

                        else if (currentKeyboardState.IsKeyUp(Keys.Escape) || pad.IsButtonUp(Buttons.Start))
                            escape = false;
                    }
                }

                
            }

            else if (currentGameState.CurrentState == GameState.InShop)
            {
                if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                {
                    if (currentKeyboardState.IsKeyDown(Keys.Left) || (pad.ThumbSticks.Left.X < -0.1f))
                    {
                        menuLeft = true;
                        previousSelectTime = gameTime.TotalGameTime;
                    }

                    else if (currentKeyboardState.IsKeyUp(Keys.Left) || (pad.ThumbSticks.Left.X == 0))
                        menuLeft = false;

                    if (currentKeyboardState.IsKeyDown(Keys.Right) || (pad.ThumbSticks.Left.X > 0.1f))
                    {
                        menuRight = true;
                        previousSelectTime = gameTime.TotalGameTime;
                    }

                    else if (currentKeyboardState.IsKeyUp(Keys.Right) || (pad.ThumbSticks.Left.X == 0))
                        menuRight = false;

                    if (currentKeyboardState.IsKeyDown(Keys.Enter) || pad.IsButtonDown(Buttons.A))
                    {
                        menuSelect = true;
                        previousSelectTime = gameTime.TotalGameTime;
                    }

                    else if (currentKeyboardState.IsKeyUp(Keys.Enter) || pad.IsButtonDown(Buttons.A))
                        menuSelect = false;

                    if (currentKeyboardState.IsKeyDown(Keys.Escape) || pad.IsButtonDown(Buttons.Start))
                    {
                        menuExit = true;
                        previousSelectTime = gameTime.TotalGameTime;
                    }

                    else if (currentKeyboardState.IsKeyUp(Keys.Escape) || pad.IsButtonDown(Buttons.Start))
                        menuExit = false;
                }
            }
        }
    }
}