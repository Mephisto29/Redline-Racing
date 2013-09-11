/*
 * This class is used to play the ingame music, has appropriate methods for sound change
 * 
 * This class uses threads as all songs are loaded
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

namespace RacingGame.Engine.Audio
{
    class MPlayer
    {
        //protected data members
        protected ContentManager Content;
        protected InputHandler input;
        protected List<Song> playList;      //list of songs to play
        protected List<String> songNames;   //list of songs to play, names
        protected int currentSongIndex;

        //current song data
        protected Song currentSong;
        protected TimeSpan currentPlayingTime;
        protected TimeSpan totalPlayingTime;
        protected TimeSpan nextDelay;
        protected TimeSpan previousSongChangeTime;
        protected GameTime gameTime;

        //current song modifiers
        protected float musicVolume;
        protected float effectVolume;
        protected bool finishedLoading;
        protected bool active;

        //getters and setters
        public int SongIndex
        {
            get { return currentSongIndex; }
        }

        public int TotalSongs
        {
            get { return playList.Count - 1; }
        }

        public TimeSpan CurrentPlayingTime
        {
            get { return currentPlayingTime; }
        }

        public TimeSpan TotalPlayingTime
        {
            get { return totalPlayingTime; }
        }

        public String SongName
        {
            get { return songNames.ElementAt(currentSongIndex); }
        }

        public void AdjustVolume(float newVolume)
        {
            musicVolume = newVolume;
        }
        public void AdjustEffectVolume(float newVolume)
        {
            effectVolume = newVolume;
        }

        public bool Loaded
        {
            get { return finishedLoading; }
        }

        public bool Active
        {
            set { active = value; }
        }

        //constructor
        public MPlayer(ContentManager content, ref InputHandler newInput)
        {
            Content = content;
            input = newInput;

            finishedLoading = false;
            musicVolume = 0.2f;
            playList = new List<Song>();
            songNames = new List<String>();
            LoadPlayList();
            active = true;
            nextDelay = TimeSpan.FromSeconds(1.0f);

            MediaPlayer.Volume = musicVolume;
            finishedLoading = true;
        }

        //load song content
        private void LoadPlayList()
        {
            //add song names
            songNames.Add("Her Voice Resides");
            songNames.Add("Four Words");
            songNames.Add("Tears Don't Fall");
            songNames.Add("Hand Of Blood");
            songNames.Add("I'm so sick");
            songNames.Add("All the Small Things");
            songNames.Add("Adam's song");
            songNames.Add("Fireflies");
            songNames.Add("Stay together for the kids");
            songNames.Add("Hello Seattle");

            //load the first song
            currentSongIndex = 5;
            LoadSong();
        }

        //load a new song
        public void LoadSong()
        {
            currentSong = null;

            try
            {
                //load the new song
                Content.Unload();
                currentSong = Content.Load<Song>("Sound//Music//" + (currentSongIndex + 1).ToString());

                currentPlayingTime = MediaPlayer.PlayPosition;
                totalPlayingTime = currentSong.Duration;
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        //public methods
        public void UpdateInput(GameTime time)
        {
            gameTime = time;
        }

        public void Update()
        {
            try
            {
                //while playing do this
                while (active)
                {
                    if (active)
                    {
                        //set volume
                        MediaPlayer.Volume = musicVolume;

                        //if ended play next in list
                        if (currentPlayingTime >= totalPlayingTime)
                            Next();

                        //otherwise play
                        else
                            Play();

                        currentPlayingTime = MediaPlayer.PlayPosition;

                        //handle musicplayer input
                        if (input.MusicNext)
                            if (gameTime.TotalGameTime - previousSongChangeTime > nextDelay)
                            {
                                previousSongChangeTime = gameTime.TotalGameTime;
                                Next();
                            }

                        if (input.MusicPrevious)
                            if (gameTime.TotalGameTime - previousSongChangeTime > nextDelay)
                            {
                                previousSongChangeTime = gameTime.TotalGameTime;
                                Previous();
                            }
                    }

                    else
                        break;
                }
            }

            catch
            {
                Stop();
            }
        }

        //stop song
        public void Stop()
        {
            active = false;
            MediaPlayer.Stop();
        }

        //skip to next song
        public void Next()
        {
            currentSongIndex++;
            if (currentSongIndex > songNames.Count - 1)
                currentSongIndex = 0;

            LoadSong();
            MediaPlayer.Stop();
            Play();
        }

        //skip to previous song
        public void Previous()
        {
            currentSongIndex--;
            if (currentSongIndex < 0)
                currentSongIndex = songNames.Count - 1;

            LoadSong();
            MediaPlayer.Stop();
            Play();
        }
    
        //play the song
        public void Play()
        {
            if (MediaPlayer.State != MediaState.Playing)
                MediaPlayer.Play(currentSong);
        }
    }
}
