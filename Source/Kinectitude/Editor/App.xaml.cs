using Kinectitude.Editor.Views.Dialogs;
using Kinectitude.Editor.Views.Scenes;
using Kinectitude.Editor.Views.Utils;
using System.Windows;

namespace Kinectitude.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DialogService.RegisterWindow<EntityDialog>(DialogService.Constants.EntityDialog);
            DialogService.RegisterWindow<AddEntityDialog>(DialogService.Constants.AddEntityDialog);
            DialogService.RegisterWindow<NameDialog>(DialogService.Constants.NameDialog);
            DialogService.RegisterWindow<SceneDialog>(DialogService.Constants.SceneDialog);
        }
    }
}
