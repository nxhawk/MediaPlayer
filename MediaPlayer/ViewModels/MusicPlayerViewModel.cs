﻿using MaterialDesignThemes.Wpf;
using MediaPlayer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MediaPlayer.ViewModels
{
    public class MusicPlayerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public int MediaIndex { get; set; }
        public Playlist CurrentPlaylist { get; set; }
        [JsonIgnore]
        public MediaElement MediaElement { get; set; } = new MediaElement();

        private Media? _currentMedia;
        public TimeSpan CurrentTime {  get; set; }
        public string CurrentState { get; set; } = "Stop";
        public TimeSpan Duration { get; set; } = new TimeSpan(0, 0, 0, 0, 0);
        public float storeVolume { get; set; } = 1;
        [JsonIgnore]
        public PackIcon iconPlay { get; set; } = new PackIcon();
        [JsonIgnore]
        public string typeContinueMusic { get; set; } = "Linear";

        public double Progress { get; set; } = 0;

        private DispatcherTimer timer;

        [JsonIgnore]
        public Media CurrentMedia
        {
            get
            {
                return _currentMedia!;
            }
            set
            {
                _currentMedia = value;
                MediaElement.Source = new Uri(_currentMedia.Fullpath);
            }
        }

        public void setNewSong()
        {
            if (CurrentMedia != null)
            {
                MediaElement.Stop();
            }
            if (CurrentPlaylist != null)
            {
                CurrentMedia = CurrentPlaylist.Medias[MediaIndex];
                CurrentTime = new TimeSpan(0, 0, 0, 0, 0);
            }
        }

        public void PlaySound()
        {
            if (CurrentMedia != null)
            {
                CurrentState = "Play";
                MediaElement.LoadedBehavior = MediaState.Manual;
                MediaElement.UnloadedBehavior = MediaState.Manual;
                MediaElement.Volume = storeVolume;
                MediaElement.MediaOpened += MediaElement_MediaOpened;
                MediaElement.Play();
                Progress = MediaElement.Position.TotalMilliseconds;

                var mainwindow = (MainWindow)Application.Current.MainWindow;
                mainwindow.changeSizeVideoScreen();
                if (Path.GetExtension(CurrentMedia.Fullpath).ToLower() == ".mp4")
                {
                    mainwindow.imageScreenMusic.Visibility = Visibility.Collapsed;
                    mainwindow.imageScreenMusic.Height = 0;
                }
                else
                {
                    mainwindow.imageScreenMusic.Visibility = Visibility.Visible;
                    mainwindow.imageScreenMusic.Height = 500;

                }
                
                iconPlay.Kind = PackIconKind.Pause;

                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += new EventHandler(timer_Tick!);
                timer.Start();


            }
        }

        private void changeStatus()
        {
            if (CurrentState == "Play") {
                CurrentTime = MediaElement.Position;
                Progress = MediaElement.Position.TotalMilliseconds;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            changeStatus();
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            Duration = MediaElement.NaturalDuration.TimeSpan;
        }

        public void changeStateMusic()
        {
            if (CurrentMedia != null)
            {
                if (CurrentState == "Play")
                {
                    MediaElement.Pause();
                    iconPlay.Kind = PackIconKind.Play;
                    CurrentState = "Pause";
                   
                }
                else
                {
                    MediaElement.Play();
                    iconPlay.Kind = PackIconKind.Pause;
                    CurrentState = "Play";
                }
            }
        }

        public void setNextSong(int idx)
        {
            try
            {
                if (CurrentPlaylist == null) return;
                MediaIndex = MediaIndex + idx;
                if (MediaIndex >= CurrentPlaylist.Medias.Count) MediaIndex -= CurrentPlaylist.Medias.Count;
                if (MediaIndex < 0) MediaIndex += CurrentPlaylist.Medias.Count;

                setNewSong();
                PlaySound();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }

        public void setSong(int idx)
        {
            if (CurrentPlaylist == null || idx < 0 || idx > CurrentPlaylist.Medias.Count - 1) return;
            MediaIndex = idx;

            setNewSong();
            PlaySound();
        }

        public void setRandomSong()
        {
            Random rand = new Random();
            int index = rand.Next() % CurrentPlaylist.Medias.Count;
            while (index == MediaIndex)
            {
                index = rand.Next() % CurrentPlaylist.Medias.Count;
            }
            setSong(index);
        }

        public void MediaContinue()
        {
            try
            {
                MediaElement.Stop();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            

            if (typeContinueMusic == "Linear" || CurrentPlaylist.Medias.Count == 1) { 
                setNextSong(1);
            }else
            {
                // TODO: shuffle playlist not random media
                setRandomSong();
                
            }
        }

        public void redoSong()
        {
            if (CurrentMedia == null && CurrentPlaylist != null)
            {
                CurrentMedia = CurrentPlaylist.Medias[MediaIndex];
                MediaElement.LoadedBehavior = MediaState.Manual;
                MediaElement.UnloadedBehavior = MediaState.Manual;
                MediaElement.Position = CurrentTime;
                MediaElement.Volume = storeVolume;
                CurrentState = "Play";
                PlaySound();
                changeStateMusic();
            }
        }

       
        // handle delete playlist
        public bool CanDeletePlaylist(Playlist Playlist)
        {
            if (CurrentPlaylist.Name == Playlist.Name)
            {
                return false;
            }
            return true;
        }
    }
}
