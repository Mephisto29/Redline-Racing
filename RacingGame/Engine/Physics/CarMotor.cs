/*
 * This class is used to simulate the engine of a car
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
    class CarMotor
    {
        //private data members
        private const float DRIVETRAIN_MULTIPLIER = 34;

        private List<float> carPower;

        private float maxPowerFromEngine;
        private float engineRedLineRPM;
        private GearBox currentGearBox;
        private float currentRPM;
        private float previousRPM;
        private float previousGearRPM;
        private float throttle;
        private float currentPower;
        private float rpmLimiter;
        private float previousSpeed;

        //if a gear is engaged the engine produces power otherwise no power is produced
        public float CurrentPower
        {
            get
            {
                if (currentGearBox.gearEngaged)
                    return currentPower * throttle * currentGearBox.CurrentGearRatio;
                else
                    return 0;
            }
        }

        //getters and setters
        public float MaxPower
        {
            get { return maxPowerFromEngine; }
            set { maxPowerFromEngine = value; }
        }

        public float CurrentRPM
        {
            get { return currentRPM; }
        }

        public bool AtRedLineRPM
        {
            get { return currentRPM >= engineRedLineRPM; }
        }

        public float Throttle
        {
            get { return throttle; }
            set { throttle = value; }
        }

        public float RedLineRPM
        {
            get { return engineRedLineRPM; }
            set { engineRedLineRPM = value; }
        }

        public bool isVehicleAccelerating
        {
            get { return currentRPM > previousRPM; }
        }

        public GearBox GearBoxUsed
        {
            get { return currentGearBox; }
        }
        //end of getters and setters

        //constructor
        public CarMotor(List<float> power, float maximumPower, float redline, GearBox newGearBox)
        {
            carPower = power;
            maxPowerFromEngine = maximumPower;
            engineRedLineRPM = redline;
            currentGearBox = newGearBox;

            currentGearBox.CurrentGear = 0;
            currentGearBox.Motor = this;
        }

        //update method
        public void update(GameTime gameTime, float currentSpeed)
        {
            //store previous data
            previousRPM = currentRPM;
            previousSpeed = currentSpeed;

            //set max RPM if over
            if (currentRPM > RedLineRPM)
                currentRPM = RedLineRPM;

            if (rpmLimiter > 0)
            {
                currentPower = 0;
                rpmLimiter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            else
                currentPower = maxPowerFromEngine * MathHelper.Lerp(carPower[(int)currentRPM], carPower[(int)currentRPM + 1], currentRPM - (int)currentRPM);

            //if a gear is engaged the motor car move the car forwards
            if (currentGearBox.gearEngaged)
            {
                if (currentGearBox.CurrentGear == 0)
                {
                    if (throttle == 0.0f || rpmLimiter > 0)
                    {
                        currentRPM -= (float)gameTime.ElapsedGameTime.TotalSeconds * 4.4f;

                        if (currentRPM < 0.8f)
                            currentRPM = 0.8f;
                    }

                    else
                        currentRPM += (float)gameTime.ElapsedGameTime.TotalSeconds * throttle * 5f;
                }

                else
                {
                    currentRPM = currentSpeed * currentGearBox.CurrentGearRatio / DRIVETRAIN_MULTIPLIER;

                    if (currentRPM < 0.8f)
                        currentRPM = 0.8f;
                }

                previousGearRPM = currentRPM;
            }

            else
                currentRPM = MathHelper.Lerp(previousGearRPM, currentSpeed * currentGearBox.NextGearRatio / DRIVETRAIN_MULTIPLIER, currentGearBox.Clutch);

            if (currentRPM < 0.8)
                currentRPM = 0.8f;

            if (currentRPM > engineRedLineRPM)
                rpmLimiter = 0.2f;

            currentGearBox.update(currentRPM / engineRedLineRPM, gameTime);
        }

        //method to obtain power
        public float GetPowerAtRPMforGear(float rpm, int gear)
        {
            float currentPowerOutput = maxPowerFromEngine * MathHelper.Lerp(carPower[(int)engineRedLineRPM], carPower[(int)engineRedLineRPM + 1], engineRedLineRPM - (int)engineRedLineRPM);
            currentPowerOutput *= currentGearBox.GearRatios[gear];
            return currentPowerOutput;
        }

        //method to obtain RPM
        public float GetRPMforGear(int gear)
        {
            return previousSpeed * currentGearBox.GearRatios[gear] / DRIVETRAIN_MULTIPLIER;
        }

        //can gear be changed down
        public bool CanChangeDown
        {
            get
            {
                return GetRPMforGear(currentGearBox.CurrentGear) / engineRedLineRPM < 0.9f;
            }
        }
    }
}
