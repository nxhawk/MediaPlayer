using MediaPlayer.Models;
using MediaPlayer.Views.Dialog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.ViewModels
{
    public class PlaylistViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Playlist> Playlists { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public PlaylistViewModel()
        {
            Playlists = new ObservableCollection<Playlist>();
        }
    }
}
