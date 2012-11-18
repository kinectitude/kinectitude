using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Kinectitude.Editor.Views
{
    internal class ControlAdorner : Adorner
    {
        private Control child;

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        public Control Child
        {
            get { return child; }
            set
            {
                if (null != child)
                {
                    RemoveVisualChild(child);
                }

                child = value;
                
                if (null != child)
                {
                    AddVisualChild(child);
                }
            }
        }

        public ControlAdorner(UIElement adornedElement) : base(adornedElement)
        {

        }

        protected override System.Windows.Media.Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return child;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            child.Measure(constraint);
            return child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            child.Arrange(new Rect(new Point(0, 0), finalSize));
            return new Size(child.ActualWidth, child.ActualHeight);
        }
    }
}
