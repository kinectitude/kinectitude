//-----------------------------------------------------------------------
// <copyright file="EntityConverter.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Views.Scenes.Presenters;
using System;
using System.Windows.Data;

namespace Kinectitude.Editor.Views.Utils
{
    internal sealed class EntityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object result = null;

            if (null != value)
            {
                result = ((EntityPresenter)value).Entity;
            }

            return result;
        }
    }
}
