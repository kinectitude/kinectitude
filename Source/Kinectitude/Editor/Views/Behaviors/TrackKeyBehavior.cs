//-----------------------------------------------------------------------
// <copyright file="TrackKeyBehavior.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Kinectitude.Editor.Views.Behaviors
{
    internal sealed class TrackKeyBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty IsKeyDownProperty =
            DependencyProperty.Register("IsKeyDown", typeof(bool), typeof(TrackKeyBehavior));

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(Key), typeof(TrackKeyBehavior));

        public bool IsKeyDown
        {
            get { return (bool)GetValue(IsKeyDownProperty); }
            set { SetValue(IsKeyDownProperty, value); }
        }

        public Key Key
        {
            get { return (Key)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
            AssociatedObject.PreviewKeyUp += OnPreviewKeyUp;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
            AssociatedObject.PreviewKeyUp -= OnPreviewKeyUp;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key)
            {
                IsKeyDown = true;
            }
        }

        private void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key)
            {
                IsKeyDown = false;
            }
        }
    }
}
