/*
 * This class represents the alpine race track
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
using RacingGame.Engine.TerrainLoader;
using RacingGame.Engine.HelperFunctions;
using RacingGame.Enviroment;

namespace RacingGame.Enviroment.Tracks
{
    class Plains : Race
    {
        //constructor
        public Plains(ref GraphicsDeviceManager newGraphics, ref HeightMapGenerator Generator, ref Texture2D newmap, ref SkyDome dome, ref Effect neweffect, bool addBots, ref Texture2D treeMap)
            : base(ref newGraphics, ref Generator, ref dome, ref neweffect, 0, 7, 16, 35, addBots)
        {
            this.Map = newmap;
            this.treeMap = treeMap;

            graphics = newGraphics;
            device = graphics.GraphicsDevice;

            //set the starting points for the track
            startingpoints[0] = new Vector3(355.7f, 0f, -474.7f);
            startingpoints[1] = new Vector3(385.33f, 0f, -479.5f);
            startingpoints[2] = new Vector3(323f, 0f, -1916f);
            startingpoints[3] = new Vector3(327f, 0f, -1887f);

            this.StartPoints = startingpoints;
            LoadContent();

            heightCollision = 51;
        }

        public void LoadContent()
        {
            //set the waypoints for the track
            wayPoints = new Vector3[7];
            wayPoints[0] = new Vector3(370.6f, 0f, -280.1f);
            wayPoints[1] = new Vector3(884.3f, base.generator.getHeight(new Vector3(884.3f, 0f, 217f)), -217f);
            wayPoints[2] = new Vector3(1187.5f, base.generator.getHeight(new Vector3(1187.5f, 0f, 856.5f)), -856.5f);
            wayPoints[3] = new Vector3(250.5f, base.generator.getHeight(new Vector3(250.5f, 0f, 1304.85f)), -1304.85f);
            wayPoints[4] = new Vector3(109.9f, base.generator.getHeight(new Vector3(109.9f, 0f, 1691.88f)), -1691.88f);
            wayPoints[5] = wayPoints[4];
            wayPoints[6] = wayPoints[4];

            direction = startingpoints[0] - wayPoints[0];

            waypointSpheres = new BoundingSphere[wayPoints.Length];
            for (int i = 0; i < wayPoints.Length; i++)
                waypointSpheres[i] = new BoundingSphere(wayPoints[i] / 20f, 1f);
        }
    }
}
