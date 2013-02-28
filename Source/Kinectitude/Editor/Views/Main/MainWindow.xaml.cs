using Kinectitude.Editor.Models;
using Kinectitude.Editor.Views.Utils;
using System.ComponentModel;
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
            InitializeComponent();

            Workspace.Instance.Initialize();
            DataContext = Workspace.Instance;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Workspace.Instance.WarnOnClose(r =>
            {
                if (r == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            });
        }
    }
}
