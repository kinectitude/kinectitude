using System.Windows;
using Kinectitude.Editor.Views;

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
            DialogService.RegisterWindow<AddEntityDialog>(DialogService.Constants.AddEntityDialog);
            DialogService.RegisterWindow<RenameDialog>(DialogService.Constants.RenameDialog);
        }
    }
}
