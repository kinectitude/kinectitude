using System;
using System.Collections.Generic;
using System.Windows;

namespace Kinectitude.Editor.Views
{
    internal static class DialogService
    {
        public delegate void DialogCallback(bool? result);
        public delegate void FileDialogCallback(bool? result, string file);

        public static class Constants
        {
            public static readonly string EntityDialog = typeof(EntityDialog).Name;
            public static readonly string ComponentDialog = typeof(ComponentDialog).Name;
            public static readonly string AddEntityDialog = typeof(AddEntityDialog).Name;
            public static readonly string NameDialog = typeof(NameDialog).Name;
            public static readonly string SceneDialog = typeof(SceneDialog).Name;
        }

        private static readonly Dictionary<string, Type> views = new Dictionary<string, Type>();

        public static void ShowDialog<TViewModel>(string name, TViewModel viewModel, DialogCallback onDialogClose)
        {
            Window view = GetWindow(name);
            view.DataContext = viewModel;

            if (null != onDialogClose)
            {
                view.Closed += (sender, args) => onDialogClose(view.DialogResult);
            }
            view.ShowDialog();
        }

        public static void ShowDialog<TViewModel>(string name, TViewModel viewModel)
        {
            ShowDialog(name, viewModel, null);
        }

        public static void ShowLoadDialog(FileDialogCallback onClose)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.DefaultExt = ".xml";
            dialog.Filter = "Kinectitude XML Files (.xml)|*.xml";

            bool? result = dialog.ShowDialog();

            if (null != onClose)
            {
                onClose(result, dialog.FileName);
            }
        }

        public static void ShowSaveDialog(FileDialogCallback onClose)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();

            dialog.DefaultExt = ".xml";
            dialog.Filter = "Kinectitude XML Files (.xml)|*.xml";

            bool? result = dialog.ShowDialog();

            if (null != onClose)
            {
                onClose(result, dialog.FileName);
            }
        }

        public static void ShowMessageDialog(string title, string message, DialogCallback onClose)
        {

        }

        private static Window GetWindow(string name)
        {
            Type type;
            views.TryGetValue(name, out type);
            return Activator.CreateInstance(type) as Window;
        }

        public static void RegisterWindow<TWindow>(string name) where TWindow : Window
        {
            views[name] = typeof(TWindow);
        }
    }
}
