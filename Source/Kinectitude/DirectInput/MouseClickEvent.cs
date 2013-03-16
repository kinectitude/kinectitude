using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;
using System.Windows.Forms;

namespace Kinectitude.Input
{
    [Plugin("when mouse button {Button} is clicked", "Mouse Event")]
    public class MouseClickEvent : Event
    {
        private MouseManager mouseManager;

        [PluginProperty("Button", "Standard button to check", MouseButtons.Left)]
        public MouseButtons Button { get; set; }

        public override void OnInitialize()
        {
            mouseManager = GetManager<MouseManager>();
            mouseManager.RegisterMouseClick(this);
        }

        public override void Destroy()
        {
            mouseManager.RemoveMouseClick(this);
        }
    }
}
