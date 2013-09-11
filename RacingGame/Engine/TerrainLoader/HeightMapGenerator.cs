/*
 * This class is used to load heighmaps based on a 2D image, this class is used for the free roam enviroment as well as the races
 * It also is responsible for the water and gobal effects
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

using RacingGame.Enviroment;
using RacingGame.Engine;

namespace RacingGame.Engine.TerrainLoader
{
    //this struct is used to create multiple textures based on a weight provided
    public struct VertexMultitextured
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector4 TextureCoordinate;
        public Vector4 TexWeights;

        public static int SizeInBytes = (3 + 3 + 4 + 4) * sizeof(float);
        public static VertexElement[] VertexElements = new VertexElement[]
         {
             new VertexElement( 0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0 ),
             new VertexElement( 0, sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0 ),
             new VertexElement( 0, sizeof(float) * 6, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0 ),
             new VertexElement( 0, sizeof(float) * 10, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1 ),
         };
    }

    class HeightMapGenerator : IDisposable
    {
        //graphics details
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        GameStateManager currentState;

        //textures to be used, the current height map is the map loaded
        //texture0 = the lowest texture
        //texture1 = the 2nd lowest texture
        //texture2 = the 2nd highest texture
        //texture3 = the highest texture
        Texture2D currentHeightMap;
        Texture2D currentHeightMapCollision;
        Texture2D aiMap;
        Texture2D texture0;
        Texture2D texture1;
        Texture2D texture2;
        Texture2D texture3;
        Texture2D waterBumpMap;
        float texture0Height;
        float texture1Height;
        float texture2Height;
        float texture3Height;
        float textureOverlap;

        //the terrain details
        int terrainWidth;
        int terrainHeight;
        //start point of the height map
        Vector3 startPosition;

        //the array of height values for each x-z value
        float[,] heightData;
        float[,] collisionData;
        float minimumHeight;
        float maximumHeight;
        float scale;
        float scaleval = 0.2f;
        float multiplier;

        //vertex assemblies used to create the height map
        VertexMultitextured[] terrainVertices;
        int[] terrainIndices;

        VertexBuffer terrainVertexBuffer;
        IndexBuffer terrainIndexBuffer;
        VertexDeclaration terrainVertexDeclaration;

        //effect to be used, this is obtained from one of of reimers tutorials
        Effect effect;
        BasicEffect fogEffect;

        //variables used for the water generation
        float heightOfWater = 0.12f;

        Matrix reflectionMatrix;

        RenderTarget2D refraction;
        Texture2D refractionMap;
        RenderTarget2D reflection;
        Texture2D reflectionMap;

        VertexBuffer waterVerticesBuffer;
        VertexDeclaration waterVertexDeclaration;

        //AI map
        //AItile[,] aiMap;
        float collisionHeight;
        List<Vector3> aiWaypoints;

        //billboarding
        VertexBuffer treeBuffer;
        VertexDeclaration treeDeclaration;
        Texture2D tree;
        Texture2D treeMap;
        float treeHeight = 5.0f;

        //skydome rotation
        float domeRotation = 0.0f;

        //getters and setters
        public float HeightOffset
        {
            get { return startPosition.Y; }
        }

        public float Texture0Height
        {
            set { texture0Height = value; }
        }
        public float Texture1Height
        {
            set { texture1Height = value; }
        }
        public float Texture2Height
        {
            set { texture2Height = value; }
        }
        public float Texture3Height
        {
            set { texture3Height = value; }
        }

        public Texture2D Texture0
        {
            set { texture0 = value; }
        }
        public Texture2D Texture1
        {
            set { texture1 = value; }
        }
        public Texture2D Texture2
        {
            set { texture2 = value; }
        }
        public Texture2D Texture3
        {
            set { texture3 = value; }
        }

        public float TextureOverlap
        {
            set { textureOverlap = value; }
        }

        public Texture2D Map
        {
            set { currentHeightMap = value; }
        }
        public Texture2D MapCol
        {
            set { currentHeightMapCollision = value; }
        }

        public float CollisionHeight
        {
            set { collisionHeight = value; }
        }

        public List<Vector3> AIwaypoints
        {
            get { return aiWaypoints; }
        }

        public float setTreeHeight
        {
            set { treeHeight = value; }
        }

        public Texture2D setTreeMap
        {
            set { treeMap = value; }
        }

        /*public AItile[,] AImap
        {
            get { return aiMap; }
            set { aiMap = value; }
        }*/

        //constructor
        public HeightMapGenerator(ref GraphicsDeviceManager manager,
                                   ref Texture2D currentHeightmap,
                                   ref Texture2D newMapCollision,
                                   Texture2D newAiMap,
                                   ref Texture2D texture0,
                                   ref Texture2D texture1,
                                   ref Texture2D texture2,
                                   ref Texture2D texture3,
                                   ref Texture2D waterBumpMap,
                                   ref Texture2D tree,
                                   ref Texture2D treeMap,
                                   ref Effect effect,
                                   float scale,
                                   ref GameStateManager newState,
                                   float newTexture0Height,
                                   float newTexture1Height,
                                   float newTexture2Height,
                                   float newTexture3Height,
                                   float newTextureOverlap,
                                   ref float collidingHeight)
        {
            currentState = newState;
            graphics = manager;
            device = graphics.GraphicsDevice;

            PresentationParameters pp = device.PresentationParameters;
            refraction = new RenderTarget2D(device, pp.BackBufferWidth, pp.BackBufferHeight, 1, device.DisplayMode.Format);
            reflection = new RenderTarget2D(device, pp.BackBufferWidth, pp.BackBufferHeight, 1, device.DisplayMode.Format);

            minimumHeight = float.MaxValue;
            maximumHeight = float.MinValue;

            this.currentHeightMap = currentHeightmap;
            this.currentHeightMapCollision = newMapCollision;
            this.aiMap = newAiMap;
            this.texture0 = texture0;
            this.texture1 = texture1;
            this.texture2 = texture2;
            this.texture3 = texture3;
            this.waterBumpMap = waterBumpMap;
            this.tree = tree;
            this.treeMap = treeMap;
            this.scale = scale;
            this.collisionHeight = collidingHeight;

            texture0Height = newTexture0Height;
            texture1Height = newTexture1Height;
            texture2Height = newTexture2Height;
            texture3Height = newTexture3Height;
            textureOverlap = newTextureOverlap;

            this.effect = effect;
            this.fogEffect = new BasicEffect(device, null);

            if (currentState.CurrentState == GameState.InGame)
                multiplier = 1.0f;
            else
                multiplier = 0.05f;

            loadHeightMap(currentHeightMap);
            loadCollisionMap(currentHeightMapCollision);
            SmoothTerrainHeights(50);
            buildTerrain();
        }

        //dispose
        public void Dispose()
        {
            heightData = null;
            collisionData = null;
            terrainVertices = null;
            terrainIndices = null;

            terrainVertexBuffer.Dispose();
            terrainVertexBuffer = null;
            terrainIndexBuffer.Dispose();
            terrainIndexBuffer = null;
            terrainVertexDeclaration.Dispose();
            terrainVertexDeclaration = null;

            waterVerticesBuffer.Dispose();
            waterVerticesBuffer = null;
            waterVertexDeclaration.Dispose();
            waterVertexDeclaration = null;
        }

        private List<Vector3> GenerateTreePositions(Texture2D treeMap, VertexMultitextured[] terrainVertices)
        {
            Color[] treeMapColors = new Color[treeMap.Width * treeMap.Height];
            treeMap.GetData(treeMapColors);

            int[,] noiseData = new int[treeMap.Width, treeMap.Height];
            for (int x = 0; x < treeMap.Width; x++)
                for (int y = 0; y < treeMap.Height; y++)
                    noiseData[x, y] = treeMapColors[y + x * treeMap.Height].R;


            List<Vector3> treeList = new List<Vector3>(); 
            Random random = new Random();

            for (int x = terrainWidth-1; x > -1; x--)
            {
                for (int y = terrainHeight-1; y > -1; y--)
                {
                    float height = heightData[x, y];
                    if ((height > treeHeight))
                    {
                        float relx = (float)x / (float)terrainWidth;
                        float rely = (float)y / (float)terrainHeight;

                        float noiseValueAtCurrentPosition = noiseData[(int)(relx * treeMap.Width), (int)(rely * treeMap.Height)];
                        float treeDensity;
                        if (noiseValueAtCurrentPosition > 200)
                            treeDensity = 5;
                        else if (noiseValueAtCurrentPosition > 150)
                            treeDensity = 4;
                        else if (noiseValueAtCurrentPosition > 100)
                            treeDensity = 3;
                        else
                            treeDensity = 0;

                        for (int currDetail = 0; currDetail < treeDensity; currDetail++)
                        {
                            float rand1 = (float)random.Next(1000) / 1000.0f;
                            float rand2 = (float)random.Next(1000) / 1000.0f;

                            Vector3 treePos = getPositionInHeightmap((int)(x - rand1), (int)((y - rand2)));
                            treeList.Add(treePos / 20);
                        }
                    }
                }
            }

            return treeList;
        }

        private void CreateBillboard(List<Vector3> treeList)
        {
            VertexPositionTexture[] billboardVertices = new VertexPositionTexture[treeList.Count * 6];
            int i = 0;
            foreach (Vector3 currentV3 in treeList)
            {
                billboardVertices[i++] = new VertexPositionTexture(currentV3, new Vector2(0, 0));
                billboardVertices[i++] = new VertexPositionTexture(currentV3, new Vector2(1, 0));
                billboardVertices[i++] = new VertexPositionTexture(currentV3, new Vector2(1, 1));

                billboardVertices[i++] = new VertexPositionTexture(currentV3, new Vector2(0, 0));
                billboardVertices[i++] = new VertexPositionTexture(currentV3, new Vector2(1, 1));
                billboardVertices[i++] = new VertexPositionTexture(currentV3, new Vector2(0, 1));
            }

            treeBuffer = new VertexBuffer(device, billboardVertices.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
            treeBuffer.SetData(billboardVertices);
            treeDeclaration = new VertexDeclaration(device, VertexPositionTexture.VertexElements);
        }

        //load the required content and build the height map
        public void LoadContent(Vector3 startPosition)
        {
            this.startPosition = startPosition;
        }

        public void buildTerrain()
        {
            //set up vertices and indices
            terrainVertices = null;
            terrainVertices = SetUpTerrainVertices();

            terrainIndices = null;
            terrainIndices = SetUpIndices();
            terrainVertices = GetNormals(terrainVertices, terrainIndices);

            //cast to the required buffers
            castToTerrainBuffers(terrainVertices, terrainIndices);
            terrainVertexDeclaration = new VertexDeclaration(graphics.GraphicsDevice, VertexMultitextured.VertexElements);

            //create water vertices
            CreateWaterVertices();
            waterVertexDeclaration = new VertexDeclaration(device, VertexPositionTexture.VertexElements);

            //create billboard data
            List<Vector3> treePositions = GenerateTreePositions(treeMap, terrainVertices);
            CreateBillboard(treePositions);
        }

        public bool isPositionInMap(Vector3 position)
        {
            Vector3 positionOnHeightmap = (position - startPosition) * (0.05f / scaleval);

            float x = (positionOnHeightmap.X % scale) / scale;
            float y = (positionOnHeightmap.Z % scale) / scale;

            return (x > 0 &&
                    x < terrainWidth &&
                    y < 0 &&
                    y > -terrainHeight);
        }

        //load the data from the given texture
        private void loadHeightMap(Texture2D heightmap)
        {
            //maximum and minimum heigts on the height map
            terrainWidth = heightmap.Width;
            terrainHeight = heightmap.Height;

            //the colours at each pixel in the height map
            Color[] heightMapColors = new Color[terrainWidth * terrainHeight];
            heightmap.GetData(heightMapColors);

            heightData = new float[terrainWidth, terrainHeight];

            //iterate over eache pixel and set height value appropriatly
            for (int x = 0; x < terrainWidth - 1; x++)
                for (int y = 0; y < terrainHeight - 1; y++)
                {
                    heightData[x, y] = (heightMapColors[x + y * terrainWidth].R) * multiplier;

                    if (heightData[x, y] > maximumHeight)
                        maximumHeight = heightData[x, y];
                    if (heightData[x, y] < minimumHeight)
                        minimumHeight = heightData[x, y];
                }
        }

        //load the data from the given texture for collision detection
        private void loadCollisionMap(Texture2D heightmapCollision)
        {
            //the colours at each pixel in the height map
            Color[] heightMapColors = new Color[terrainWidth * terrainHeight];
            heightmapCollision.GetData(heightMapColors);

            collisionData = new float[terrainWidth, terrainHeight];

            //iterate over eache pixel and set height value appropriatly
            for (int x = 0; x < terrainWidth - 1; x++)
                for (int y = 0; y < terrainHeight - 1; y++)
                    collisionData[x, y] = (heightMapColors[x + y * terrainWidth].R) * multiplier;
        }

        public void loadAiMap()
        {
            aiWaypoints = new List<Vector3>();

            //the colours at each pixel in the height map
            Color[] heightMapColors = new Color[terrainWidth * terrainHeight];
            aiMap.GetData(heightMapColors);

            //iterate over eache pixel and set height value appropriatly
            for (int x = 0; x < terrainWidth - 1; x++)
                for (int y = 0; y < terrainHeight - 1; y++)
                    if (((heightMapColors[x + y * terrainWidth].R) * multiplier) >= 12)
                        aiWaypoints.Add(getPositionInHeightmap(x, y));
        }

        public Vector3 getPositionInHeightmap(int x, int z)
        {
            float height;

            float newX = (x * scale) / (0.05f / scaleval);
            float newZ = (z * scale) / (0.05f / scaleval);
            Vector3 position = new Vector3(newX, 0, newZ);

            height = getHeight(position);
            position.Y = height;
            position.Z *= -1;

            return position;
        }

        //set up the vertices that define the map
        private VertexMultitextured[] SetUpTerrainVertices()
        {
            //the array of vertices to be used
            VertexMultitextured[] vertices = new VertexMultitextured[terrainHeight * terrainWidth];

            for (int x = 0; x < terrainWidth; x++)
                for (int y = 0; y < terrainHeight; y++)
                {
                    //set the position and the texture coordinate of the vertex
                    vertices[x + y * terrainWidth].Position = (new Vector3(x, heightData[x, y], -y));
                    vertices[x + y * terrainWidth].TextureCoordinate.X = (float)x / 30;
                    vertices[x + y * terrainWidth].TextureCoordinate.Y = (float)y / 30;

                    //associate each vertex with a weight, this is determined by the height of the current vertex, and this is used to determine the
                    //texture to apply to the given height
                    vertices[x + y * terrainWidth].TexWeights.X = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - texture0Height) / textureOverlap, 0, 1);
                    vertices[x + y * terrainWidth].TexWeights.Y = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - texture1Height) / textureOverlap, 0, 1);
                    vertices[x + y * terrainWidth].TexWeights.Z = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - texture2Height) / textureOverlap, 0, 1);
                    vertices[x + y * terrainWidth].TexWeights.W = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - texture3Height) / textureOverlap, 0, 1);

                    float total = vertices[x + y * terrainWidth].TexWeights.X;
                    total += vertices[x + y * terrainWidth].TexWeights.Y;
                    total += vertices[x + y * terrainWidth].TexWeights.Z;
                    total += vertices[x + y * terrainWidth].TexWeights.W;

                    vertices[x + y * terrainWidth].TexWeights.X /= total;
                    vertices[x + y * terrainWidth].TexWeights.Y /= total;
                    vertices[x + y * terrainWidth].TexWeights.Z /= total;
                    vertices[x + y * terrainWidth].TexWeights.W /= total;
                }

            return vertices;
        }

        //method used to create the vertices for the water plane
        private void CreateWaterVertices()
        {
            VertexPositionTexture[] waterVertices = new VertexPositionTexture[6];

            waterVertices[0] = new VertexPositionTexture(new Vector3(0, heightOfWater, 0), new Vector2(0, 1));
            waterVertices[2] = new VertexPositionTexture(new Vector3(terrainWidth, heightOfWater, -terrainHeight), new Vector2(1, 0));
            waterVertices[1] = new VertexPositionTexture(new Vector3(0, heightOfWater, -terrainHeight), new Vector2(0, 0));

            waterVertices[3] = new VertexPositionTexture(new Vector3(0, heightOfWater, 0), new Vector2(0, 1));
            waterVertices[5] = new VertexPositionTexture(new Vector3(terrainWidth, heightOfWater, 0), new Vector2(1, 1));
            waterVertices[4] = new VertexPositionTexture(new Vector3(terrainWidth, heightOfWater, -terrainHeight), new Vector2(1, 0));

            waterVerticesBuffer = new VertexBuffer(device, waterVertices.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
            waterVerticesBuffer.SetData(waterVertices);
        }

        //set up the vertices that define the map
        private VertexMultitextured[] GetNormals(VertexMultitextured[] vertices, int[] indices)
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < indices.Length / 3; i++)
            {
                //get first 3 indices
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                //obtain the normal of the given triangle
                Vector3 A = vertices[index1].Position - vertices[index3].Position;
                Vector3 B = vertices[index1].Position - vertices[index2].Position;
                Vector3 normalVector = Vector3.Cross(A, B);

                //set the normal of the 3 vertices defined by the indices
                vertices[index1].Normal += normalVector;
                vertices[index2].Normal += normalVector;
                vertices[index3].Normal += normalVector;
            }

            //normalize each normal vector
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();

            return vertices;
        }

        //set up the indices for the vertices
        private int[] SetUpIndices()
        {
            //if you have 3x4 vertices you have exactly 6 triangles thus 18 vertices to determine indices
            //(3-1)*(4-1) = 2*3 = 6 * 3 = 18....the size of the indices
            
            int[] indices = null;
            indices = new int[(terrainWidth - 1) * (terrainHeight - 1) * 6];

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

        //this method uses weighted averaging, it takes the current cell and averages it against the surrounding heights and creates a smoother surface
        private void SmoothTerrainHeights(int numberOfPasses)
        {
            //create new temp array
            float[,] changedHeightData = new float[terrainWidth, terrainHeight];

            while (numberOfPasses > 0)
            {
                //decrease number of passes
                numberOfPasses--;

                //iterate over current terrain data and average the heights with the surrounding heights
                for (int a = 0; a < terrainWidth; a++)
                {
                    for (int b = 0; b < terrainHeight; b++)
                    {
                        int adjacentCellsWithValues = 0;
                        float currentHeightTotal = 0.0f;

                        //check left of current cell
                        if ((a - 1) > 0)
                        {
                            currentHeightTotal += heightData[a - 1, b];
                            adjacentCellsWithValues++;

                            //check left and up
                            if ((b - 1) > 0)
                            {
                                currentHeightTotal += heightData[a - 1, b - 1];
                                adjacentCellsWithValues++;
                            }

                            //check left and down
                            if ((b + 1) < terrainHeight)
                            {
                                currentHeightTotal += heightData[a - 1, b + 1];
                                adjacentCellsWithValues++;
                            }
                        }

                        //check right of current cell
                        if ((a + 1) < terrainWidth)
                        {
                            currentHeightTotal += heightData[a + 1, b];
                            adjacentCellsWithValues++;

                            //check right and up
                            if ((b - 1) > 0)
                            {
                                currentHeightTotal += heightData[a + 1, b - 1];
                                adjacentCellsWithValues++;
                            }

                            //check right and down
                            if ((b + 1) < terrainHeight)
                            {
                                currentHeightTotal += heightData[a + 1, b + 1];
                                adjacentCellsWithValues++;
                            }
                        }

                        //check up of current cell
                        if ((b - 1) > 0)
                        {
                            currentHeightTotal += heightData[a, b - 1];
                            adjacentCellsWithValues++;
                        }

                        //check down of current cell
                        if ((b + 1) < terrainHeight)
                        {
                            currentHeightTotal += heightData[a, b + 1];
                            adjacentCellsWithValues++;
                        }

                        changedHeightData[a, b] = (heightData[a, b] + (currentHeightTotal / adjacentCellsWithValues)) * 0.5f;
                    }
                }
            }

            //update the existing terrain heights with the new ones
            for (int a = 0; a < terrainWidth; a++)
                for (int b = 0; b < terrainHeight; b++)
                    heightData[a, b] = changedHeightData[a, b];
        }

        private void castToTerrainBuffers(VertexMultitextured[] vertices, int[] indices)
        {
            terrainVertexBuffer = null;
            terrainVertexBuffer = new VertexBuffer(graphics.GraphicsDevice, vertices.Length * VertexMultitextured.SizeInBytes, BufferUsage.WriteOnly);
            terrainVertexBuffer.SetData(vertices);

            terrainIndexBuffer = null;
            terrainIndexBuffer = new IndexBuffer(graphics.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            terrainIndexBuffer.SetData(indices);
        }

        //this method returns a height at a given postion
        public float getHeight(Vector3 position)
        {
            float height;
            Vector3 positionOnHeightmap = (position - startPosition) * (0.05f / scaleval);

            int left, top;
            left = (int)positionOnHeightmap.X / (int)scale;
            top = (int)positionOnHeightmap.Z / (int)scale;

            float xNormalized = (positionOnHeightmap.X % scale) / scale;
            float zNormalized = (positionOnHeightmap.Z % scale) / scale;

            if (left < 0)
                left = 0;
            else if (left > 510)
                left = 510;

            if (top < 0)
                top = 0;
            if (top > 510)
                top = 510;

            float topHeight = MathHelper.Lerp(
                 heightData[left, top],
                 heightData[left + 1, top],
                 xNormalized);

            float bottomHeight = MathHelper.Lerp(
                heightData[left, top + 1],
                heightData[left + 1, top + 1],
                xNormalized);

            height = MathHelper.Lerp(topHeight, bottomHeight, zNormalized) / (0.05f / scaleval);

            return height;
        }

        //returns if a collision has occured
        public float getCollision(Vector3 position)
        {
            float height;
            Vector3 positionOnHeightmap = (position - startPosition) * (0.05f / scaleval);
            positionOnHeightmap.Z = -(position.Z - startPosition.Z) * (0.05f / scaleval);

            int left, top;
            left = (int)positionOnHeightmap.X / (int)scale;
            top = (int)positionOnHeightmap.Z / (int)scale;

            if (left < 0)
                left = 0;
            else if (left > 510)
                left = 510;

            if (top < 0)
                top = 0;
            if (top > 510)
                top = 510;

            float xNormalized = (positionOnHeightmap.X % scale) / scale;
            float zNormalized = (positionOnHeightmap.Z % scale) / scale;

            float topHeight = MathHelper.Lerp(
                 collisionData[Math.Abs(left), Math.Abs(top)],
                 collisionData[Math.Abs(left + 1), Math.Abs(top)],
                 xNormalized);

            float bottomHeight = MathHelper.Lerp(
                collisionData[Math.Abs(left), Math.Abs(top + 1)],
                collisionData[Math.Abs(left + 1), Math.Abs(top + 1)],
                xNormalized);

            height = MathHelper.Lerp(topHeight, bottomHeight, zNormalized) / (0.05f / scaleval);

            return height;
        }

        //this method returns the position of a given point in the 2D texture
        public Vector2 getPositionInTexture(Vector3 position)
        {
            Vector3 positionOnHeightmap = (position - startPosition) * (0.05f / scaleval);

            int left, top;

            left = (int)positionOnHeightmap.X / (int)scale;
            top = (int)positionOnHeightmap.Z / (int)scale;

            return new Vector2(top, left);
        }

        //checks if a position is on the heightmap
        public bool IsOnHeightMap(Vector3 currentPosition, float carScaleValue, float cityScaleValue)
        {
            Vector3 position = (currentPosition - (startPosition * cityScaleValue)) * (carScaleValue / scaleval);

            return (position.X > 0 &&
                    position.X < terrainWidth &&
                    position.Z < 0 &&
                    position.Z > -terrainHeight);
        }

        //return the height and the normal of a given point
        public void GetHeightAndNormal(Vector3 position, out float height, out Vector3 normal, float carscale)
        {

            Vector3 positionOnHeightmap = (position - startPosition) * (carscale / scaleval);
            positionOnHeightmap.Z = -(position.Z - startPosition.Z) * (carscale / scaleval);

            int left, top;
            left = (int)positionOnHeightmap.X / (int)scale;
            top = (int)positionOnHeightmap.Z / (int)scale;

            float xNormalized = (positionOnHeightmap.X % scale) / scale;
            float zNormalized = (positionOnHeightmap.Z % scale) / scale;

            if (left < 0)
                left = 0;
            else if (left > 510)
                left = 510;

            if (top < 0)
                top = 0;
            if (top > 510)
                top = 510;

            float topHeight;
            float bottomHeight;
            try
            {
                topHeight = MathHelper.Lerp(
                     heightData[left, top],
                     heightData[left + 1, top],
                     xNormalized);

                bottomHeight = MathHelper.Lerp(
                    heightData[left, top + 1],
                    heightData[left + 1, top + 1],
                    xNormalized);
            }
            catch (Exception e)
            {
                topHeight = heightData[left, top];
                bottomHeight = heightData[left, top];
                e.ToString();
            }

            // next, interpolate between those two values to calculate the height at our
            // position.
            height = MathHelper.Lerp(topHeight, bottomHeight, zNormalized) / (carscale / scaleval);

            Vector3 topNormal = Vector3.Lerp(
                terrainVertices[left + (top * terrainWidth)].Normal,
                terrainVertices[left + 1 + (top * (terrainWidth))].Normal,
                xNormalized);

            Vector3 bottomNormal = Vector3.Lerp(
                terrainVertices[left + (top * terrainWidth) + 1].Normal,
                terrainVertices[left + 1 + (top * terrainWidth) + 1].Normal,
                xNormalized);

            normal = Vector3.Lerp(topNormal, bottomNormal, zNormalized);
            normal.Normalize();
            normal = normal * carscale;
            normal.Normalize();

        }

        //updates the matrix that reflects the world
        private void UpdateRefelectionMatrix(ChaseCamera camera)
        {
            Vector3 reflectionCameraPosition = camera.Position;
            reflectionCameraPosition.Y = -camera.Position.Y + heightOfWater * 2;
            Vector3 reflectionCameraTargetPostion = camera.chasePosition;
            reflectionCameraTargetPostion.Y = -camera.chasePosition.Y + heightOfWater * 2;

            reflectionMatrix = Matrix.CreateLookAt(reflectionCameraPosition, reflectionCameraTargetPostion, camera.up);
        }

        public void Draw(GameTime gameTime, ChaseCamera camera, City cityToRender, SkyDome skyDome, Viewport view, bool enableWater)
        {
            domeRotation += 0.01f;
            if (domeRotation > 360)
                domeRotation = 0;

            graphics.GraphicsDevice.Viewport = view;
            float time = (float)gameTime.TotalGameTime.TotalMilliseconds / 100.0f;
            //device.RenderState.FillMode = FillMode.WireFrame;

            if (enableWater)
            {
                UpdateRefelectionMatrix(camera);
                DrawRefractionMap(camera.view, camera.projection);
                DrawReflectionMap(camera.view, camera.projection, skyDome);
                device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
            }

            drawEnviroment(camera.view, camera.projection);
            if (cityToRender != null)
                cityToRender.Draw(gameTime, camera.view, camera.projection);
            skyDome.DrawSkyDome(camera.view, camera.projection, domeRotation);

            if (enableWater)
                drawWater(camera.view, camera.projection, camera.Position, time);

            //create tree billboards
            DrawBillboards(camera.view, camera.projection, camera.Position);
        }

        private void DrawBillboards(Matrix viewMatrix, Matrix projectionMatrix, Vector3 position)
        {
            effect.CurrentTechnique = effect.Techniques["CylBillboard"];
            effect.Parameters["worldMatrix"].SetValue(Matrix.Identity);
            effect.Parameters["viewMatrix"].SetValue(viewMatrix);
            effect.Parameters["projectionMatrix"].SetValue(projectionMatrix);
            effect.Parameters["cameraPosition"].SetValue(position);
            effect.Parameters["xAllowedRotDir"].SetValue(new Vector3(0, 1, 0));
            effect.Parameters["xBillboardTexture"].SetValue(tree);

            effect.Begin();

            device.Vertices[0].SetSource(treeBuffer, 0, VertexPositionTexture.SizeInBytes);
            device.VertexDeclaration = treeDeclaration;
            int noVertices = treeBuffer.SizeInBytes / VertexPositionTexture.SizeInBytes;
            int noTriangles = noVertices / 3;
            device.RenderState.AlphaTestEnable = true;
            device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;
            device.RenderState.ReferenceAlpha = 200;

            effect.CurrentTechnique.Passes[0].Begin();
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, noTriangles);
            effect.CurrentTechnique.Passes[0].End();
            device.RenderState.DepthBufferWriteEnable = false;

            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            device.RenderState.AlphaTestEnable = true;
            device.RenderState.AlphaFunction = CompareFunction.Less;
            device.RenderState.ReferenceAlpha = 200;

            effect.CurrentTechnique.Passes[0].Begin();
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, noTriangles);
            effect.CurrentTechnique.Passes[0].End();

            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.DepthBufferWriteEnable = true;
            device.RenderState.AlphaTestEnable = false;
            effect.End();
        }

        private void drawEnviroment(Matrix viewMatrix, Matrix projectionMatrix)
        {
            //using effect for multitexturing
            effect.CurrentTechnique = effect.Techniques["MultiTextured"];
            effect.Parameters["xTexture0"].SetValue(texture0);
            effect.Parameters["xTexture1"].SetValue(texture1);
            effect.Parameters["xTexture2"].SetValue(texture2);
            effect.Parameters["xTexture3"].SetValue(texture3);

            Matrix worldMatrix = Matrix.Identity * Matrix.CreateScale(scaleval);
            effect.Parameters["worldMatrix"].SetValue(worldMatrix);
            effect.Parameters["viewMatrix"].SetValue(viewMatrix);
            effect.Parameters["projectionMatrix"].SetValue(projectionMatrix);

            effect.Parameters["enableLighting"].SetValue(true);
            effect.Parameters["ambientIntensity"].SetValue(0.9f);
            effect.Parameters["lightDirection"].SetValue(new Vector3(6.0f, 0, 0f));

            device.Vertices[0].SetSource(terrainVertexBuffer, 0, VertexMultitextured.SizeInBytes);
            device.Indices = terrainIndexBuffer;
            device.VertexDeclaration = terrainVertexDeclaration;

            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                int noVertices = terrainVertexBuffer.SizeInBytes / VertexMultitextured.SizeInBytes;
                int noTriangles = terrainIndexBuffer.SizeInBytes / sizeof(int) / 3;

                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, noVertices, 0, noTriangles);
                pass.End();
            }
            effect.End();
        }

        //use effect file water technique to create the water bump mapping effect
        private void drawWater(Matrix viewMatrix, Matrix projectionMatrix, Vector3 cameraPosition,float time)
        {
            effect.CurrentTechnique = effect.Techniques["Water"];
            Matrix worldMatrix = Matrix.Identity;
            effect.Parameters["worldMatrix"].SetValue(worldMatrix);
            effect.Parameters["viewMatrix"].SetValue(viewMatrix);
            effect.Parameters["reflectionViewMatrix"].SetValue(reflectionMatrix);
            effect.Parameters["projectionMatrix"].SetValue(projectionMatrix);
            effect.Parameters["xReflectionMap"].SetValue(reflectionMap);
            effect.Parameters["xRefractionMap"].SetValue(refractionMap);
            effect.Parameters["xWaterBumpMap"].SetValue(waterBumpMap);
            effect.Parameters["waveLength"].SetValue(0.005f);
            effect.Parameters["waveHeight"].SetValue(0.3f);
            effect.Parameters["cameraPosition"].SetValue(cameraPosition);

            effect.Parameters["time"].SetValue(time);
            effect.Parameters["windForce"].SetValue(0.00005f);
            effect.Parameters["windDirection"].SetValue(new Vector3(1,0,0));

            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                device.Vertices[0].SetSource(waterVerticesBuffer, 0, VertexPositionTexture.SizeInBytes);
                device.VertexDeclaration = waterVertexDeclaration;
                int noVertices = waterVerticesBuffer.SizeInBytes / VertexPositionTexture.SizeInBytes;
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, noVertices / 3);

                pass.End();
            }
            effect.End();
        }

        //create the plane on which the bump mapping effect will be applied
        private Plane CreatePlane(float height, Vector3 planeNormalDirection, Matrix currentViewMatrix, Matrix projectionMatrix, bool clipSide)
        {
            planeNormalDirection.Normalize();
            Vector4 planeCoeffs = new Vector4(planeNormalDirection, height);
            if (clipSide)
                planeCoeffs *= -1;

            Matrix worldViewProjection = currentViewMatrix * projectionMatrix;
            Matrix inverseWorldViewProjection = Matrix.Invert(worldViewProjection);
            inverseWorldViewProjection = Matrix.Transpose(inverseWorldViewProjection);

            planeCoeffs = Vector4.Transform(planeCoeffs, inverseWorldViewProjection);
            Plane finalPlane = new Plane(planeCoeffs);

            return finalPlane;
        }

        //draw the refraction map onto the plane
        public void DrawRefractionMap(Matrix viewMatrix, Matrix projectionMatrix)
        {
            Plane refractionPlane = CreatePlane(heightOfWater + 1.5f, new Vector3(0, -1, 0), viewMatrix, projectionMatrix, false);
            device.ClipPlanes[0].Plane = refractionPlane;
            device.ClipPlanes[0].IsEnabled = true;
            device.SetRenderTarget(0, refraction);

            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
            drawEnviroment(viewMatrix, projectionMatrix);

            device.ClipPlanes[0].IsEnabled = false;

            device.SetRenderTarget(0, null);
            refractionMap = refraction.GetTexture();
        }

        //draw the reflectionMap onto the plane
        public void DrawReflectionMap(Matrix viewMatrix, Matrix projectionMatrix, SkyDome skyDome)
        {
            Plane reflectionPlane = CreatePlane(heightOfWater - 0.5f, new Vector3(0, -1, 0), reflectionMatrix, projectionMatrix, true);
            device.ClipPlanes[0].Plane = reflectionPlane;
            device.ClipPlanes[0].IsEnabled = true;
            device.SetRenderTarget(0, reflection);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
            drawEnviroment(reflectionMatrix, projectionMatrix);
            skyDome.DrawSkyDome(reflectionMatrix, projectionMatrix, domeRotation);
            device.ClipPlanes[0].IsEnabled = false;

            device.SetRenderTarget(0, null);
            reflectionMap = reflection.GetTexture();
        }
    }
}
