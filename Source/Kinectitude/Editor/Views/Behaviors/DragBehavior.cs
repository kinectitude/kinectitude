//-----------------------------------------------------------------------
// <copyright file="DragBehavior.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Kinectitude.Editor.Views.Behaviors
{
    internal sealed class DragBehavior : Behavior<FrameworkElement>
    {
        public static DependencyProperty DragParameterProperty =
            DependencyProperty.Register("DragParameter", typeof(object), typeof(DragBehavior));

        public static DependencyProperty DragScopeProperty =
            DependencyProperty.Register("DragScope", typeof(FrameworkElement), typeof(DragBehavior));

        public const double DragThreshold = 0.5d;

        private Point startPoint;
        private Point previousPoint;

        public object DragParameter
        {
            get { return GetValue(DragParameterProperty); }
            set { SetValue(DragParameterProperty, value); }
        }

        public FrameworkElement DragScope
        {
            get { return (FrameworkElement)GetValue(DragScopeProperty); }
            set { SetValue(DragScopeProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            AssociatedObject.PreviewMouseMove += OnPreviewMouseMove;
            AssociatedObject.LostMouseCapture += OnLostMouseCapture;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
            AssociatedObject.PreviewMouseMove -= OnPreviewMouseMove;
            AssociatedObject.LostMouseCapture -= OnLostMouseCapture;
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(DragScope);
            previousPoint = startPoint;
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {

        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var currentPoint = e.GetPosition(DragScope);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var delta = currentPoint - startPoint;

                if (Math.Abs(delta.X) > DragThreshold || Math.Abs(delta.Y) > DragThreshold)
                {
                    DragDrop.DoDragDrop(AssociatedObject, new DataObject(typeof(object), DragParameter), DragDropEffects.Move);
                }
            }

            previousPoint = currentPoint;
        }
    }
}
