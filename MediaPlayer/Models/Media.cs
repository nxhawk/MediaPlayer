using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MediaPlayer.Models
{
    public class Media : INotifyPropertyChanged
    {
        [JsonProperty("Fullpath")]
        public string Fullpath { get; set; }

        private TagLib.File _tagFile;
        public string Title
        {
            get
            {
                if (Path.GetExtension(Fullpath).ToLower() == ".mp4")
                {
                    
                    return Path.GetFileNameWithoutExtension(Fullpath);
                }
                return _tagFile.Tag.Title;
            }
        }
        [JsonProperty("Artist")]
        public string Artist
        {
            get
            {
                return _tagFile.Tag.FirstPerformer;
            }
        }

        [JsonProperty("Album")]
        public string Album
        {
            get
            {
                return _tagFile.Tag.Album;
            }
        }

        [JsonProperty("Year")]
        public uint Year
        {
            get
            {
                return _tagFile.Tag.Year;
            }
        }

        [JsonProperty("Genre")]
        public string Genre
        {
            get
            {
                return _tagFile.Tag.FirstGenre;
            }
        }

        [JsonProperty("Duration")]
        public string Duration
        {
            get
            {
                return _tagFile.Properties.Duration.ToString(@"mm\:ss");
            }
        }

        private BitmapImage _artCover;

        public event PropertyChangedEventHandler PropertyChanged;

        [JsonIgnore]
        public BitmapImage ArtCover
        {
            get
            {
                if (_artCover == null)
                {
                    if (_tagFile.Tag.Pictures.Length > 0)
                    {
                        var firstPicture = _tagFile.Tag.Pictures.FirstOrDefault();
                        if (firstPicture != null)
                        {
                            byte[] pData = firstPicture.Data.Data;

                            var mStream = new MemoryStream(pData);
                            mStream.Seek(0, SeekOrigin.Begin);

                            _artCover = new BitmapImage();
                            _artCover.BeginInit();
                            _artCover.StreamSource = mStream;
                            _artCover.EndInit();
                        }
                    }
                    else
                    {
                        _artCover = new BitmapImage();
                        _artCover.BeginInit();
                        _artCover.UriSource = new Uri("pack://application:,,,/Assets/Images/zing.jpg", UriKind.RelativeOrAbsolute);
                        _artCover.EndInit();
                    }
                }
                return _artCover!;
            }
        }

        public Media(string fullpath)
        {
            Fullpath = fullpath;
            _tagFile = TagLib.File.Create(fullpath);
        }
    }
}
