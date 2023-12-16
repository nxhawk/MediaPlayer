using MaterialDesignThemes.Wpf;
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
            if (mainWindow.Width < 900 || MusicPlayerViewModel.CurrentPlaylist == null)
            {
                textInfo.Visibility = Visibility.Collapsed;
                return;
            }
                textInfo.Visibility = Visibility.Visible;

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
            else
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
            }
            else
            {
                MusicPlayerViewModel.typeContinueMusic = "Linear";
                iconTypeNextMusic.Kind = PackIconKind.ShuffleDisabled;
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
    }
}
