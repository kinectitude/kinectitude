using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Kinectitude.Editor.Views.Behaviors
{
    internal class DragAdorner : Adorner
    {
        private readonly Rectangle rectangle;
        private readonly TranslateTransform translate;

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        public DragAdorner(FrameworkElement draggedElement, FrameworkElement dragScope) : base(dragScope)
        {
            rectangle = new Rectangle()
            {
                Fill = new VisualBrush(draggedElement) { Opacity = 0.90d },
                Width = draggedElement.ActualWidth,
                Height = draggedElement.ActualHeight
            };

            translate = new TranslateTransform();
        }

        protected override Visual GetVisualChild(int index)
        {
            return rectangle;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            rectangle.Measure(constraint);
            return rectangle.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            rectangle.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup group = new GeneralTransformGroup();
            group.Children.Add(base.GetDesiredTransform(transform));
            group.Children.Add(translate);
            return group;
        }

        public void UpdatePosition(Point position)
        {
            Console.WriteLine("Position: " + position.X);

            translate.X = position.X;
            translate.Y = position.Y;

            InvalidateMeasure();
        }
    }
}
