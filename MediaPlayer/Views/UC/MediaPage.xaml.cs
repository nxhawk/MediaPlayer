using MediaPlayer.Models;
using MediaPlayer.ViewModels;
using MediaPlayer.Views.Dialog;
using Microsoft.Win32;
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
    /// Interaction logic for MediaPage.xaml
    /// </summary>
    public partial class MediaPage : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private MainWindow mainWindow;
        public MusicPlayerViewModel MusicPlayerViewModel { get; set; }
        public Playlist Playlist { get; set; }

        public MediaPage()
        {
            InitializeComponent();
            Loaded += MediaPage_Loaded;
        }

        private void MediaPage_Loaded(object sender, RoutedEventArgs e)
        {
           mainWindow = (MainWindow)Application.Current.MainWindow;
           MusicPlayerViewModel = mainWindow.MusicPlayerViewModel;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.CurrentComponent.Content = mainWindow.PrevPage;
            mainWindow.PrevPage = null;
        }

        private void RenameBtn_Click(object sender, RoutedEventArgs e)
        {
            RenamePlaylist dialog = new RenamePlaylist(Playlist.Name);
            dialog.Owner = mainWindow;
            dialog.NameChanged += screen_PlaylistNameChanged;
            var playlistName = Playlist.Name;

            if (dialog.ShowDialog()==false)
            {
                Playlist.Name = playlistName;
                return;
            }
            Playlist.Name = dialog.newName;
        }

        private void screen_PlaylistNameChanged(string newPlaylistName)
        {
            Playlist.Name = newPlaylistName;
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MusicPlayerViewModel.CanDeletePlaylist(Playlist))
            {
                ConfirmDeletePlaylist dialog = new ConfirmDeletePlaylist(Playlist.Name);
                dialog.Owner = mainWindow;

                if (dialog.ShowDialog() == false)
                {
                    return;
                }
                mainWindow.PlaylistViewModel.Playlists.Remove(Playlist);
                mainWindow.CurrentComponent.Content = mainWindow.PrevPage;
                mainWindow.PrevPage = null;
            }
            else
            {
                DontDeleteThis dialog2 = new DontDeleteThis();
                dialog2.Owner = mainWindow;
                dialog2.ShowDialog();
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();
            screen.Filter = "Media files|*.mp3;*.mp4;*.wav;*.flac;*.ogg;*.avi;*.mkv|All files|*.*"; ;
            screen.Multiselect = true;

            if (screen.ShowDialog() == true)
            {
                screen.FileNames.ToList().ForEach(item =>
                {
                    var fullPath = item;
                    Playlist.Medias.Add(new Media(fullPath));

                    if (Playlist.Medias.Count <= 4)
                    {
                        Playlist.NotifyOnPlaylistChanged();
                    }
                });
            }
        }

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.Medias.Count == 0) return;
            MusicPlayerViewModel.MediaIndex = 0;
            MusicPlayerViewModel.CurrentPlaylist = Playlist;
            MusicPlayerViewModel.setNewSong();
            MusicPlayerViewModel.PlaySound();

            //mainWindow.changeSizeVideoScreen();
        }

        private void removeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = tracksListView.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= Playlist.Medias.Count)
            {
                return;
            }
            if (selectedIndex == MusicPlayerViewModel.MediaIndex && Playlist.Name == MusicPlayerViewModel.CurrentPlaylist?.Name)
            {
                return;
            }

            Playlist.Medias.RemoveAt(selectedIndex);

        }

        private void tracksListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int selectedIndex = tracksListView.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= Playlist.Medias.Count)
            {
                return;
            }

            if (MusicPlayerViewModel.CurrentPlaylist != Playlist)
            {
                MusicPlayerViewModel.CurrentPlaylist = Playlist;
            }

            MusicPlayerViewModel.setSong(selectedIndex);
        }
    }
}
