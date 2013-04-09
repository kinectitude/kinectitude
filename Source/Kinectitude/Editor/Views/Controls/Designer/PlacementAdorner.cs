//-----------------------------------------------------------------------
// <copyright file="PlacementAdorner.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Kinectitude.Editor.Views.Controls.Designer
{
    internal sealed class PlacementAdorner : Adorner
    {
        private readonly Point startPoint;
        private readonly SolidColorBrush fill;
        private readonly Pen stroke;
        private Point endPoint;

        public PlacementAdorner(DesignerCanvas canvas, Point startPoint) : base(canvas)
        {
            this.startPoint = startPoint;

            fill = new SolidColorBrush(Colors.CornflowerBlue) { Opacity = 0.2 };
            stroke = new Pen(new SolidColorBrush(Colors.CornflowerBlue), 1.0d);
            stroke.DashStyle = new DashStyle(Enumerable.Repeat(2.0d, 2), 0);
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
            InvalidateVisual();
        }
    }
}
