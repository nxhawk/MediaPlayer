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
        [JsonProperty("PlayListName")]
        public string PlayListName { get; set; } = "";

        [JsonProperty("Fullpath")]
        public string Fullpath { get; set; }

        private TagLib.File _tagFile;
        
        public string Title
        {
            get
            {
                string extension = Path.GetExtension(Fullpath).ToLower();
                if (extension == ".mp4" || extension == ".avi" || extension == ".mkv")
                {
                    return Path.GetFileNameWithoutExtension(Fullpath);
                }
                return _tagFile.Tag.Title;
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

        public event PropertyChangedEventHandler? PropertyChanged;

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

        public Media(string fullpath, string name)
        {
            Fullpath = fullpath;
            try
            {
                _tagFile = TagLib.File.Create(fullpath);
                PlayListName = name;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
