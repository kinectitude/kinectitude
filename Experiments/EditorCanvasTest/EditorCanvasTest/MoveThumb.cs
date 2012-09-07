using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using EditorCanvasTest.Models;

namespace EditorCanvasTest
{
    public class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs args)
        {
            Entity entity = this.DataContext as Entity;

            if (null != entity)
            {
                entity.X += args.HorizontalChange;
                entity.Y += args.VerticalChange;
            }
        }
    }
}
