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

    interface IDialogService
    {
        void ShowDialog<TWindow>(object viewModel, DialogCallback onDialogClose) where TWindow : Window, new();
        void ShowDialog<TWindow>(object viewModel = null) where TWindow : Window, new();
        void ShowLoadDialog(FileDialogCallback onClose);
        void ShowSaveDialog(FileDialogCallback onClose);
        void ShowFolderDialog(FolderDialogCallback onClose);
    }
}
