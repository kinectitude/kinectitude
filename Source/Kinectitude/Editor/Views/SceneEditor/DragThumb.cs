using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Controls;

namespace Kinectitude.Editor.Views
{
    internal class DragThumb : Thumb
    {
        public DragThumb()
        {
            DragDelta += DragThumb_DragDelta;
        }

        void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            EntityItem entityItem = this.DataContext as EntityItem;
            SceneCanvas canvas = VisualTreeHelper.GetParent(entityItem) as SceneCanvas;
            if (entityItem != null && canvas != null && entityItem.IsSelected)
            {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;

                var selectedItems = canvas.SelectionService.SelectedItems.OfType<EntityItem>();

                foreach (EntityItem item in selectedItems)
                {
                    double left = Canvas.GetLeft(item);
                    double top = Canvas.GetTop(item);

                    minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                    minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);
                }

                double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
                double deltaVertical = Math.Max(-minTop, e.VerticalChange);

                foreach (EntityItem item in selectedItems)
                {
                    double left = Canvas.GetLeft(item);
                    double top = Canvas.GetTop(item);

                    if (double.IsNaN(left)) left = 0;
                    if (double.IsNaN(top)) top = 0;

                    Canvas.SetLeft(item, left + deltaHorizontal);
                    Canvas.SetTop(item, top + deltaVertical);
                }

                canvas.InvalidateMeasure();
                e.Handled = true;
            }
        }
    }
}
