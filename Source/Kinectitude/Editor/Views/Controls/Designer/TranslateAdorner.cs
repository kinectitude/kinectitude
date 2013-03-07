﻿using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Kinectitude.Editor.Views.Controls.Designer
{
    internal sealed class TranslateAdorner : Adorner
    {
        private readonly DesignerCanvas canvas;
        private readonly Pen stroke;

        private Rect rect;

        public TranslateAdorner(DesignerCanvas canvas) : base(canvas)
        {
            this.canvas = canvas;

            stroke = new Pen(new SolidColorBrush(Colors.DarkGray), 3);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(null, stroke, rect);
        }

        public void Update()
        {
            rect = VisualTreeHelper.GetDescendantBounds(canvas.SelectedItems.First());

            foreach (var item in canvas.SelectedItems.Skip(1))
            {
                rect.Union(VisualTreeHelper.GetDescendantBounds(item));
            }

            rect.Inflate(3.0d, 3.0d);
            InvalidateVisual();
        }
    }
}