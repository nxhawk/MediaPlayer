using MediaPlayer.Keys;
using MediaPlayer.ViewModels;
using MediaPlayer.Views.UC;
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
using System.Windows.Threading;

namespace MediaPlayer
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public PlaylistViewModel PlaylistViewModel { get; set; }
        public MusicPlayerViewModel MusicPlayerViewModel { get; set; }
        public UserControl? PrevPage { get; set; }
        private bool isShowVideoScreen = false;
        private bool redirect = true;

        public IDictionary<string, UserControl> userControls { get; set; } = new Dictionary<string, UserControl>();
        public MainWindow()
        {
            InitializeComponent();
            // loaded window
            Loaded += MainWindow_Loaded;
            Uri icon = new Uri("pack://application:,,,/Assets/Images/title.png", UriKind.RelativeOrAbsolute);
            Icon = BitmapFrame.Create(icon);

        }

        private void configMyHotKey()
        {
            // press control + space to continue music
            GlobalHotkey playStopHotkey = new GlobalHotkey(ModifierKeys.Control, Key.Space,
               () =>
               {
                   MediaService mediaControl = (MediaService)userControls["MediaService"];
                   mediaControl.changeStateMusic();
               },
               true
               );
            HotkeysManager.AddHotkey(playStopHotkey);

            //press key left to next music
            GlobalHotkey playNextHotkey = new GlobalHotkey(ModifierKeys.Control, Key.Left,
               () =>
               {
                   MediaService mediaControl = (MediaService)userControls["MediaService"];
                   mediaControl.playNextSong();
               },
               true
               );
            HotkeysManager.AddHotkey(playNextHotkey);

            //press key ctrl right to previous music
            GlobalHotkey playPrevHotkey = new GlobalHotkey(ModifierKeys.Control, Key.Right,
               () =>
               {
                   MediaService mediaControl = (MediaService)userControls["MediaService"];
                   mediaControl.playPrevSong();
               },
               true
               );
            HotkeysManager.AddHotkey(playPrevHotkey);

            //press key ctrl r to prevent/not to redirect video next song
            GlobalHotkey playRedirHotkey = new GlobalHotkey(ModifierKeys.Control, Key.R,
               () =>  redirect = !redirect
               ,true
               );
            HotkeysManager.AddHotkey(playRedirHotkey);

            //press key ctrl p to open current playlist
            GlobalHotkey playOpenPlHotkey = new GlobalHotkey(ModifierKeys.Control, Key.P,
               () => showCurrentPlayList()
               , true
               );
            HotkeysManager.AddHotkey(playOpenPlHotkey);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            userControls.Clear();
            // add components
            userControls.Add("ListPlaylist", new ListPlaylist());
            userControls.Add("MediaPage", new MediaPage());
            userControls.Add("EmptyPage", new EmptyPage());
            userControls.Add("MediaService", new MediaService());
            userControls.Add("RecentlyMediaPage", new RecentlyMediaPage());

            CurrentComponent.Content = userControls["ListPlaylist"];
            MediaService.Content = new MediaService();
            MusicPlayerViewModel.MediaElement = mediaElement;
            MusicPlayerViewModel.redoSong();

            mediaElement.LoadedBehavior = MediaState.Manual;
            mediaElement.UnloadedBehavior = MediaState.Manual;
            configMyHotKey();
        }

        public void changeSizeVideoScreen(int type=0)
        {
            if(type == 1)
            {
                isShowVideoScreen = false;
                videoScreen.Visibility = Visibility.Collapsed;
                return;
            }
            if (!redirect && type!=2) return;
            if (isShowVideoScreen) return;
            EmptyPage newPage = (EmptyPage)userControls["EmptyPage"];
            PrevPage = (UserControl)CurrentComponent.Content;
            CurrentComponent.Content = newPage;
            videoScreen.Visibility = Visibility.Visible;
            isShowVideoScreen = true;

        }

        public void showCurrentPlayList()
        {
            if (MusicPlayerViewModel.CurrentPlaylist == null) return;
            MediaPage newPage = (MediaPage)userControls["MediaPage"];
            newPage.Playlist = MusicPlayerViewModel.CurrentPlaylist;
            CurrentComponent.Content = newPage;
            PrevPage = userControls["ListPlaylist"];
            isShowVideoScreen = false;
            videoScreen.Visibility = Visibility.Collapsed;
        }

        public void showRecentlyMedia()
        {
            RecentlyMediaPage newPage = (RecentlyMediaPage)userControls["RecentlyMediaPage"];
            PrevPage = (UserControl)CurrentComponent.Content;
            CurrentComponent.Content = newPage;
            isShowVideoScreen = false;
            videoScreen.Visibility = Visibility.Collapsed;
        }

        public event PropertyChangedEventHandler? PropertyChanged; 

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            CurrentComponent.Content = PrevPage;
            PrevPage = userControls["ListPlaylist"];
            changeSizeVideoScreen(1);
        }

        private void mediaElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MusicPlayerViewModel.changeStateMusic();
        }

        private void update_size(object sender, SizeChangedEventArgs e)
        {
            double changeSize = (1200 - this.Width) * (-1);
            containerSt.Margin = new Thickness(changeSize, 30, 50, 0);
        }
    }
}
