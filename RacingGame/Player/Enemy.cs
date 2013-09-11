/*
 * This class represents a player with all  the required player data
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

using RacingGame.Engine.Physics;
using RacingGame.Engine;

namespace RacingGame.PlayerData
{
    class Enemy
    {
        //private data members
        private Vehicle currentPlayerCar;
        private InputHandler input;
        private string playerName;
        private int currentWaypoint = 0;
        private List<Vector3> wayPoints;

        //protected variables
        protected Vector3 position;
        protected Vector3 nextPosition;

        //getters and setters
        public int CurrentWaypoint
        {
            get { return currentWaypoint; }
            set { currentWaypoint = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 NewPosition
        {
            get { return nextPosition; }
            set { nextPosition = value; }
        }

        public Vehicle Car
        {
            get { return currentPlayerCar; }
            set { currentPlayerCar = value; }
        }

        public string Name
        {
            get { return playerName; }
            set { playerName = value; }
        }

        public InputHandler Input
        {
            get { return input; }
        }

        public List<Vector3> WaypointList
        {
            get { return wayPoints; }
            set { wayPoints = value; }
        }

        //constructor
        public Enemy(String newPlayerName, GameStateManager currentState, List<Vector3> aiWaypoints)
        {
            playerName = newPlayerName;
            input = new InputHandler(currentState);
            input.Player = 2;
            wayPoints = aiWaypoints;
        }
    }
}