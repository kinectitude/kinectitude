//-----------------------------------------------------------------------
// <copyright file="IDialogService.cs" company="Kinectitude">
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
using System.Windows.Forms;

namespace Kinectitude.Editor.Views.Utils
{
    delegate void DialogCallback(bool? result);
    delegate void FileDialogCallback(bool? result, string file);
    delegate void FolderDialogCallback(DialogResult result, string folder);
    delegate void MessageBoxCallback(MessageBoxResult result);

    interface IDialogService
    {
        void ShowDialog<TWindow>(object viewModel, DialogCallback onDialogClose) where TWindow : Window, new();
        void ShowDialog<TWindow>(object viewModel = null) where TWindow : Window, new();
        void ShowLoadDialog(FileDialogCallback onClose);
        void ShowSaveDialog(FileDialogCallback onClose);
        void ShowFolderDialog(FolderDialogCallback onClose);
        void Warn(string title, string message, MessageBoxButton buttons, MessageBoxCallback onClose = null);
    }
}
