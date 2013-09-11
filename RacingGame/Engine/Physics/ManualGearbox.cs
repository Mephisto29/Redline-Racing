/*
 * This class is used to simulate a manual gearbox, i.e. you use keys to change from one gear to the next
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
    class ManualGearbox : GearBox
    {
        //constructor, using initializer list to initialize base class data
        public ManualGearbox(List<float> gearRatios, float changeTime) : base(gearRatios, changeTime) { }

        public override void update(float motorRpmPercent, Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (CanChangeGear)
            {
                if (currentState.GearUp && currentgear < GearRatios.Count - 1)
                    gearUp();
                if (currentState.GearDown && currentgear > 0 && currentMotor.CanChangeDown)
                    gearDown();
            }

            base.update(motorRpmPercent, gameTime);
        }
    }
}
