using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RPiLCDPiServer.Converters
{
    public class PathToBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = (string)value;

            return new Bitmap(path);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
