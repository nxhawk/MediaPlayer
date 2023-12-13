using MediaPlayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace MediaPlayer.Views.Dialog
{
    /// <summary>
    /// Interaction logic for RenamePlaylist.xaml
    /// </summary>
    public partial class RenamePlaylist : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public delegate void NameChangedHandler(string newPlaylistName);
        public event NameChangedHandler NameChanged;
        public string newName {  get; set; }
        public RenamePlaylist(string oldName)
        {
            InitializeComponent();
            Uri icon = new Uri("pack://application:,,,/Assets/Images/title.png", UriKind.RelativeOrAbsolute);
            Icon = BitmapFrame.Create(icon);

            newName = oldName;
        }

        private void RenameBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }


        private void playlistName_TextChanged(object sender, TextChangedEventArgs e)
        {
            NameChanged?.Invoke(playlistName.Text);
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DialogResult = true;
            }
        }
    }
}
