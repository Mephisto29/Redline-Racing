/*
 * This class is used to play the ingame sound effects of the car and the enviroment sounds
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

namespace RacingGame.Engine.Audio
{
    class SoundEffectPlayer
    {
        //protected data members
        protected ContentManager Content;
        protected Vehicle currentCar;
        protected String currentDirectory;

        //sound effect instances of the different soundeffects so that they can be played and paused
        protected SoundEffectInstance engineOnLowSoundEffect;
        protected SoundEffectInstance engineOnHighSoundEffect;
        protected SoundEffectInstance engineOffLowSoundEffect;
        protected SoundEffectInstance engineOffHighSoundEffect;
        protected SoundEffectInstance slidingSoundEffect;
        protected SoundEffectInstance collisionSoundEffect;
        protected SoundEffectInstance nosSoundEffect;

        //current sounde effect volume
        public float soundVolume;

        //method to adjust game volume
        public void AdjustVolume(float newVolume)
        {
            soundVolume = newVolume;
        }

        //constructor
        public SoundEffectPlayer(ContentManager content,Vehicle currentVehicle, String soundDirectory)
        {
            Content = content;
            currentCar = currentVehicle;
            currentDirectory = soundDirectory;
            soundVolume = 0.2f;

            SoundEffect temp;   //temp soundeffect
            //load soundeffects and store in soundeffectinstances, set data appropriatey
            temp = Content.Load<SoundEffect>(soundDirectory + "//engine-on-low");
            engineOnLowSoundEffect = temp.CreateInstance();
            engineOnLowSoundEffect.Volume = soundVolume;
            engineOnLowSoundEffect.Pitch = 0;
            engineOnLowSoundEffect.Pan = 0;
            engineOnLowSoundEffect.IsLooped = true;

            temp = Content.Load<SoundEffect>(soundDirectory + "//engine-on-high");
            engineOnHighSoundEffect = temp.CreateInstance();
            engineOnHighSoundEffect.Volume = soundVolume;
            engineOnHighSoundEffect.Pitch = 0;
            engineOnHighSoundEffect.Pan = 0;
            engineOnHighSoundEffect.IsLooped = true;

            temp = Content.Load<SoundEffect>(soundDirectory + "//engine-off-low");
            engineOffLowSoundEffect = temp.CreateInstance();
            engineOffLowSoundEffect.Volume = soundVolume;
            engineOffLowSoundEffect.Pitch = 0;
            engineOffLowSoundEffect.Pan = 0;
            engineOffLowSoundEffect.IsLooped = true;

            temp = Content.Load<SoundEffect>(soundDirectory + "//engine-off-high");
            engineOffHighSoundEffect = temp.CreateInstance();
            engineOffHighSoundEffect.Volume = soundVolume;
            engineOffHighSoundEffect.Pitch = 0;
            engineOffHighSoundEffect.Pan = 0;
            engineOffHighSoundEffect.IsLooped = true;

            temp = Content.Load<SoundEffect>("Sound//SoundEffects//sliding");
            slidingSoundEffect = temp.CreateInstance();
            temp = Content.Load<SoundEffect>("Sound//SoundEffects//collision");
            collisionSoundEffect = temp.CreateInstance();
            temp = Content.Load<SoundEffect>("Sound//SoundEffects//NOS");
            nosSoundEffect = temp.CreateInstance();

            //pause all sounds so that they can be played when needed
            engineOnLowSoundEffect.Pause();
            engineOnHighSoundEffect.Pause();
            engineOffLowSoundEffect.Pause();
            engineOffHighSoundEffect.Pause();
        }

        //update the sound effects
        public void UpdateEngineSounds()
        {
            float currentRPMFraction = (currentCar.Motor.CurrentRPM - 0.8f) / currentCar.Motor.RedLineRPM;  //based on engine data of vehicle

            //this chooses the sounds to play and at what volume
            SoundEffectInstance lowSound;
            SoundEffectInstance highSound;

            if (currentCar.Motor.Throttle == 0 && !currentCar.Motor.AtRedLineRPM)
            {
                engineOnLowSoundEffect.Pause();
                engineOnHighSoundEffect.Pause();
                engineOffLowSoundEffect.Resume();
                engineOffHighSoundEffect.Resume();
                lowSound = engineOffLowSoundEffect;
                highSound = engineOffHighSoundEffect;
            }

            else
            {
                engineOnLowSoundEffect.Resume();
                engineOnHighSoundEffect.Resume();
                engineOffLowSoundEffect.Pause();
                engineOffHighSoundEffect.Pause();
                lowSound = engineOnLowSoundEffect;
                highSound = engineOnHighSoundEffect;
            }

            //low sound volume
            if (currentRPMFraction > 0.55f)
                lowSound.Volume = 0;
            else
                lowSound.Volume = soundVolume;

            //high sound volume
            if (currentRPMFraction < 0.45f)
                highSound.Volume = 0;
            else
                highSound.Volume = soundVolume;

            if (currentRPMFraction < 0.1f)
                lowSound.Volume = Math.Abs(MathHelper.Lerp(0.1f, soundVolume, (currentRPMFraction) * 10));

            if (currentRPMFraction > 0.45f && currentRPMFraction < 0.55f)
            {
                lowSound.Volume = Math.Abs(MathHelper.Lerp(soundVolume, 0f, (currentRPMFraction - 0.45f) * 10));
                highSound.Volume = Math.Abs(MathHelper.Lerp(0, soundVolume, (currentRPMFraction - 0.45f) * 10));
            }

            if (currentRPMFraction > 1)
                currentRPMFraction = 1;

            if (currentRPMFraction > 0)
            {
                lowSound.Pitch = currentRPMFraction - 0.1f;
                highSound.Pitch = currentRPMFraction - 0.5f;
            }
        }

        //used to pause all sounds
        public void PauseAllSounds()
        {
            engineOnLowSoundEffect.Pause();
            engineOnHighSoundEffect.Pause();
            engineOffLowSoundEffect.Pause();
            engineOffHighSoundEffect.Pause();
            slidingSoundEffect.Pause();
            collisionSoundEffect.Pause();
        }

        //used to pay sound for sliding
        public void playSlidingSound(bool play)
        {
            slidingSoundEffect.Volume = soundVolume;

            if (play)
                slidingSoundEffect.Resume();

            else
                slidingSoundEffect.Pause();
        }

        //used to pay sound for nos
        public void playNOSSound(bool play)
        {
            slidingSoundEffect.Volume = soundVolume;

            if (play)
                nosSoundEffect.Resume();

            else
                nosSoundEffect.Pause();
        }

        //used to play the sound of a collision
        public void playCollisionSound(bool play)
        {
            float currentRPMFraction = (currentCar.Motor.CurrentRPM - 0.8f) / currentCar.Motor.RedLineRPM;

            if (currentRPMFraction >= 0)
            {
                collisionSoundEffect.Volume = soundVolume * currentRPMFraction;

                if (play)
                    collisionSoundEffect.Resume();

                else
                    collisionSoundEffect.Pause();
            }
        }
    }
}
