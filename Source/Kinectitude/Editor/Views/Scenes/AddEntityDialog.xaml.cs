using System.Windows;

namespace Kinectitude.Editor.Views.Scenes
{
    /// <summary>
    /// Interaction logic for AddEntityDialog.xaml
    /// </summary>
    public partial class AddEntityDialog : Window
    {
        public AddEntityDialog()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
