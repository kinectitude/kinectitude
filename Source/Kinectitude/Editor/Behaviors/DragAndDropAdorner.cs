using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Kinectitude.Editor.AttachedBehaviours
{
    internal class DragAndDropAdorner : Adorner
    {
        private readonly Rectangle rectangle;

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        public DragAndDropAdorner(FrameworkElement adornedElement)
            : base(adornedElement)
        {
            rectangle = new Rectangle()
            {
                Fill = new VisualBrush(adornedElement) { Opacity = 0.75 },
                Width = adornedElement.ActualWidth,
                Height = adornedElement.ActualHeight
            };
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
            group.Children.Add(new TranslateTransform(50, 50));
            return group;
        }
    }
}
