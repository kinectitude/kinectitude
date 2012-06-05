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
            ModalDialogService.RegisterWindow<EntityDialog>(ModalDialogService.Constants.EntityDialog);
            ModalDialogService.RegisterWindow<ComponentDialog>(ModalDialogService.Constants.ComponentDialog);
        }
    }
}
