using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Kinectitude.Editor.Behaviors
{
    internal sealed class DropTargetBehavior : Behavior<FrameworkElement>
    {
        public static DependencyProperty DropCommandProperty =
            DependencyProperty.Register("DropCommand", typeof(ICommand), typeof(DropTargetBehavior));

        public ICommand DropCommand
        {
            get { return (ICommand)GetValue(DropCommandProperty); }
            set { SetValue(DropCommandProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.AllowDrop = true;
            AssociatedObject.Drop += OnDrop;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.AllowDrop = false;
            AssociatedObject.Drop -= OnDrop;
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
        }
    }
}
