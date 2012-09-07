using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace EditorCanvasTest
{
    public class ResizeRotateAdorner : Adorner
    {
        private VisualCollection visuals;
        private ResizeRotateChrome chrome;

        protected override int VisualChildrenCount
        {
            get { return this.visuals.Count; }
        }

        public ResizeRotateAdorner(ContentControl designerItem) : base(designerItem)
        {
            SnapsToDevicePixels = true;
            this.chrome = new ResizeRotateChrome();
            this.chrome.DataContext = designerItem;
            this.visuals = new VisualCollection(this);
            this.visuals.Add(this.chrome);
        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            this.chrome.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }
    }
}
