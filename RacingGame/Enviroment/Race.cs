/*
 * This class represents a race track, all tracks inherit from this class
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
using RacingGame.Enviroment;
using RacingGame.Engine.TerrainLoader;
using RacingGame.PlayerData;
using RacingGame.Engine.Physics;

namespace RacingGame.Enviroment
{
    class Race
    {
        public float heightCollision = 51;

        //device variables
        protected GraphicsDeviceManager graphics;
        protected GraphicsDevice device;
        protected GameStateManager currentState;

        //skydome data
        protected SkyDome dome;

        protected Vector3[] startingpoints = new Vector3[4];
        protected Vector3[] wayPoints;
        protected Vector3 direction;
        protected BoundingSphere[] waypointSpheres;
        protected Player leader;
        protected int point;
        protected int ppoint;
        protected int players;
        protected int finishers;
        protected bool finished;
        protected string leaderName;
        protected string winner;

        protected HeightMapGenerator generator;

        //loading camera
        protected ChaseCamera camera;

        protected Texture2D map;
        protected Texture2D mapCollision;
        protected Texture2D[] textures = new Texture2D[5];
        protected float texture0Height;
        protected float texture1Height;
        protected float texture2Height;
        protected float texture3Height;

        protected float winnings;

        protected Effect effect;

        //ai data
        protected bool addBots;
        protected List<Vector3> enemyAI;
        protected List<Vector3> enemyAIreset;
        protected Enemy[] enemies;
        protected Vector3 nextPosition;
        protected Vector3 nextWaypoint;
        protected Vector3 nextDirection;

        protected double angle;

        //treemap data
        protected Texture2D treeMap;

        //getters and setters
        public Vector3[] StartPoints
        {
            get { return startingpoints; }
            set { startingpoints = value; }
        }

        public List<Vector3> EnemyAI
        {
            get { return enemyAI; }
            set { enemyAI = value; }
        }

        public List<Vector3> EnemyAIReset
        {
            get { return enemyAIreset; }
            set { enemyAIreset = value; }
        }

        public string LeaderName
        {
            get { return leaderName; }
            set { leaderName = value; }
        }

        public float Winnings
        {
            get { return winnings; }
            set { winnings = value; }
        }

        public int Players
        {
            get { return players; }
            set { players = value; }
        }

        public Player Leader
        {
            get { return leader; }
            set { leader = value; }
        }

        public Enemy[] Enemies
        {
            get { return enemies; }
        }

        public Vector3 Direction
        {
            get { return direction; }
        }

        public double Angle
        {
            get { return angle; }
        }

        public HeightMapGenerator CurrentMapGenerator
        {
            get { return generator; }
        }

        public bool Finished
        {
            get { return finished; }
            set { finished = value; }
        }

        public Vector3[] WayPoints
        {
            get { return wayPoints; }
            set { wayPoints = value; }
        }

        public Vector3 NextWayPoint
        {
            get { return nextWaypoint; }
            set { nextWaypoint = value; }
        }

        public BoundingSphere[] WaypointSpheres
        {
            get { return waypointSpheres; }
            set { waypointSpheres = value; }
        }

        public Texture2D[] Textures
        {
            get { return textures; }
            set { textures = value; }
        }

        public Texture2D Map
        {
            get { return map; }
            set { map = value; }
        }
        public Texture2D MapCollision
        {
            get { return mapCollision; }
            set { mapCollision = value; }
        }

        public int CurrentWaypoint
        {
            get { return ppoint; }
            set { ppoint = value; }
        }

        public bool AIenabled
        {
            get { return addBots; }
        }

        public string Winner
        {
            get { return winner; }
            set { winner = value; }
        }

        public Enemy[] AIBots
        {
            get { return enemies; }
            set { enemies = value; }
        }

        //Constructor
        public Race(ref GraphicsDeviceManager newGraphics, ref HeightMapGenerator Generator, ref SkyDome dome, ref Effect neweffect, float newTexture0Height, float newTexture1Height, float newTexture2Height, float newTexture3Height, bool addAIBots)
        {
            graphics = newGraphics;
            generator = Generator;

            this.dome = dome;
            this.effect = neweffect;
            point = 0;
            ppoint = 0;
            finished = false;
            leader = null;
            finishers = 0;
            angle = 0;

            texture0Height = newTexture0Height;
            texture1Height = newTexture1Height;
            texture2Height = newTexture2Height;
            texture3Height = newTexture3Height;

            addBots = addAIBots;
        }

        //load the content for the Race
        public void LoadContent(ref Texture2D t0, ref Texture2D t1, ref Texture2D t2, ref Texture2D t3, ref Texture2D waterBump, ref ChaseCamera newcamera, ref GameStateManager newState, ref Texture2D newMapCollision)
        {
            //set data
            this.camera = newcamera;
            this.currentState = newState;
            textures[0] = t0;
            textures[1] = t1;
            textures[2] = t2;
            textures[3] = t3;
            textures[4] = waterBump;
            mapCollision = newMapCollision;

            //set the current device
            device = graphics.GraphicsDevice;
            generator.Texture0 = textures[0];
            generator.Texture1 = textures[1];
            generator.Texture2 = textures[2];
            generator.Texture3 = textures[3];

            generator.Texture0Height = texture0Height;
            generator.Texture1Height = texture1Height;
            generator.Texture2Height = texture2Height;
            generator.Texture3Height = texture3Height;
            generator.setTreeHeight = heightCollision/20;
            generator.setTreeMap = treeMap;

            generator.buildTerrain();
            generator.LoadContent(new Vector3(0, 0, 0));

            if (addBots)
                ResetEnemy();
        }

        public void PassWaypoint(Vehicle enemy)
        {
            if (enemy.Sphere.Contains(waypointSpheres[point]) != ContainmentType.Disjoint)
            {
                if (point < waypointSpheres.Length - 1)
                {
                    point++;
                    if (point > ppoint)
                        leaderName = "Enemy";
                }
                else
                {
                    winner = "Enemy";
                    finished = true;
                }
            }
        }

        public void PassWaypoint(Player player)
        {
            if (player.Car.Sphere.Contains(waypointSpheres[ppoint]) != ContainmentType.Disjoint)
            {
                if (ppoint < waypointSpheres.Length - 1)
                {
                    //point++;
                    ppoint++;
                    if (ppoint > point)
                        leaderName = player.Name;
                }
                else
                {
                    winner = player.Name;
                    leader = player;
                    finished = true;
                }
            }
        }

        public void updateAI(GameTime gameTime, bool canRace, bool canPlaySound, ref Player player, float soundVolume)
        {
            if (addBots)
            {
                if (canRace)
                {
                    for (int a = 0; a < enemies.Length; a++)
                    {
                        enemyAI = enemies[a].WaypointList;

                        if ((angle < 0.8 && angle > 0) || (angle > -0.8 && angle < 0))
                        {
                            enemies[a].Input.IsAccelerating = true;
                            enemies[a].Input.HandBrakeEngaged = false;
                        }
                        else
                        {
                            enemies[a].Input.HandBrakeEngaged = true;
                            enemies[a].Input.IsAccelerating = false;
                        }

                        //if at created, set to the first ai waypoint
                        if (enemies[a].NewPosition == Vector3.Zero)
                        {
                            enemies[a].NewPosition = findClosestPoint(enemies[a].Car.Position);
                            nextDirection = enemies[a].Car.Position - enemies[a].NewPosition;
                            enemies[a].Car.Direction = nextDirection;
                        }

                        //check if in range with a ai waypoint
                        if (enemies[a].Car.Position.X > (enemies[a].NewPosition.X - 15) && enemies[a].Car.Position.X < (enemies[a].NewPosition.X + 15) &&
                            enemies[a].Car.Position.Z > (enemies[a].NewPosition.Z - 15) && enemies[a].Car.Position.Z < (enemies[a].NewPosition.Z + 15))
                        {
                            nextPosition = findClosestPoint(enemies[a].Car.Position);

                            if (nextPosition != Vector3.Zero)
                            {
                                enemyAI.Remove(nextPosition);
                                enemies[a].WaypointList = enemyAI;
                            }
                            else
                                nextPosition = wayPoints[wayPoints.Length - 1];

                            enemies[a].NewPosition = nextPosition;

                            nextDirection = enemies[a].Car.Position - enemies[a].NewPosition;
                            //enemies[a].Car.Direction = nextDirection;
                        }
                        nextDirection = enemies[a].Car.Position - enemies[a].NewPosition;

                        //enable ai car to turn using physics engine
                        Vector3 v1crossv2 = Vector3.Cross(enemies[a].Car.Direction, nextDirection);
                        angle = Math.Atan2(Vector3.Dot(Vector3.Up,(v1crossv2)), Vector3.Dot(enemies[a].Car.Direction, nextDirection));

                        if (angle > 0 && !(angle < 0.05))
                        {
                            enemies[a].Input.IsTurningLeft = true;
                            enemies[a].Input.IsTurningRight = false;
                        }

                        else if (angle < 0 && !(angle > -0.05))
                        {
                            enemies[a].Input.IsTurningLeft = false;
                            enemies[a].Input.IsTurningRight = true;
                        }

                        else
                        {
                            enemies[a].Input.IsTurningRight = false;
                            enemies[a].Input.IsTurningLeft = false;
                        }

                        //adjust AI bots sounds
                        float distance = 1-Vector3.Distance(player.Car.Position, enemies[a].Car.Position)/220;

                        if(distance >= 1 || distance < 0)
                            enemies[a].Car.Sound.AdjustVolume(0);
                        else
                            enemies[a].Car.Sound.AdjustVolume(distance * soundVolume);

                        enemies[a].Car.update(gameTime, canRace, canPlaySound);
                        PassWaypoint(enemies[a].Car);
                    }
                }
            }
        }

        public void Draw(GameTime gameTime, ChaseCamera cam, Viewport port, bool enableWater)
        {
            generator.Draw(gameTime, cam, null, dome, port, enableWater);

            if (addBots)
                for (int a = 0; a < enemies.Length; a++)
                    enemies[a].Car.draw(cam, port,gameTime);
        }

        public Vector3 findClosestPoint(Vector3 position)
        {
            float distanceToCurrentPoint = float.MaxValue;
            Vector3 currentPoint = Vector3.Zero;

            if (enemyAI.Count > 0)
            {
                for (int a = 0; a < enemyAI.Count - 1; a++)
                {
                    float distance = Vector3.Distance(position, enemyAI[a]);

                    if (distance < distanceToCurrentPoint)
                    {
                        distanceToCurrentPoint = distance;
                        currentPoint = enemyAI[a];
                    }
                }
            }

            return currentPoint;
        }

        public void ResetEnemy()
        {
            generator.loadAiMap();
            enemyAI = generator.AIwaypoints;
            enemyAIreset = enemyAI;

            enemies = new Enemy[1];

            for (int a = 0; a < enemies.Length; a++)
            {
                Vector3[] originalData = enemyAI.ToArray();
                List<Vector3> newList = new List<Vector3>(originalData);

                enemies[a] = new Enemy("Bot" + (a + 1), currentState, newList);
            }
        }
    }
}
