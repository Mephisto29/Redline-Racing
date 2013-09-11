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
    class Alpine : Race
    {
        //constructor
        public Alpine(ref GraphicsDeviceManager newGraphics, ref HeightMapGenerator Generator, ref Texture2D newmap, ref SkyDome dome, ref Effect neweffect, bool addBots, ref Texture2D treeMap)
            : base(ref newGraphics, ref Generator, ref dome, ref neweffect, 0, 7, 16, 35,addBots)
        {
            this.Map = newmap;
            this.treeMap = treeMap;

            graphics = newGraphics;
            device = graphics.GraphicsDevice;

            //set the starting points for the track
            startingpoints[0] = new Vector3(300f, 0f, -1880f);
            startingpoints[1] = new Vector3(289f, 0f, -1905f);
            startingpoints[2] = new Vector3(323f, 0f, -1916f);
            startingpoints[3] = new Vector3(327f, 0f, -1887f);

            this.StartPoints = startingpoints;
            LoadContent();

            heightCollision = 44;
        }

        public void LoadContent()
        {
            //set the waypoints for the track
            wayPoints = new Vector3[7];
            wayPoints[0] = new Vector3(614.3f, 6f, -1953.5f);
            wayPoints[1] = new Vector3(571.0f, base.generator.getHeight(new Vector3(571.0f, 0f, 1245.6f)), -1245.6f);
            wayPoints[2] = new Vector3(929.4f, base.generator.getHeight(new Vector3(929.4f, 0f, 1408.8f)), -1408.8f);
            wayPoints[3] = new Vector3(1551.3f, base.generator.getHeight(new Vector3(1551.3f, 0f, 1032.4f)), -1032.4f);
            wayPoints[4] = new Vector3(1078.5f, base.generator.getHeight(new Vector3(1078.5f, 0f, 675.0f)), -675.0f);
            wayPoints[5] = wayPoints[3];
            wayPoints[6] = new Vector3(1193.6f, base.generator.getHeight(new Vector3(1193.6f, 0f, 1547.3f)), -1547.3f);

            direction = startingpoints[0] - wayPoints[0];

            waypointSpheres = new BoundingSphere[wayPoints.Length];
            for (int i = 0; i < wayPoints.Length; i++)
                waypointSpheres[i] = new BoundingSphere(wayPoints[i] / 20f, 1f);
        }
    }
}
