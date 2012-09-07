using System.Windows;
using Kinectitude.Editor.Views;
using System;
using System.Reflection;
using System.IO;

namespace Kinectitude.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DialogService.RegisterWindow<SceneDialog>(DialogService.Constants.SceneDialog);
            DialogService.RegisterWindow<EntityDialog>(DialogService.Constants.EntityDialog);
            DialogService.RegisterWindow<ComponentDialog>(DialogService.Constants.ComponentDialog);
        }
    }
}
