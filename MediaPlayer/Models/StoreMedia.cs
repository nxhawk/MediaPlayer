using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.Models
{
    public class StoreMedia : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Media Media { get; set; }
        public TimeSpan storeTime { get; set; }
        public Playlist playlist { get; set; }
        public StoreMedia(Media media, TimeSpan timeSpan, Playlist _playlist)
        {
            Media = new Media(media.Fullpath, media.PlayListName);
            storeTime = timeSpan;
            playlist = _playlist;
        }
    }
}
