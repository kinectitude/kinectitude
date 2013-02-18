using Kinectitude.Editor.Models.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Kinectitude.Editor.Views.Utils
{
    internal sealed class ValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = "";

            var typedValue = value as Value;
            if (null != typedValue)
            {
                result = typedValue.Initializer;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Value(value.ToString());
        }
    }
}
