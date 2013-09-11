/*
 * This class is used to create the shop where vehicles are bought from, is renders models and does cash transactions if any
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

using RacingGame.PlayerData;
using RacingGame.Engine;
using RacingGame.Vehicles;

namespace RacingGame.Enviroment.Shop
{
    class CarShop
    {
        //protected data members
        protected GraphicsDeviceManager graphics;
        protected ContentManager content;
        protected InputHandler input;
        protected SpriteBatch spriteBatch;
        protected SpriteFont font;

        protected Player player;
        protected ChaseCamera camera;
        protected GameStateManager currentState;

        protected ShopVehicle[] vehicles = new ShopVehicle[3];

        protected int currentCarIndex;
        protected TimeSpan previousSelectTime;
        protected TimeSpan selectCooldown = TimeSpan.FromSeconds(0.2f);

        protected bool changeState;
        protected bool carBought;
        protected bool confirmation;
        protected string car;

        protected Model cube;
        protected Texture2D background;
        protected Texture2D shopHUD;
        protected Texture2D confirmDialog;

        //getters and setters
        public int GetSelectedCar
        {
            get { return currentCarIndex; }
        }

        public bool ChangeState
        {
            get { return changeState; }
            set { changeState = value; }
        }

        public string Car
        {
            get { return car; }
            set { car = value; } 
        }

        public bool CarBought
        {
            get { return carBought; }
            set { carBought = value; } 
        }

        //constructor
        public CarShop(ref GraphicsDeviceManager newGraphics, ContentManager Content, ref Player curentPlayer, ref InputHandler currentHandler, ref GameStateManager state, ref SpriteBatch newSpriteBatch, ref SpriteFont newFont, ref Model[] cars)
        {
            graphics = newGraphics;
            player = curentPlayer;
            content = Content;
            input = currentHandler;
            currentState = state;
            currentCarIndex = 0;
            changeState = false;
            carBought = false;
            confirmation = false;
            spriteBatch = newSpriteBatch;
            font = newFont;

            //camera used to view the different cars
            camera = new ChaseCamera();
            camera.desiredPositionOffset = new Vector3(0.0f, 0.5f, -1.3f);
            camera.nearPlaneDistance = 0.2f;
            camera.farPlaneDistance = 10000.0f;
            camera.aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height;
            camera.Reset();

            cube = Content.Load<Model>("Models//Objects//Cube");
            background = Content.Load<Texture2D>("Textures//Menu Images//carParkBackground");
            shopHUD = Content.Load<Texture2D>("Textures//Menu Images//shopHUD");
            confirmDialog = Content.Load<Texture2D>("Textures//Menu Images//confirmDialog");

            loadCars(ref cars);
        }

        //loading car models
        private void loadCars(ref Model[] cars)
        {
            vehicles[0] = new ShopVehicle(ref cars[1], new Vector3(0, 0, 0), 30000);
            vehicles[0].SetInformation("Koenigsegg CCX", "348 km/h", "6", content.Load<Texture2D>("Textures//Car Logos//Koenigsegg_logo"));
            vehicles[1] = new ShopVehicle(ref cars[0], new Vector3(0, 0, 0), 10000);
            vehicles[1].SetInformation("Ford Mustang GT500", "284 km/h", "5", content.Load<Texture2D>("Textures//Car Logos//ford_logo"));
            vehicles[2] = new ShopVehicle(ref cars[2], new Vector3(0, 0, 0), 20000);
            vehicles[2].SetInformation("Mercedes SLR Mclaren", "328 km/h", "5", content.Load<Texture2D>("Textures//Car Logos//mercedesBenzLogo"));
        }

        //update used to rotate cars as well as used for input handling for choosing cars
        public void Update(GameTime gameTime)
        {
            input.update(Keyboard.GetState(), gameTime);

            if (!confirmation)
            {
                //update all car rotations
                for (int a = 0; a < vehicles.Length; a++)
                    vehicles[a].update(gameTime);

                if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                {
                    previousSelectTime = gameTime.TotalGameTime;

                    //move left
                    if (input.LeftInMenu)
                        if (currentCarIndex > 0)
                            currentCarIndex--;

                    //move right
                    if (input.RightInMenu)
                        if (currentCarIndex < vehicles.Length - 1)
                            currentCarIndex++;

                    //select and buy car
                    if (input.SelectInMenu)
                    {
                        if (player.CurrentCash >= vehicles[currentCarIndex].Cost)
                        {
                            confirmation = true;
                            input.reset();
                        }
                    }

                    //exit menu
                    if (input.ExitMenu)
                        changeState = true;
                }
            }

            else
            {
                if (gameTime.TotalGameTime - previousSelectTime > selectCooldown)
                {
                    previousSelectTime = gameTime.TotalGameTime;

                    if (input.SelectInMenu)
                    {
                        player.SubtractFromCash(vehicles[currentCarIndex].Cost);
                        carBought = true;

                        if (currentCarIndex == 0)
                            car = "Koenigsegg CCX";
                        else if (currentCarIndex == 1)
                            car = "Ford Mustang GT500";
                        else if (currentCarIndex == 2)
                            car = "Mercedes SLR Mclaren";

                        changeState = true;
                    }

                    if (input.ExitMenu)
                        confirmation = false;
                }
            }
        }

        //draw shop
        public void Draw(GameTime gameTime)
        {
           vehicles[currentCarIndex].Draw(gameTime, camera.view, camera.projection);
           drawTexturedCube();

           // Center the text in the viewport.
           Viewport viewport = graphics.GraphicsDevice.Viewport;
           Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
           Vector2 textSize = font.MeasureString(vehicles[currentCarIndex].Name);
           Vector2 textPosition = (viewportSize - textSize) / 2;

           spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
           if(confirmation)
               spriteBatch.Draw(confirmDialog, Vector2.Zero, Color.White);
           spriteBatch.Draw(shopHUD, Vector2.Zero, Color.White);
           spriteBatch.Draw(vehicles[currentCarIndex].Logo, new Vector2(textPosition.X - 100, 80), null, Color.White, 0.0f, new Vector2(vehicles[currentCarIndex].Logo.Width / 2, vehicles[currentCarIndex].Logo.Height / 2), 1.0f, SpriteEffects.None, 1.0f);

           //draw HUD text
           spriteBatch.DrawString(font, vehicles[currentCarIndex].Name, new Vector2(textPosition.X, 0), Color.White);
           spriteBatch.DrawString(font, "   Top Speed: " + vehicles[currentCarIndex].Speed.ToString(), new Vector2(textPosition.X, 30), Color.White, 0, Vector2.Zero, 0.65f, SpriteEffects.None, 1.0f);
           spriteBatch.DrawString(font, "   Number of gears: "+vehicles[currentCarIndex].Gears.ToString(), new Vector2(textPosition.X, 50), Color.White, 0, Vector2.Zero, 0.65f, SpriteEffects.None, 1.0f);
           spriteBatch.DrawString(font, "Cost: R " + vehicles[currentCarIndex].Cost, new Vector2(0, viewport.Height - 80), Color.White);
           spriteBatch.DrawString(font, "Cash: R " + player.CurrentCash, new Vector2(0, viewport.Height - 50), Color.White);
           spriteBatch.End();
        }
        
        //draw cube with texture representing garage
        public void drawTexturedCube()
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[cube.Bones.Count];
            cube.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in cube.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.TextureEnabled = true;
                    effect.Texture = background;

                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(0.5f) * Matrix.CreateRotationZ(MathHelper.ToRadians(180)) * Matrix.CreateTranslation(0, 0.2f, 1.3f);
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                }

                mesh.Draw();
            }
        }
    }
}
