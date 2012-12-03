using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;

namespace Kinectitude.DirectInput
{
    [Plugin("Mouse button is clicked: (Button: {Button}, Button Number: {ButtonNumber})", "")]
    public class MouseClickEvent : Event
    {
        private MouseManager mouseManager;

        [PluginProperty("Button", "Button to create evt with")]
        public Button Button { get; set; }

        [PluginProperty("Button Number", "Button number for other buttons. If button type is other, the button with id ButtonNumber is used")]
        public int ButtonNumber { get; set; }

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
