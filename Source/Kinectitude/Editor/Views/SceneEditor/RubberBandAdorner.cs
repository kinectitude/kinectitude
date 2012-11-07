using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Kinectitude.Editor.Views
{
    internal class RubberBandAdorner : Adorner
    {
        private SceneCanvas canvas;
        private Point? startPoint;
        private Point? endPoint;
        private Pen pen;

        public RubberBandAdorner(SceneCanvas canvas, Point? startPoint) : base(canvas)
        {
            this.canvas = canvas;
            this.startPoint = startPoint;

            pen = new Pen(Brushes.LightSlateGray, 1);
            pen.DashStyle = new DashStyle(new[] { 2.0d }, 1);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!IsMouseCaptured)
                {
                    CaptureMouse();
                }

                endPoint = e.GetPosition(this);
                UpdateSelection();
                InvalidateVisual();
            }
            else if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }

            e.Handled = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
            if (null != adornerLayer)
            {
                adornerLayer.Remove(this);
            }

            e.Handled = true;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

            if (startPoint.HasValue && endPoint.HasValue)
            {
                dc.DrawRectangle(Brushes.Transparent, pen, new Rect(startPoint.Value, endPoint.Value));
            }
        }

        private void UpdateSelection()
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.None)
            {
                foreach (EntityItem item in canvas.EntityItems)
                {
                    item.IsSelected = false;
                }
            }

            Rect boxedSelection = new Rect(startPoint.Value, endPoint.Value);

            foreach (EntityItem item in canvas.EntityItems)
            {
                Rect itemRect = VisualTreeHelper.GetDescendantBounds(item);
                Rect itemBounds = item.TransformToAncestor(canvas).TransformBounds(itemRect);

                if (boxedSelection.Contains(itemBounds))
                {
                    item.IsSelected = true;
                }
            }
        }
    }
}
