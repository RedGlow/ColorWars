using ColorManagment;
using ColorWars.Controller.Colors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ColorWars.View.Converters
{
    class ColorRGBConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var sourceDye = (Dye)values[0];
            var material = (Material)values[1];
            var sourceColor = sourceDye.GetColor(material);
            var destinationBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(
                (byte)(sourceColor.R * 255),
                (byte)(sourceColor.G * 255),
                (byte)(sourceColor.B * 255)));
            return destinationBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Not implemented yet, to be honest it is possible.");
        }
    }
}
