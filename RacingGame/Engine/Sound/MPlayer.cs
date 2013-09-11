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

namespace RacingGame.Engine.Sound
{
    class MPlayer
    {
        protected ContentManager Content;

        protected TimeSpan currentSongTime;
        protected TimeSpan totalSongTime;

        protected String[] songNames;
        protected int currentIndex;

        protected Song currentSong;

        public MPlayer(ContentManager content)
        {
            Content = content;

            songNames = new String[1];
            songNames[0] = "slipknotsnuff";

            currentIndex = 0;
        }

        public void Update()
        {
            if (MediaPlayer.State != MediaState.Playing)
            {
                LoadSong();
                Play(currentSong);
            }

            currentSongTime = MediaPlayer.PlayPosition;
            totalSongTime = currentSong.Duration;
        }

        private void LoadSong()
        {
            currentSong = Content.Load<Song>("Sound//Music//" + songNames[currentIndex]);
            currentIndex++;

            if (currentIndex == songNames.Length - 1)
                currentIndex = 0;
        }

        public void Play(Song song)
        {
            MediaPlayer.Play(song);
        }
    }
}
