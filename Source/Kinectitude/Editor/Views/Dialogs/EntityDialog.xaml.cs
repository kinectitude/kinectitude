using System.Windows;

namespace Kinectitude.Editor.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for EntityDialog.xaml
    /// </summary>
    public partial class EntityDialog : Window
    {
        public EntityDialog()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
