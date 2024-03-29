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
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace MediaPlayer.ViewModels
{
    public class MusicPlayerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        [JsonIgnore]
        public ObservableCollection<StoreMedia> RecentlyPlayed { get; set; } = new ObservableCollection<StoreMedia>();
        public int MediaIndex { get; set; }
        public Playlist CurrentPlaylist { get; set; }
        [JsonIgnore]
        public List<int> myShufflePlaylist { get; set; }
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
        [JsonIgnore]
        public bool IsDragging { get; set; } = false;

        private DispatcherTimer timer;
        [JsonIgnore]
        private List<WriteableBitmap> storeImagePreview = new List<WriteableBitmap>();
        [JsonIgnore]
        public Playlist tmp { get; set; } = null;

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

        [JsonIgnore]
        public Media NextMedia { get; set; }

        public void setNewSong()
        {
            if (CurrentMedia != null)
            {
                MediaElement.Stop();

            }
            if (CurrentPlaylist != null)
            {

                addRecentlyPlayed();
                if (RecentlyPlayed?.Count > 100)
                {
                    RecentlyPlayed.RemoveAt(100);
                }

                CurrentMedia = CurrentPlaylist.Medias[myShufflePlaylist[MediaIndex]];

                // get next music
                int nextIdx = MediaIndex + 1;
                if (nextIdx >= CurrentPlaylist.Medias.Count) nextIdx = 0;

                NextMedia = CurrentPlaylist.Medias[myShufflePlaylist[nextIdx]];
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
                //MediaElement.Volume = storeVolume;
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
                timer.Interval = TimeSpan.FromMilliseconds(10);
                timer.Tick += new EventHandler(timer_Tick!);
                timer.Start();
            }
        }

        private void changeStatus()
        {
            if (CurrentState == "Play"&& !IsDragging) {
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
            
            CurrentTime = TimeSpan.Zero;
            setNextSong(1);
        }

        public void addRecentlyPlayed()
        {
            if (CurrentMedia == null) return;
            StoreMedia storeMedia = new StoreMedia(CurrentMedia, CurrentTime, CurrentPlaylist);
            if (tmp != null)
            {
                storeMedia = new StoreMedia(CurrentMedia, CurrentTime, tmp);
                tmp = null;
            }
            RecentlyPlayed.Insert(0, storeMedia);
        }

        public void redoSong()
        {
            if (CurrentMedia == null && CurrentPlaylist != null)
            {
                CurrentMedia = CurrentPlaylist.Medias[MediaIndex];
                shufflePlaylist();
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
            if (CurrentPlaylist?.Name == Playlist.Name)
            {
                return false;
            }
            return true;
        }

        public void showPreviewVideo()
        {
            var extension = Path.GetExtension(CurrentMedia.Fullpath).ToLower();
            if (extension == ".mp4" || extension == ".avi" || extension == ".mkv")
            {
                int width = (int)MediaElement.ActualWidth;
                int height = (int)MediaElement.ActualHeight;
                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(width, height, 60, 60, PixelFormats.Pbgra32);
                renderTargetBitmap.Render(MediaElement);
                WriteableBitmap bitmap = new WriteableBitmap(renderTargetBitmap);
                var mainwindow = (MainWindow)Application.Current.MainWindow;
                mainwindow.changeSizeVideoScreen();
                mainwindow.previewImage.Source = bitmap;
                mainwindow.canvasPreview.Visibility = Visibility.Visible;

                // calculate position screen
                var currentSize = mainwindow.ActualWidth;
                var pos = (CurrentTime.TotalMilliseconds - Duration.TotalMilliseconds/2) * (currentSize/ Duration.TotalMilliseconds)*3/2;
                
                Thickness margin = mainwindow.canvasPreview.Margin;
                margin.Left=pos;
                mainwindow.canvasPreview.Margin = margin;
            }
        }

        public void retypeMusicPlayType()
        {
            if (typeContinueMusic == "Shuffle")
            {
                typeContinueMusic = "Shuffle";
                shufflePlaylist(1);
            }
            else
            {
                typeContinueMusic = "Linear";
                shufflePlaylist(0);
            }
        }

        public void shufflePlaylist(int type=0)
        {
            if (CurrentPlaylist == null) return;

            // 0: linear
            // 1: shuffle
            myShufflePlaylist = new List<int>();
            for (int i = 0; i < CurrentPlaylist.Medias.Count; i++)
            {
                myShufflePlaylist.Add(i);
            }
            if (type == 0)
            {

                for (int i = 0; i < CurrentPlaylist.Medias.Count; i++)
                {

                    if (CurrentPlaylist.Medias[i].Title == CurrentMedia?.Title)
                    {
                        MediaIndex = i;
                        break;
                    }
                }


            }
            else
            {
                int currentSelecting = CurrentPlaylist.Medias.Count * 5;
                while (currentSelecting > 0)
                {
                    currentSelecting--;
                    Random rand = new Random();
                    int index_1 = rand.Next() % CurrentPlaylist.Medias.Count;
                    int index_2 = rand.Next() % CurrentPlaylist.Medias.Count;
                    if (myShufflePlaylist[index_1] == MediaIndex || myShufflePlaylist[index_2] == MediaIndex)
                        continue;
                    int swapTemp = myShufflePlaylist[index_1];
                    myShufflePlaylist[index_1] = myShufflePlaylist[index_2];
                    myShufflePlaylist[index_2] = swapTemp;


                }
            }

            int nextIdx = MediaIndex + 1;
            if (nextIdx >= CurrentPlaylist.Medias.Count) nextIdx = 0;

            NextMedia = CurrentPlaylist.Medias[myShufflePlaylist[nextIdx]];
        }


        public void changePlaylistName(string oldName, Playlist _playlist)
        {
            foreach (var item in RecentlyPlayed)
            {
                if (item.title == oldName)
                {
                    item.changeName(_playlist);
                }
            }
        }
    }
}
