using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MediaPlayer.Converter
{
    public class CheckCurrentMediaIsPlayingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string firstValue  = (string)values[0];
            string secondValue = (string)values[1];
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow.MusicPlayerViewModel.CurrentState == "Play"
                && secondValue == mainWindow.MusicPlayerViewModel.CurrentMedia.PlayListName
                && firstValue == mainWindow.MusicPlayerViewModel.CurrentMedia.Title)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
