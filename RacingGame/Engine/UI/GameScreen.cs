/*
 * This class is used for the game screens, this is the super class the manages the transitions as well as maintaining the array of menu strings
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
using RacingGame.Engine.Physics;

namespace RacingGame.Engine.UI
{
    //current screen state
    public enum ScreenState
    {
        Active,
        Deactivated,
        TransitionOn,
        TransitionOff
    }

    abstract class GameScreen : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //current game state
        protected GameStateManager gameState;
        protected Game game;

        //menu items
        protected String[] menuItems;
        protected String menuTitle;
        //currently selected index
        protected int selectedIndex;

        //colors of the text
        protected Color normal = Color.White;
        protected Color selected = Color.Red;
        //Texture2D selectedItem;
        protected Texture2D menuBackground;

        //keyboard states
        protected InputHandler input;

        protected SpriteBatch spriteBatch;
        protected SpriteFont spriteFont;

        //position of the text
        protected Vector2 position;
        protected float scale = 0f;

        protected TimeSpan controllerButtonCooldown;
        protected TimeSpan previousControllerButtonCooldown;

        //menu is active or not
        protected bool menuActive;
        protected bool gameStateChanged = false;

        //transition data
        protected ScreenState screenState = ScreenState.TransitionOn;
        protected TimeSpan transitionOnTime = TimeSpan.FromSeconds(0.5f);
        protected TimeSpan transitionOffTime = TimeSpan.FromSeconds(0.5f);
        protected float transitionPosition = 1;
        protected float selectionFade;

        //getters and setters
        public ScreenState ScreenState
        {
            get { return screenState; }
            set { screenState = value; }
        }

        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            set { transitionOnTime = value; }
        }

        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            set { transitionOffTime = value; }
        }

        public float TransitionPosition
        {
            get { return transitionPosition; }
            set { transitionPosition = value; }
        }

        public bool Status
        {
            get { return menuActive; }
            set { menuActive = value; }
        }

        public bool GameStateChanged
        {
            get { return gameStateChanged; }
            set { gameStateChanged = value; }
        }

        public byte AlphaChange
        {
            get { return (byte)(255 - TransitionPosition * 255); }
        }

        //method to change and get the selected index
        public int SelectedIndex
        {
            get { return selectedIndex; }

            set
            {
                selectedIndex = value;
                if (selectedIndex < 0)
                    selectedIndex = 0;
                if (selectedIndex >= menuItems.Length)
                    selectedIndex = menuItems.Length - 1;
            }
        }

        public TimeSpan PreviousControllerButtonCooldown
        {
            get {return previousControllerButtonCooldown; }

            set {previousControllerButtonCooldown = value;}
        }

        //set the information of the screen
        public GameScreen(ref GraphicsDeviceManager graphics, Game game, ref SpriteBatch spriteBatch, ref SpriteFont spriteFont, ref String[] menuItems, ref InputHandler newInput, ref Texture2D newBackground, ref GameStateManager currentState, ref GameTime timeActivated)
            : base(game)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;
            this.menuItems = menuItems;
            this.menuBackground = newBackground;
            this.gameState = currentState;
            this.previousControllerButtonCooldown = timeActivated.TotalGameTime;

            scale = graphics.GraphicsDevice.Viewport.Width / newBackground.Width;
            input = newInput;
            controllerButtonCooldown = TimeSpan.FromSeconds(0.3);
            position = new Vector2(10,10);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        //update the menu
        public override void Update(GameTime gameTime)
        {
            if (screenState == ScreenState.Active)
            {
                input.update(Keyboard.GetState(), gameTime);

                //check key presses for up or down
                if (input.MoveDownInMenu)
                {
                    if (gameTime.TotalGameTime - previousControllerButtonCooldown > controllerButtonCooldown)
                    {
                        previousControllerButtonCooldown = gameTime.TotalGameTime;
                        selectedIndex++;
                        if (selectedIndex == menuItems.Length)
                            selectedIndex = 0;
                    }
                }

                else if (input.MoveUpInMenu)
                {
                    if (gameTime.TotalGameTime - previousControllerButtonCooldown > controllerButtonCooldown)
                    {
                        previousControllerButtonCooldown = gameTime.TotalGameTime;
                        selectedIndex--;
                        if (selectedIndex < 0)
                            selectedIndex = menuItems.Length - 1;
                    }
                }
            }

            else if (screenState == ScreenState.TransitionOn)
            {
                if (UpdateTransition(gameTime, transitionOnTime, -1))
                    screenState = ScreenState.TransitionOn;
                else
                    screenState = ScreenState.Active;
            }

            else if (screenState == ScreenState.TransitionOff)
            {
               if (UpdateTransition(gameTime, transitionOffTime, 1))
                   screenState = ScreenState.TransitionOff;
               else
                   screenState = ScreenState.Deactivated;
            }

            base.Update(gameTime);
        }

        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            float transitionValue;

            if (time == TimeSpan.Zero)
                transitionValue = 1;
            else
                transitionValue = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

            transitionPosition += transitionValue * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (transitionPosition <= 0)) || ((direction > 0) && (transitionPosition >= 1)))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            return true;
        }

        //draw the menu
        public override void Draw(GameTime gameTime)
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 location = position;
            spriteBatch.DrawString(spriteFont, "-- " + menuTitle + " --", location, Color.White);
            location.Y += 60.0f;
            Color tint;

            if (ScreenState == ScreenState.TransitionOn)
                location.X -= transitionOffset * 256;
            else
                location.X += transitionOffset * 512;

            for (int i = 0; i < menuItems.Length; i++)
            {
                if (i == selectedIndex)
                {
                    tint = selected;
                    tint = new Color(tint.R, tint.G, tint.B, AlphaChange);
                    //spriteBatch.Draw(selectedItem, location, Color.White);

                    // Pulsate the size of the selected menu entry.
                    float speedToFade = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;
                    selectionFade = Math.Min(0.5f + speedToFade, 1);

                    double currentTime = gameTime.TotalGameTime.TotalSeconds;
                    float pulse = (float)Math.Sin(currentTime * 6) + 1;
                    float scale = 1 + pulse * 0.05f * selectionFade;

                    //spriteBatch.DrawString(spriteFont, menuItems[i], location, tint);
                    spriteBatch.DrawString(spriteFont, menuItems[i], location, tint, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    location.Y += spriteFont.LineSpacing + 2;
                }
                else
                {
                    tint = normal; 
                    
                    spriteBatch.DrawString(spriteFont, menuItems[i], location, tint);
                    location.Y += spriteFont.LineSpacing + 2;
                }
            }

            base.Draw(gameTime);
        }
    }
}
