using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace MediaPlayer.Models
{
    public class StoreMedia : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Media Media { get; set; }
        public TimeSpan storeTime { get; set; }
        public Playlist playlist { get; set; }
        public string title { get; set; }
        public StoreMedia(Media media, TimeSpan timeSpan, Playlist _playlist)
        {
            Media = new Media(media.Fullpath, _playlist.Name);
            storeTime = timeSpan;
            title = _playlist.Name;
            playlist = _playlist;
        }

        public void changeName(Playlist _playlist)
        {
            title = _playlist.Name;
            Media = new Media(Media.Fullpath, _playlist.Name);
            playlist = _playlist;
        }
    }
}
