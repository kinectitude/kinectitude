using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Kinectitude.Editor.Views.Behaviors
{
    internal sealed class DropTargetBehavior : Behavior<FrameworkElement>
    {
        public static DependencyProperty DropCommandProperty =
            DependencyProperty.Register("DropCommand", typeof(ICommand), typeof(DropTargetBehavior));

        public static DependencyProperty DragScopeProperty =
            DependencyProperty.Register("DragScope", typeof(FrameworkElement), typeof(DropTargetBehavior));

        public static DependencyProperty CanExecuteProperty =
            DependencyProperty.RegisterAttached("CanExecute", typeof(bool), typeof(DropTargetBehavior));

        public static bool GetCanExecute(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanExecuteProperty);
        }

        public static void SetCanExecute(DependencyObject obj, bool value)
        {
            obj.SetValue(CanExecuteProperty, value);
        }

        public ICommand DropCommand
        {
            get { return (ICommand)GetValue(DropCommandProperty); }
            set { SetValue(DropCommandProperty, value); }
        }

        public FrameworkElement DragScope
        {
            get { return (FrameworkElement)GetValue(DragScopeProperty); }
            set { SetValue(DragScopeProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.AllowDrop = true;
            AssociatedObject.Drop += OnDrop;
            AssociatedObject.DragEnter += OnDragEnter;
            AssociatedObject.DragLeave += OnDragLeave;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.AllowDrop = false;
            AssociatedObject.Drop -= OnDrop;
            AssociatedObject.DragEnter -= OnDragEnter;
            AssociatedObject.DragLeave -= OnDragLeave;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (null != DropCommand)
            {
                object parameter = e.Data.GetData(typeof(object));
                
                if (DropCommand.CanExecute(parameter))
                {
                    DropCommand.Execute(parameter);
                }
            }

            SetCanExecute(AssociatedObject, false);
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            bool canExecute = false;

            if (null != DropCommand)
            {
                object parameter = e.Data.GetData(typeof(object));
                canExecute = DropCommand.CanExecute(parameter);
            }

            SetCanExecute(AssociatedObject, canExecute);
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            SetCanExecute(AssociatedObject, false);
        }
    }
}
