/*
 * This class is used to manage the state of the game
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RacingGame.Engine
{
    //state of the game
    enum GameState
    {
        MainMenu,
        InGameMenu,
        InGame,
        Racing,
        InShop,
        Loading,
        MultiPlayer
    }

    class GameStateManager
    {
        //current state
        private GameState currentGameState;

        //constructor
        public GameStateManager()
        {
            currentGameState = GameState.MainMenu;
        }

        //set the new state
        public GameState CurrentState
        {
            get { return currentGameState; }
            set { currentGameState = value; }
        }
    }
}
