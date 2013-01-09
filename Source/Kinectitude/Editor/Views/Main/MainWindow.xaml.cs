using Kinectitude.Editor.Models;
using System.Windows;

namespace Kinectitude.Editor.Views.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = Workspace.Instance;
            Workspace.Instance.Initialize();

            Workspace.Instance.NewProject();

            InitializeComponent();
        }
    }
}
