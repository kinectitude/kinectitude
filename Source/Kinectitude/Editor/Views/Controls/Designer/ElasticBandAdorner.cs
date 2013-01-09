using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Kinectitude.Editor.Views.Controls.Designer
{
    internal sealed class ElasticBandAdorner : Adorner
    {
        private readonly DesignerCanvas canvas;
        private readonly Point startPoint;
        private readonly SolidColorBrush fill;
        private readonly Pen stroke;
        private Point endPoint;

        public ElasticBandAdorner(DesignerCanvas canvas, Point startPoint)
            : base(canvas)
        {
            this.canvas = canvas;
            this.startPoint = startPoint;

            fill = new SolidColorBrush(Colors.CornflowerBlue) { Opacity = 0.2 };
            stroke = new Pen(new SolidColorBrush(Colors.CornflowerBlue), 1.0d);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect rect = new Rect(startPoint, endPoint);
            rect.Offset(-0.5d, -0.5d);

            drawingContext.DrawRectangle(fill, stroke, rect);
        }

        public void Update(Point endPoint)
        {
            this.endPoint = endPoint;

            Rect selectionRect = new Rect(startPoint, endPoint);

            foreach (var item in canvas.Items)
            {
                var designerItem = canvas.ItemContainerGenerator.ContainerFromItem(item) as DesignerItem;
                var bounds = VisualTreeHelper.GetDescendantBounds(designerItem);

                if (selectionRect.Contains(bounds))
                {
                    canvas.Select(designerItem);
                }
                else
                {
                    canvas.Deselect(designerItem);
                }
            }

            InvalidateVisual();
        }
    }
}
