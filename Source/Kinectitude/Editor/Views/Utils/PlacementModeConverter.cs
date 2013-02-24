using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Kinectitude.Editor.Views.Utils
{
    internal sealed class PlacementModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return parameter.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((bool)value) == true)
            {
                return parameter;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
