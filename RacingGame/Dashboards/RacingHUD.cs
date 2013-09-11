/*
 * This class is used to draw the racing HUD, i.e. the countdown and the minimap
 * 
 * to do the minimap the heightmap image is converted to a an array, the position in game is translated to a position on the
 * texture where a marker texture is drawn
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
using RacingGame.PlayerData;

namespace RacingGame.Dashboards
{
    class RacingHUD
    {
        //private data members
        private Player currentPlayer;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameTime time;
        private TimeSpan countdownTimer;
        private TimeSpan second;
        private TimeSpan twoSecond;

        private SpriteFont font;
        private int countdown;

        private bool canRace;

        private Race race;
        private Texture2D raceInformationHud;
        private Texture2D raceMap;
        private Texture2D playerIndicator;
        private Texture2D enemyIndicator;
        private Texture2D waypointIndicator;
        private Vector2 playerPositionInTexture;
        private Vector2 enemyPositionInTexture;
        private Vector2 wayPointPositionInTexture;

        private float scale;
        private float hudScale;
        private uint[] textureArray;

        //waypoint data of the race
        private int totalWaypoints;
        private int currentWaypoint;

        //timer data
        private TimeSpan timer;
        private bool started = false;
        private bool startedchanged = true;
        private int minutes;
        private int seconds;
        private int milliseconds;

        //leader data
        string leaderName = "";

        //offtrack data
        private bool offtrack = false;

        //method to state if in race
        public bool CanRace
        {
            get { return canRace; }
        }

        public bool OffTrack
        {
            set { offtrack = value; }
        }

        public float setMapScale
        {
            set { scale = value; }
        }

        public float setHUDScale
        {
            set { hudScale = value; }
        }

        public string leader
        {
            set { leaderName = value; }
        } 

        public string getTime
        {
            get { return minutes.ToString() + ":" + (seconds < 10 ? "0" + seconds.ToString() : seconds.ToString()) + ":" + (milliseconds < 10 ? "0" + milliseconds.ToString() : milliseconds.ToString()); }
        }

        //constructor
        public RacingHUD(ref GraphicsDeviceManager newGraphics, ref SpriteBatch newSpriteBatch, ref Race currentRace, Texture2D newPlayerIndicator, Texture2D newEnemyIndicator,Texture2D newWaypointIndicator, ref SpriteFont newFont, ref Texture2D raceInformation, GameTime newTime, ref Player player)
        {
            graphics = newGraphics;
            spriteBatch = newSpriteBatch;
            time = newTime;
            countdownTimer = time.TotalGameTime;
            second = TimeSpan.FromSeconds(1);
            twoSecond = TimeSpan.FromSeconds(2);
            currentPlayer = player;

            canRace = false;

            race = currentRace;
            raceMap = race.Map;
            playerIndicator = newPlayerIndicator;
            enemyIndicator = newEnemyIndicator;
            waypointIndicator = newWaypointIndicator;
            raceInformationHud = raceInformation;
            textureArray = new uint[race.Map.Width * race.Map.Height];
            font = newFont;

            totalWaypoints = race.WayPoints.Length;
            currentWaypoint = race.CurrentWaypoint;

            countdown = 4;
            scale = 0.6f;
            hudScale = 1.0f;
        }

        //update for the countdown of the race 
        public void update(Vector3 player1Postion, Vector3 player2Postion, GameTime gameTime)
        {
            if (canRace)
            {
                if (started && startedchanged)
                {
                    timer = time.TotalGameTime;
                    seconds = 0;
                    minutes = 0;
                    startedchanged = false;
                }
            }

            if ((time.TotalGameTime - timer) >= second)
                milliseconds++;
            if (milliseconds >= 60)
            {
                seconds++;
                milliseconds = 0;
            }
            if (seconds >= 60)
            {
                minutes++;
                seconds = 0;
            }

            playerPositionInTexture = race.CurrentMapGenerator.getPositionInTexture(player1Postion);
            if (race.Enemies != null)
                enemyPositionInTexture = race.CurrentMapGenerator.getPositionInTexture(race.Enemies[0].Car.Position);
            else if(player2Postion!= Vector3.Zero)
                enemyPositionInTexture = race.CurrentMapGenerator.getPositionInTexture(player2Postion);
            wayPointPositionInTexture = race.CurrentMapGenerator.getPositionInTexture(race.WayPoints[currentPlayer.Car.Point]);

            if (countdown != 0)
            {
                if ((time.TotalGameTime - countdownTimer) >= second)
                {
                    countdownTimer = time.TotalGameTime;
                    countdown--;
                }
            }
        }

        public void updateLeaderName(string leader)
        {
            leaderName = race.LeaderName;
        }

        public void updateCurrentWaypoint(int newWaypoint)
        {
            currentWaypoint = newWaypoint;
        }

        //draw the HUD
        public void Draw()
        {
            //draw map with markers
            spriteBatch.Draw(raceMap, new Vector2(0, graphics.GraphicsDevice.Viewport.Height - raceMap.Height * scale), null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(playerIndicator, new Vector2(Math.Abs(playerPositionInTexture.Y) * scale, (graphics.GraphicsDevice.Viewport.Height - (raceMap.Height) * scale) + (Math.Abs(playerPositionInTexture.X) * scale)), null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(enemyIndicator, new Vector2(Math.Abs(enemyPositionInTexture.Y) * scale, (graphics.GraphicsDevice.Viewport.Height - (raceMap.Height) * scale) + (Math.Abs(enemyPositionInTexture.X) * scale)), null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(waypointIndicator, new Vector2(Math.Abs(wayPointPositionInTexture.Y) * scale, (graphics.GraphicsDevice.Viewport.Height - (raceMap.Height) * scale) + (Math.Abs(wayPointPositionInTexture.X) * scale)), null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1.0f);

            //draw waypoint count text
            spriteBatch.Draw(raceInformationHud, new Vector2(graphics.GraphicsDevice.Viewport.Width - raceInformationHud.Width * hudScale, 0), null, Color.White, 0.0f, Vector2.Zero, hudScale, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(font, "Current Race Leader: " + leaderName, new Vector2(graphics.GraphicsDevice.Viewport.Width - 350 * hudScale, 10 * hudScale), Color.Red, 0, Vector2.Zero, hudScale - 0.2f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(font, currentWaypoint.ToString() + "/" + totalWaypoints.ToString(), new Vector2(graphics.GraphicsDevice.Viewport.Width - 290 * hudScale, 45 * hudScale), Color.Red, 0, Vector2.Zero, hudScale - 0.2f, SpriteEffects.None, 1.0f);

            //draw current time
            if (canRace)
            {
                if (seconds >= 10)
                    spriteBatch.DrawString(font, "Time: " + minutes + ":" + seconds, new Vector2(graphics.GraphicsDevice.Viewport.Width - 270 * hudScale, 80 * hudScale), Color.Red, 0, Vector2.Zero, hudScale-0.2f, SpriteEffects.None, 1.0f);
                else
                    spriteBatch.DrawString(font, "Time: " + minutes + ":0" + seconds, new Vector2(graphics.GraphicsDevice.Viewport.Width - 270 * hudScale, 80 * hudScale), Color.Red, 0, Vector2.Zero, hudScale-0.2f, SpriteEffects.None, 1.0f);
            }

            //draw countdown
            if (countdown != 0)
                spriteBatch.DrawString(font, countdown.ToString(), new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2 - 100), Color.Red, 0, Vector2.Zero, 3f, SpriteEffects.None, 0);
            if ((countdown == 0) && (time.TotalGameTime - countdownTimer) <= twoSecond)
            {
                canRace = true;
                started = true;
                spriteBatch.DrawString(font, "GO!!", new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2 - 100), Color.Red, 0, Vector2.Zero, 3f, SpriteEffects.None, 0);
            }

            //if off track draw warning message
            if (offtrack)
                spriteBatch.DrawString(font, "WARNING, get back on track", new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 25, graphics.GraphicsDevice.Viewport.Height / 2 - 50), Color.Red, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }
}
