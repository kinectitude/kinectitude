using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Views
{
    internal sealed class ManipulationMode : AbstractMode
    {
        private readonly IEnumerable<EntityItem> selectedItems;

        public ManipulationMode(SceneCanvas canvas, IEnumerable<EntityItem> selectedItems)
            : base(canvas)
        {
            this.selectedItems = selectedItems;
        }
    }
}
