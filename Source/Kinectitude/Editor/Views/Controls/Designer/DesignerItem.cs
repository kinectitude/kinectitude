//-----------------------------------------------------------------------
// <copyright file="DesignerItem.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kinectitude.Editor.Views.Controls.Designer
{
    internal sealed class DesignerItem : ContentControl
    {
        public static DependencyProperty DesignLeftProperty =
            DependencyProperty.Register("DesignLeft", typeof(double), typeof(DesignerItem));

        public static DependencyProperty DesignTopProperty =
            DependencyProperty.Register("DesignTop", typeof(double), typeof(DesignerItem));

        public static DependencyProperty DesignWidthProperty =
            DependencyProperty.Register("DesignWidth", typeof(double), typeof(DesignerItem));

        public static DependencyProperty DesignHeightProperty =
            DependencyProperty.Register("DesignHeight", typeof(double), typeof(DesignerItem));

        public static DependencyProperty DesignRotationProperty =
            DependencyProperty.Register("DesignRotation", typeof(double), typeof(DesignerItem));

        public static DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(DesignerItem));

        public static DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.Register("DoubleClickCommand", typeof(ICommand), typeof(DesignerItem));

        //private static void OnDesignChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var item = d as DesignerItem;
        //    item.canvas.Dispatcher.BeginInvoke(new Action(() => item.canvas.Update()), null);
        //}

        private readonly DesignerCanvas canvas;

        public double DesignLeft
        {
            get { return (double)GetValue(DesignLeftProperty); }
            set { SetValue(DesignLeftProperty, value); }
        }

        public double DesignTop
        {
            get { return (double)GetValue(DesignTopProperty); }
            set { SetValue(DesignTopProperty, value); }
        }

        public double DesignWidth
        {
            get { return (double)GetValue(DesignWidthProperty); }
            set { SetValue(DesignWidthProperty, value); }
        }

        public double DesignHeight
        {
            get { return (double)GetValue(DesignHeightProperty); }
            set { SetValue(DesignHeightProperty, value); }
        }

        public double DesignRotation
        {
            get { return (double)GetValue(DesignRotationProperty); }
            set { SetValue(DesignRotationProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public ICommand DoubleClickCommand
        {
            get { return (ICommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }

        public DesignerItem(DesignerCanvas canvas)
        {
            this.canvas = canvas;
        }

        protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (null != DoubleClickCommand)
            {
                DoubleClickCommand.Execute(null);
            }
        }
    }
}
