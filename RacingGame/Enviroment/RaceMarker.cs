/*
 * This class represents the marker for a race track
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

using RacingGame.Enviroment.Tracks;
using RacingGame.Engine;
using RacingGame.Engine.TerrainLoader;

namespace RacingGame.Enviroment
{
    public enum RaceMapType
    {
        Alpine,
        Mountain,
        Plains
    }

    class RaceMarker
    {
        //protected data members
        protected GraphicsDeviceManager graphics;
        protected GraphicsDevice device;
        protected GameStateManager currentState;
        protected Race race;
        protected RaceMapType currentRaceType;

        protected Model raceMarkerModel;
        protected Vector3 position;

        protected HeightMapGenerator mapGenerator;
        protected Texture2D map;
        protected Texture2D mapCollision;
        protected Texture2D mapAI;
        protected Effect effect;

        protected SkyDome skyDome;
        protected ChaseCamera camera;

        protected BoundingSphere collisionSphere;

        protected string mapInformation;

        //getters and setters
        public Race currentRace
        {
            get { return race; }
        }

        public RaceMapType SetRaceType
        {
            get { return currentRaceType; }
            set { currentRaceType = value; }
        }

        public HeightMapGenerator MapGenerator
        {
            get { return mapGenerator; }
        }

        public BoundingSphere CollisionSphere
        {
            get { return collisionSphere; }
        }

        public string MapInformation
        {
            get { return mapInformation; }
            set { mapInformation = value; }
        }

        public Texture2D currentRaceMap
        {
            get { return map; }
        }
        public Texture2D currentRaceMapCollision
        {
            get { return mapCollision; }
        }

        //constructor
        public RaceMarker(ref GraphicsDeviceManager newGraphics, ref GameStateManager state, ref Model newModel, Vector3 newPosition, ref Effect newEffect, ref Texture2D newMap, ref Texture2D newMapCollision, ref SkyDome dome, ref Texture2D newMapAI)
        {
            graphics = newGraphics;
            device = graphics.GraphicsDevice;
            currentState = state;

            raceMarkerModel = newModel;
            position = newPosition;
            map = newMap;
            mapCollision = newMapCollision;
            mapAI = newMapAI;
            effect = newEffect;
            skyDome = dome;

            collisionSphere = new BoundingSphere(position, 0.2f);
        }

        //load the content for the race
        public void LoadRaceContent(ref Texture2D texture0, ref Texture2D texture1, ref Texture2D texture2, ref Texture2D texture3, ref Texture2D waterBumpMap, ref ChaseCamera newCamera, bool addBots, ref HeightMapGenerator newMapGenerator, ref Texture2D tree, ref Texture2D treeMap)
        {
            float defaultHeight = 51;
            mapGenerator = new HeightMapGenerator(
                               ref graphics,
                               ref map,
                               ref mapCollision,
                               mapAI,
                               ref texture0,
                               ref texture1,
                               ref texture2,
                               ref texture3,
                               ref waterBumpMap,
                               ref tree,
                               ref treeMap,
                               ref effect,
                               1.0f,
                               ref currentState,
                               0,
                               18,
                               40,
                               68,
                               6.0f,
                               ref defaultHeight);


            camera = newCamera;

            if (currentRaceType == RaceMapType.Alpine)
                race = new Alpine(ref graphics, ref mapGenerator, ref map, ref skyDome, ref effect, addBots, ref treeMap);
            else if (currentRaceType == RaceMapType.Mountain)
                race = new Mountain(ref graphics, ref mapGenerator, ref map, ref skyDome, ref effect, addBots, ref treeMap);
            else if (currentRaceType == RaceMapType.Plains)
                race = new Plains(ref graphics, ref mapGenerator, ref map, ref skyDome, ref effect, addBots, ref treeMap);

            mapGenerator.CollisionHeight = race.heightCollision;
            mapGenerator.loadAiMap();

            race.LoadContent(ref texture0, ref texture1, ref texture2, ref texture3, ref waterBumpMap, ref camera, ref currentState, ref mapCollision);
        }

        //check for collision with the race marker
        public bool checkCollision(BoundingSphere carShere)
        {
            if (collisionSphere.Contains(carShere) != ContainmentType.Disjoint)
                return true;
            else
                return false;
        }

        //draw the race marker
        public void Draw(ChaseCamera camera)
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[raceMarkerModel.Bones.Count];
            raceMarkerModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in raceMarkerModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(0.2f) * Matrix.CreateTranslation(position);

                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                }

                mesh.Draw();
            }
        }
    }
}
