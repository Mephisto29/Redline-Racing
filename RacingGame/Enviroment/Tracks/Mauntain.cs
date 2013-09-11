/*
 * This class represents the mauntine race track
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
    class Mountain : Race
    {
        //contructor
        public Mountain(ref GraphicsDeviceManager newGraphics, ref HeightMapGenerator Generator, ref Texture2D newmap, ref SkyDome dome, ref Effect neweffect, bool addBots, ref Texture2D treeMap)
            : base(ref newGraphics, ref Generator, ref dome, ref neweffect, 0, 7, 16, 35, addBots)
        {
            this.Map = newmap;
            this.treeMap = treeMap;

            graphics = newGraphics;
            device = graphics.GraphicsDevice;

            //starting points
            startingpoints[0] = new Vector3(300f, 0f, -1880f);
            startingpoints[1] = new Vector3(289f, 0f, -1905f);
            startingpoints[2] = new Vector3(323f, 0f, -1916f);
            startingpoints[3] = new Vector3(327f, 0f, -1887f);

            this.StartPoints = startingpoints;
            LoadContent();

            heightCollision = 30;
        }

        public void LoadContent()
        {
            //waypoints
            wayPoints = new Vector3[7];
            wayPoints[0] = new Vector3(614.3f, 6f, -1953.5f);
            wayPoints[1] = new Vector3(567.9f, 2.6f, -1263.8f);
            wayPoints[2] = new Vector3(963.1f, 2.6f, -1408.8f);
            wayPoints[3] = new Vector3(1551.3f, base.generator.getHeight(new Vector3(1551.3f, 0f, 1032.4f)), -1032.4f);
            wayPoints[4] = new Vector3(648.9f, 2.3f, -441.7f);
            wayPoints[5] = new Vector3(400.0f, base.generator.getHeight(new Vector3(400.0f, 0f, 129.9f)), -129.9f);
            wayPoints[6] = new Vector3(1776.3f, base.generator.getHeight(new Vector3(1776.3f, 0f, 312.2f)), -312.2f);

            direction = startingpoints[0] - wayPoints[0];

            waypointSpheres = new BoundingSphere[wayPoints.Length];
            for (int i = 0; i < wayPoints.Length; i++)
                waypointSpheres[i] = new BoundingSphere(wayPoints[i] / 20f, 1.5f);
        }
    }
}
