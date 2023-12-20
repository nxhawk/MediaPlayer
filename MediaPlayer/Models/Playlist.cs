using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace MediaPlayer.Models
{
    public class Playlist : INotifyPropertyChanged
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Medias")]
        public ObservableCollection<Media> Medias { get; set; } = new ObservableCollection<Media>();

        private BitmapImage _playlistCover;

        public event PropertyChangedEventHandler? PropertyChanged;

        [JsonIgnore]
        public BitmapImage PlaylistCover
        {
            get
            {
                if (_playlistCover == null)
                {
                    _playlistCover = CombineImage();
                }

                return _playlistCover;
            }
            private set
            {
                _playlistCover = value;
            }
        }
        public Playlist(string name, ObservableCollection<Media> medias)
        {
            Name = name;
            Medias = medias;
        }

        private BitmapImage CombineImage()
        {
            if (Medias.Count == 0)
            {
                return null;
            }

            BitmapFrame bitmapFrame0 = Medias.Count > 0
                ? BitmapFrame.Create(Medias[0].ArtCover)
                : BitmapDecoder.Create(new Uri("pack://application:,,,/Assets/Images/white.png", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
            BitmapFrame bitmapFrame1 = Medias.Count > 1
                ? BitmapFrame.Create(Medias[1].ArtCover)
                : BitmapDecoder.Create(new Uri("pack://application:,,,/Assets/Images/white.png", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
            BitmapFrame bitmapFrame2 = Medias.Count > 2
                ? BitmapFrame.Create(Medias[2].ArtCover)
                : BitmapDecoder.Create(new Uri("pack://application:,,,/Assets/Images/white.png", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
            BitmapFrame bitmapFrame3 = Medias.Count > 3
                ? BitmapFrame.Create(Medias[3].ArtCover)
                : BitmapDecoder.Create(new Uri("pack://application:,,,/Assets/Images/white.png", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();

            var imageWidth = bitmapFrame0.PixelWidth;
            var imageHeight = bitmapFrame0.PixelHeight;
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(bitmapFrame0, new Rect(0, 0, imageWidth, imageHeight));
                drawingContext.DrawImage(bitmapFrame1, new Rect(imageWidth, 0, imageWidth, imageHeight));
                drawingContext.DrawImage(bitmapFrame2, new Rect(0, imageHeight, imageWidth, imageHeight));
                drawingContext.DrawImage(bitmapFrame3, new Rect(imageWidth, imageHeight, imageWidth, imageHeight));
            }
            // Converts the Visual (DrawingVisual) into a BitmapSource
            RenderTargetBitmap bmp = new RenderTargetBitmap(imageWidth * 2, imageHeight * 2, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            // Creates a PngBitmapEncoder and adds the BitmapSource to the frames of the encoder
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));

            var bitmapImage = new BitmapImage();
            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(bmp));

            using (var stream = new MemoryStream())
            {
                bitmapEncoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        /// <summary>
        /// This function notifies the playlist's art cover and update the new one when 'count' < 4
        /// </summary>
        public void NotifyOnPlaylistChanged()
        {
            PlaylistCover = CombineImage();
        }
    
    }
}
