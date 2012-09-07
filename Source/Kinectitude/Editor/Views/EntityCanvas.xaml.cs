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
using System.Windows.Controls.Primitives;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Views
{
    /// <summary>
    /// Interaction logic for EntityCanvas.xaml
    /// </summary>
    public partial class EntityCanvas : UserControl
    {
        public EntityCanvas()
        {
            InitializeComponent();
        }

        private void Thumb_OnDragDelta(object sender, DragDeltaEventArgs args)
        {
            EntityViewModel entity = this.DataContext as EntityViewModel;

            if (null != entity)
            {
                entity.X += args.HorizontalChange;
                entity.Y += args.VerticalChange;
            }
        }
    }
}
