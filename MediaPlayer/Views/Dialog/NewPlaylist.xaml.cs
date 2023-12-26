using MediaPlayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using TagLib;

namespace MediaPlayer.Views.Dialog
{
    public partial class NewPlaylist : Window
    {
        public Playlist newPlaylist { get; set; }
        public NewPlaylist()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string trimmed = String.Concat(playlistName.Text.Where(c => !Char.IsWhiteSpace(c)));

            if (trimmed.Length == 0)
            {
                return;
            }
            newPlaylist = new Playlist(playlistName.Text, new ObservableCollection<Media>());
            DialogResult = true;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string trimmed = String.Concat(playlistName.Text.Where(c => !Char.IsWhiteSpace(c)));

                if (trimmed.Length == 0)
                {
                    return;
                }

                newPlaylist = new Playlist(playlistName.Text, new ObservableCollection<Media>());
                DialogResult = true;
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
