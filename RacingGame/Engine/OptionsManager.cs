/*
 * This class is used to manage all game options
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

namespace RacingGame.Engine
{
    class OptionsManager
    {
        //private data members
        private string[] gears = new string[] { "Automatic", "Manual" };

        private int gearIndex1;
        private int gearIndex2;

        private bool soundFXEnabled;
        private bool musicEnabled;
        private bool gearBox1;
        private bool gearBox2;

        private float musicVolume;
        private float effectVolume;

        //getters and setters
        public bool SoundFXEnabled
        {
            get { return soundFXEnabled; }
            set { soundFXEnabled = value; }
        }
        public bool MusicEnabled
        {
            get { return musicEnabled; }
            set { musicEnabled = value; }
        }
        public float MusicVolume
        {
            get { return (musicVolume / 100f); }
            set { musicVolume = value; }
        }
        public float EffectVolume
        {
            get { return (effectVolume / 100f); }
            set { effectVolume = value; }
        }
        public bool GearBox1
        {
            get { return gearBox1; }
            set { gearBox1 = value; }
        }
        public int GearIndex1
        {
            get { return gearIndex1; }
            set { gearIndex1 = value; }
        }

        public bool GearBox2
        {
            get { return gearBox2; }
            set { gearBox2 = value; }
        }
        public int GearIndex2
        {
            get { return gearIndex2; }
            set { gearIndex2 = value; }
        }

        //constructor
        public OptionsManager()
        {
            soundFXEnabled = false;
            musicEnabled = false;
            gearBox1 = false;
            gearIndex1 = 0;

            gearBox2 = false;
            gearIndex2 = 0;
            musicVolume = 20f;
            effectVolume = 20f;
        }
    }
}