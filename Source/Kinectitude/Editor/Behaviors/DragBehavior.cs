using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Kinectitude.Editor.Behaviors
{
    internal sealed class DragBehavior : Behavior<FrameworkElement>
    {
        public static DependencyProperty DragParameterProperty =
            DependencyProperty.Register("DragParameter", typeof(object), typeof(DragBehavior));

        public static DependencyProperty DragScopeProperty =
            DependencyProperty.Register("DragScope", typeof(FrameworkElement), typeof(DragBehavior));

        private Point? startPoint;

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
            AssociatedObject.PreviewMouseMove += OnPreviewMouseMove;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove -= OnPreviewMouseMove;
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - startPoint.Value.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - startPoint.Value.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    DragDrop.DoDragDrop(AssociatedObject, new DataObject(typeof(object), DragParameter), DragDropEffects.Copy);
                }
            }
        }
    }
}
