using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;


namespace RPiLCDPiServer.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public const string Invert = "Invert";

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {

            bool? bValue = (bool?)value;

            if (parameter != null && parameter as string == Invert)
                bValue = !bValue;

            return bValue.HasValue && bValue.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
