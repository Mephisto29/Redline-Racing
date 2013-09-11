/*
 * This class represents the city used for the free roam enviroment
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
using RacingGame.Engine.HelperFunctions;

namespace RacingGame.Enviroment
{
    class City
    {
        //device variables
        GraphicsDeviceManager graphics;
        GraphicsDevice device;

        //effect used to build city
        BasicEffect effect;

        //Textures to use for texturing of enviroment
        Texture2D textures;
        int numberOfBuildingsInTexture;

        //terain detail
        int terrainWidth;
        int terrainHeight;
        float scale = 20.0f;

        int[,] floorPlan;
        int[] buildingHeights;

        VertexPositionNormalTexture[] cityVertexBuffer;
        Vector3 startPoint;
        Vector3 cityPosition;

        Vector3 normal;
        Vector3 normal2;
        Vector3 position;
        Vector3 maxposition;

        //collision boxes
        BoundingBox[] buildingBoundingBoxes;
        BoundingBox completeCityBox;

        bool cameraColision;

        //getters and setters
        public Vector3 getStartPoint
        {
            get { return startPoint; }
        }

        public Vector3 CityPosition
        {
            get { return cityPosition; }
            set { cityPosition = value; }
        }

        public Vector3 Normal
        {
            get { return normal; }
        }

        public Vector3 Position
        {
            get { return (position * 20); }
        }

        public Vector3 MaxPosition
        {
            get { return (maxposition * 20); }
        }

        public Vector3 Normal2
        {
            get { return normal2; }
        }

        public int Width
        {
            get { return terrainWidth; }
        }

        public int Height
        {
            get { return terrainHeight; }
        }

        public Vector3 getNormalFromPoint(int x, int y)
        {
            if (x < 0)
                x = x * -1;

            if (y < 0)
                y = y * -1;

            return cityVertexBuffer[x + y * terrainWidth].Normal;
        }

        public float Scale
        {
            get { return scale; }
        }

        //Constructor
        public City(ref GraphicsDeviceManager newGraphics)
        {
            graphics = newGraphics;
        }

        //load the content for the city
        public void LoadContent(ref Texture2D newTextures)
        {
            //set heights of buildings
            buildingHeights = new int[] { 2, 2, 6, 5, 4, 3 };

            //set the number of buildings that exist in the texture
            numberOfBuildingsInTexture = buildingHeights.Length - 1;

            //set the current device
            device = graphics.GraphicsDevice;

            //load effect and textures
            effect = new BasicEffect(device, null);
            textures = newTextures;

            //load city data
            LoadFloorPlan();
            SetUpVertices();
            SetUpIndices();

            //set boudning boxes for collidable objects
            Vector3[] boundaryPoints = new Vector3[2];
            boundaryPoints[0] = new Vector3(0, 0, 0);
            boundaryPoints[1] = new Vector3(terrainWidth, scale, -terrainHeight);
            completeCityBox = BoundingBox.CreateFromPoints(boundaryPoints);

            setUpBoundingBoxes();
        }

        public List<Vector3> getSideWalkTiles()
        {
            List<Vector3> positions = new List<Vector3>();

             for (int x = 1; x < floorPlan.GetLength(0); x++)
                for (int y = 1; y < floorPlan.GetLength(1); y++)
                    if (floorPlan[x, y] == 13)
                    {
                        Vector3 position = new Vector3(-(floorPlan.GetLength(0)-x) +0.5f, 0.8f, -y -0.5f);
                        positions.Add(position);
                    }

             return positions;
        }

        //city map to be loaded
        //-1 = map edge or non walkable area
        //0 = street tile up
        //1 = Building 1
        //2 = Building 1
        //3 = Building 2
        //4 = Building 3
        //5 = Building 4
        //6 = Building 5
        //7 = street tile corner
        //8 = street tile side
        //9 = street tile
        //10 = player start point
        //11 = sidewalk

        //load and create the floor of the city
        private void LoadFloorPlan()
        {
            //create 2D array representing the floor
            floorPlan = new int[,]
             {
                  {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},    //city begin
                  {-1,11,13,11,11,13,11,11,13,11,11,13,11,11,13,11,11,13,10,-1},
                  {-1,11,7,0,0,0,0,0,0,7,0,0,7,0,0,0,0,7,11,-1},
                  {-1,13,8,13,11,11,13,11,13,8,11,11,8,13,11,11,13,8,13,-1},
                  {-1,11,8,11,-1,-1,-1,-1,11,8,13,12,8,-1,-1,-1,11,8,11,-1},
                  {-1,11,8,11,-1,-1,-1,-1,11,8,11,-1,8,-1,-1,-1,13,8,13,-1},
                  {-1,13,8,13,11,13,11,11,13,7,0,0,7,0,7,-1,11,8,11,-1},
                  {-1,11,7,0,0,7,0,0,0,7,11,-1,-1,-1,8,-1,11,8,13,-1},
                  {-1,11,8,13,11,8,13,11,13,8,13,-1,-1,-1,8,-1,11,8,11,-1},
                  {-1,11,8,11,-1,8,-1,-1,11,8,11,-1,-1,-1,8,-1,11,8,11,-1},
                  {-1,13,8,11,-1,8,-1,-1,11,8,11,-1,-1,-1,8,-1,13,8,13,-1},
                  {-1,11,8,13,-1,8,-1,-1,13,7,0,0,7,-1,8,-1,11,8,11,-1},
                  {-1,11,8,11,-1,8,-1,-1,11,8,13,-1,8,-1,8,-1,11,8,11,-1},
                  {-1,13,8,11,-1,8,-1,-1,11,8,11,-1,8,-1,8,-1,13,8,13,-1},
                  {-1,11,8,13,-1,8,-1,-1,13,8,11,-1,7,0,7,-1,11,8,11,-1},
                  {-1,11,8,11,-1,8,-1,-1,11,8,11,-1,-1,-1,-1,-1,11,8,11,-1},
                  {-1,13,8,11,13,8,11,13,11,8,13,11,11,13,11,11,13,8,13,-1},
                  {-1,11,7,0,0,7,0,0,0,7,0,0,0,0,0,0,0,7,11,-1},
                  {-1,11,13,11,11,13,11,11,13,8,13,11,11,13,11,11,13,11,11,-1},
                  {-1,-1,-1,-1,-1,-1,-1,-1,-1,8,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},    //city end




                  /*{-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},    //city begin
                  {-1,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,11,10,-1},
                  {-1,11,7,0,0,0,0,0,0,7,0,0,7,0,0,0,0,7,11,-1},
                  {-1,11,8,11,11,11,11,11,11,8,11,11,8,11,11,11,11,8,11,-1},
                  {-1,11,8,11,-1,-1,-1,-1,11,8,11,12,8,-1,-1,-1,11,8,11,-1},
                  {-1,11,8,11,-1,-1,-1,-1,11,8,11,-1,8,-1,-1,-1,11,8,11,-1},
                  {-1,11,8,11,11,11,11,11,11,7,0,0,7,0,7,-1,11,8,11,-1},
                  {-1,11,7,0,0,7,0,0,0,7,11,-1,-1,-1,8,-1,11,8,11,-1},
                  {-1,11,8,11,11,8,11,11,11,8,11,-1,-1,-1,8,-1,11,8,11,-1},
                  {-1,11,8,11,-1,8,-1,-1,11,8,11,-1,-1,-1,8,-1,11,8,11,-1},
                  {-1,11,8,11,-1,8,-1,-1,11,8,11,-1,-1,-1,8,-1,11,8,11,-1},
                  {-1,11,8,11,-1,8,-1,-1,11,7,0,0,7,-1,8,-1,11,8,11,-1},
                  {-1,11,8,11,-1,8,-1,-1,11,8,11,-1,8,-1,8,-1,11,8,11,-1},
                  {-1,11,8,11,-1,8,-1,-1,11,8,11,-1,8,-1,8,-1,11,8,11,-1},
                  {-1,11,8,11,-1,8,-1,-1,11,8,11,-1,7,0,7,-1,11,8,11,-1},
                  {-1,11,8,11,-1,8,-1,-1,11,8,11,-1,-1,-1,-1,-1,11,8,11,-1},
                  {-1,11,8,11,11,8,11,11,11,8,11,11,11,11,11,11,11,8,11,-1},
                  {-1,11,7,0,0,7,0,0,0,7,0,0,0,0,0,0,0,7,11,-1},
                  {-1,11,11,11,11,11,11,11,11,8,11,11,11,11,11,11,11,11,11,-1},
                  {-1,-1,-1,-1,-1,-1,-1,-1,-1,8,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},    //city end*/
             };

            //choose random height for each building
            Random random = new Random();
            int differentBuildings = buildingHeights.Length - 1;
            for (int x = 0; x < floorPlan.GetLength(0); x++)
                for (int y = 0; y < floorPlan.GetLength(1); y++)
                    if (floorPlan[x, y] == -1)
                        floorPlan[x, y] = random.Next(differentBuildings) + 1;
        }

        private void SetUpVertices()
        {
            float imagesInTexture = 14;

            //set terrain width and heights
            terrainWidth = floorPlan.GetLength(0);
            terrainHeight = floorPlan.GetLength(1);

            //the list of vertices
            List<VertexPositionNormalTexture> verticesList = new List<VertexPositionNormalTexture>();

            //iterate over the enitre size of city
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int z = 0; z < terrainHeight; z++)
                {
                    //current value in the floor plan
                    int currentbuilding = floorPlan[x, z];

                    if (currentbuilding == 10)
                        startPoint = new Vector3((cityPosition.X + 56), cityPosition.Y, cityPosition.Z - 210);

                    //sidewalk tile
                    if (currentbuilding == 11 || currentbuilding == 10 || currentbuilding == 12 || currentbuilding == 13)
                    {
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0f, -z), new Vector3(0, 1, 0), new Vector2(13 / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0f, -z - 1), new Vector3(0, 1, 0), new Vector2((13) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0f, -z), new Vector3(0, 1, 0), new Vector2((1 + 13) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0f, -z - 1), new Vector3(0, 1, 0), new Vector2((13) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0f, -z - 1), new Vector3(0, 1, 0), new Vector2((1 + 13) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0f, -z), new Vector3(0, 1, 0), new Vector2((1 + 13) / imagesInTexture, 1)));

                        //front wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((imagesInTexture) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0f, -z - 1), new Vector3(0, 0, -1), new Vector2((imagesInTexture - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((imagesInTexture - 1) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0f, -z - 1), new Vector3(0, 0, -1), new Vector2((imagesInTexture - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((imagesInTexture) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0f, -z - 1), new Vector3(0, 0, -1), new Vector2((imagesInTexture) / imagesInTexture, 0)));

                        //back wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 0, 1), new Vector2((imagesInTexture) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(0, 0, 1), new Vector2((imagesInTexture - 1) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0f, -z), new Vector3(0, 0, 1), new Vector2((imagesInTexture - 1) / imagesInTexture, 0)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0f, -z), new Vector3(0, 0, 1), new Vector2((imagesInTexture - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0f, -z), new Vector3(0, 0, 1), new Vector2((imagesInTexture) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 0, 1), new Vector2((imagesInTexture) / imagesInTexture, 1)));

                        //left wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(-1, 0, 0), new Vector2((imagesInTexture) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(-1, 0, 0), new Vector2((imagesInTexture - 1) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0f, -z - 1), new Vector3(-1, 0, 0), new Vector2((imagesInTexture - 1) / imagesInTexture, 0)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0f, -z - 1), new Vector3(-1, 0, 0), new Vector2((imagesInTexture - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0f, -z), new Vector3(-1, 0, 0), new Vector2((imagesInTexture) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(-1, 0, 0), new Vector2((imagesInTexture) / imagesInTexture, 1)));

                        //right wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(1, 0, 0), new Vector2((imagesInTexture) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0f, -z - 1), new Vector3(1, 0, 0), new Vector2((imagesInTexture - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(1, 0, 0), new Vector2((imagesInTexture - 1) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0f, -z - 1), new Vector3(1, 0, 0), new Vector2((imagesInTexture - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(1, 0, 0), new Vector2((imagesInTexture) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0f, -z), new Vector3(1, 0, 0), new Vector2((imagesInTexture) / imagesInTexture, 0)));
                    }

                    //street up tile
                    else if (currentbuilding == 0)
                    {
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(0, 1, 0), new Vector2(currentbuilding * 2 + 10 / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 10) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1 + 10) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 10) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1 + 10) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1 + 10) / imagesInTexture, 1)));
                    }

                    //street corner tile
                    else if (currentbuilding == 7)
                    {
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(0, 1, 0), new Vector2(11 / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 1, 0), new Vector2((11) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 1, 0), new Vector2((1 + 11) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 1, 0), new Vector2((11) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 1, 0), new Vector2((1 + 11) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 1, 0), new Vector2((1 + 11) / imagesInTexture, 1)));
                    }

                    //street side tile
                    else if (currentbuilding == 8)
                    {
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(0, 1, 0), new Vector2(12 / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 1, 0), new Vector2((12) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 1, 0), new Vector2((1 + 12) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 1, 0), new Vector2((12) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 1, 0), new Vector2((1 + 12) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 1, 0), new Vector2((1 + 12) / imagesInTexture, 1)));
                    }

                    //building texture
                    else if (currentbuilding >= 1 && currentbuilding <= 6)
                    {
                        //front wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));

                        //back wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));

                        //left wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));

                        //right wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z - 1), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z - 1), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));

                        //floor or ceiling
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z), new Vector3(0, 1, 0), new Vector2(currentbuilding * 2 / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1) / imagesInTexture, 1)));
                    }
                }
            }

            cityVertexBuffer = verticesList.ToArray();
        }


        //set up the indices for the vertices
        private int[] SetUpIndices()
        {
            //if you have 3x4 vertices you have exactly 6 triangles thus 18 vertices to determine indices
            //(3-1)*(4-1) = 2*3 = 6 * 3 = 18....the size of the indices
            int[] indices = new int[(terrainWidth - 1) * (terrainHeight - 1) * 6];

            int counter = 0;

            for (int y = 0; y < terrainHeight - 1; y++)
            {
                for (int x = 0; x < terrainWidth - 1; x++)
                {
                    int lowerLeftVertex = x + y * terrainWidth;
                    int lowerRightVertex = (x + 1) + y * terrainWidth;
                    int topLeftVertex = x + (y + 1) * terrainWidth;
                    int topRightVertex = (x + 1) + (y + 1) * terrainWidth;

                    //1st triangle
                    indices[counter++] = topLeftVertex;
                    indices[counter++] = lowerRightVertex;
                    indices[counter++] = lowerLeftVertex;

                    //2nd triangle
                    indices[counter++] = topLeftVertex;
                    indices[counter++] = topRightVertex;
                    indices[counter++] = lowerRightVertex;
                }
            }

            return indices;
        }

        //set up the bounding boxes of the city buildings, this is just a box as high as the building and as wide
        private void setUpBoundingBoxes()
        {
            //list of boxes
            List<BoundingBox> boudingBoxes = new List<BoundingBox>();

            //loop through 2D array and create boxes adding them to list
            for (int x = 0; x < terrainHeight; x++)
            {
                for (int z = 0; z < terrainWidth; z++)
                {
                    int buildingType = floorPlan[x, z];

                    if (buildingType >= 1 && buildingType <= 6)
                    {
                        int height = buildingHeights[buildingType];

                        Vector3[] buildingPoints = new Vector3[2];
                        buildingPoints[0] = new Vector3(x, 0, -z);
                        buildingPoints[1] = new Vector3(x + 1, height, -z - 1);
                        buildingPoints[0].Y += cityPosition.Y / scale * 0.05f;
                        buildingPoints[1].Y += cityPosition.Y / scale * 0.05f;
                        buildingPoints[0] = Vector3.Transform(buildingPoints[0], Matrix.CreateTranslation(cityPosition * 0.05f));
                        buildingPoints[1] = Vector3.Transform(buildingPoints[1], Matrix.CreateTranslation(cityPosition * 0.05f));

                        BoundingBox buildingBox = BoundingBox.CreateFromPoints(buildingPoints);
                        boudingBoxes.Add(buildingBox);
                    }
                }
            }

            //convert list to array
            buildingBoundingBoxes = boudingBoxes.ToArray();
        }

        //check f0r collition with building
        public bool checkCollision(BoundingSphere sphere)
        {
            if (completeCityBox.Contains(sphere) != ContainmentType.Contains)
            {
                for (int i = 0; i < buildingBoundingBoxes.Length; i++)
                    if (buildingBoundingBoxes[i].Contains(sphere) != ContainmentType.Disjoint)
                    {
                        position = buildingBoundingBoxes[i].Min;
                        maxposition = buildingBoundingBoxes[i].Max;
                        normal = buildingBoundingBoxes[i].Min - (buildingBoundingBoxes[i].Min + new Vector3(2, 0, 0));
                        normal2 = buildingBoundingBoxes[i].Min - (buildingBoundingBoxes[i].Min + new Vector3(0, 0, 2));
                        return true;
                    }
            }

            else
                return false;

            return false;
        }

        public bool checkCollision(BoundingBox box)
        {
            if (completeCityBox.Contains(box) != ContainmentType.Contains)
            {
                for (int i = 0; i < buildingBoundingBoxes.Length; i++)
                    if (buildingBoundingBoxes[i].Intersects(box))
                        return true;
            }

            else
                return false;

            return false;
        }

        public bool checkCollision(Vector3 point)
        {
            //point = Vector3.Multiply(point, 0.05f);            

            for (int i = 0; i < buildingBoundingBoxes.Length; i++)
            {
                cameraColision = true;

                if (point.X < buildingBoundingBoxes[i].Min.X)
                    cameraColision = false;
                if (point.X > buildingBoundingBoxes[i].Max.X)
                    cameraColision = false;
                if (point.Y < buildingBoundingBoxes[i].Min.Y)
                    cameraColision = false;
                if (point.Y > buildingBoundingBoxes[i].Max.Y)
                    cameraColision = false;
                if (point.Z < buildingBoundingBoxes[i].Min.Z)
                    cameraColision = false;
                if (point.Z > buildingBoundingBoxes[i].Max.Z)
                    cameraColision = false;

                if (cameraColision)
                    return cameraColision;
            }

            return cameraColision;
        }

        //draw the city
        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            DrawCity(gameTime, view, projection);
        }

        public void DrawCity(GameTime gameTime, Matrix view, Matrix projection)
        {
            device.VertexDeclaration = new VertexDeclaration(device, VertexPositionNormalTexture.VertexElements);

            effect.World = Matrix.Identity * Matrix.CreateTranslation(cityPosition * 0.05f);
            effect.View = view;
            effect.Projection = projection;
            effect.TextureEnabled = true;
            effect.Texture = textures;

            //enable fogging
            effect.FogEnabled = true;
            effect.FogColor = Color.White.ToVector3();
            effect.FogStart = 0.0f;
            effect.FogEnd = 100.0f;

            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                device.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, cityVertexBuffer, 0, cityVertexBuffer.Length / 3);
                pass.End();
            }
            effect.End();
        }
    }
}
