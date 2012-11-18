using Kinectitude.Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace Kinectitude.Editor.Views
{
    internal sealed class EntityPreviewAdorner : Adorner
    {
        private readonly Entity entity;

        public EntityPreviewAdorner(SceneCanvas canvas, Entity entity) : base(canvas)
        {
            this.entity = entity;
        }
    }
}
