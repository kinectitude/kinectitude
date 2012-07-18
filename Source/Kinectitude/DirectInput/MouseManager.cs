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

    public enum Button {Left = 0, Right, Scroll, Other};

    public class MouseManager : Manager<MouseFollowComponent>
    {
        private static Mouse mouse;
        private static DirectInputService service;

        private readonly List<MouseClickEvent> mouseClickEvens = new List<MouseClickEvent>();

        public MouseManager()
        {
            if (mouse == null)
            {
                service = GetService<DirectInputService>();
                mouse = new Mouse(service.DirectInput);
                service.InitDevice<Mouse>(mouse);
            }
            Cursor.Hide();
        }

        public override void OnUpdate(float frameDelta)
        {
            if (mouse.Acquire().IsFailure || mouse.Poll().IsFailure) return;
            MouseState state = mouse.GetCurrentState();
            if (Result.Last.IsFailure) return;

            foreach (MouseFollowComponent mfc in Children)
            {
                mfc.UpdateDelta(state.X, state.Y);
                mfc.OnUpdate(frameDelta);
            }

            bool [] buttonsPressed = state.GetButtons();
            for (int i = 0; i < 3 && i < buttonsPressed.Length; i++)
            {
                if (!buttonsPressed[i]) continue;
                foreach (MouseClickEvent clickEvent in mouseClickEvens.Where
                    (input => (int)input.Button == i || input.Button == Button.Other && i == input.ButtonNumber))
                {
                    clickEvent.DoActions();
                }
            }

            for (int i = 3; i < buttonsPressed.Length; i++)
            {
                if (!buttonsPressed[i]) continue;
                foreach (MouseClickEvent clickEvent in mouseClickEvens.Where
                    (input => input.Button == Button.Other &&  i == input.ButtonNumber))
                {
                    clickEvent.DoActions();
                }
            }

        }

        public void RegisterMouseClick(MouseClickEvent evt)
        {
            mouseClickEvens.Add(evt);
        }

        public void RemoveMouseClick(MouseClickEvent evt)
        {
            mouseClickEvens.Remove(evt);
        }
    }
}
