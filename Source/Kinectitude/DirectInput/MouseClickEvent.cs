using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Attributes;

namespace Kinectitude.DirectInput
{
    [Plugin("Mouse clicking event","")]
    public class MouseClickEvent : Event
    {

        private MouseManager mouseManager;

        [Plugin("Button to create action with", "")]
        public Button Button { get; set; }

        [Plugin("Button number for other buttons", "If button type is other, the button with id ButtonNumber is used")]
        public int ButtonNumber { get; set; }

        public override void OnInitialize()
        {
            mouseManager = GetManager<MouseManager>();
            mouseManager.RegisterMouseClick(this);
        }

        public override void Destroy()
        {
            mouseManager.RegisterMouseClick(this);
        }
    }
}
