using MediaPlayer.Models;
using MediaPlayer.ViewModels;
using MediaPlayer.Views.Dialog;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

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
            Playlist.Name = generatePlaylistName(dialog.newName);
        }

        private string generatePlaylistName(string playlistName)
        {
            playlistName = playlistName.Trim();
            Regex trimmer = new Regex(@"\s\s+");
            playlistName = trimmer.Replace(playlistName, " ");

            PlaylistViewModel playlistViewModel = mainWindow.PlaylistViewModel;
            int index = 0;
            string newName = playlistName;
            bool ck;
            do
            {
                ck = true;
                for (int i = 0; i < playlistViewModel.Playlists.Count; i++)
                {
                    if (playlistViewModel.Playlists[i] != Playlist && playlistName.Equals(playlistViewModel.Playlists[i].Name))
                    {
                        ck = false;
                        break;
                    }
                }
                if (!ck)
                {
                    index++;
                    playlistName = newName + $" ({index})";
                }
            } while (!ck);

            return playlistName;
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
            List<int> _tmp = new List<int>();
            var screen = new OpenFileDialog();
            screen.Filter = "Media files|*.mp3;*.mp4;*.wav;*.flac;*.ogg;*.avi;*.mkv|All files|*.*"; ;
            screen.Multiselect = true;

            if (screen.ShowDialog() == true)
            {
                screen.FileNames.ToList().ForEach(item =>
                {
                    var fullPath = item;
                    var namePlaylist = Playlist.Name;
                    // check exsited
                    Models.Media _media = new Models.Media(fullPath, namePlaylist);
                    bool chk = true;
                    for (int i = 0; i < Playlist.Medias.Count; i++)
                    {
                        if (_media.Title == Playlist.Medias[i].Title)
                        {
                            chk = false;
                            break;
                        }
                    }

                    if (chk)
                    {
                        // all good
                        Playlist.Medias.Add(_media);
                        _tmp.Add(Playlist.Medias.Count - 1);
                        if (Playlist.Medias.Count <= 4)
                        {
                            Playlist.NotifyOnPlaylistChanged();
                        }
                    }
                });

                if (Playlist.Name == MusicPlayerViewModel?.CurrentPlaylist?.Name)
                {
                    //TODO: shuffle tmp

                    foreach (var item in _tmp)
                    {
                        MusicPlayerViewModel.myShufflePlaylist.Add(item);
                    }
                }
            }
        }

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {

            if (Playlist.Medias.Count == 0) return;
            MusicPlayerViewModel.MediaIndex = 0;
            MusicPlayerViewModel.CurrentPlaylist = Playlist;
            MusicPlayerViewModel.retypeMusicPlayType();
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
            if (selectedIndex == MusicPlayerViewModel.myShufflePlaylist[MusicPlayerViewModel.MediaIndex] && Playlist.Name == MusicPlayerViewModel.CurrentPlaylist?.Name)
            {
                return;
            }

            int idx = -1;
            for (int i = 0; i < MusicPlayerViewModel.myShufflePlaylist.Count; i++)
            {
                if (MusicPlayerViewModel.myShufflePlaylist[i] > selectedIndex)
                    MusicPlayerViewModel.myShufflePlaylist[i]--;
                else if (MusicPlayerViewModel.myShufflePlaylist[i] == selectedIndex)
                    idx = i;
            }
            if (idx != -1)
            {
                MusicPlayerViewModel.myShufflePlaylist.RemoveAt(idx);
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

            if (MusicPlayerViewModel?.CurrentPlaylist != Playlist)
            {

                MusicPlayerViewModel.tmp = MusicPlayerViewModel.CurrentPlaylist;
                MusicPlayerViewModel.CurrentPlaylist = Playlist;
            }


            MusicPlayerViewModel.shufflePlaylist(0);

            MusicPlayerViewModel.setSong(selectedIndex);
            MusicPlayerViewModel.retypeMusicPlayType();

        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select a Folder store Playlist"
            };

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string selectedFolder = folderDialog.FileName;
                string nameFolder = "My Playlist";
                string newFolderPath = "";
                int index = 0;
                do {
                    newFolderPath = System.IO.Path.Combine(selectedFolder, nameFolder);
                    nameFolder = $"My Playlist ({++index})";
                } while (Directory.Exists(newFolderPath));
                // good folder path => new folder
                Directory.CreateDirectory(newFolderPath);

                // store media to this folder
                foreach (var item in Playlist.Medias)
                {
                    string fileName = item.Fullpath;
                    string[] folderPath = fileName.Split('\\');

                    fileName = folderPath[folderPath.Length - 1];
                    string filePath = System.IO.Path.Combine(newFolderPath, fileName);

                    WebClient webClient = new WebClient();
                    try
                    {
                        webClient.DownloadFile((new Uri(item.Fullpath)).AbsoluteUri, filePath);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                CustomMessageBox dialog = new CustomMessageBox("Save your playlist success!");
                dialog.Owner = mainWindow;
                dialog.ShowDialog();
            }
        }
    }
}
