using Kinectitude.Editor.Views.Dialogs;
using Kinectitude.Editor.Views.Scenes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

namespace Kinectitude.Editor.Views.Utils
{
    internal sealed class DialogService : IDialogService
    {
        public void ShowDialog<TWindow>(object viewModel, DialogCallback onDialogClose) where TWindow : Window, new()
        {
            Window view = new TWindow();
            view.DataContext = viewModel;

            if (null != onDialogClose)
            {
                view.Closed += (sender, args) => onDialogClose(view.DialogResult);
            }
            view.ShowDialog();
        }

        public void ShowDialog<TWindow>(object viewModel = null) where TWindow : Window, new()
        {
            ShowDialog<TWindow>(viewModel, null);
        }

        public void ShowLoadDialog(FileDialogCallback onClose)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.DefaultExt = ".xml";
            dialog.Filter = "Kinectitude XML Files (.xml)|*.xml";

            bool? result = dialog.ShowDialog();

            if (null != onClose)
            {
                onClose(result, dialog.FileName);
            }
        }

        public void ShowSaveDialog(FileDialogCallback onClose)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();

            dialog.DefaultExt = ".xml";
            dialog.Filter = "Kinectitude XML Files (.xml)|*.xml";

            bool? result = dialog.ShowDialog();

            if (null != onClose)
            {
                onClose(result, dialog.FileName);
            }
        }

        public void ShowFolderDialog(FolderDialogCallback onClose)
        {
            var dialog = new FolderBrowserDialog();
            //dialog.SelectedPath = Environment.SpecialFolder.Personal;

            var result = dialog.ShowDialog();
            
            if (null != onClose)
            {
                onClose(result, dialog.SelectedPath);
            }
        }

        public void Warn(string title, string message, MessageBoxButton buttons, MessageBoxCallback onClose)
        {
            var result = System.Windows.MessageBox.Show(message, title, buttons, MessageBoxImage.Warning);

            if (null != onClose)
            {
                onClose(result);
            }
        }
    }
}
