using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MediaPlayer.Converter
{
    public class TimeSpanToMinuteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan timeSpan = (TimeSpan)value;
            int minutes = (int)timeSpan.Minutes;
            int seconds = (int)timeSpan.Seconds;
            string result = minutes < 10?$"0{minutes}:":$"{minutes}:";
            result += seconds < 10 ? $"0{seconds}" : $"{seconds}";
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
