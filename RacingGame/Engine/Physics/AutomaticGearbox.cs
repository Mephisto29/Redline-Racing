/*
 * This class is used for an automatic gearbox, i.e. the gears are changed for you
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

namespace RacingGame.Engine.Physics
{
    class AutomaticGearbox : GearBox
    {
        private const float ChangeUpPoint = 0.94f;
        private const float ChangeDownPoint = 0.65f;

        //constructor, using initializer list to initialize base class data
        public AutomaticGearbox(List<float> gearRatios, float changeTime) : base(gearRatios, changeTime) { }

        public override void update(float motorRpmPercent, Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (CanChangeGear)
            {
                //if in neutral, gear up when accelerating
                if ((currentgear == GEAR_NEUTRAL || currentgear == GEAR_REVERSE) && currentState.IsAccelerating)
                    gearUp();

                //gear up to last gear when accelerating
                else if (currentState.IsAccelerating && motorRpmPercent >= ChangeUpPoint)
                {
                    if (CurrentGear < GearRatios.Count - 2)
                        gearUp();
                }

                //wheen rpm while driving drops belowe change down point, change gear down
                else if (currentState.IsAccelerating && motorRpmPercent <= ChangeDownPoint)
                    if (CurrentGear > GEAR_NEUTRAL)
                        if (CurrentGear == GEAR_1 && motorRpmPercent <= 0.2f)
                            gearDown();
                        else if (CurrentGear > GEAR_1)
                            gearDown();


                //gear down as you are not accelerating, gear down at correct RPM
                if (!currentState.IsAccelerating && motorRpmPercent <= ChangeDownPoint)
                {
                    if (CurrentGear > GEAR_REVERSE)
                        gearDown();
                }

                if (currentState.IsDecelerating && currentgear == GEAR_NEUTRAL)
                    gearDown();
            }

            base.update(motorRpmPercent, gameTime);
        }
    }
}
