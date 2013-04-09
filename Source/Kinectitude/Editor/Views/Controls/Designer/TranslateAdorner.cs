//-----------------------------------------------------------------------
// <copyright file="TranslateAdorner.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;
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
            var first = canvas.SelectedItems.First();
            //rect = VisualTreeHelper.GetDescendantBounds(canvas.SelectedItems.First());
            rect.X = first.DesignLeft;
            rect.Y = first.DesignTop;
            rect.Width = first.ActualWidth;
            rect.Height = first.ActualHeight;     

            foreach (var item in canvas.SelectedItems.Skip(1))
            {
                rect.Union(VisualTreeHelper.GetDescendantBounds(item));
            }

            rect.Inflate(3.0d, 3.0d);
            InvalidateVisual();
        }
    }
}
