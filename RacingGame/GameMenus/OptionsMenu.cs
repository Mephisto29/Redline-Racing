/*
 * This class represents the options menu
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
    class OptionsMenu : GameScreen
    {
        //private data members
        private Texture2D background;
        private Texture2D controls;

        private OptionsManager optionsM;

        //Game state
        private GameState nextState;
        private Race currentRace;
        private Player currentplayer;
        private string[] gears = new string[] { "Automatic", "Manual" };
        private int gearIndex;
        private bool soundFXEnabled;
        private bool musicEnabled;
        private bool gearBox;
        private bool gearboxChanged;
        private bool controlDisplay;
        private bool drawable;
        private float musicVolume;
        private float effectsVolume;

        //select data
        private TimeSpan previousSelectTime;
        private TimeSpan selectCooldown = TimeSpan.FromSeconds(0.2f);

        //getters and setters
        public GameState NextState
        {
            get { return nextState; }
        }
        public bool SoundFXEnabled
        {
            get { return soundFXEnabled; }
        }
        public bool MusicEnabled
        {
            get { return musicEnabled; }
        }
        public bool GearBox
        {
            get { return gearBox; }
        }
        public bool Drawable
        {
            get { return drawable; }
        }
        public bool ControlDisplay
        {
            get { return controlDisplay; }
            set { controlDisplay = value; }
        }
        public bool GearBoxChanged
        {
            get { return gearboxChanged; }
            set { gearboxChanged = value;}
        }

        //constructor
        public OptionsMenu(ref GraphicsDeviceManager graphics, Game game, ref SpriteBatch spriteBatch, ref SpriteFont spriteFont, ref String[] menuItems, ref InputHandler newInput, ref Texture2D newBackground, ref GameStateManager currentState, String newMenuTitle, ref Race newRace, ref Player newcurrentplayer, ref GameTime activated, ref Texture2D controlBackground, ref OptionsManager optionsManager)
            : base(ref graphics, game, ref spriteBatch, ref spriteFont, ref menuItems, ref newInput, ref newBackground, ref currentState, ref activated)
        {

            optionsM = optionsManager;
            soundFXEnabled = optionsM.SoundFXEnabled;
            musicEnabled = optionsM.MusicEnabled;
            gearBox = optionsM.GearBox1;
            gearIndex = optionsM.GearIndex1;
            musicVolume = (optionsM.MusicVolume * 100);
            effectsVolume = (optionsM.EffectVolume * 100);

            currentplayer = newcurrentplayer;
            background = newBackground;
            currentRace = newRace;
            menuTitle = newMenuTitle;
            menuActive = false;
            controlDisplay = false;
            controls = controlBackground;
            drawable = true;

        }

        //update used for input handing, changing option values and changing game states
        public override void Update(GameTime gameTime)
        {
            if (input.SelectInMenu)
            {
                // Controls
                if (gameTime.TotalGameTime - previousControllerButtonCooldown > (controllerButtonCooldown + transitionOnTime))
                {
                    if (selectedIndex == 0)
                    {
                        controlDisplay = true;
                    }
                    input.reset();
                }

                // Back
                if (gameTime.TotalGameTime - previousControllerButtonCooldown > (controllerButtonCooldown + transitionOnTime))
                {
                    if (selectedIndex == 6)
                    {
                        if (controlDisplay == true)
                            controlDisplay = false;
                        else
                        {
                            nextState = GameState.InGameMenu;
                            gameStateChanged = true;
                        }
                        input.reset();
                    }
                }
            }

            // Controls
            if (selectedIndex == 0)
            {
                if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                {
                    previousSelectTime = gameTime.TotalGameTime;

                    if (input.RightInMenu)
                        controlDisplay = true;
                }
            }

            // Sound Effects
            if (selectedIndex == 1)
            {
                if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                {
                    previousSelectTime = gameTime.TotalGameTime;

                    if (input.RightInMenu)
                    {
                        soundFXEnabled = false;
                        optionsM.SoundFXEnabled = false;
                    }

                    if (input.LeftInMenu)
                    {
                        soundFXEnabled = true;
                        optionsM.SoundFXEnabled = true;
                    }
                }
            }

            // Music
            if (selectedIndex == 2)
            {
                if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                {
                    previousSelectTime = gameTime.TotalGameTime;

                    if (input.RightInMenu)
                    {
                        musicEnabled = false;
                        optionsM.MusicEnabled = false;
                    }

                    if (input.LeftInMenu)
                    {
                        musicEnabled = true;
                        optionsM.MusicEnabled = true;
                    }
                }
            }

            // Music Volume
            if (selectedIndex == 3)
            {
                if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                {
                    previousSelectTime = gameTime.TotalGameTime;

                    if (input.RightInMenu)
                    {
                        musicVolume += 10f;
                        if (musicVolume > 100)
                            musicVolume = 100;
                        optionsM.MusicVolume = musicVolume;
                    }

                    if (input.LeftInMenu)
                    {
                        musicVolume -= 10f;
                        if (musicVolume < 0)
                            musicVolume = 0;
                        optionsM.MusicVolume = musicVolume;
                    }
                }
            }

            // Effects Volume
            if (selectedIndex == 4)
            {
                if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                {
                    previousSelectTime = gameTime.TotalGameTime;

                    if (input.RightInMenu)
                    {
                        effectsVolume += 10f;
                        if (effectsVolume > 100)
                            effectsVolume = 100;
                        optionsM.EffectVolume = effectsVolume;
                    }

                    if (input.LeftInMenu)
                    {
                        effectsVolume -= 10f;
                        if (effectsVolume < 0)
                            effectsVolume = 0;
                        optionsM.EffectVolume = effectsVolume;
                    }
                }
            }

            // gearBox Automatic = false, Manual = true
            if (selectedIndex == 5)
            {
                if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                {
                    previousSelectTime = gameTime.TotalGameTime;

                    // automatic
                    if (input.RightInMenu)
                    {
                        gearboxChanged = true;
                        gearBox = false;
                        gearIndex = 0;
                        optionsM.GearBox1 = false;
                        optionsM.GearIndex1 = gearIndex;
                    }

                    // manual
                    if (input.LeftInMenu)
                    {
                        gearboxChanged = true;
                        gearBox = true;
                        gearIndex = 1;
                        optionsM.GearBox1 = true;
                        optionsM.GearIndex1 = gearIndex;
                    }
                }
            }

            base.Update(gameTime);
        }

        //draw the menu
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(background, Vector2.Zero, Color.White);

            //if controls are to be displayed
            if (controlDisplay)
            {
                spriteBatch.Draw(controls, new Vector2(400, 125), Color.White);
                drawable = false;

                if (input.Escape)
                {
                    drawable = true;
                    controlDisplay = false;
                    previousSelectTime = gameTime.TotalGameTime;
                }

                input.reset();
            }
            //otherwise display the menu
            else
            {
                spriteBatch.DrawString(spriteFont, soundFXEnabled.ToString(), new Vector2(400, 125), Color.Red);
                spriteBatch.DrawString(spriteFont, musicEnabled.ToString(), new Vector2(400, 175), Color.Red);
                spriteBatch.DrawString(spriteFont, musicVolume.ToString(), new Vector2(400, 225), Color.Red);
                spriteBatch.DrawString(spriteFont, effectsVolume.ToString(), new Vector2(400, 275), Color.Red);
                spriteBatch.DrawString(spriteFont, gears[gearIndex], new Vector2(400, 325), Color.Red);
            }

            base.Draw(gameTime);
        }
    }
}
