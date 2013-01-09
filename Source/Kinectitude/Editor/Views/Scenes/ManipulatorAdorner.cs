using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Kinectitude.Editor.Views
{
    internal sealed class ManipulatorAdorner : Adorner
    {
        private readonly SceneCanvas canvas;
        private readonly Pen boundingBoxPen;
        //private readonly VisualCollection children;

        //protected override int VisualChildrenCount
        //{
        //    get { return children.Count; }
        //}

        public ManipulatorAdorner(SceneCanvas canvas)
            : base(canvas)
        {
            this.canvas = canvas;

            boundingBoxPen = new Pen(Brushes.CornflowerBlue, 2);

            AddVisualChild(new Button() { Content = "Hello" });
        }

        //protected override Visual GetVisualChild(int index)
        //{
        //    return children[index];
        //}

        //protected override Size MeasureOverride(Size constraint)
        //{
        //    return base.MeasureOverride(constraint);
        //}

        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    return base.ArrangeOverride(finalSize);
        //}

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            EntityItem first = canvas.SelectionService.SelectedItems.First();
            Rect itemRect = VisualTreeHelper.GetDescendantBounds(first);
            Rect itemBounds = first.TransformToAncestor(canvas).TransformBounds(itemRect);

            double left = itemBounds.Left;
            double right = itemBounds.Right;
            double top = itemBounds.Top;
            double bottom = itemBounds.Bottom;

            foreach (EntityItem item in canvas.SelectionService.SelectedItems)
            {
                itemRect = VisualTreeHelper.GetDescendantBounds(item);
                itemBounds = item.TransformToAncestor(canvas).TransformBounds(itemRect);

                if (itemBounds.Left < left)
                {
                    left = itemBounds.Left;
                }

                if (itemBounds.Top < top)
                {
                    top = itemBounds.Top;
                }

                if (itemBounds.Right > right)
                {
                    right = itemBounds.Right;
                }

                if (itemBounds.Bottom > bottom)
                {
                    bottom = itemBounds.Bottom;
                }
            }

            dc.DrawRectangle(Brushes.Transparent, boundingBoxPen, new Rect(left, top, right - left, bottom - top));
        }
    }
}
