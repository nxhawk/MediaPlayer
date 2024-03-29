﻿using MaterialDesignThemes.Wpf;
using MediaPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MediaPlayer.Views.UC
{
    /// <summary>
    /// Interaction logic for MediaService.xaml
    /// </summary>
    public partial class MediaService : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public MainWindow mainWindow { get; private set; }
        public MusicPlayerViewModel MusicPlayerViewModel { get; set; }
        private float storeVolume = 1;
        private bool isEnter = false;
        public MediaService()
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;
            MusicPlayerViewModel = mainWindow.MusicPlayerViewModel;
            mainWindow.SizeChanged += MainWindow_SizeChanged;

            Loaded += MediaService_Loaded;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double changeSize = 0;
            if (mainWindow.Width < 900 || MusicPlayerViewModel.CurrentPlaylist == null)
            {
                textInfo.Visibility = Visibility.Collapsed;
                changeSize = 100;
            }
            else
            {
                textInfo.Visibility = Visibility.Visible;
            }
            changeSize = 812 - (1200 - mainWindow.Width);
            
            mainWindow.canvasRecentlyMedia.Margin = new Thickness(changeSize,0,0,63);

        }

        private void MediaService_Loaded(object sender, RoutedEventArgs e)
        {
            MusicPlayerViewModel.iconPlay = iconPlay;
            MusicPlayerViewModel.MediaElement.MediaEnded += MediaElement_MediaEnded;
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            
            MusicPlayerViewModel.MediaContinue();
        }

        private bool isDragging = false;

        private void MediaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            var seekValue = (int)MediaSlider.Value;
            if (isDragging)
            {
                if (MusicPlayerViewModel.CurrentState == "Pause")
                {
                    MusicPlayerViewModel.changeStateMusic();
                }
                MusicPlayerViewModel.CurrentTime = new TimeSpan(0, 0, 0, 0, seekValue);
                MusicPlayerViewModel.showPreviewVideo();
                DateTime now = DateTime.Now;
                if (now - lastPositionUpdateTime >= updateInterval)
                { 
                    lastPositionUpdateTime = now;
                    MusicPlayerViewModel.MediaElement.Volume = 0;
                    MusicPlayerViewModel.MediaElement.Position = new TimeSpan(0, 0, 0, 0, seekValue);
                    MusicPlayerViewModel.MediaElement.Volume = MusicPlayerViewModel.storeVolume;
                }
            }

            bool mouseIsDown = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed;
            if (mouseIsDown && isEnter && !isDragging)
            {
                MusicPlayerViewModel.IsDragging = true;
                MusicPlayerViewModel.CurrentTime = new TimeSpan(0, 0, 0, 0, seekValue);
                MusicPlayerViewModel.MediaElement.Position = new TimeSpan(0, 0, 0, 0, seekValue);
                MusicPlayerViewModel.IsDragging = false;
            }
        }

        private TimeSpan updateInterval = TimeSpan.FromSeconds(0.2);
        private DateTime lastPositionUpdateTime;

        private void MediaSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var seekValue = (int)MediaSlider.Value;
            
            if (MusicPlayerViewModel.CurrentState == "Pause")
            {
                MusicPlayerViewModel.changeStateMusic();
            }
            MusicPlayerViewModel.MediaElement.Position = new TimeSpan(0, 0, 0, 0, seekValue);
            MusicPlayerViewModel.MediaElement.Play();
            MusicPlayerViewModel.CurrentTime = new TimeSpan(0, 0, 0, 0, seekValue);
            if (isDragging) MusicPlayerViewModel.IsDragging = false;
            isDragging = false;
            mainWindow.canvasPreview.Visibility = Visibility.Collapsed;
        }

        private void MediaSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            MusicPlayerViewModel.IsDragging = true;
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            changeStateMusic();
        }

        public void changeStateMusic()
        {
            MusicPlayerViewModel.changeStateMusic();
        }
        private void btnNextMusic_Click(object sender, RoutedEventArgs e)
        {
            playNextSong();
        }

        public void playNextSong()
        {
            MusicPlayerViewModel.setNextSong(1);
        }

        private void btnPrevMusic_Click(object sender, RoutedEventArgs e)
        {
            playPrevSong();
        }
        public void playPrevSong()
        {
            MusicPlayerViewModel.setNextSong(-1);
        }

        private void VolumeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var seekValue = (float)volumeSlider.Value;
            MusicPlayerViewModel.MediaElement.Volume = seekValue;
            MusicPlayerViewModel.storeVolume = seekValue;
            if (seekValue == 0) {
                volumeIcon.Kind = PackIconKind.VolumeOff;
            }else
            {
                volumeIcon.Kind = PackIconKind.VolumeHigh;
            }
        }

        private void VolumeSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var seekValue = (float)volumeSlider.Value;
            if (seekValue == 0)
            {
                volumeIcon.Kind = PackIconKind.VolumeOff;
            }
            else
            {
                volumeIcon.Kind = PackIconKind.VolumeHigh;
            }
        }

        private void volumeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MusicPlayerViewModel.MediaElement.Volume > 0)
            {
                MusicPlayerViewModel.storeVolume = (float)MusicPlayerViewModel.MediaElement.Volume;
                MusicPlayerViewModel.MediaElement.Volume = 0;
                volumeIcon.Kind = PackIconKind.VolumeOff;
            }
            else if (MusicPlayerViewModel.storeVolume > 0)
            {
                MusicPlayerViewModel.MediaElement.Volume = MusicPlayerViewModel.storeVolume;
                volumeIcon.Kind = PackIconKind.VolumeHigh;
            }
        }

        private void typeNextMusicBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MusicPlayerViewModel.typeContinueMusic == "Linear")
            {
                iconTypeNextMusic.Kind = PackIconKind.Shuffle;
                MusicPlayerViewModel.typeContinueMusic = "Shuffle";
                MusicPlayerViewModel.shufflePlaylist(1);
            }
            else
            {
                MusicPlayerViewModel.typeContinueMusic = "Linear";
                iconTypeNextMusic.Kind = PackIconKind.ShuffleDisabled;
                MusicPlayerViewModel.shufflePlaylist(0);
            }
        }

        private void showVideoScreen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MusicPlayerViewModel.CurrentMedia == null) return;
            mainWindow.changeSizeVideoScreen(2);
        }

        private void showCurrentPlayList_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.showCurrentPlayList();
        }

        private void MediaSlider_MouseEnter(object sender, MouseEventArgs e)
        {
            isEnter = true;
        }

        private void MediaSlider_MouseLeave(object sender, MouseEventArgs e)
        {
            isEnter = false;
        }

        private void oldMediaBtn_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.showRecentlyMedia();
        }

        private void btnNextMusic_MouseEnter(object sender, MouseEventArgs e)
        {
            if (MusicPlayerViewModel.NextMedia == null) return;
            mainWindow.canvasNextMusic.Visibility = Visibility.Visible;
        }

        private void btnNextMusic_MouseLeave(object sender, MouseEventArgs e)
        {
            mainWindow.canvasNextMusic.Visibility = Visibility.Hidden;
        }

        private void typeNextMusicBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            mainWindow.canvasPlayMode.Visibility = Visibility.Visible;
        }

        private void typeNextMusicBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            mainWindow.canvasPlayMode.Visibility = Visibility.Hidden;
        }

        private void showCurrentPlayList_MouseEnter(object sender, MouseEventArgs e)
        {
            mainWindow.canvasCurrentPlaylist.Visibility = Visibility.Visible;
        }

        private void showCurrentPlayList_MouseLeave(object sender, MouseEventArgs e)
        {
            mainWindow.canvasCurrentPlaylist.Visibility = Visibility.Hidden;
        }

        private void oldMediaBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            mainWindow.canvasRecentlyMedia.Visibility = Visibility.Visible;
        }

        private void oldMediaBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            mainWindow.canvasRecentlyMedia.Visibility = Visibility.Hidden;
        }
    }
}
