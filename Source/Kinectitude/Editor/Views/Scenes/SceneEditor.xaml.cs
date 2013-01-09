using Kinectitude.Editor.Views.Utils;
using System.Windows;
using System.Windows.Controls;

namespace Kinectitude.Editor.Views.Scenes
{
    /// <summary>
    /// Interaction logic for SceneEditor.xaml
    /// </summary>
    public partial class SceneEditor : UserControl
    {
        public SceneEditor()
        {
            InitializeComponent();
        }

        private void AddEntity_Click(object sender, RoutedEventArgs e)
        {
            DialogService.ShowDialog(DialogService.Constants.AddEntityDialog, DataContext);
        }
    }
}
