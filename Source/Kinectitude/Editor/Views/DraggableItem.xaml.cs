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

namespace Kinectitude.Editor.Views
{
    /// <summary>
    /// Interaction logic for DraggableItem.xaml
    /// </summary>
    public partial class DraggableItem : UserControl
    {
        public DraggableItem()
        {
            InitializeComponent();
        }

        private void DraggableItem_MouseMove(object sender, MouseEventArgs args)
        {
            DraggableItem item = sender as DraggableItem;

            if (null != item && args.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(item, new DragDropData(item.DataContext as Plugin), DragDropEffects.Copy);
            }
        }
    }
}
