using Kinectitude.Editor.Models.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Kinectitude.Editor.Views.Utils
{
    internal sealed class StringNullOrEmptyToVisibilityConverter : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var s = value as string;
            return string.IsNullOrEmpty(s) ? Visibility.Collapsed : Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        private static StringNullOrEmptyToVisibilityConverter instance;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (instance == null)
                instance = new StringNullOrEmptyToVisibilityConverter();
            return instance;
        }
    }
}
