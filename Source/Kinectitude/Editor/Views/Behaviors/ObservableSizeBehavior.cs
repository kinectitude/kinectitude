﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;

namespace Kinectitude.Editor.Views.Behaviors
{
    internal sealed class ObservableSizeBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty ObservedWidthProperty =
            DependencyProperty.Register("ObservedWidth", typeof(double), typeof(ObservableSizeBehavior));

        public static readonly DependencyProperty ObservedHeightProperty =
            DependencyProperty.Register("ObservedHeight", typeof(double), typeof(ObservableSizeBehavior));

        public double ObservedWidth
        {
            get { return (double)GetValue(ObservedWidthProperty); }
            set { SetValue(ObservedWidthProperty, value); }
        }

        public double ObservedHeight
        {
            get { return (double)GetValue(ObservedHeightProperty); }
            set { SetValue(ObservedHeightProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.SizeChanged += OnSizeChanged;
            UpdateSizes();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SizeChanged -= OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateSizes();
        }

        private void UpdateSizes()
        {
            ObservedWidth = AssociatedObject.ActualWidth;
            ObservedHeight = AssociatedObject.ActualHeight;
        }
    }
}