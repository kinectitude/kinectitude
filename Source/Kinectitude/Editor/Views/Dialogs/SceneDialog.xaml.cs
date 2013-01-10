using System.Windows;

namespace Kinectitude.Editor.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for SceneDialog.xaml
    /// </summary>
    public partial class SceneDialog : Window
    {
        public SceneDialog()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
