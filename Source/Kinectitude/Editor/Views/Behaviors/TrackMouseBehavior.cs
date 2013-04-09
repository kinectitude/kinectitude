//-----------------------------------------------------------------------
// <copyright file="TrackMouseBehavior.cs" company="Kinectitude">
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
    internal sealed class TrackMouseBehavior : Behavior<FrameworkElement>
    {
        public static DependencyProperty MouseXProperty =
            DependencyProperty.Register("MouseX", typeof(double), typeof(TrackMouseBehavior));

        public static DependencyProperty MouseYProperty =
            DependencyProperty.Register("MouseY", typeof(double), typeof(TrackMouseBehavior));

        public double MouseX
        {
            get { return (double)GetValue(MouseXProperty); }
            set { SetValue(MouseXProperty, value); }
        }

        public double MouseY
        {
            get { return (double)GetValue(MouseYProperty); }
            set { SetValue(MouseYProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseMove += OnPreviewMouseMove;
            AssociatedObject.PreviewDragOver += OnPreviewDragOver;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseMove -= OnPreviewMouseMove;
        }

        private void Update(Point position)
        {
            MouseX = position.X;
            MouseY = position.Y;
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            Update(e.GetPosition(AssociatedObject));
            
        }

        private void OnPreviewDragOver(object sender, DragEventArgs e)
        {
            Update(e.GetPosition(AssociatedObject));
        }
    }
}
