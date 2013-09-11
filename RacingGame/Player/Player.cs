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
    class Player
    {
        //private data members
        private Vehicle currentPlayerCar;
        private InputHandler input;

        private string playerName;
        private float currentAmmountOfMoney;
        private int level;

        //getters and setters
        public Vehicle Car
        {
            get { return currentPlayerCar; }
            set { currentPlayerCar = value; }
        }

        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public InputHandler Input
        {
            get { return input; }
        }

        public string Name
        {
            get { return playerName; }
            set { playerName = value; }
        }

        public float CurrentCash
        {
            get { return currentAmmountOfMoney; }
            set { currentAmmountOfMoney = value; }
        }

        public int CurrentLevel
        {
            get { return level; }
            set { level = value; }
        }

        //modifying player cash
        public void AddToCash(float cashWon)
        {
            currentAmmountOfMoney += cashWon;
        }

        public void SubtractFromCash(float cashUsed)
        {
            currentAmmountOfMoney -= cashUsed;
        }

        //constructor
        public Player(String newPlayerName, GameStateManager currentState, int playerNumber)
        {
            playerName = newPlayerName;

            currentAmmountOfMoney = 1000;
            level = 1;

            input = new InputHandler(currentState);
            input.Player = playerNumber;
        }
    }
}