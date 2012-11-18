using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Kinectitude.Editor.Views
{
    internal class AbstractMode
    {
        private readonly SceneCanvas canvas;

        protected SceneCanvas SceneCanvas
        {
            get { return canvas; }
        }

        protected AbstractMode(SceneCanvas canvas)
        {
            this.canvas = canvas;
        }

        public virtual void Initialize() {}
        public virtual void Pause() {}
        public virtual void Resume() {}
        public virtual void Uninitialize() {}

        public virtual void OnMouseDown(MouseEventArgs e) { }
        public virtual void OnMouseMove(MouseEventArgs e) { }

        public virtual void OnEntityMouseDown(EntityItemEventArgs e) { }
        public virtual void OnEntityMouseUp(EntityItemEventArgs e) { }
    }
}
