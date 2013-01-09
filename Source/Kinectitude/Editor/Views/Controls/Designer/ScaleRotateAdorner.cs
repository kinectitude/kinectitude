using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Kinectitude.Editor.Views.Controls.Designer
{
    internal sealed class ScaleRotateAdorner : Adorner
    {
        private readonly Control control;
        private readonly DesignerItem item;
        private readonly DesignerCanvas canvas;

        private Thumb rotatorThumb;

        private Point startPoint;
        private Point previousPoint;
        private bool mouseDown;

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        public ScaleRotateAdorner(Control control, DesignerItem item, DesignerCanvas canvas) : base(item)
        {
            this.control = control;
            this.item = item;
            this.canvas = canvas;

            AddVisualChild(control);

            Loaded += OnLoaded;
        }

        private T FindTemplateElement<T>(string name) where T : DependencyObject
        {
            return control.Template.FindName(name, control) as T;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //originThumb = FindTemplateElement<Thumb>("Origin");
            ////origin.DragDelta += origin_DragDelta;

            //rotatorThumb = FindTemplateElement<Thumb>("Rotator");
            //rotatorThumb.PreviewMouseLeftButtonUp += rotator_MouseLeftButtonUp;
            //rotatorThumb.PreviewMouseLeftButtonDown += rotator_MouseLeftButtonDown;
            //rotatorThumb.PreviewMouseMove += rotator_MouseMove;
        }

        void rotator_MouseMove(object sender, MouseEventArgs e)
        {
            //var currentPoint = 
        }

        void rotator_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void rotator_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(this);

            // get original angle from origin to this
            // use a variable to track the logical origin instead of using the visual location of the origin

            //var transform = originThumb.TransformToAncestor(this);
            //var pt = transform.Transform(new Point());
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return control;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            return base.GetDesiredTransform(transform);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(item.ActualWidth, item.ActualHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            control.Arrange(new Rect(new Point(0, 0), finalSize));
            return new Size(control.ActualWidth, control.ActualHeight);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (mouseDown)
            {
                // TODO: Render preview outlines

                //drawingContext.DrawEllipse(new SolidColorBrush(Colors.Red), new Pen(new SolidColorBrush(Colors.Black), 2.0), new Point(0, 0), 50, 50);

            }
        }

        //public void Update()
        //{
        //    var rect = VisualTreeHelper.GetDescendantBounds(canvas.SelectedItems.First());

        //    foreach (var item in canvas.SelectedItems.Skip(1))
        //    {
        //        rect.Union(VisualTreeHelper.GetDescendantBounds(item));
        //    }

        //    InvalidateMeasure();
        //}

        private Thumb FindThumbAt(Point point)
        {
            Thumb thumb = null;

            var result = VisualTreeHelper.HitTest(control, point);
            if (null != result)
            {
                var element = result.VisualHit as FrameworkElement;
                thumb = element.FindAncestorOfType<Thumb>();
            }

            return thumb;
        }
    }
}
