/*
 *  This is the main game file, all files and models are used and called from here
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using RacingGame.Engine;
using RacingGame.Engine.UI;
using RacingGame.Engine.TerrainLoader;
using RacingGame.Engine.HelperFunctions;
using RacingGame.Engine.Audio;
using RacingGame.PlayerData;
using RacingGame.Vehicles;
using RacingGame.GameMenus;
using RacingGame.Enviroment;
using RacingGame.Enviroment.Tracks;
using RacingGame.Enviroment.Shop;
using RacingGame.Dashboards;
using RacingGame.Engine.PaticleEngine;

namespace RacingGame
{
    class RedLineRacing : Microsoft.Xna.Framework.Game
    {
        #region private data memebers

            #region content manager data
                ContentManager defaultContent;
                ContentManager freeRoamContent;
                ContentManager shopContent;
                ContentManager racingContent;
                ContentManager mainMenuContent;
                ContentManager musicManager;
                ContentManager carModels;
                ContentManager aiCarModels;
                ContentManager imageContent;
            #endregion

            #region Viewport data
                Viewport defaultViewport;
                Viewport[] viewPorts = new Viewport[1];
            #endregion

            #region Freeroam data members
            //basic effect to use
            BasicEffect basicEffect;
            Effect effect;

            //story data
            bool storyDisplay = true;
            bool finalDisplay = true;
            bool storyEnables = true;
            int displayTimer = 30;

            //loading data
            bool defaultData = false;
            bool debugData = false;

            //spritebatch and current device
            GraphicsDeviceManager graphics;
            SpriteBatch spriteBatch;

            //city data
            Texture2D buildingTextures;
            Texture2D sky;
            Vector3 cityHeightPos = new Vector3(4, 0, 190);
            float cityHeightOffset = 0.0f;
            double angle;
            double angle2;

            //sky to be added
            Model skydome;
            City city;
            SkyDome skyDome;
            HeightMapGenerator eniviroment;

            //racing markers
            bool racemarkersLoaded;    
            List<RaceMarker> ingameRaceMarkers;
            Model raceMarkerModel;

            //current marker collided with
            RaceMarker currentCollidedRaceMarker;
            TimeSpan currentRacemarkerCollisionCooldown;
            TimeSpan markerCollisionTime;
            string alpineInfo;
            string mountainInfo;
            string plainsInfo;

            //coming out of a race data
            Vector3 savedState;
            Vector3 savedDirection;
            int removeMarker = 0;

            //game time
            GameTime time;
            
            //countdown timer
            int counterTimer = 120;

            //ingame HUD
            IngameHUD ingameHUD;
            Texture2D ingameHudTexture;

            //lamp post in city
            Model lampPostModel;
            Lamppost[] lampposts;
            #endregion

            #region Cars in game
            Model[] vehicleModels = new Model[3];
            Model[] aiModels = new Model[3];

            String mustang = "Cars//Mustang//Mustang";
            String mercedes = "Cars//Mercedes SLR Mclaren//SLR";
            String koenigsegg = "Cars//Koenigsegg CCX//CCX";
            #endregion

            #region racing data members
                int counter = 0;
                Race currentRace;

                Vector3[] startingpositions = new Vector3[4];
                Texture2D alpineMap;
                Texture2D alpineCollision;
                Texture2D alpineAI;
                Texture2D mountainMap;
                Texture2D mountainCollision;
                Texture2D mountainAI;
                Texture2D plainsMap;
                Texture2D plainsCollision;
                Texture2D plainsAI;
                Texture2D raceInformationHUD;

                RacingHUD[] racingHud = new RacingHUD[1];

                int leaderWaypoint = 0;
                string leaderName = "";
                float defaultHeight = 51;
                int aiVehicle = 0;
            #endregion

            #region Game state and input handling data members
            //input handling
            InputHandler input;

            //Options Manager
            OptionsManager optionsManager;

            //current game state
            GameStateManager currentState;
            #endregion

            #region Game menu data members
            //different menus
            //mainMenu
            MainMenu mainMenu;
            Texture2D mainMenuBackground;

            //Story menu
            StoryMenu story;
            Texture2D storyMenuBackground;

            //gameMarker menu
            RaceMarkerMenu raceMenu;
            Texture2D raceMenuBackGround;

            //Race finish menu
            RaceFinishMenu finishMenu;
            MultiplayerRaceFinishMenu mFinishMenu;
            Texture2D raceFinishBackground;
            string finishTime;

            //Race menu
            InRaceMenu inRaceMenu;
            Texture2D inRaceMenuBackground;

            //MultiPlayer Menu
            InMultiRaceMenu inMultiRaceMenu;
            Texture2D inMultiRaceMenuBackground;

            //Free roam menu
            FreeRoamMenu roamMenu;
            Texture2D roamMenuBackground;

            //Options Menu
            OptionsMenu options;
            Texture2D optionsBackground;
            int menuIndex;

            //Multi Player Menu
            MultiPlayerMenu multiMenu;
            Texture2D multiBackground;
            Texture2D chosenMap;
            Texture2D chosenCollision;

            Texture2D controls;

            //loading menu
            LoadingScreen loading;
            #endregion

            #region Font to be used
            //font
            SpriteFont gameFont;
            SpriteFont arial;
            #endregion

            #region map textures
                Texture2D texture0;
                Texture2D texture1;
                Texture2D texture2;
                Texture2D texture3;
                Texture2D texture4;
                Texture2D waterBumpMap;
                Texture2D treeTexture;
                Texture2D treeMap;
                Texture2D outsideTerrain;
                Texture2D outsideCollision;
            #endregion

            #region Player data
            //loading camera
            ChaseCamera[] camera = new ChaseCamera[1];

            //loading car
            Player[] gamePlayers = new Player[1];
            #endregion

            #region Carshop data
            CarShop carShop;

            Model shopModel;
            ShopMarker shopMarker;
            ShopMenu shopmenu;
            Texture2D shopMenuBackground;
            #endregion

            #region Music player data
            MPlayer media;
            Thread mediaPlayerThread;
            Texture2D mplayerHUD;
            #endregion
        #endregion

            //constructor
        public RedLineRacing()
        {
            graphics = new GraphicsDeviceManager(this);

            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;

            //add the onscreen keyboard for entering player names
            Components.Add(new GamerServicesComponent(this));

            Content.RootDirectory = "Content";
        }

        //initialize data members
        protected override void Initialize()
        {
            //initialize different content loaders
            defaultContent = new ContentManager(Services, "Content");
            freeRoamContent = new ContentManager(Services, "Content");
            racingContent = new ContentManager(Services, "Content");
            mainMenuContent = new ContentManager(Services, "Content");
            shopContent = new ContentManager(Services, "Content");
            musicManager = new ContentManager(Services, "Content");
            carModels = new ContentManager(Services, "Content");
            aiCarModels = new ContentManager(Services, "Content");
            imageContent = new ContentManager(Services, "Content");

            defaultContent.Unload();

            // Initialize renderer
            if (!debugData)
            {
                DebugShapeRenderer.Initialize(GraphicsDevice);
                debugData = true;
            }

            //initialize state
            currentState = new GameStateManager();
            currentState.CurrentState = GameState.MainMenu;

            //initialize input handler
            input = new InputHandler(currentState);

            //initialize option manager
            optionsManager = new OptionsManager();

            //initialize spritebatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //load font
            gameFont = defaultContent.Load<SpriteFont>("Fonts//gameFont");
            arial = defaultContent.Load<SpriteFont>("Fonts//arial");

            //initialize the racemarker list
            ingameRaceMarkers = new List<RaceMarker>();

            //initialize racemarker load boolean
            racemarkersLoaded = false;

            //initilaize loading screen
            loading = new LoadingScreen(ref graphics, ref gameFont, ref spriteBatch);
            loading.Active = false;

            //load menu backgrounds
            multiBackground = imageContent.Load<Texture2D>("Textures//Menu Backgrounds//multiplayerMenuBackground");
            mainMenuBackground = imageContent.Load<Texture2D>("Textures//Menu Backgrounds//MainMenuBackGround");
            shopMenuBackground = imageContent.Load<Texture2D>("Textures//Menu Backgrounds//shopBackground");
            raceMenuBackGround = imageContent.Load<Texture2D>("Textures//Menu Backgrounds//raceMenuBackground");
            raceFinishBackground = imageContent.Load<Texture2D>("Textures//Menu Backgrounds//raceEndBackground");
            inRaceMenuBackground = imageContent.Load<Texture2D>("Textures//Menu Backgrounds//inRaceMenuBackground");
            inMultiRaceMenuBackground = imageContent.Load<Texture2D>("Textures//Menu Backgrounds//inRaceMenuBackground");
            roamMenuBackground = imageContent.Load<Texture2D>("Textures//Menu Backgrounds//MainMenuBackGround");
            optionsBackground = imageContent.Load<Texture2D>("Textures//Menu Backgrounds//shopBackground");
            storyMenuBackground = imageContent.Load<Texture2D>("Textures//Menu Backgrounds//PlotBackground");
            controls = imageContent.Load<Texture2D>("Textures//Menu Images//controls_image");
            outsideTerrain = imageContent.Load<Texture2D>("Textures//City//CityMountains");
            outsideCollision = imageContent.Load<Texture2D>("Textures//City//CityMountainsC");
            raceInformationHUD = imageContent.Load<Texture2D>("Textures//RacingHUD//raceInformation");

            alpineInfo = "Alpine racetrack\n\n" +
                                        "This race takes place within the forest areas\n" +
                                        "lots of open space makes it easy to overtake or\n" +
                                        "be overtaken.";
            mountainInfo = "Mountain racetrack\n\n" +
                                        "This race takes place within the hight mountain tops,\n" +
                                        "tight corners, high walls will test the driver's skills\n" +
                                        "to the limit.";
            plainsInfo = "Plains racetrack\n\n" +
                                        "This race takes place within the hight mountain tops,\n" +
                                        
                                        "tight corners, high walls will test the driver's skills\n" +
                                        "to the limit.";

            //load media player hud texture
            mplayerHUD = defaultContent.Load<Texture2D>("Textures//Music Player//MusicPlayerHUD");

            //load car models
            vehicleModels[0] = carModels.Load<Model>(mustang);

            //set up chase camera initial information
            camera[0] = new ChaseCamera();
            camera[0].desiredPositionOffset = new Vector3(0.0f, 0.5f, -1.3f);
            camera[0].nearPlaneDistance = 0.2f;
            camera[0].farPlaneDistance = 1000.0f;
            camera[0].aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height;
            camera[0].Reset();

            //initialize player data
            intializeSinglePlayer();

            //set a global variable keeping track of time
            time = new GameTime();

            base.Initialize();
        }

        public void intializeSinglePlayer()
        {
            //initialize player data
            gamePlayers = new Player[1];
            gamePlayers[0] = new Player("Nemesis", currentState, 0);
            gamePlayers[0].Car = new FordMustangGT500(ref graphics, Content, ref spriteBatch, new Vector3(0, 0, -3), input, ref arial, ref vehicleModels[0], this);

            camera[0].aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height;


            if (optionsManager.GearBox1)
                gamePlayers[0].Car.changeGearBoxToManual();
        }

        public void intializeLamposts()
        {
            //initialize lamppost
            lampPostModel = defaultContent.Load<Model>("Models//Objects//old_style_lamp");

            List<Vector3> positions = city.getSideWalkTiles();
            lampposts = new Lamppost[positions.Count];

            for (int i = 0; i < positions.Count; i++)
            {
                Vector3 position = positions.ElementAt(i);
                lampposts[i] = new Lamppost(ref lampPostModel, position);
            }
        }

        //load method that loads data based on the current state  of the game
        protected override void LoadContent()
        {
            freeRoamContent.Unload();
            racingContent.Unload();
            mainMenuContent.Unload();
            shopContent.Unload();
            musicManager.Unload();

            gamePlayers[0].Car.reset();

            if (currentState.CurrentState == GameState.InGame || currentState.CurrentState == GameState.Racing || currentState.CurrentState == GameState.MultiPlayer)
            {
                //load media player
                if(optionsManager.MusicEnabled)
                    loadMediaPlayerContent();
                else
                    musicManager.Unload();

                if (!defaultData)
                {
                    //load map textures
                    texture0 = defaultContent.Load<Texture2D>("Textures//Heightmap textures//sand");
                    texture1 = defaultContent.Load<Texture2D>("Textures//Heightmap textures//grass");
                    texture2 = defaultContent.Load<Texture2D>("Textures//Heightmap textures//rock");
                    texture3 = defaultContent.Load<Texture2D>("Textures//Heightmap textures//snow");
                    texture4 = defaultContent.Load<Texture2D>("Textures//Heightmap textures//concrete");
                    treeTexture = defaultContent.Load<Texture2D>("Textures//Heightmap textures//tree");
                    treeMap = defaultContent.Load<Texture2D>("Textures//Heightmap textures//treeMap");
                    waterBumpMap = defaultContent.Load<Texture2D>("Textures//Heightmap textures//waterbump");

                    //initialize the skydome data
                    skydome = defaultContent.Load<Model>("Models//SkyDome//dome");
                    sky = defaultContent.Load<Texture2D>("Models//SkyDome//cloudMap");

                    //create skyDome
                    basicEffect = new BasicEffect(graphics.GraphicsDevice, null);
                    skyDome = new SkyDome(ref skydome, ref sky, ref basicEffect, ref graphics);

                    //load effect
                    effect = defaultContent.Load<Effect>("Effects//Effects");

                    //load race marker model
                    raceMarkerModel = defaultContent.Load<Model>("Models//Objects//SphereHighPoly");

                    //initialize carshop marker
                    shopModel = defaultContent.Load<Model>("Models//Objects//garage");
                    shopMarker = new ShopMarker(ref graphics, ref currentState, ref shopModel, new Vector3(-310, 6, -205) / 20, ref effect);

                    defaultData = true;
                }
            }

            #region game in MainMenu state
            if (currentState.CurrentState == GameState.MainMenu)
            {
                string[] mainmenuItems = { "Start Game", "MultiPlayer", "Options", "End Game" };
                input.Player = 0;
                mainMenu = new MainMenu(ref graphics,
                                          this,
                                          ref spriteBatch,
                                          ref gameFont,
                                          ref mainmenuItems,
                                          ref input,
                                          Content.Load<Model>("Cars//Mazda MX-5//Car"),
                                          Content.Load<Model>("Cars//Mazda MX-5//Wheel"),
                                          Content.Load<Model>("Models//Objects//cube"),
                                          ref mainMenuBackground,
                                          ref currentState,
                                          "Main Menu",
                                          ref time);
            }
            #endregion

            #region game in InGame state
            else if (currentState.CurrentState == GameState.InGame)
            {
                counterTimer = 120;
                gamePlayers[0].Car.MaxCollisionHeight = 51;

                //set viewport array
                viewPorts = new Viewport[1];

                //load viewport data
                viewPorts[0] = graphics.GraphicsDevice.Viewport;
                defaultViewport = graphics.GraphicsDevice.Viewport;

                //initialize the city
                city = new City(ref graphics);
                float cityHeightCollision = 51;

                //initialize the outside city terrain
               // if (eniviroment == null)
               // {
                if (eniviroment != null)
                {
                    eniviroment.Dispose();
                    eniviroment = null;
                }

                    eniviroment = new HeightMapGenerator(ref graphics,
                                                            ref outsideTerrain,
                                                            ref outsideCollision,
                                                            null,
                                                            ref texture0,
                                                            ref texture1,
                                                            ref texture2,
                                                            ref texture3,
                                                            ref waterBumpMap,
                                                            ref treeTexture,
                                                            ref treeMap,
                                                            ref effect,
                                                            1.0f,
                                                            ref currentState,
                                                            0,
                                                            18,
                                                            40,
                                                            68,
                                                            15.0f,
                                                            ref cityHeightCollision);                
                //}
                    

                //set city position
                Vector3 cityPos = new Vector3(-20 / 0.05f, eniviroment.getHeight(cityHeightPos) + cityHeightOffset, 0);
                city.CityPosition = cityPos;

                //create car model for player and load the model
                //car = new LamborghiniMurcielagoLP640(GraphicsDevice, Content, spriteBatch, new Vector3(0, 0, -3), input, arial, ousideCityTerrain, ousideCityTerrain.getHeight(cityHeightPos));
                gamePlayers[0].Car.Position = new Vector3(0, 0, -3);
                gamePlayers[0].Car.setHeightmapGenerator(eniviroment);
                gamePlayers[0].Car.setNewHeightOffset(eniviroment.getHeight(cityHeightPos));

                //load assets for the current state items
                buildingTextures = freeRoamContent.Load<Texture2D>("Textures//City//CityTextures");
                city.LoadContent(ref buildingTextures);
                eniviroment.LoadContent(new Vector3(0, 0, 0));
                gamePlayers[0].Car.Position = city.getStartPoint;

                //initialize the lampposts
                intializeLamposts();

                //setting racemarkers
                if(!racemarkersLoaded)
                    loadRaceMarkerDetails();

                //reset NOS
                if (gamePlayers[0].Car.Nos)
                    gamePlayers[0].Car.NosAmmount = 300;

                //ai model unload
                if (aiModels != null)
                    aiModels = null;

                //load ingame HUD
                ingameHudTexture = freeRoamContent.Load<Texture2D>("Textures//RacingHUD//ingameHUD");
                ingameHUD = new IngameHUD(ref graphics, ref spriteBatch, ref gamePlayers[0], ref ingameHudTexture, ref arial);
            }
            #endregion

            #region game in Racing state
            else if (currentState.CurrentState == GameState.Racing)
            {
                leaderName = "";
                leaderWaypoint = 0;

                if(currentCollidedRaceMarker.SetRaceType == RaceMapType.Alpine)
                    currentCollidedRaceMarker.LoadRaceContent(ref texture4, ref texture3, ref texture2, ref texture3, ref waterBumpMap, ref camera[0], true, ref eniviroment, ref treeTexture, ref treeMap);
                else if (currentCollidedRaceMarker.SetRaceType == RaceMapType.Mountain)
                    currentCollidedRaceMarker.LoadRaceContent(ref texture4, ref texture2, ref texture2, ref texture3, ref waterBumpMap, ref camera[0], true, ref eniviroment, ref treeTexture, ref treeMap);
                else if (currentCollidedRaceMarker.SetRaceType == RaceMapType.Plains)
                    currentCollidedRaceMarker.LoadRaceContent(ref texture4, ref texture1, ref texture2, ref texture3, ref waterBumpMap, ref camera[0], true, ref eniviroment, ref treeTexture, ref treeMap);

                for (int a = 0; a < gamePlayers.Length; a++)
                {
                    //setting racemarkers                    
                    currentRace = currentCollidedRaceMarker.currentRace;
                    currentRace.Winnings = raceMenu.Bet * 4;

                    gamePlayers[a].CurrentCash -= raceMenu.Bet;
                    racingHud[a] = new RacingHUD(ref graphics, ref spriteBatch, ref currentRace, racingContent.Load<Texture2D>("Textures//Minimap//playerMapIndicator"), racingContent.Load<Texture2D>("Textures//Minimap//enemyMapIndicator"), racingContent.Load<Texture2D>("Textures//Minimap//waypointMapIndicator"), ref arial, ref raceInformationHUD, time, ref gamePlayers[a]);

                    //create car model for player and load the model
                    gamePlayers[a].Car.Position = new Vector3(0, 0, -3);
                    gamePlayers[a].Car.setHeightmapGenerator(currentCollidedRaceMarker.MapGenerator);
                    gamePlayers[a].Car.setNewHeightOffset(currentCollidedRaceMarker.MapGenerator.getHeight(cityHeightPos));

                    gamePlayers[a].Car.Position = currentRace.StartPoints[a];
                    gamePlayers[a].Car.Direction = currentRace.Direction;

                    //reset NOS
                    if (gamePlayers[0].Car.Nos)
                        gamePlayers[0].Car.NosAmmount = 300;
                }

                //-------------------------------add enemy cars here
                if (currentRace.AIenabled)
                {
                    Random randomNumber = new Random();
                    aiModels = new Model[3];

                    for (int i = 0; i < currentRace.AIBots.Length; i++)
                    {
                        float randomvalue = randomNumber.Next(0, 3);
                        aiVehicle = (int)randomvalue;

                        if (randomvalue == 0)
                        {
                            if (aiModels[0] == null)
                                aiModels[0] = aiCarModels.Load<Model>(mustang);

                            currentRace.AIBots[i].Car = new FordMustangGT500(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), currentRace.AIBots[i].Input, ref arial, ref aiModels[0], this);
                        }
                        else if (randomvalue == 1)
                        {
                            if (aiModels[1] == null)
                                aiModels[1] = aiCarModels.Load<Model>(koenigsegg);

                            currentRace.AIBots[i].Car = new KoenigseggCCX(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), currentRace.AIBots[i].Input, ref arial, ref aiModels[1], this);
                        }
                        else if (randomvalue == 2)
                        {
                            if (aiModels[2] == null)
                                aiModels[2] = aiCarModels.Load<Model>(mercedes);

                            currentRace.AIBots[i].Car = new MercedesSLRMclaren(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), currentRace.AIBots[i].Input, ref arial, ref aiModels[2], this);
                        }

                        currentRace.AIBots[i].Car.Position = currentRace.StartPoints[i+1];
                        currentRace.AIBots[i].Car.Direction = currentRace.Direction;

                        currentRace.AIBots[i].Car.setHeightmapGenerator(currentCollidedRaceMarker.MapGenerator);
                        currentRace.AIBots[i].Car.setNewHeightOffset(currentCollidedRaceMarker.MapGenerator.getHeight(cityHeightPos));
                        currentRace.AIBots[i].Car.Input = currentRace.AIBots[i].Input;
                    }
                }
                //--------------------------------------------------
            }
            #endregion

            #region game in InShop state
            else if (currentState.CurrentState == GameState.InShop)
            {
                vehicleModels[0] = carModels.Load<Model>(mustang);
                vehicleModels[1] = carModels.Load<Model>(koenigsegg);
                vehicleModels[2] = carModels.Load<Model>(mercedes);
                carShop = new CarShop(ref graphics, shopContent, ref gamePlayers[0], ref input, ref currentState, ref spriteBatch, ref arial, ref vehicleModels);
            }
            #endregion
        }

        protected void loadMediaPlayerContent()
        {
            if (optionsManager.MusicEnabled)
            {
                if (media == null)
                {
                    //initialize media player
                    media = new MPlayer(musicManager, ref input);
                    mediaPlayerThread = new Thread(new ThreadStart(media.Update));
                    mediaPlayerThread.Start();
                }
            }

            if (!optionsManager.MusicEnabled)
            {
                if (media != null)
                {                    
                    media.Stop();
                    media = null;
                    //mediaPlayerThread.Interrupt();
                }
            }
        }

        protected void loadRaceMarkerDetails()
        {
            ingameRaceMarkers.Clear();

            //set cooldown for collision time
            currentRacemarkerCollisionCooldown = TimeSpan.FromSeconds(2.0);

            //setting racemarkers
            //adding alpine map to list of markers
            alpineMap = defaultContent.Load<Texture2D>("Textures//Track Heightmaps//Alpine");
            alpineCollision = defaultContent.Load<Texture2D>("Textures//Track Heightmaps//AlpineCollision");
            alpineAI = defaultContent.Load<Texture2D>("Textures//Track Heightmaps//AlpineAI");
            RaceMarker alpine = new RaceMarker(ref graphics, ref currentState, ref raceMarkerModel, new Vector3(180, 4, -309) / city.Scale, ref effect, ref alpineMap, ref alpineCollision, ref skyDome, ref alpineAI);
            RaceMarker alpine1 = new RaceMarker(ref graphics, ref currentState, ref raceMarkerModel, new Vector3(1402, 4, -1062) / city.Scale, ref effect, ref alpineMap, ref alpineCollision, ref skyDome, ref alpineAI);
            RaceMarker alpine2 = new RaceMarker(ref graphics, ref currentState, ref raceMarkerModel, new Vector3(1104, 4, -1644) / city.Scale, ref effect, ref alpineMap, ref alpineCollision, ref skyDome, ref alpineAI);
            alpine.MapInformation = alpineInfo;
            alpine1.MapInformation = alpineInfo;
            alpine2.MapInformation = alpineInfo;
            alpine.SetRaceType = RaceMapType.Alpine;
            alpine1.SetRaceType = RaceMapType.Alpine;
            alpine2.SetRaceType = RaceMapType.Alpine;
            ingameRaceMarkers.Add(alpine);
            ingameRaceMarkers.Add(alpine1);
            ingameRaceMarkers.Add(alpine2);

            //adding mountain map
            mountainMap = defaultContent.Load<Texture2D>("Textures//Track Heightmaps//Mountain");
            mountainCollision = defaultContent.Load<Texture2D>("Textures//Track Heightmaps//MountainCollision");
            mountainAI = defaultContent.Load<Texture2D>("Textures//Track Heightmaps//MountainAI");
            RaceMarker mountain = new RaceMarker(ref graphics, ref currentState, ref raceMarkerModel, new Vector3(445, 4, -973) / city.Scale, ref effect, ref mountainMap, ref mountainCollision, ref skyDome, ref mountainAI);
            RaceMarker mountain1 = new RaceMarker(ref graphics, ref currentState, ref raceMarkerModel, new Vector3(-330, 4, -100) / city.Scale, ref effect, ref mountainMap, ref mountainCollision, ref skyDome, ref mountainAI);
            RaceMarker mountain2 = new RaceMarker(ref graphics, ref currentState, ref raceMarkerModel, new Vector3(-205, 4, -290) / city.Scale, ref effect, ref mountainMap, ref mountainCollision, ref skyDome, ref mountainAI);
            mountain.MapInformation = mountainInfo;
            mountain1.MapInformation = mountainInfo;
            mountain2.MapInformation = mountainInfo;
            mountain.SetRaceType = RaceMapType.Mountain;
            mountain1.SetRaceType = RaceMapType.Mountain;
            mountain2.SetRaceType = RaceMapType.Mountain;
            ingameRaceMarkers.Add(mountain);
            ingameRaceMarkers.Add(mountain1);
            ingameRaceMarkers.Add(mountain2);

            //adding plains map
            plainsMap = defaultContent.Load<Texture2D>("Textures//Track Heightmaps//Plains");
            plainsCollision = defaultContent.Load<Texture2D>("Textures//Track Heightmaps//PlainsCollision");
            plainsAI = defaultContent.Load<Texture2D>("Textures//Track Heightmaps//PlainsAI");
            RaceMarker plains = new RaceMarker(ref graphics, ref currentState, ref raceMarkerModel, new Vector3(403, 4, -751) / city.Scale, ref effect, ref plainsMap, ref plainsCollision, ref skyDome, ref plainsAI);
            RaceMarker plains1 = new RaceMarker(ref graphics, ref currentState, ref raceMarkerModel, new Vector3(963, 4, -1018) / city.Scale, ref effect, ref plainsMap, ref plainsCollision, ref skyDome, ref plainsAI);
            RaceMarker plains2 = new RaceMarker(ref graphics, ref currentState, ref raceMarkerModel, new Vector3(1160, 4, -761) / city.Scale, ref effect, ref plainsMap, ref plainsCollision, ref skyDome, ref plainsAI);
            RaceMarker plains3 = new RaceMarker(ref graphics, ref currentState, ref raceMarkerModel, new Vector3(-36, 4, -364) / city.Scale, ref effect, ref plainsMap, ref plainsCollision, ref skyDome, ref plainsAI);
            plains.MapInformation = plainsInfo;
            plains1.MapInformation = plainsInfo;
            plains2.MapInformation = plainsInfo;
            plains3.MapInformation = plainsInfo;
            plains.SetRaceType = RaceMapType.Plains;
            plains1.SetRaceType = RaceMapType.Plains;
            plains2.SetRaceType = RaceMapType.Plains;
            plains3.SetRaceType = RaceMapType.Plains;
            ingameRaceMarkers.Add(plains);
            ingameRaceMarkers.Add(plains1);
            ingameRaceMarkers.Add(plains2);
            ingameRaceMarkers.Add(plains3);

            racemarkersLoaded = true;
        }

        protected override void Update(GameTime gameTime)
        {
            //update input
            input.update(Keyboard.GetState(), gameTime);
            time = gameTime;

            //update the media players
            if (currentState.CurrentState != GameState.MainMenu && media != null && optionsManager.MusicEnabled)
            {
                media.UpdateInput(gameTime);
                media.AdjustVolume(optionsManager.MusicVolume);
                media.AdjustEffectVolume(optionsManager.EffectVolume);
            }

            //update sound effects
            if (currentState.CurrentState != GameState.MainMenu && optionsManager.SoundFXEnabled)
            {
                for (int a = 0; a < gamePlayers.Length; a++)
                    gamePlayers[a].Car.AdjustVolume(optionsManager.EffectVolume);
            }

            //update player gearbox if changed
            if (options != null && options.GearBoxChanged)
            {
                if (!optionsManager.GearBox1)
                    gamePlayers[0].Car.changeGearBoxToAutomatic();
                else
                    gamePlayers[0].Car.changeGearBoxToManual();

                options.GearBoxChanged = false;
            }

            #region Loading state and Menus
            if (currentState.CurrentState == GameState.Loading)
            {
                #region MainMenu
                //if in main menu and game state has changed
                if (mainMenu.GameStateChanged)
                {
                    mainMenu.GameStateChanged = false;
                    mainMenu.Status = false;

                    currentState.CurrentState = mainMenu.NextState;

                    if (currentState.CurrentState == GameState.InGameMenu)
                    {
                        string[] optionMenuItems = { "Controls", "Sound FX", "Music", "music Volume", "Effects Volume", "GearBox", "Back" };
                        options = new OptionsMenu(ref graphics, this, ref spriteBatch, ref gameFont, ref optionMenuItems,
                                                  ref input, ref optionsBackground, ref currentState, "Options Menu", ref currentRace, 
                                                  ref gamePlayers[0], ref gameTime, ref controls, ref optionsManager);
                        options.Status = true;
                        menuIndex = 0;
                    }

                    else if (currentState.CurrentState == GameState.MultiPlayer)
                    {
                        string[] multiPlayerMenuItems = { "Player 1 Name:", "Player 1 Car:", "Player 1 GearBox:", "", "Player 2 Name:",
                                                            "Player 2 Car:", "Player 2 GearBox:", "", "Select Map:", "", "Race!", "", "Back" };
                        multiMenu = new MultiPlayerMenu(ref graphics, this, ref spriteBatch, ref gameFont, ref multiPlayerMenuItems, ref input,
                                                        ref multiBackground, ref currentState, "MultiPlayer Menu", ref currentRace, ref gamePlayers[0],
                                                        ref gameTime, ref optionsManager);
                        storyEnables = false;
                        multiMenu.Status = true;
                        menuIndex = 0;
                        currentState.CurrentState = GameState.InGameMenu;
                    }

                    else
                    {
                        //set single player data
                        intializeSinglePlayer();
                        string[] plotstring = {"Continue"};
                        story = new StoryMenu(ref graphics, this, ref spriteBatch, ref gameFont, ref plotstring, ref input, ref storyMenuBackground, ref currentState, "Plot", ref gameTime);
                        storyDisplay = true;
                        finalDisplay = true;
                        storyEnables = true;

                        LoadContent();
                        loadMediaPlayerContent();
                    }
                    loading.Active = false;
                }

                #endregion

                #region MultiMenu

                //if in multiplayer menu menu and game state has changed
                else if (multiMenu != null && multiMenu.GameStateChanged)
                {
                    multiMenu.GameStateChanged = false;
                    multiMenu.Status = false;
                    currentState.CurrentState = multiMenu.NextState;

                    //if in multiplayer state
                    if (currentState.CurrentState == GameState.MultiPlayer)
                    {
                        LoadContent();
                        loadMediaPlayerContent();

                        leaderName = "";
                        leaderWaypoint = 0;

                        //load chosen map
                        chosenMap = racingContent.Load<Texture2D>("Textures//Track Heightmaps//" + multiMenu.Map);
                        //load chose collision map
                        chosenCollision = racingContent.Load<Texture2D>("Textures//Track Heightmaps//" + multiMenu.Map + "Collision");

                        //load player cameras
                        camera = new ChaseCamera[2];
                        camera[0] = new ChaseCamera();
                        camera[0].desiredPositionOffset = new Vector3(0.0f, 0.5f, -1.3f);
                        camera[0].nearPlaneDistance = 0.2f;
                        camera[0].farPlaneDistance = 1000.0f;
                        camera[0].Reset();
                        camera[0].aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width*2 / graphics.GraphicsDevice.Viewport.Height;

                        camera[1] = new ChaseCamera();
                        camera[1].desiredPositionOffset = new Vector3(0.0f, 0.5f, -1.3f);
                        camera[1].nearPlaneDistance = 0.2f;
                        camera[1].farPlaneDistance = 1000.0f;
                        camera[1].Reset();
                        camera[1].aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width*2 / graphics.GraphicsDevice.Viewport.Height;

                        //create spitscreen viewports
                        viewPorts = new Viewport[2];

                        defaultViewport = graphics.GraphicsDevice.Viewport;
                        viewPorts[0] = graphics.GraphicsDevice.Viewport;
                        viewPorts[1] = graphics.GraphicsDevice.Viewport;
                        viewPorts[0].Height = graphics.GraphicsDevice.Viewport.Height / 2;
                        viewPorts[1].Height = graphics.GraphicsDevice.Viewport.Height / 2;
                        viewPorts[1].Y = viewPorts[0].Height + 1;

                        //create heightmap generator  
                        eniviroment = new HeightMapGenerator(ref graphics, ref chosenMap, ref chosenCollision, null,ref texture0, ref texture1,
                                                                   ref texture2, ref texture3, ref waterBumpMap, ref treeTexture, ref treeMap, ref effect,
                                                                   1.0f, ref currentState, 0, 7, 16, 35, 6, ref defaultHeight);

                        eniviroment.LoadContent(Vector3.Zero);

                        //create race
                        if (multiMenu.Map.Equals("Alpine"))
                            currentRace = new Alpine(ref graphics, ref eniviroment, ref chosenMap, ref skyDome, ref effect, false, ref treeMap);
                        if (multiMenu.Map.Equals("Mountain"))
                            currentRace = new Mountain(ref graphics, ref eniviroment, ref chosenMap, ref skyDome, ref effect, false, ref treeMap);
                        if (multiMenu.Map.Equals("Plains"))
                            currentRace = new Plains(ref graphics, ref eniviroment, ref chosenMap, ref skyDome, ref effect, false, ref treeMap);
                        currentRace.LoadContent(ref texture4, ref texture1, ref texture2, ref texture3, ref waterBumpMap, ref camera[0], ref currentState, ref chosenCollision);

                        gamePlayers = new Player[2];
                        gamePlayers[0] = new Player(multiMenu.Player1, currentState, 0);

                        //load appropriate car for player 1
                        if (multiMenu.Car1.Equals("Ford Mustang GT500"))
                        {
                            vehicleModels[0] = carModels.Load<Model>(mustang);
                            gamePlayers[0].Car = new FordMustangGT500(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), gamePlayers[0].Input, ref arial, ref vehicleModels[0], this);
                        }
                        else if (multiMenu.Car1.Equals("Koenigsegg CCX"))
                        {
                            vehicleModels[1] = carModels.Load<Model>(koenigsegg);
                            gamePlayers[0].Car = new KoenigseggCCX(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), gamePlayers[0].Input, ref arial, ref vehicleModels[1], this);
                        }
                        else if (multiMenu.Car1.Equals("Mercedes SLR Mclaren"))
                        {
                            vehicleModels[2] = carModels.Load<Model>(mercedes);
                            gamePlayers[0].Car = new MercedesSLRMclaren(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), gamePlayers[0].Input, ref arial, ref vehicleModels[2], this);
                        }

                        //set player 1 data
                        gamePlayers[0].Car.Player = 0;
                        gamePlayers[0].Car.DrawSpheres = true;
                        gamePlayers[0].Car.createGauge(viewPorts[0]);

                        if (multiMenu.GearBox1)
                            gamePlayers[0].Car.changeGearBoxToManual();

                        gamePlayers[1] = new Player(multiMenu.Player2, currentState, 1);

                        //load appropriate car for player 2
                        if (multiMenu.Car2.Equals("Ford Mustang GT500"))
                        {
                            vehicleModels[0] = carModels.Load<Model>(mustang);
                            gamePlayers[1].Car = new FordMustangGT500(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), gamePlayers[1].Input, ref arial, ref vehicleModels[0], this);
                        }
                        else if (multiMenu.Car2.Equals("Koenigsegg CCX"))
                        {
                            vehicleModels[1] = carModels.Load<Model>(koenigsegg);
                            gamePlayers[1].Car = new KoenigseggCCX(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), gamePlayers[1].Input, ref arial, ref vehicleModels[1], this);
                        }
                        else if (multiMenu.Car2.Equals("Mercedes SLR Mclaren"))
                        {
                            vehicleModels[2] = carModels.Load<Model>(mercedes);
                            gamePlayers[1].Car = new MercedesSLRMclaren(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), gamePlayers[1].Input, ref arial, ref vehicleModels[2], this);
                        }

                        //set player 2 data
                        gamePlayers[1].Car.Player = 1;
                        gamePlayers[1].Car.DrawSpheres = true;
                        gamePlayers[1].Car.createGauge(viewPorts[1]);

                        if (multiMenu.GearBox2)
                            gamePlayers[1].Car.changeGearBoxToManual();
                        
                        //set race and starting positions
                        currentRace.Players = 2;
                        gamePlayers[0].Car.Waypoints = currentRace.WaypointSpheres;
                        gamePlayers[0].Car.Point = 0;
                        gamePlayers[1].Car.Waypoints = currentRace.WaypointSpheres;
                        gamePlayers[0].Car.Point = 0;

                        gamePlayers[0].Car.Nos = true;
                        gamePlayers[1].Car.Nos = true;

                        //create car model for player and load the model
                        for (int i = 0; i < gamePlayers.Length; i++)
                        {
                            gamePlayers[i].Car.setHeightmapGenerator(eniviroment);
                            gamePlayers[i].Car.setNewHeightOffset(eniviroment.getHeight(cityHeightPos));

                            gamePlayers[i].Car.Position = currentRace.StartPoints[i];
                            gamePlayers[i].Car.Direction = currentRace.Direction;
                        }

                        //create racing huds
                        racingHud = new RacingHUD[2];
                        racingHud[0] = new RacingHUD(ref graphics, ref spriteBatch, ref currentRace, racingContent.Load<Texture2D>("Textures//Minimap//playerMapIndicator"), racingContent.Load<Texture2D>("Textures//Minimap//enemyMapIndicator"), racingContent.Load<Texture2D>("Textures//Minimap//waypointMapIndicator"), ref arial, ref raceInformationHUD, time, ref gamePlayers[0]);
                        racingHud[1] = new RacingHUD(ref graphics, ref spriteBatch, ref currentRace, racingContent.Load<Texture2D>("Textures//Minimap//playerMapIndicator"), racingContent.Load<Texture2D>("Textures//Minimap//enemyMapIndicator"), racingContent.Load<Texture2D>("Textures//Minimap//waypointMapIndicator"), ref arial, ref raceInformationHUD, time, ref gamePlayers[1]);
                        racingHud[0].setMapScale = 0.3f;
                        racingHud[1].setMapScale = 0.3f;
                        racingHud[0].setHUDScale = 0.6f;
                        racingHud[1].setHUDScale = 0.6f;

                        currentRace.NextWayPoint = currentRace.WayPoints[0];

                        mainMenu.ScreenState = ScreenState.TransitionOff;
                        currentState.CurrentState = GameState.Racing;
                    }

                    if (currentState.CurrentState == GameState.InGameMenu)
                    {
                        mainMenu.Status = true;
                        mainMenu.GameStateChanged = false;
                        mainMenu.ScreenState = ScreenState.TransitionOn;
                        currentState.CurrentState = GameState.MainMenu;
                    }

                    loading.Active = false;
                }

                #endregion

                #region OptionsMenu

                //if in options menu and game state has changed
                else if (options != null && options.GameStateChanged)
                {
                    currentState.CurrentState = options.NextState;

                    if (currentState.CurrentState == GameState.InGameMenu)
                    {
                        options.GameStateChanged = false;
                        options.Status = false;

                        if (menuIndex == 0)
                        {
                            mainMenu.Status = true;
                            mainMenu.GameStateChanged = false;
                            mainMenu.ScreenState = ScreenState.TransitionOn;
                            currentState.CurrentState = GameState.MainMenu;
                        }
                        else if (menuIndex == 1)
                        {
                            roamMenu.ScreenState = ScreenState.TransitionOn;
                            roamMenu.Status = true;
                            roamMenu.GameStateChanged = false;
                            loadMediaPlayerContent();
                        }
                        else if (menuIndex == 2)
                        {
                            inRaceMenu.ScreenState = ScreenState.TransitionOn;
                            inRaceMenu.Status = true;
                            inRaceMenu.GameStateChanged = false;
                            loadMediaPlayerContent();
                        }
                        else if (menuIndex == 3)
                        {
                            inMultiRaceMenu.ScreenState = ScreenState.TransitionOn;
                            inMultiRaceMenu.Status = true;
                            inMultiRaceMenu.GameStateChanged = false;
                            loadMediaPlayerContent();
                        }
                    }
                    loading.Active = false;
                }

                #endregion

                #region RaceMenu

                //if in race marker menu and game state has changed
                else if (raceMenu != null && raceMenu.GameStateChanged)
                {
                    raceMenu.GameStateChanged = false;
                    raceMenu.Status = false;
                    currentState.CurrentState = raceMenu.NextState;

                    if (currentState.CurrentState == GameState.Racing)
                    {
                        input.reset();
                        if (eniviroment != null)
                        {
                            eniviroment.Dispose();
                            eniviroment = null;
                        }
                        LoadContent();
                        gamePlayers[0].Car.Waypoints = currentRace.WaypointSpheres;
                        gamePlayers[0].Car.DrawSpheres = true;
                        gamePlayers[0].Car.Point = 0;
                        gamePlayers[0].Car.MaxCollisionHeight = currentRace.heightCollision;
                    }

                    else
                        markerCollisionTime = gameTime.TotalGameTime;

                    loading.Active = false;
                }

                #endregion

                #region RoamMenu

                //if in in free roam menu and game state has changed
                else if (roamMenu != null && roamMenu.GameStateChanged)
                {
                    roamMenu.GameStateChanged = false;
                    roamMenu.Status = false;
                    
                    currentState.CurrentState = roamMenu.NextState;

                    /*if (currentState.CurrentState == GameState.Racing)
                    {
                        LoadContent();
                        gamePlayers[0].Car.DrawSpheres = true;
                        gamePlayers[0].Car.Point = 0;
                    }*/

                    if (currentState.CurrentState == GameState.MainMenu)
                    {
                        racemarkersLoaded = false;
                        LoadContent();

                        if(media != null)
                            media.Stop();

                        media = null;
                    }

                    if (currentState.CurrentState == GameState.InGameMenu)
                    {
                        string[] optionMenuItems = { "Controls", "Sound FX", "Music", "music Volume", "Effects Volume", "GearBox", "Back" };
                        options = new OptionsMenu(ref graphics, this, ref spriteBatch, ref gameFont, ref optionMenuItems, ref input, ref optionsBackground, ref currentState, "Options Menu", ref currentRace, ref gamePlayers[0], ref gameTime, ref controls, ref optionsManager);
                        options.Status = true;
                        menuIndex = 1;
                    }

                    loading.Active = false;
                }

                #endregion

                #region FinishMenu

                //if in in race finish menu and game state has changed
                else if (finishMenu != null && finishMenu.GameStateChanged)
                {
                    finishMenu.GameStateChanged = false;
                    finishMenu.Status = false;
                    currentState.CurrentState = finishMenu.NextState;

                    if (currentState.CurrentState == GameState.Racing)
                    {
                        gamePlayers[0].CurrentCash += finishMenu.GetBet;

                        if (currentRace.AIenabled)
                        {
                            currentRace.ResetEnemy();

                            if (aiVehicle == 0)
                            {
                                if (aiModels[0] == null)
                                    aiModels[0] = aiCarModels.Load<Model>(mustang);

                                currentRace.AIBots[0].Car = new FordMustangGT500(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), currentRace.AIBots[0].Input, ref arial, ref aiModels[0], this);
                            }
                            else if (aiVehicle == 1)
                            {
                                if (aiModels[1] == null)
                                    aiModels[1] = aiCarModels.Load<Model>(koenigsegg);

                                currentRace.AIBots[0].Car = new KoenigseggCCX(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), currentRace.AIBots[0].Input, ref arial, ref aiModels[1], this);
                            }
                            else if (aiVehicle == 2)
                            {
                                if (aiModels[2] == null)
                                    aiModels[2] = aiCarModels.Load<Model>(mercedes);

                                currentRace.AIBots[0].Car = new MercedesSLRMclaren(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), currentRace.AIBots[0].Input, ref arial, ref aiModels[2], this);
                            }

                            currentRace.AIBots[0].Car.Position = currentRace.StartPoints[1];
                            currentRace.EnemyAI = currentRace.EnemyAIReset;
                            currentRace.AIBots[0].NewPosition = Vector3.Zero;
                            currentRace.AIBots[0].Car.Speed = 0f;

                            currentRace.AIBots[0].Car.setHeightmapGenerator(currentCollidedRaceMarker.MapGenerator);
                            currentRace.AIBots[0].Car.setNewHeightOffset(currentCollidedRaceMarker.MapGenerator.getHeight(cityHeightPos));
                            currentRace.AIBots[0].Input.reset();
                            currentRace.AIBots[0].Car.Input = currentRace.AIBots[0].Input;
                        }
                        
                        

                        counter = 0;
                        leaderName = "";
                        leaderWaypoint = 0;
                        currentRace.CurrentWaypoint = 0;
                        currentRace.Finished = false;

                        gamePlayers[0].Car.Position = currentRace.StartPoints[0];
                        gamePlayers[0].Car.Waypoints = currentRace.WaypointSpheres;
                        gamePlayers[0].Car.DrawSpheres = true;
                        gamePlayers[0].Car.Point = 0;
                        gamePlayers[0].Car.MaxCollisionHeight = currentRace.heightCollision;
                        gamePlayers[0].Car.Motor.GearBoxUsed.CurrentGear = 0;
                        racingHud[0] = new RacingHUD(ref graphics, ref spriteBatch, ref currentRace, racingContent.Load<Texture2D>("Textures//Minimap//playerMapIndicator"), racingContent.Load<Texture2D>("Textures//Minimap//enemyMapIndicator"), racingContent.Load<Texture2D>("Textures//Minimap//waypointMapIndicator"), ref arial, ref raceInformationHUD, time, ref gamePlayers[0]);
                        loading.Active = false;

                        //reset NOS
                        if (gamePlayers[0].Car.Nos)
                            gamePlayers[0].Car.NosAmmount = 300;
                    }

                    if (currentState.CurrentState == GameState.InGame)
                    {
                        aiModels = new Model[3];
                        aiCarModels.Unload();

                        if (currentRace.Winner == gamePlayers[0].Name)
                        {
                            gamePlayers[0].Level = gamePlayers[0].Level + 1;
                            gamePlayers[0].CurrentCash += currentRace.Winnings;
                        }

                        LoadContent();

                        gamePlayers[0].Car.Position = savedState;
                        gamePlayers[0].Car.Direction = savedDirection;
                        gamePlayers[0].Car.Speed = 0f;
                        gamePlayers[0].Car.DrawSpheres = false;
                        gamePlayers[0].Car.Point = 0;
                        gamePlayers[0].Car.HeightMapCollision = false;
                        gamePlayers[0].Car.Motor.GearBoxUsed.CurrentGear = 0;

                        if (currentRace.Winner == gamePlayers[0].Name)
                        {
                            ingameRaceMarkers.Remove(currentCollidedRaceMarker);// removeMarker = 1;
                        }
                        else
                            markerCollisionTime = gameTime.TotalGameTime;

                        loading.Active = false;
                    }
                }

                #endregion

                #region MultiPlayerFinishMenu

                //if in in race finish menu and game state has changed
                else if (mFinishMenu != null && mFinishMenu.GameStateChanged)
                {
                    mFinishMenu.GameStateChanged = false;
                    mFinishMenu.Status = false;
                    currentState.CurrentState = mFinishMenu.NextState;

                    if (currentState.CurrentState == GameState.MainMenu)
                    {
                        LoadContent();
                        loading.Active = false;

                        if(media != null)
                            media.Stop();

                        media = null;
                    }
                }

                #endregion

                #region InRaceMenu

                //if in in race menu and game state has changed
                else if (inRaceMenu != null && inRaceMenu.GameStateChanged)
                {
                    inRaceMenu.GameStateChanged = false;
                    inRaceMenu.Status = false;
                    currentState.CurrentState = inRaceMenu.NextState;

                    if (currentState.CurrentState == GameState.Racing)
                        loading.Active = false;

                    if (currentState.CurrentState == GameState.Loading)
                    {
                        if (eniviroment != null)
                            eniviroment.Dispose();
                        currentState.CurrentState = GameState.Racing;
                        gamePlayers[0].CurrentCash += currentRace.Winnings / 4;
                        gamePlayers[0].Car.Position = currentRace.StartPoints[0];
                        counter = 0;
                        leaderName = "";
                        leaderWaypoint = 0;
                        currentRace.LeaderName = "";
                        currentRace.CurrentWaypoint = 0;
                        currentRace.Finished = false;

                        if (currentRace.AIenabled)
                        {
                            currentRace.ResetEnemy();

                            if (aiVehicle == 0)
                            {
                                if (aiModels[0] == null)
                                    aiModels[0] = aiCarModels.Load<Model>(mustang);

                                currentRace.AIBots[0].Car = new FordMustangGT500(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), currentRace.AIBots[0].Input, ref arial, ref aiModels[0], this);
                            }
                            else if (aiVehicle == 1)
                            {
                                if (aiModels[1] == null)
                                    aiModels[1] = aiCarModels.Load<Model>(koenigsegg);

                                currentRace.AIBots[0].Car = new KoenigseggCCX(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), currentRace.AIBots[0].Input, ref arial, ref aiModels[1], this);
                            }
                            else if (aiVehicle == 2)
                            {
                                if (aiModels[2] == null)
                                    aiModels[2] = aiCarModels.Load<Model>(mercedes);

                                currentRace.AIBots[0].Car = new MercedesSLRMclaren(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), currentRace.AIBots[0].Input, ref arial, ref aiModels[2], this);
                            }

                            //currentRace.AIBots[0].Car = new FordMustangGT500(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), currentRace.AIBots[0].Input, ref arial, ref aiModels[0], this);
                            currentRace.AIBots[0].Car.Position = currentRace.StartPoints[1];
                            currentRace.EnemyAI = currentRace.EnemyAIReset;
                            currentRace.AIBots[0].NewPosition = Vector3.Zero;
                            currentRace.AIBots[0].Car.Speed = 0f;

                            currentRace.AIBots[0].Car.setHeightmapGenerator(currentCollidedRaceMarker.MapGenerator);
                            currentRace.AIBots[0].Car.setNewHeightOffset(currentCollidedRaceMarker.MapGenerator.getHeight(cityHeightPos));
                            currentRace.AIBots[0].Input.reset();
                            currentRace.AIBots[0].Car.Input = currentRace.AIBots[0].Input;
                        }

                        gamePlayers[0].Car.Waypoints = currentRace.WaypointSpheres;
                        gamePlayers[0].Car.DrawSpheres = true;
                        gamePlayers[0].Car.Point = 0;
                        gamePlayers[0].Car.MaxCollisionHeight = currentRace.heightCollision;
                        racingHud[0] = new RacingHUD(ref graphics, ref spriteBatch, ref currentRace, racingContent.Load<Texture2D>("Textures//Minimap//playerMapIndicator"), racingContent.Load<Texture2D>("Textures//Minimap//enemyMapIndicator"), racingContent.Load<Texture2D>("Textures//Minimap//waypointMapIndicator"), ref arial, ref raceInformationHUD, time, ref gamePlayers[0]);
                        loading.Active = false;

                        //reset NOS
                        if (gamePlayers[0].Car.Nos)
                            gamePlayers[0].Car.NosAmmount = 300;
                    }

                    if (currentState.CurrentState == GameState.InGameMenu)
                    {
                        string[] optionMenuItems = { "Controls", "Sound FX", "Music", "Music Volume", "Effects Volume", "GearBox", "Back" };
                        options = new OptionsMenu(ref graphics, this, ref spriteBatch, ref gameFont, ref optionMenuItems, ref input, ref optionsBackground, ref currentState, "Options Menu", ref currentRace, ref gamePlayers[0], ref gameTime, ref controls, ref optionsManager);
                        options.Status = true;
                        menuIndex = 2;
                        loading.Active = false;
                    }
                    if (currentState.CurrentState == GameState.InGame)
                    {

                        markerCollisionTime = gameTime.TotalGameTime;
                        LoadContent();

                        gamePlayers[0].Car.Position = savedState;
                        gamePlayers[0].Car.Direction = savedDirection;
                        gamePlayers[0].Car.Speed = 0f;
                        gamePlayers[0].Car.DrawSpheres = false;
                        gamePlayers[0].Car.HeightMapCollision = false;

                        loading.Active = false;
                    }
                }

                #endregion

                #region InMultiRaceMenu

                //if in in race menu and game state has changed
                else if (inMultiRaceMenu != null && inMultiRaceMenu.GameStateChanged)
                {
                    inMultiRaceMenu.GameStateChanged = false;
                    inMultiRaceMenu.Status = false;
                    currentState.CurrentState = inMultiRaceMenu.NextState;

                    if (currentState.CurrentState == GameState.Racing)
                        loading.Active = false;

                    if (currentState.CurrentState == GameState.Loading)
                    {
                        //if (eniviroment != null)
                        //eniviroment.Dispose();
                        currentState.CurrentState = GameState.Racing;
                        for (int a = 0; a < gamePlayers.Length; a++)
                        {
                            gamePlayers[a].Car.Position = currentRace.StartPoints[a];
                            gamePlayers[a].Car.Waypoints = currentRace.WaypointSpheres;
                            gamePlayers[a].Car.DrawSpheres = true;
                            gamePlayers[a].Car.Point = 0;
                            gamePlayers[a].Car.MaxCollisionHeight = currentRace.heightCollision;
                            racingHud[a] = new RacingHUD(ref graphics, ref spriteBatch, ref currentRace, racingContent.Load<Texture2D>("Textures//Minimap//playerMapIndicator"), racingContent.Load<Texture2D>("Textures//Minimap//enemyMapIndicator"), racingContent.Load<Texture2D>("Textures//Minimap//waypointMapIndicator"), ref arial, ref raceInformationHUD, time, ref gamePlayers[a]);
                            racingHud[a].setMapScale = 0.3f;
                            racingHud[a].setHUDScale = 0.6f;
                            racingHud[a].leader = "";

                            //reset NOS
                            if (gamePlayers[a].Car.Nos)
                                gamePlayers[a].Car.NosAmmount = 300;
                        }

                        counter = 0;
                        leaderName = "";
                        leaderWaypoint = 0;
                        currentRace.LeaderName = "";
                        currentRace.CurrentWaypoint = 0;
                        currentRace.Finished = false;

                        /*if (currentRace.AIenabled)
                        {
                            currentRace.ResetEnemy();
                            currentRace.AIBots[0].Car = new MitsubishiLancerEvoX(ref graphics, racingContent, ref spriteBatch, new Vector3(0, 0, -3), currentRace.AIBots[0].Input, ref arial, ref aiModels[0], this);
                            currentRace.AIBots[0].Car.Position = currentRace.StartPoints[1];
                            currentRace.EnemyAI = currentRace.EnemyAIReset;
                            currentRace.AIBots[0].NewPosition = Vector3.Zero;
                            currentRace.AIBots[0].Car.Speed = 0f;

                            currentRace.AIBots[0].Car.setHeightmapGenerator(currentCollidedRaceMarker.MapGenerator);
                            currentRace.AIBots[0].Car.setNewHeightOffset(currentCollidedRaceMarker.MapGenerator.getHeight(cityHeightPos));
                            currentRace.AIBots[0].Input.reset();
                            currentRace.AIBots[0].Car.Input = currentRace.AIBots[0].Input;
                        }*/

                        loading.Active = false;
                    }
                    if (currentState.CurrentState == GameState.MainMenu)
                    {
                        LoadContent();

                        loading.Active = false;
                    }
                }

                #endregion

                #region ShopMenu

                //if in shop marker menu and game state has changed
                else if (shopmenu != null && shopmenu.GameStateChanged)
                {
                    shopmenu.GameStateChanged = false;
                    shopmenu.Status = false;
                    currentState.CurrentState = shopmenu.NextState;

                    if (currentState.CurrentState == GameState.InShop)
                        LoadContent();

                    else
                        markerCollisionTime = gameTime.TotalGameTime;

                    loading.Active = false;
                }

                #endregion

                #region StoryMenu

                else if (story != null && story.GameStateChanged)
                {
                    currentState.CurrentState = story.NextState;
                    story.Status = false;
                    story.GameStateChanged = false;
                    storyDisplay = false;
                    finalDisplay = false;

                    loading.Active = false;
                }

                #endregion

                #region CarShop

                //if in shop and game state has changed
                else if (carShop != null && carShop.ChangeState)
                {
                    currentState.CurrentState = GameState.InGame;
                    carShop.ChangeState = false;
                    markerCollisionTime = gameTime.TotalGameTime;

                    LoadContent();

                    gamePlayers[0].Car.Position = new Vector3(-306.9f, 4.0f, -190.1f);
                    gamePlayers[0].Car.Direction = savedDirection;
                    gamePlayers[0].Car.Speed = 0f;

                    loading.Active = false;
                }

                #endregion

                #region CurrentRace

                else if (currentRace != null && currentRace.Finished)
                {
                    if (gamePlayers.Length < 2)
                    {
                        currentState.CurrentState = GameState.InGameMenu;
                        string[] raceFinishMenuItems = { "Continue", "Restart" };
                        finishMenu = new RaceFinishMenu(ref graphics, this, ref spriteBatch, ref gameFont, ref raceFinishMenuItems, ref input, ref raceFinishBackground, ref currentState, "Post Race Menu", ref currentRace, ref gameTime);
                        finishMenu.Status = true;
                        finishMenu.FinishTime = finishTime;

                        for (int a = 0; a < currentRace.AIBots.Length; a++)
                            currentRace.AIBots[a].Car.PauseSound();
                    }

                    else
                    {
                        currentState.CurrentState = GameState.InGameMenu;
                        string[] raceFinishMenuItems = { "Continue"};
                        mFinishMenu = new MultiplayerRaceFinishMenu(ref graphics, this, ref spriteBatch, ref gameFont, ref raceFinishMenuItems, ref input, ref raceFinishBackground, ref currentState, "Post Race Menu", ref currentRace, ref gameTime);
                        mFinishMenu.Status = true;
                        mFinishMenu.FinishTime = finishTime;

                        for (int i = 0; i < gamePlayers.Length; i++)
                            gamePlayers[i].Car.PauseSound();

                        input.Player = 0;

                        //eniviroment = null;
                    }

                    loading.Active = false;
                }

                #endregion
            }
            #endregion

            #region Game in MainMenu state
            if (currentState.CurrentState == GameState.MainMenu)
            {
                mainMenu.Update(gameTime);
                if (mainMenu.Exit)
                {
                    if (media != null)
                    {
                        media.Stop();
                        media.Active = false;
                    }
                    Exit();
                }

                if (mainMenu.GameStateChanged)
                {
                    if (mainMenu.ScreenState != ScreenState.Deactivated)
                        mainMenu.ScreenState = ScreenState.TransitionOff;
                    else
                    {
                        currentState.CurrentState = GameState.Loading;
                        GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
                        loading.Active = true;
                    }
                }
            }
            #endregion

            #region Game in InGameMenu state
            if (currentState.CurrentState == GameState.InGameMenu)
            {
                if (raceMenu != null && raceMenu.Status)
                {
                    raceMenu.Update(gameTime);

                    if (raceMenu.GameStateChanged)
                    {
                        if (raceMenu.ScreenState != ScreenState.Deactivated)
                            raceMenu.ScreenState = ScreenState.TransitionOff;

                        else
                        {
                            currentState.CurrentState = GameState.Loading;
                            loading.Active = true;
                        }
                    }
                }

                else if (multiMenu != null && multiMenu.Status)
                {
                    multiMenu.Update(gameTime);

                    if (multiMenu.GameStateChanged)
                    {
                        if (multiMenu.ScreenState != ScreenState.Deactivated)
                            multiMenu.ScreenState = ScreenState.TransitionOff;

                        else
                        {
                            currentState.CurrentState = GameState.Loading;
                            loading.Active = true;
                        }
                    }
                }

                else if (shopmenu != null && shopmenu.Status)
                {
                    shopmenu.Update(gameTime);

                    if (shopmenu.GameStateChanged)
                    {
                        if (shopmenu.ScreenState != ScreenState.Deactivated)
                            shopmenu.ScreenState = ScreenState.TransitionOff;

                        else
                        {
                            currentState.CurrentState = GameState.Loading;
                            input.reset();
                            loading.Active = true;
                        }
                    }
                }

                else if (story != null && story.Status)
                {
                    story.Update(gameTime);

                    if (story.GameStateChanged)
                    {
                        if (story.ScreenState != ScreenState.Deactivated)
                            story.ScreenState = ScreenState.TransitionOff;

                        else
                        {
                            currentState.CurrentState = GameState.Loading;
                            //loading.Active = true;
                        }
                    }
                }

                else if (finishMenu != null && finishMenu.Status)
                {
                    finishMenu.Update(gameTime);
                    gamePlayers[0].Car.PauseSound();

                    if (finishMenu.GameStateChanged)
                    {

                        if (finishMenu.ScreenState != ScreenState.Deactivated)
                            finishMenu.ScreenState = ScreenState.TransitionOff;

                        else
                        {
                            currentState.CurrentState = GameState.Loading;
                            loading.Active = true;
                        }
                    }
                }

                else if (mFinishMenu != null && mFinishMenu.Status)
                {
                    mFinishMenu.Update(gameTime);
                    gamePlayers[0].Car.PauseSound();

                    if (mFinishMenu.GameStateChanged)
                    {

                        if (mFinishMenu.ScreenState != ScreenState.Deactivated)
                            mFinishMenu.ScreenState = ScreenState.TransitionOff;

                        else
                        {
                            currentState.CurrentState = GameState.Loading;
                            loading.Active = true;
                        }
                    }
                }

                else if (inRaceMenu != null && inRaceMenu.Status)
                {
                    inRaceMenu.Update(gameTime); 
                    
                    //pause AI sounds
                    for (int a = 0; a < currentRace.AIBots.Length; a++)
                        currentRace.AIBots[a].Car.PauseSound();

                    if (inRaceMenu.GameStateChanged)
                    {
                        if (inRaceMenu.ScreenState != ScreenState.Deactivated)
                            inRaceMenu.ScreenState = ScreenState.TransitionOff;

                        else
                        {
                            currentState.CurrentState = GameState.Loading;
                            loading.Active = true;
                        }
                    }
                }

                else if (inMultiRaceMenu != null && inMultiRaceMenu.Status)
                {
                    inMultiRaceMenu.Update(gameTime);

                    //pause AI sounds
                    for (int a = 0; a < gamePlayers.Length; a++)
                        gamePlayers[a].Car.PauseSound();

                    if (inMultiRaceMenu.GameStateChanged)
                    {
                        if (inMultiRaceMenu.ScreenState != ScreenState.Deactivated)
                            inMultiRaceMenu.ScreenState = ScreenState.TransitionOff;

                        else
                        {
                            currentState.CurrentState = GameState.Loading;
                            loading.Active = true;
                        }
                    }
                }

                else if (roamMenu != null && roamMenu.Status)
                {
                    roamMenu.Update(gameTime);

                    if (roamMenu.GameStateChanged)
                    {
                        if (roamMenu.ScreenState != ScreenState.Deactivated)
                            roamMenu.ScreenState = ScreenState.TransitionOff;

                        else
                        {
                            currentState.CurrentState = GameState.Loading;
                            loading.Active = true;
                        }
                    }
                }
                if (options != null && options.Status)
                {
                    options.Update(gameTime);

                    if (options.GameStateChanged)
                    {
                        GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

                        currentState.CurrentState = GameState.Loading;
                        GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
                        loading.Active = true;
                    }
                }
            }
            #endregion

            #region Game in InGame state
            if (currentState.CurrentState == GameState.InGame)
            {
                gamePlayers[0].Car.update(gameTime, true, optionsManager.SoundFXEnabled);

                if (gamePlayers[0].Car.Position.Y <= 1)
                {
                    gamePlayers[0].Car.resetPosition();
                }

                if ((gamePlayers[0].Level == 4 || gamePlayers[0].Level == 8))
                {
                    storyDisplay = true;
                    if (gamePlayers[0].Level == 4)
                        gamePlayers[0].Car.Nos = true;

                    if (story.PlayerLevel != gamePlayers[0].Level)
                    {
                        story.PlayerLevel = gamePlayers[0].Level;
                        story.Story();
                        if(!finalDisplay)
                            finalDisplay = true;
                    }
                }

                camera[0].Update(gameTime);
                UpdateCameraChaseTarget(0);
                
                if (input.Escape)
                {
                    gamePlayers[0].Car.PauseSound();
                    currentState.CurrentState = GameState.InGameMenu;
                    string[] roamMenuItems = { "Continue", "Options", "MainMenu" };
                    roamMenu = new FreeRoamMenu(ref graphics, this, ref spriteBatch, ref gameFont, ref roamMenuItems, ref input, ref roamMenuBackground, ref currentState, "Free Roam Menu", ref currentRace, ref gamePlayers[0], ref gameTime);
                    roamMenu.Status = true;
                }
                #region City Collisions
                if (city.checkCollision(gamePlayers[0].Car.Sphere))
                {
                    Vector3 carNormal = gamePlayers[0].Car.Direction;

                    if (gamePlayers[0].Car.Speed < 0)
                        carNormal = carNormal * -1;

                    Vector3 Normal = city.Normal;
                    Vector3 Normal2 = city.Normal2;
                    angle = Vector3.Dot(carNormal, Normal) / (carNormal.Length() * Normal.Length());
                    angle2 = Vector3.Dot(carNormal, Normal2) / (carNormal.Length() * Normal2.Length());
                    //angle = Math.Acos(angle);

                    // check direction
                    if (angle >= 0 && angle2 >= 0)
                    {
                        if (gamePlayers[0].Car.Position.X > city.Position.X)
                        {
                            // Face 1 - towards shop
                            if ((angle < 0.7f && angle >= 0) && (angle2 > 0.71f && angle2 <= 1))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                gamePlayers[0].Car.Speed = 0.0f;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                            else if ((angle >= 0.7f) && (angle2 <= 0.71f))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                if (gamePlayers[0].Car.Speed < 0)
                                    gamePlayers[0].Car.Direction = Normal * -1;
                                else
                                    gamePlayers[0].Car.Direction = Normal;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                        }

                        else if (gamePlayers[0].Car.Position.X < city.Position.X)
                        {

                            // Face 4 - map marker side }
                            if ((angle > 0.7f && angle <= 1) && (angle2 < 0.71f))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                gamePlayers[0].Car.Speed = 0.0f;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                            else if ((angle <= 0.7f) && (angle2 >= 0.71f))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                if (gamePlayers[0].Car.Speed < 0)
                                    gamePlayers[0].Car.Direction = Normal2 *-1;
                                else
                                    gamePlayers[0].Car.Direction = Normal2;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                            else if ((angle <= 0.7f) && (angle2 <= 0.71f))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                if (gamePlayers[0].Car.Speed < 0)
                                    gamePlayers[0].Car.Direction = Normal2 ;
                                else
                                    gamePlayers[0].Car.Direction = Normal2* -1;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                        }
                    }
                    else if (angle >= 0 && angle2 <= 0)
                    {
                        if (gamePlayers[0].Car.Position.X > city.Position.X)
                        {
                            // Face 2 - away from shop
                            if ((angle < 0.7f) && (angle2 < -0.71f))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                gamePlayers[0].Car.Speed = 0.0f;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                            else if ((angle >= 0.7f) && (angle2 >= -0.71f))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                if (gamePlayers[0].Car.Speed < 0)
                                    gamePlayers[0].Car.Direction = Normal * -1;
                                else
                                    gamePlayers[0].Car.Direction = Normal;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                        }

                        else if ((gamePlayers[0].Car.Position.X < city.Position.X))
                        {
                            // Face 4 - map marker side
                            if ((angle > 0.7f && angle <= 1) && (angle2 > -0.71f && angle2 <= 0))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                gamePlayers[0].Car.Speed = 0.0f;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                            else if ((angle > 0.7f && angle <= 1) && (angle2 < 0.71f && angle2 >= 0))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                gamePlayers[0].Car.Speed = 0.0f;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                            else if ((angle < 0.7f) && (angle2 < -0.71f))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                if (gamePlayers[0].Car.Speed < 0)
                                    gamePlayers[0].Car.Direction = Normal2;
                                else
                                    gamePlayers[0].Car.Direction = Normal2 * -1;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                        }
                        
                    }

                    else if (angle <= 0 && angle2 <= 0)
                    {
                        if (gamePlayers[0].Car.Position.X < city.MaxPosition.X)
                        {

                            // Face 2 - away from shop
                            if ((angle > -0.7f) && (angle2 < -0.71f))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                gamePlayers[0].Car.Speed = 0.0f;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                            else if ((angle < 0.7f) && (angle2 < -0.71f))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                gamePlayers[0].Car.Speed = 0.0f;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                            else if ((angle < -0.7f) && (angle2 > -0.71f))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                if (gamePlayers[0].Car.Speed < 0)
                                    gamePlayers[0].Car.Direction = Normal;
                                else
                                    gamePlayers[0].Car.Direction = Normal * -1;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                        }
                        else if (gamePlayers[0].Car.Position.X > city.MaxPosition.X)
                        {

                            // Face 3 - opposite map marker
                            if ((angle < -0.7f && angle >= -1) && (angle2 < 0.71f && angle2 >= 0))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                gamePlayers[0].Car.Speed = 0.0f;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                            else if ((angle < -0.7f && angle >= -1) && (angle2 > -0.71f && angle2 <= 0))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                gamePlayers[0].Car.Speed = 0.0f;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                            else if ((angle > -0.7f) && (angle2 < -0.71f))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                if (gamePlayers[0].Car.Speed < 0)
                                    gamePlayers[0].Car.Direction = Normal2;
                                else
                                    gamePlayers[0].Car.Direction = Normal2 * -1;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                        }
                    }
                    else if (angle <= 0 && angle2 >= 0)
                    {
                        if (gamePlayers[0].Car.Position.X < city.MaxPosition.X)
                        {
                            // Face 1 - towards shop
                            if ((angle < 0.7f && angle >= 0) && (angle2 > 0.71f && angle2 <= 1))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                gamePlayers[0].Car.Speed = 0.0f;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                            else if ((angle > -0.7f && angle <= 0) && (angle2 > 0.71f && angle2 <= 1))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                gamePlayers[0].Car.Speed = 0.0f;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                            else if ((angle < -0.7f) && (angle2 < 0.71f))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                if (gamePlayers[0].Car.Speed < 0)
                                    gamePlayers[0].Car.Direction = Normal;
                                else
                                    gamePlayers[0].Car.Direction = Normal * -1;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                        }
                        if (gamePlayers[0].Car.Position.X > city.MaxPosition.X)
                        {
                            // Face 3 - opposite map marker
                            if ((angle < -0.7f && angle >= -1) && (angle2 < 0.71f && angle2 >= 0))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                gamePlayers[0].Car.Speed = 0.0f;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                            else if ((angle < -0.7f && angle >= -1) && (angle2 > -0.71f && angle2 <= 0))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                gamePlayers[0].Car.Speed = 0.0f;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                            else if ((angle > -0.7f) && (angle2 > 0.71f))
                            {
                                gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                                if (gamePlayers[0].Car.Speed < 0)
                                    gamePlayers[0].Car.Direction = Normal2 * -1;
                                else
                                    gamePlayers[0].Car.Direction = Normal2;

                                if (optionsManager != null && optionsManager.SoundFXEnabled)
                                {
                                    gamePlayers[0].Car.PlayCollisionSound();
                                    gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.5f;
                                }
                            }
                        }
                    }
                }
                #endregion

                //check car collision with each marker
                checkCollisions(gameTime, 0);

                //check camera point collisions in city
                if (city.checkCollision(camera[0].Position) && counterTimer == 0)
                {
                    graphics.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
                    camera[0].Position = camera[0].PreviousPosition;
                }
                else
                    if (counterTimer > 0)
                        counterTimer--;

                //check collisions in outside enviroment
                if (gamePlayers[0].Car.Position.Y > 10)
                {
                    gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                    gamePlayers[0].Car.Speed = 0;
                }
            }
            #endregion

            #region Game in Racing state
            if (currentState.CurrentState == GameState.Racing)
            {
                if (gamePlayers[0].Level == 10)
                {
                    storyDisplay = true;
                    if (story.PlayerLevel != gamePlayers[0].Level)
                    {
                        story.PlayerLevel = gamePlayers[0].Level;
                        story.Story();
                        if (!finalDisplay)
                        {
                            finalDisplay = true;
                        }
                    }
                }
                for (int a = 0; a < gamePlayers.Length; a++)
                {
                    input = gamePlayers[a].Input;
                    input.update(Keyboard.GetState(), gameTime);

                    gamePlayers[a].Car.update(gameTime, racingHud[a].CanRace, optionsManager.SoundFXEnabled);
                    gamePlayers[a].Car.PassWaypoint(gamePlayers[a]);
                    gamePlayers[a].Car.HeightMapCollision = true;

                    if (!racingHud[a].CanRace)
                        gamePlayers[a].Car.resetGears();

                    for (int i = 0; i < gamePlayers.Length; i++)
                    {
                        if (gamePlayers[a].Car.Point > leaderWaypoint)
                        {
                            leaderWaypoint = gamePlayers[a].Car.Point;
                            leaderName = gamePlayers[a].Name;
                        }
                    }

                    racingHud[0].updateLeaderName(leaderName);
                    if(gamePlayers.Length>1)
                        racingHud[1].updateLeaderName(leaderName);

                    racingHud[a].OffTrack = gamePlayers[a].Car.OffTrack;
                    racingHud[a].updateCurrentWaypoint(gamePlayers[a].Car.Point);

                    if(gamePlayers.Length > 1)
                        racingHud[a].update(gamePlayers[0].Car.Position, gamePlayers[1].Car.Position, gameTime);
                    else
                        racingHud[a].update(gamePlayers[0].Car.Position, Vector3.Zero, gameTime);

                    camera[a].Update(gameTime);
                    UpdateCameraChaseTarget(a);

                    currentRace.PassWaypoint(gamePlayers[a]);
                    if(currentRace.AIenabled)
                        currentRace.PassWaypoint(currentRace.AIBots[0].Car);
                    currentRace.Players = gamePlayers.Length;

                    currentRace.updateAI(gameTime, racingHud[a].CanRace, optionsManager.SoundFXEnabled, ref gamePlayers[0], optionsManager.EffectVolume);

                    //check playr and ai collisions
                    if (currentRace.AIenabled)
                    {
                        if (gamePlayers[0].Car.Sphere.Contains(currentRace.Enemies[0].Car.Sphere) != ContainmentType.Disjoint)
                        {
                            Vector3 playerDirection = gamePlayers[0].Car.Direction;
                            Vector3 playerPosition = gamePlayers[0].Car.Position;
                            float playerSpeed = gamePlayers[0].Car.Speed;

                            double angleBetweenDirections = Math.Acos(Vector3.Dot(playerDirection, currentRace.Enemies[0].Car.Direction) / (playerDirection.Length() * currentRace.Enemies[0].Car.Direction.Length()));

                            gamePlayers[0].Car.Position = gamePlayers[0].Car.Position + ((gamePlayers[0].Car.Position - currentRace.Enemies[0].Car.Position) * 0.05f);
                            currentRace.Enemies[0].Car.Position = currentRace.Enemies[0].Car.Position + ((currentRace.Enemies[0].Car.Position - playerPosition) * 0.05f);

                            if (angleBetweenDirections < MathHelper.ToRadians(45))
                            {
                                gamePlayers[0].Car.Direction += currentRace.Enemies[0].Car.Direction;
                                currentRace.Enemies[0].Car.Direction += playerDirection;
                            }
                            else
                            {
                                gamePlayers[0].Car.Direction += currentRace.Enemies[0].Car.Direction;
                                currentRace.Enemies[0].Car.Direction += playerDirection;
                                gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.95f;
                                currentRace.Enemies[0].Car.Speed = currentRace.Enemies[0].Car.Speed * 0.95f;
                            }
                        }
                    }

                    else if (gamePlayers[1] != null)
                    {
                        if (gamePlayers[0].Car.Sphere.Contains(gamePlayers[1].Car.Sphere) != ContainmentType.Disjoint)
                        {
                            Vector3 playerDirection = gamePlayers[0].Car.Direction;
                            Vector3 playerPosition = gamePlayers[0].Car.Position;
                            float playerSpeed = gamePlayers[0].Car.Speed;

                            double angleBetweenDirections = Math.Acos(Vector3.Dot(playerDirection, gamePlayers[1].Car.Direction) / (playerDirection.Length() * gamePlayers[1].Car.Direction.Length()));

                            gamePlayers[0].Car.Position = gamePlayers[0].Car.Position + ((gamePlayers[0].Car.Position - gamePlayers[1].Car.Position) * 0.05f);
                            gamePlayers[1].Car.Position = gamePlayers[1].Car.Position + ((gamePlayers[1].Car.Position - playerPosition) * 0.05f);
                            //currentRace.Enemies[0].Car.Position = currentRace.Enemies[0].Car.Position + (currentRace.Enemies[0].Car.Direction * (currentRace.Enemies[0].Car.Speed * 0.1f));

                            if (angleBetweenDirections < MathHelper.ToRadians(45))
                            {
                                gamePlayers[0].Car.Direction += gamePlayers[1].Car.Direction;
                                gamePlayers[1].Car.Direction += playerDirection;
                            }
                            else
                            {
                                gamePlayers[0].Car.Direction += gamePlayers[1].Car.Direction;
                                gamePlayers[1].Car.Direction += playerDirection;
                                gamePlayers[0].Car.Speed = gamePlayers[0].Car.Speed * 0.95f;
                                gamePlayers[1].Car.Speed = gamePlayers[1].Car.Speed * 0.95f;
                            }
                        }
                    }

                    if (input.Escape)
                    {
                        if (gamePlayers.Length < 2)
                        {
                            gamePlayers[a].Car.PauseSound();
                            currentState.CurrentState = GameState.InGameMenu;
                            string[] inRacePauseMenu = { "Continue", "Restart", "Options", "Exit to Free Roam" };
                            inRaceMenu = new InRaceMenu(ref graphics, this, ref spriteBatch, ref gameFont, ref inRacePauseMenu, ref input, ref inRaceMenuBackground, ref currentState, "In Race Menu", ref currentRace, ref gamePlayers[0], ref gameTime);
                            inRaceMenu.Status = true;
                        }
                        else if (gamePlayers.Length == 2)
                        {
                            //gamePlayers[a].Car.PauseSound();
                            currentState.CurrentState = GameState.InGameMenu;
                            string[] inMultiRacePauseMenu = { "Continue", "Restart", "Exit to Main Menu" };
                            inMultiRaceMenu = new InMultiRaceMenu(ref graphics, this, ref spriteBatch, ref gameFont, ref inMultiRacePauseMenu, ref input, ref inRaceMenuBackground, ref currentState, "In Race Menu", ref currentRace, ref gamePlayers[0], ref gameTime);
                            inMultiRaceMenu.Status = true;
                        }
                    }

                    if (currentRace.Finished)
                    {
                        currentState.CurrentState = GameState.Loading;
                        loading.Active = true;

                        viewPorts = new Viewport[1];
                        viewPorts[0] = defaultViewport;

                        finishTime = racingHud[a].getTime;

                        break;
                    }
                }
            }
            #endregion

            #region Game in InShop state
            if (currentState.CurrentState == GameState.InShop)
            {
                if (!carShop.ChangeState)
                    carShop.Update(gameTime);
                else
                {
                    if (carShop.CarBought)
                    {
                        vehicleModels = new Model[3];
                        carModels.Unload();

                        if (carShop.Car == "Koenigsegg CCX")
                        {
                            vehicleModels[1] = carModels.Load<Model>(koenigsegg);
                            gamePlayers[0].Car = new KoenigseggCCX(ref graphics, Content, ref spriteBatch, new Vector3(0, 0, 0), input, ref arial, ref vehicleModels[1], this);
                        }
                        else if (carShop.Car == "Ford Mustang GT500")
                        {
                            vehicleModels[0] = carModels.Load<Model>(mustang);
                            gamePlayers[0].Car = new FordMustangGT500(ref graphics, Content, ref spriteBatch, new Vector3(0, 0, 0), input, ref arial, ref vehicleModels[0], this);
                        }
                        else if (carShop.Car == "Mercedes SLR Mclaren")
                        {
                            vehicleModels[2] = carModels.Load<Model>(mercedes);
                            gamePlayers[0].Car = new MercedesSLRMclaren(ref graphics, Content, ref spriteBatch, new Vector3(0, 0, 0), input, ref arial, ref vehicleModels[2], this);
                        }

                        if (gamePlayers[0].Level > 3)
                            gamePlayers[0].Car.Nos = true;

                        carShop.CarBought = false;
                        carShop.Car = "";
                    }

                    currentState.CurrentState = GameState.Loading;
                    loading.Active = true;
                }
            }
            #endregion

            base.Update(gameTime);
        }

        //check for collisions with markers
        private void checkCollisions(GameTime gameTime, int playerIndex)
        {
            //check collisions with racemarkers
            string[] raceGameMenuItems = { "Accept", "Bet","Decline" };
            foreach (RaceMarker currentMarker in ingameRaceMarkers)
            {
                if (gameTime.TotalGameTime - markerCollisionTime > currentRacemarkerCollisionCooldown)
                {
                    if (currentMarker.checkCollision(gamePlayers[playerIndex].Car.Sphere))
                    {
                        if (gamePlayers[playerIndex].Car.Speed == 0 && removeMarker == 1)
                        {
                            currentCollidedRaceMarker = currentMarker;
                            break;
                        }
                        else
                        {
                            raceMenu = new RaceMarkerMenu(ref graphics, this, ref spriteBatch, ref gameFont, ref raceGameMenuItems, ref input, ref raceMenuBackGround, currentMarker.MapInformation, ref currentState, "Race Menu", ref gameTime);
                            raceMenu.RaceMap = currentMarker.currentRaceMap;
                            raceMenu.Status = true;

                            savedDirection = gamePlayers[playerIndex].Car.Direction;
                            savedState = gamePlayers[playerIndex].Car.Position;

                            currentCollidedRaceMarker = currentMarker;
                            currentState.CurrentState = GameState.InGameMenu;
                            raceMenu.MaxBet = gamePlayers[0].CurrentCash;
                            gamePlayers[0].Car.PauseSound();
                            break;
                        }
                    }
                }
            }

            //check collisions with shop markers
            if (shopMarker.checkCollision(gamePlayers[playerIndex].Car.Sphere))
            {
                if (gameTime.TotalGameTime - markerCollisionTime > currentRacemarkerCollisionCooldown)
                {
                    string[] shopMenuItems = { "Enter Shop", "Exit" };

                    shopmenu = new ShopMenu(ref graphics, this, ref spriteBatch, ref gameFont, ref shopMenuItems, ref input, ref shopMenuBackground, ref currentState, "Shop Menu", ref gameTime);
                    shopmenu.Status = true;

                    savedDirection = gamePlayers[playerIndex].Car.Direction;
                    savedState = gamePlayers[playerIndex].Car.Position;

                    currentState.CurrentState = GameState.InGameMenu;
                    gamePlayers[0].Car.PauseSound();
                }

                else
                {
                    gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                    gamePlayers[0].Car.resetPosition();
                }
            }

            //check collisios with lampposts
            for (int i = 0; i < lampposts.Length; i++)
            {
                if (lampposts[i].checkCollision(gamePlayers[playerIndex].Car.Sphere))
                {
                    gamePlayers[0].Car.Position = gamePlayers[0].Car.PreviousPosition;
                    gamePlayers[0].Car.Speed = 0.0f;
                }
            }
        }

        //update the chase target
        private void UpdateCameraChaseTarget(int a)
        {
            camera[a].chasePosition = gamePlayers[a].Car.Position * gamePlayers[a].Car.CarScale;
            if (gamePlayers[a].Car.Speed > 20 || gamePlayers[a].Car.Speed < -20)
                camera[a].chaseDirection = gamePlayers[a].Car.Direction * (gamePlayers[a].Car.Speed / (Math.Abs(gamePlayers[a].Car.Speed) + 1));
            else
                camera[a].chaseDirection = gamePlayers[a].Car.Direction;
            //camera[a].up = gamePlayers[a].Car.Normal;
        }

        //check if objects are in the view and ony then render the object otherwise do not render the object
        private bool inView(BoundingFrustum viewFrustum, BoundingSphere objectSphere)
        {
            if (viewFrustum.Intersects(objectSphere))
                    return true;
            else
                return false;
        }

        //draw the world
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            float frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

            BoundingFrustum viewFrustum = new BoundingFrustum(camera[0].view * camera[0].projection);

            //draw loading screen
            if (loading.Active)
                loading.Draw(gameTime);
            
            //draw menu for given game state
            else
            {
                #region Game in MainMenu state
                if (currentState.CurrentState == GameState.MainMenu)
                {
                    spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
                    mainMenu.Draw(gameTime);
                    spriteBatch.End();
                }
                #endregion

                #region Game in InGameMenu state
                if (currentState.CurrentState == GameState.InGameMenu)
                {
                    spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

                    if (raceMenu != null && raceMenu.Status)
                        raceMenu.Draw(gameTime);
                    else if (shopmenu != null && shopmenu.Status)
                        shopmenu.Draw(gameTime);
                    else if (finishMenu != null && finishMenu.Status)
                        finishMenu.Draw(gameTime);
                    else if (mFinishMenu != null && mFinishMenu.Status)
                        mFinishMenu.Draw(gameTime);
                    else if (inRaceMenu != null && inRaceMenu.Status)
                        inRaceMenu.Draw(gameTime);
                    else if (inMultiRaceMenu != null && inMultiRaceMenu.Status)
                        inMultiRaceMenu.Draw(gameTime);
                    else if (roamMenu != null && roamMenu.Status)
                        roamMenu.Draw(gameTime);
                    else if (options != null && options.Status)
                        options.Draw(gameTime);
                    else if (multiMenu != null && multiMenu.Status)
                        multiMenu.Draw(gameTime);
                    else if (story != null && story.Status)
                        story.Draw(gameTime);

                    spriteBatch.End();
                }
                #endregion

                #region Game in InGame state
                else if (currentState.CurrentState == GameState.InGame)
                {
                    eniviroment.Draw(gameTime, camera[0], city, skyDome, viewPorts[0], true);

                    for (int a = 0; a < gamePlayers.Length; a++)
                    {
                        gamePlayers[a].Car.draw(camera[0], viewPorts[0], gameTime);
                        gamePlayers[a].Car.drawGauge();
                    }

                    if (storyDisplay && finalDisplay && storyEnables)
                    {
                        displayTimer--;
                        if (displayTimer <= 0)
                        {
                            story.Status = true;
                            displayTimer = 30;
                            storyDisplay = false;
                            story.PreviousState = currentState.CurrentState;
                            story.ScreenState = ScreenState.TransitionOn;
                            currentState.CurrentState = GameState.InGameMenu;
                        }
                    }

                    drawLampPosts(viewFrustum);
                    drawRaceMarkers(viewFrustum);
                    drawShopMarkers(viewFrustum);

                    spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
                    ingameHUD.Draw();
                    if (optionsManager.MusicEnabled)
                        drawMusicHUD();
                    //spriteBatch.DrawString(arial, gamePlayers[0].Car.Rotation.ToString(), new Vector2(0, 100), Color.Red);
                    //spriteBatch.DrawString(arial, angle.ToString(), new Vector2(0, 100), Color.Red);
                    //spriteBatch.DrawString(arial, angle2.ToString(), new Vector2(0, 150), Color.Red);
                    //spriteBatch.DrawString(arial, gamePlayers[0].Car.Normal.ToString(), new Vector2(0, 200), Color.Red);
                    //spriteBatch.DrawString(arial, gamePlayers[0].Car.Position.ToString(), Vector2.Zero, Color.Red);
                    //spriteBatch.DrawString(arial, gamePlayers[0].Level.ToString(), new Vector2(0, 175), Color.Red);
                    spriteBatch.End();
                }
                #endregion

                #region Game in Racing state
                else if (currentState.CurrentState == GameState.Racing)
                {
                    for (int i = 0; i < gamePlayers.Length; i++)
                    {
                        graphics.GraphicsDevice.Viewport = viewPorts[i];
                        GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
                        currentRace.Draw(gameTime, camera[i], viewPorts[i], false);

                        if (gamePlayers.Length > 1)
                            gamePlayers[1].Car.draw(camera[i], viewPorts[i], gameTime);

                        spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
                        racingHud[i].Draw();
                        spriteBatch.End();

                        if (counter < 10)
                        {
                            gamePlayers[i].Car.Direction = currentRace.StartPoints[i] - currentRace.WayPoints[0];
                            counter++;
                        }

                        camera[i].lookAt = gamePlayers[i].Car.Position;
                        gamePlayers[0].Car.draw(camera[i], viewPorts[i], gameTime);
                        gamePlayers[i].Car.drawGauge();
                    }

                    if (storyDisplay  && finalDisplay && storyEnables)
                    {
                        displayTimer--;
                        if (displayTimer <= 0)
                        {
                            story.Status = true;
                            displayTimer = 30;
                            storyDisplay = false;
                            story.PreviousState = currentState.CurrentState;
                            story.ScreenState = ScreenState.TransitionOn;
                            //mainMenu.ScreenState = ScreenState.Active;
                            currentState.CurrentState = GameState.InGameMenu;
                        }
                    }

                    graphics.GraphicsDevice.Viewport = defaultViewport;
                    spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
                    if (optionsManager.MusicEnabled)
                        drawMusicHUD();

                    //spriteBatch.DrawString(arial, gamePlayers[0].Car.Position.ToString(), new Vector2(0, 175), Color.Red);
                    spriteBatch.End();
                }
                #endregion

                #region Game in InShop state
                else if (currentState.CurrentState == GameState.InShop)
                {
                    carShop.Draw(gameTime);
                }
                #endregion
            }

            base.Draw(gameTime);
        }

        //draw race markers
        protected void drawRaceMarkers(BoundingFrustum viewFrustrum)
        {
            foreach(RaceMarker currentMarker in ingameRaceMarkers)
            {
                if (inView(viewFrustrum, currentMarker.CollisionSphere))
                    currentMarker.Draw(camera[0]);
            }
        }

        //draw shop markers
        protected void drawShopMarkers(BoundingFrustum viewFrustrum)
        {
            if (inView(viewFrustrum, shopMarker.CollisionSphere))
                shopMarker.Draw(camera[0]);
        }

        protected void drawLampPosts(BoundingFrustum viewFrustum)
        {
            for (int i = 0; i < lampposts.Length; i++)
            {
               if (inView(viewFrustum, lampposts[i].ColisionSphere))
                    lampposts[i].Draw(camera[0]);
            }
        }

        //draw game music HUD
        protected void drawMusicHUD()
        {
                    spriteBatch.Draw(mplayerHUD, Vector2.Zero, Color.White);
                    spriteBatch.DrawString(arial, media.SongName.ToString(), new Vector2(100, 25), Color.Red, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
                    spriteBatch.DrawString(arial, "Current Track: " + media.SongIndex.ToString(), new Vector2(100, 40), Color.Red, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);

                    if (media.CurrentPlayingTime.Seconds < 10 && media.TotalPlayingTime.Seconds < 10)
                        spriteBatch.DrawString(arial, "Playing Time: " + media.CurrentPlayingTime.Minutes.ToString() + ":0" + media.CurrentPlayingTime.Seconds.ToString() + "/" + media.TotalPlayingTime.Minutes.ToString() + ":0" + media.TotalPlayingTime.Seconds.ToString(), new Vector2(100, 55), Color.Red, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
                    else if (media.CurrentPlayingTime.Seconds < 10 && media.TotalPlayingTime.Seconds >= 10)
                        spriteBatch.DrawString(arial, "Playing Time: " + media.CurrentPlayingTime.Minutes.ToString() + ":0" + media.CurrentPlayingTime.Seconds.ToString() + "/" + media.TotalPlayingTime.Minutes.ToString() + ":" + media.TotalPlayingTime.Seconds.ToString(), new Vector2(100, 55), Color.Red, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
                    else if (media.CurrentPlayingTime.Seconds >= 10 && media.TotalPlayingTime.Seconds < 10)
                        spriteBatch.DrawString(arial, "Playing Time: " + media.CurrentPlayingTime.Minutes.ToString() + ":" + media.CurrentPlayingTime.Seconds.ToString() + "/" + media.TotalPlayingTime.Minutes.ToString() + ":0" + media.TotalPlayingTime.Seconds.ToString(), new Vector2(100, 55), Color.Red, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
                    else if (media.CurrentPlayingTime.Seconds >= 10 && media.TotalPlayingTime.Seconds >= 10)
                        spriteBatch.DrawString(arial, "Playing Time: " + media.CurrentPlayingTime.Minutes.ToString() + ":" + media.CurrentPlayingTime.Seconds.ToString() + "/" + media.TotalPlayingTime.Minutes.ToString() + ":" + media.TotalPlayingTime.Seconds.ToString(), new Vector2(100, 55), Color.Red, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);
        }
    }

    #region Entry Point
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (RedLineRacing game = new RedLineRacing())
            {
                game.Run();
            }
        }
    }
    #endregion
}