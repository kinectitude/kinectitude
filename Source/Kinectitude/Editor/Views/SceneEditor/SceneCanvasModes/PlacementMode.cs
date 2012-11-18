using Kinectitude.Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace Kinectitude.Editor.Views
{
    internal sealed class PlacementMode : AbstractMode
    {
        private readonly Entity entity;
        private EntityPreviewAdorner adorner;

        public PlacementMode(Entity entity, SceneCanvas canvas)
            : base(canvas)
        {
            this.entity = entity;
        }

        public override void Initialize()
        {
            

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this.SceneCanvas);
            if (null != layer)
            {
                adorner = new EntityPreviewAdorner(this.SceneCanvas, entity);
                layer.Add(adorner);
            }
        }

        public override void Uninitialize()
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this.SceneCanvas);
            if (null != layer && null != adorner)
            {
                layer.Remove(adorner);
            }
        }

        //public override void OnMouseDown(System.Windows.Input.MouseEventArgs e)
        //{


        //    this.SceneCanvas.PopMode();

        //    e.Handled = true;
        //}
    }
}
