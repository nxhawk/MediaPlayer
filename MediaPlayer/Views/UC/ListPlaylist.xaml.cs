using MediaPlayer.Models;
using MediaPlayer.ViewModels;
using MediaPlayer.Views.Dialog;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MediaPlayer.Views.UC
{
    /// <summary>
    /// Interaction logic for ListPlaylist.xaml
    /// </summary>
    public partial class ListPlaylist : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private MainWindow mainWindow;
        public PlaylistViewModel playlistViewModel { get; set; }
       
        public ListPlaylist()
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;
            playlistViewModel = mainWindow.PlaylistViewModel;
        }

        private void addPlaylist_Click(object sender, RoutedEventArgs e)
        {
            NewPlaylist dialog = new NewPlaylist();
            dialog.Owner = mainWindow;
            if (dialog.ShowDialog() == true)
            {
                dialog.newPlaylist.Name = generatePlaylistName(dialog.newPlaylist.Name);
               playlistViewModel.Playlists.Add(dialog.newPlaylist);
            }
        }

        private void deletePlaylist_Click(object sender, RoutedEventArgs e)
        {

        }

        private void addMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int index = playlistsListView.SelectedIndex;

            List<int> _tmp = new List<int>();
            var screen = new OpenFileDialog();
            screen.Filter = "All Media Files|*.wav;*.aac;*.wma;*.wmv;*.avi;*.mpg;*.mpeg;*.m1v;*.mp2;*.mp3;*.mpa;*.mpe;*.m3u;*.mp4;*.mov;*.3g2;*.3gp2;*.3gp;*.3gpp;*.m4a;*.cda;*.aif;*.aifc;*.aiff;*.mid;*.midi;*.rmi;*.mkv;*.WAV;*.AAC;*.WMA;*.WMV;*.AVI;*.MPG;*.MPEG;*.M1V;*.MP2;*.MP3;*.MPA;*.MPE;*.M3U;*.MP4;*.MOV;*.3G2;*.3GP2;*.3GP;*.3GPP;*.M4A;*.CDA;*.AIF;*.AIFC;*.AIFF;*.MID;*.MIDI;*.RMI;*.MKV";
            screen.Multiselect = true;

            if (screen.ShowDialog() == true)
            {
                screen.FileNames.ToList().ForEach(item =>
                {
                    var fullPath = item;
                    var namePlaylist = playlistViewModel.Playlists[index].Name;
                    // check exsited
                    Media _media = new Media(fullPath, namePlaylist);
                    bool chk = true;
                    for (int i = 0; i < playlistViewModel.Playlists[index].Medias.Count; i++)
                    {
                        if (_media.Title == playlistViewModel.Playlists[index].Medias[i].Title)
                        {
                            chk = false;
                            break;
                        }
                    }

                    if (chk)
                    {
                        // all good
                        playlistViewModel.Playlists[index].Medias.Add(_media);
                        _tmp.Add(playlistViewModel.Playlists[index].Medias.Count - 1);
                        if (playlistViewModel.Playlists[index].Medias.Count <= 4)
                        {
                            playlistViewModel.Playlists[index].NotifyOnPlaylistChanged();
                        }
                    } 
                });

                
                if (playlistViewModel.Playlists[index].Name ==mainWindow.MusicPlayerViewModel.CurrentPlaylist.Name)
                {
                    //TODO: shuffle tmp
                    foreach (var item in _tmp)
                    {
                        mainWindow.MusicPlayerViewModel.myShufflePlaylist.Add(item);
                    }
                }
            }
        }

        private void deleteMenuItem_Click(object sender, RoutedEventArgs e)
        {

            int index = playlistsListView.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            Playlist Playlist = playlistViewModel.Playlists[index];
            if (mainWindow.MusicPlayerViewModel.CanDeletePlaylist(Playlist))
            {
                ConfirmDeletePlaylist dialog = new ConfirmDeletePlaylist(Playlist.Name);
                dialog.Owner = mainWindow;
                if (dialog.ShowDialog() == false)
                {
                    return;
                }
                mainWindow.PlaylistViewModel.Playlists.Remove(Playlist);
                mainWindow.PrevPage = null;
            }
            else
            {
                DontDeleteThis dialog2 = new DontDeleteThis();
                dialog2.Owner = mainWindow;
                dialog2.ShowDialog();
            }

        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var currentSelectedItem = sender as ListViewItem;
            if (currentSelectedItem != null && currentSelectedItem.IsSelected)
            {
                int index = playlistsListView.SelectedIndex;
                if (index < 0)
                {
                    return;
                }

                MediaPage newPage = (MediaPage)mainWindow.userControls["MediaPage"];
                newPage.Playlist = playlistViewModel.Playlists[index];
                mainWindow.PrevPage =(UserControl)mainWindow.CurrentComponent.Content;
                mainWindow.CurrentComponent.Content = newPage;
            }
        }

        private void ListViewItem_Drop(object sender, DragEventArgs e)
        {

        }

        private void ListViewItem_DragOver(object sender, DragEventArgs e)
        {

        }

        private void PopupBox_OnClosed(object sender, RoutedEventArgs e)
        {

        }

        private void PopupBox_OnOpened(object sender, RoutedEventArgs e)
        {

        }

        private void importFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select a Folder your playlist"
            };

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string selectedFolderPath = folderDialog?.FileName;
                if (selectedFolderPath == null)
                {
                    return;
                }

                string[] files = Directory.GetFiles(selectedFolderPath, "*", SearchOption.AllDirectories);
                string[] folderPath = selectedFolderPath.Split('\\');
                var namePlaylist = folderPath[folderPath.Length - 1];

                //check if playlistname existed
                namePlaylist = generatePlaylistName(namePlaylist);

                playlistViewModel.Playlists.Add(new Playlist(namePlaylist, new ObservableCollection<Media>()));
                var index = playlistViewModel.Playlists.Count - 1;
                // add file music
                foreach (string item in files)
                {
                    // check file is media file
                    string extension = System.IO.Path.GetExtension(item).ToLower();
                    if (!(extension == ".mp3" || extension == ".flac" || extension == ".ogg" || extension == ".wav") && !(extension == ".mp4" || extension == ".avi" || extension == ".mkv"))
                        continue;
           
                    // add file music
                    var fullPath = item;
                    playlistViewModel.Playlists[index].Medias.Add(new Media(fullPath, namePlaylist));
                    if (playlistViewModel.Playlists[index].Medias.Count <= 4)
                    {
                        playlistViewModel.Playlists[index].NotifyOnPlaylistChanged();
                    }
                }
            }
        }

        private string generatePlaylistName(string playlistName)
        {
            playlistName = playlistName.Trim();
            Regex trimmer = new Regex(@"\s\s+");
            playlistName = trimmer.Replace(playlistName, " ");

            int index = 0;
            string newName = playlistName;
            bool ck;
            do
            {
                ck = true;
                for(int i =0; i < playlistViewModel.Playlists.Count; i++)
                {
                    if (playlistName.Equals(playlistViewModel.Playlists[i].Name))
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
            }while (!ck);

            return playlistName;
        }
    }
}
