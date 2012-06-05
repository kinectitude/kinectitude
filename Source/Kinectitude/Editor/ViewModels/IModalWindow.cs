using System;

namespace Kinectitude.Editor.ViewModels
{
    internal interface IModalWindow
    {
        bool? DialogResult { get; set; }
        object DataContext { get; set; }

        event EventHandler Closed;

        void ShowDialog();
        void Close();
    }
}
