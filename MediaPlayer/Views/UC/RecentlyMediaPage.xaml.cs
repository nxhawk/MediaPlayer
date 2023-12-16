using MediaPlayer.Models;
using MediaPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MediaPlayer.Views.UC
{
    /// <summary>
    /// Interaction logic for RecentlyMediaPage.xaml
    /// </summary>
    public partial class RecentlyMediaPage : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private MainWindow mainWindow;
        public MusicPlayerViewModel musicPlayerViewModel { get; set; }
        public RecentlyMediaPage()
        {
            InitializeComponent();

            mainWindow = (MainWindow)Application.Current.MainWindow;
            musicPlayerViewModel = mainWindow.MusicPlayerViewModel;

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.CurrentComponent.Content =(ListPlaylist)mainWindow.userControls["ListPlaylist"];
            mainWindow.PrevPage = null;
        }

        private void removeMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tracksListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int selectedIndex = tracksListView.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= musicPlayerViewModel.RecentlyPlayed.Count)
            {
                return;
            }

            if (musicPlayerViewModel.RecentlyPlayed[selectedIndex].Media.PlayListName != musicPlayerViewModel.CurrentPlaylist.Name)
            {
                musicPlayerViewModel.CurrentPlaylist = musicPlayerViewModel.RecentlyPlayed[selectedIndex].playlist;
            }

            for (int i = 0; i < mainWindow.PlaylistViewModel.Playlists.Count; i++)
            {
                if (mainWindow.PlaylistViewModel.Playlists[i].Name == musicPlayerViewModel.RecentlyPlayed[selectedIndex].Media.PlayListName)
                {
                    for (int j = 0; j < mainWindow.PlaylistViewModel.Playlists[i].Medias.Count; j++)
                    {
                        if (mainWindow.PlaylistViewModel.Playlists[i].Medias[j].Title == musicPlayerViewModel.RecentlyPlayed[selectedIndex].Media.Title)
                        {
                            var storeTime = mainWindow.MusicPlayerViewModel.RecentlyPlayed[selectedIndex].storeTime;
                            musicPlayerViewModel.setSong(j);
                            musicPlayerViewModel.CurrentTime = storeTime;
                            musicPlayerViewModel.MediaElement.Position = storeTime;
                            return;
                        }
                    }
                }
            }

            MessageBox.Show("File deleted");
        }
    }
}
