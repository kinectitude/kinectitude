using Kinectitude.Editor.Models;
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

        private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}
