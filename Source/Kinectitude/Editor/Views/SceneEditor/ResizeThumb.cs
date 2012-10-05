using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Kinectitude.Editor.Views
{
    internal class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            DragDelta += ResizeThumb_DragDelta;
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            EntityItem entityItem = this.DataContext as EntityItem;
            SceneCanvas canvas = VisualTreeHelper.GetParent(entityItem) as SceneCanvas;

            if (entityItem != null && canvas != null && entityItem.IsSelected)
            {
                double minLeft, minTop, minDeltaHorizontal, minDeltaVertical;
                double dragDeltaVertical, dragDeltaHorizontal, scale;

                IEnumerable<EntityItem> selectedItems = canvas.SelectionService.SelectedItems.OfType<EntityItem>();

                CalculateDragLimits(selectedItems, out minLeft, out minTop,
                                    out minDeltaHorizontal, out minDeltaVertical);

                foreach (EntityItem item in selectedItems)
                {
                    if (item != null)
                    {
                        switch (base.VerticalAlignment)
                        {
                            case VerticalAlignment.Bottom:
                                dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                                scale = (item.ActualHeight - dragDeltaVertical) / item.ActualHeight;
                                DragBottom(scale, item, canvas.SelectionService);
                                break;
                            case VerticalAlignment.Top:
                                double top = Canvas.GetTop(item);
                                dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                                scale = (item.ActualHeight - dragDeltaVertical) / item.ActualHeight;
                                DragTop(scale, item, canvas.SelectionService);
                                break;
                            default:
                                break;
                        }

                        switch (base.HorizontalAlignment)
                        {
                            case HorizontalAlignment.Left:
                                double left = Canvas.GetLeft(item);
                                dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                                scale = (item.ActualWidth - dragDeltaHorizontal) / item.ActualWidth;
                                DragLeft(scale, item, canvas.SelectionService);
                                break;
                            case HorizontalAlignment.Right:
                                dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                                scale = (item.ActualWidth - dragDeltaHorizontal) / item.ActualWidth;
                                DragRight(scale, item, canvas.SelectionService);
                                break;
                            default:
                                break;
                        }
                    }
                }
                e.Handled = true;
            }
        }

        private void DragLeft(double scale, EntityItem item, SelectionService selectionService)
        {
            double left = Canvas.GetLeft(item) + item.Width;
            double groupItemLeft = Canvas.GetLeft(item);
            double delta = (left - groupItemLeft) * (scale - 1);
            Canvas.SetLeft(item, groupItemLeft - delta);
            item.Width = item.ActualWidth * scale;
        }

        private void DragTop(double scale, EntityItem item, SelectionService selectionService)
        {
            double bottom = Canvas.GetTop(item) + item.Height;
            double groupItemTop = Canvas.GetTop(item);
            double delta = (bottom - groupItemTop) * (scale - 1);
            Canvas.SetTop(item, groupItemTop - delta);
            item.Height = item.ActualHeight * scale;
        }

        private void DragRight(double scale, EntityItem item, SelectionService selectionService)
        {
            double left = Canvas.GetLeft(item);
            double groupItemLeft = Canvas.GetLeft(item);
            double delta = (groupItemLeft - left) * (scale - 1);
            Canvas.SetLeft(item, groupItemLeft + delta);
            item.Width = item.ActualWidth * scale;
        }

        private void DragBottom(double scale, EntityItem item, SelectionService selectionService)
        {
            double top = Canvas.GetTop(item);
            double groupItemTop = Canvas.GetTop(item);
            double delta = (groupItemTop - top) * (scale - 1);
            Canvas.SetTop(item, groupItemTop + delta);
            item.Height = item.ActualHeight * scale;
        }

        private void CalculateDragLimits(IEnumerable<EntityItem> selectedItems, out double minLeft, out double minTop, out double minDeltaHorizontal, out double minDeltaVertical)
        {
            minLeft = double.MaxValue;
            minTop = double.MaxValue;
            minDeltaHorizontal = double.MaxValue;
            minDeltaVertical = double.MaxValue;

            foreach (EntityItem item in selectedItems)
            {
                double left = Canvas.GetLeft(item);
                double top = Canvas.GetTop(item);

                minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);

                minDeltaVertical = Math.Min(minDeltaVertical, item.ActualHeight - item.MinHeight);
                minDeltaHorizontal = Math.Min(minDeltaHorizontal, item.ActualWidth - item.MinWidth);
            }
        }
    }
}
