using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.ViewModels;
using System.Windows;

namespace Kinectitude.Editor.Views
{
    internal static class ModalDialogService
    {
        public static class Constants
        {
            public const string EntityDialog = "EntityDialog";
            public const string ComponentDialog = "ComponentDialog";
        }

        private static readonly Dictionary<string, Type> views;

        static ModalDialogService()
        {
            views = new Dictionary<string, Type>();
        }

        public static void ShowDialog<TViewModel>(string name, TViewModel viewModel, Action<bool?, TViewModel> onDialogClose)
        {
            Window view = GetWindow(name);
            view.DataContext = viewModel;

            if (null != onDialogClose)
            {
                view.Closed += (sender, args) => onDialogClose(view.DialogResult, viewModel);
            }
            view.ShowDialog();
        }

        public static void ShowDialog<TViewModel>(string name, TViewModel viewModel)
        {
            ShowDialog(name, viewModel, null);
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
