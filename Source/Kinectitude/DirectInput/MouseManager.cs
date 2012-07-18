using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using SlimDX.DirectInput;
using SlimDX;
using System.Windows.Forms;

namespace Kinectitude.DirectInput
{
    public class MouseManager : Manager<MouseFollowComponent>
    {

        private static Mouse mouse;
        private static DirectInputService service;

        public MouseManager()
        {
            if (mouse == null)
            {
                service = GetService<DirectInputService>();
                mouse = new Mouse(service.DirectInput);
                service.InitDevice<Mouse>(mouse);
            }
        }

        public override void OnUpdate(float frameDelta)
        {
            foreach (MouseFollowComponent mfc in Children)
            {
                mfc.UpdatePosition(Cursor.Position.X, Cursor.Position.Y);
                mfc.OnUpdate(frameDelta);
            }
        }
    }
}
