/*
 * This class represents the shop marker menu
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
using RacingGame.Engine.UI;

namespace RacingGame.GameMenus
{
    class StoryMenu : GameScreen
    {
        //Game state
        private GameState nextState;
        private GameState previousState;

        private int playerLevel;

        private string level1Story;
        private string level4Story;
        private string level8Story;
        private string level10Story;
        private string story;

        //getters and setters
        public GameState NextState
        {
            get { return nextState; }
        }

        public GameState PreviousState
        {
            get { return previousState; }
            set { previousState = value; }
        }

        public int PlayerLevel
        {
            get { return playerLevel; }
            set { playerLevel = value; }
        }

        //constructor
        public StoryMenu(ref GraphicsDeviceManager graphics, Game game, ref SpriteBatch spriteBatch, ref SpriteFont spriteFont, ref String[] menuItems, ref InputHandler newInput, ref Texture2D newBackground, ref GameStateManager currentState, String newMenuTitle, ref GameTime activated)
            : base(ref graphics, game, ref spriteBatch, ref spriteFont, ref menuItems, ref newInput, ref newBackground, ref currentState, ref activated)
        {
            menuActive = false;
            menuTitle = newMenuTitle;

            level1Story = "You are John Carter, a retired Miami police detective. \n" +
                          "You have single-handedly ended many illegal racer's careers. \n" +
                          "You have decided to retire to a old forgotten city, nested deep \n" +
                          "in the valleys and mountains, far from the prying eyes of those \n" +
                          "that might hold grudges against you. \n \n" + 
                          "On your arrival however, you soon heard rumours of an underground \n" +
                          "racing community that operated from your new city.\n" + 
                          "You have decided to come out of retirement one last time and \n" +
                          "bring the existence of the community into the light.\n" + 
                          "But to do that, you have to enter illegal races and gain the \n" +
                          "attention of the ring leader, and bring him out of hiding.";

            level4Story = "Your fame has grown and you are getting the attention of \n" +
                          "the racing community.  Your connections through the races \n" +
                          "so far have led you to a backyard garage where where a citizen \n" +
                          "distributes NOS canisters and in exchange for your secrecy, you \n"  +
                          "have agreed to accept some of those NOS canisters for the greater good.\n" +
                          "You figured that to succeed in his mission, he had to be faster,\n" +
                          "faster than the rest. You had to be the best.\n" +
                          "You decided you could worry about the backyard garage after you \n" +
                          "handled all of its customers. \n \n" +
                          "NOS is now available for all cars, you can use your NOS \n" +
                          "in races and freeroam using the LEFT BUMPER or RIGHT CTRL";

            level8Story = "Your fame and respect have grown, and you have gained the \n" + 
                          "attention of the criminal racing ring. \n" +
                          "They have contacted you and said that yout need to win\n" +
                          "three more races to be able to chalange their leader to a race.\n\n" +
                          "This is the final stretch to radicating the racing in \n" +
                          "Your city.  Can you complete the job?";

            level10Story = "Well this is a surprise,  Looks like the leader couldn't wait.\n" +
                           "He has shown up a race early.  His funeral.\n" +
                           "You will take him down and rid this place of scum like him.\n\n"+
                           "If you win this one, it's all over.\n" +
                           "No more races, only your city.  You will have won.";

            story = level1Story;

            input.reset();
        }

        public void Story()
        {
            if (playerLevel == 1)
                story = level1Story;
            else if (playerLevel == 4)
                story = level4Story;
            else if (playerLevel == 8)
                story = level8Story;
            else if (playerLevel == 10)
                story = level10Story;
        }

        //handeling input and changing game state
        public override void Update(GameTime gameTime)
        {
            if (input.SelectInMenu)
            {
                if (gameTime.TotalGameTime - previousControllerButtonCooldown > (controllerButtonCooldown + transitionOnTime))
                {
                    if (selectedIndex == 0)
                    {
                        nextState = previousState;
                        gameStateChanged = true;
                        input.reset();
                    }
                }
            }

            base.Update(gameTime);
        }

        // draw the menu
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(menuBackground, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(spriteFont, story, new Vector2(0, 175), Color.Gray);

            base.Draw(gameTime);
        }
    }
}
