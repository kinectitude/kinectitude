using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EditorModels.ViewModels;

namespace EditorModels.Views
{
    internal delegate void DialogCallback(bool? result);
    internal delegate void FileDialogCallback(bool? result, string file);

    internal static class DialogService
    {
        public static void ShowDialog(string title, BaseViewModel dataContext, DialogCallback onClose)
        {

        }

        public static void ShowMessageDialog(string title, string message, DialogCallback onClose)
        {

        }

        public static void ShowLoadDialog(FileDialogCallback onClose)
        {

        }

        public static void ShowSaveDialog(FileDialogCallback onClose)
        {

        }
    }
}
