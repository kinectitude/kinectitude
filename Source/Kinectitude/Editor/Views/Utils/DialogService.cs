//-----------------------------------------------------------------------
// <copyright file="DialogService.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

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

        public static void ShowFileChooserDialog(FileDialogCallback onClose, string fileFilter, string fileChooserTitle)
        {
            if (fileChooserTitle == null || fileFilter == null)
            {
                onClose(false, null);
            }

            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.Filter = fileFilter;
            dialog.Title = fileChooserTitle;

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

        public void Warn(string title, string message, MessageBoxButton buttons, MessageBoxCallback onClose = null)
        {
            var result = System.Windows.MessageBox.Show(message, title, buttons, MessageBoxImage.Warning);

            if (null != onClose)
            {
                onClose(result);
            }
        }
    }
}
