using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kinectitude.Editor.Models;
using System.Collections.Specialized;

namespace Kinectitude.Editor.Views
{
    /// <summary>
    /// Interaction logic for SceneEditor.xaml
    /// </summary>
    public partial class SceneEditor : UserControl
    {
        //private Scene scene;

        public SceneEditor()
        {
            InitializeComponent();

            //this.DataContextChanged += SceneEditor_DataContextChanged;
        }

        private void AddEntity_Click(object sender, RoutedEventArgs e)
        {
            DialogService.ShowDialog(DialogService.Constants.AddEntityDialog, DataContext);
        }

        /*private void SceneEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (null != this.scene)
            {
                this.scene.SelectedEntities.CollectionChanged -= SelectedEntities_CollectionChanged;
            }

            this.scene = e.NewValue as Scene;

            if (null != this.scene)
            {
                this.scene.SelectedEntities.CollectionChanged += SelectedEntities_CollectionChanged;
            }
        }

        private void SelectedEntities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }*/
    }
}
