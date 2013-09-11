/*
 * This class is the super class of all gearboxes, contains the required information for a gear change
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

namespace RacingGame.Engine.Physics
{
    class GearboxGearChange
    {
        public int Change;
        public float TimeTillEngaged;
    }

    abstract class GearBox
    {
        //pubic constant variables
        public const int GEAR_REVERSE = 0;
        public const int GEAR_NEUTRAL = 1;
        public const int GEAR_1 = 2;

        //private data members
        //gearbox data
        private List<float> gearRatios; //gear ratios of the car

        protected float gearChangetime; //change time to change from one gear to the next
        protected float clutch; //the clutch
        protected int currentgear;  //current gear that is engaged
        protected GearboxGearChange gearChange; //the values that define a gear change
        protected CarMotor currentMotor;    //current motor of the car

        protected bool canChangeGear;

        protected InputHandler currentState; //input handeler

        //getters and setters
        public InputHandler keyBoardState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        public bool CanChangeGear
        {
            get { return canChangeGear; }
            set { canChangeGear = value; }
        }

        public float Clutch
        {
            get { return clutch; }
            set { clutch = value; }
        }

        public List<float> GearRatios
        {
            get { return gearRatios; }
            set { gearRatios = value; }
        }

        public int CurrentGear
        {
            get { return currentgear - 1; }
            set { currentgear = value + 1; }
        }

        public int NextGear
        {
            get { return CurrentGear + gearChange.Change; }
        }

        public float CurrentGearRatio
        {
            get { return gearRatios[currentgear]; }
        }

        public float NextGearRatio
        {
            get { return gearRatios[currentgear + gearChange.Change]; }
        }

        public bool gearEngaged
        {
            get { return gearChange == null; }
        }

        public CarMotor Motor
        {
            get { return currentMotor; }
            set { currentMotor = value; }
        }

        //End of getters and setters

        //constructor
        public GearBox(List<float> newRatios, float newChangeTime)
        {
            gearRatios = newRatios;
            gearChangetime = newChangeTime;
        }

        //when gearing up or down
        public void gearUp()
        {
            if (gearChange == null)
            {
                gearChange = new GearboxGearChange();
                gearChange.Change = 1;
                gearChange.TimeTillEngaged = gearChangetime;

                clutch = 0.0f;
            }
        }

        public void gearDown()
        {
            if (gearChange == null)
            {
                gearChange = new GearboxGearChange();
                gearChange.Change = -1;
                gearChange.TimeTillEngaged = gearChangetime;

                clutch = 0.0f;
            }
        }
        //end of gearing up or downs

        //update method
        public virtual void update(float motorRpmPercent, GameTime gameTime)
        {
            currentState.update(Keyboard.GetState(), gameTime);

            if (gearChange != null)
            {
                gearChange.TimeTillEngaged -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (gearChange.TimeTillEngaged <= 0)
                {
                    if (gearChange.Change > 0)
                    {
                        if (currentgear < gearRatios.Count - 1)
                            currentgear++;
                    }

                    else
                        if (currentgear > -1)
                            currentgear--;

                    clutch = 1.0f;
                    gearChange = null;
                }

                else
                    clutch = (gearChangetime - gearChange.TimeTillEngaged) / gearChangetime;
            }
        }

        //add manualgearbox choice here
        public static GearBox Create(bool manual, List<float> ratios, float changeTime)
        {
            if (manual)
                return new ManualGearbox(ratios, changeTime);

            else
                return new AutomaticGearbox(ratios, changeTime);
        }
    }
}
