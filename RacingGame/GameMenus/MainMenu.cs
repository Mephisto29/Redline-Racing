/*
 * This class represents the main menu
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
using RacingGame.Engine.PaticleEngine;

namespace RacingGame.GameMenus
{
    class MainMenu : GameScreen
    {
        //Models
        Model car;
        Model wheel;
        Model cube;
        bool exit = false;

        //menu car
        MenuCar newCar;
        ChaseCamera camera;

        //Game state
        GameState nextState;

        //Other variables
        bool optionsDelay = true;

        //getters and setters
        public GameState NextState
        {
            get { return nextState; }
        }

        //exit menu
        public bool Exit
        {
            get { return exit; }
        }

        //constructor, takes a model which is displayed in menu
        public MainMenu(ref GraphicsDeviceManager graphics, Game game, ref SpriteBatch spriteBatch, ref SpriteFont spriteFont, ref string[] menuItems, ref InputHandler newInput, Model carModel, Model wheelModel, Model cubeModel, ref Texture2D newBackground, ref GameStateManager currentState, String newMenuTitle, ref GameTime activated)
            : base(ref graphics, game, ref spriteBatch, ref spriteFont, ref menuItems, ref newInput, ref newBackground, ref currentState, ref activated)
        {
            car = carModel;
            wheel = wheelModel;
            cube = cubeModel;
            newCar = new MenuCar(car, wheel, new Vector3(0, 0, 0));
            menuTitle = newMenuTitle;

            camera = new ChaseCamera();
            camera.desiredPositionOffset = new Vector3(0.0f, 0.5f, -1.3f);
            camera.nearPlaneDistance = 0.2f;
            camera.farPlaneDistance = 10000.0f;
            camera.aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height;
            camera.Reset();

            input.reset();

            menuActive = true;
        }

        //update the menu used for input handling, changing game state as well as the menu car rotations
        public override void Update(GameTime gameTime)
        {
            newCar.update(gameTime);

            if (input.SelectInMenu)
            {
                if (gameTime.TotalGameTime - previousControllerButtonCooldown > (controllerButtonCooldown + transitionOnTime))
                {
                    // start game
                    if (selectedIndex == 0)
                    {
                        nextState = GameState.InGame;
                        gameStateChanged = true;
                        input.reset();
                    }
                    // Multiplayer
                    if (selectedIndex == 1)
                    {
                        nextState = GameState.MultiPlayer;
                        gameStateChanged = true;
                        input.reset();
                    }
                    // Options
                    if (selectedIndex == 2)
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

                        input.reset();
                    }
                    // Exit
                    if (selectedIndex == 3)
                    {
                        input.reset();
                        exit = true;
                    }
                }
            }

            base.Update(gameTime);
        }

        //update the camera target
        private void UpdateCameraChaseTarget()
        {
            camera.chasePosition = newCar.Position;
            camera.chaseDirection = newCar.Direction;
            camera.up = newCar.Normal;
        }

        //draw the cube and the menu car
        public override void Draw(GameTime gameTime)
        {
            drawTexturedCube();
            //spriteBatch.Draw(menuBackground, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
            newCar.Draw(gameTime, camera.view, camera.projection);

            base.Draw(gameTime);
        }

        public void drawTexturedCube()
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[cube.Bones.Count];
            cube.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in cube.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = false;
                    effect.TextureEnabled = true;
                    effect.Texture = menuBackground;

                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(0.5f) * Matrix.CreateRotationZ(MathHelper.ToRadians(180)) * Matrix.CreateTranslation(0, 0, 1.3f);
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                }

                mesh.Draw();
            }
        }
    }
}
