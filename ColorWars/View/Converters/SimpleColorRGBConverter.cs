using ColorManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ColorWars.View.Converters
{
    class SimpleColorRGBConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null)
                return System.Windows.Media.Colors.Black;
            var sourceColor = (ColorRGB)value;
            var destinationBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(
                (byte)(sourceColor.R * 255),
                (byte)(sourceColor.G * 255),
                (byte)(sourceColor.B * 255)));
            return destinationBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
