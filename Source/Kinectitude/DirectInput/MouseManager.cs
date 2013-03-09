using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using SlimDX.DirectInput;
using SlimDX;
using System.Windows.Forms;
using Kinectitude.Core.Attributes;

namespace Kinectitude.DirectInput
{
    public enum Button {Left = 0, Right, Scroll, Other};

    [Plugin("Mouse Manager", "")]
    public class MouseManager : Manager<MouseFollowComponent>
    {
        private static Mouse mouse;
        private static DirectInputService service;

        private readonly List<MouseClickEvent> mouseClickEvents = new List<MouseClickEvent>();

        private bool visibleCursor = true;
        [PluginProperty("Show Cursor", "", true)]
        public bool VisibleCursor
        {
            get { return visibleCursor; }
            set
            {
                if (visibleCursor != value)
                {
                    visibleCursor = value;
                    Change("VisibleCursor");
                    if (value) Cursor.Show();
                    else Cursor.Hide();
                }
            }
        }

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
                //mfc.UpdateDelta(state.X, state.Y);
                int x = Cursor.Position.X;
                int y = Cursor.Position.Y;
                OffsetByWindow(ref x, ref y);
                mfc.UpdatePosition(ScaleX(x), ScaleY(y));
                mfc.OnUpdate(frameDelta);
            }

            if (mouse.Acquire().IsFailure || mouse.Poll().IsFailure) return;
            MouseState state = mouse.GetCurrentState();
            if (Result.Last.IsFailure) return;

            bool [] buttonsPressed = state.GetButtons();
            for (int i = 0; i < 3 && i < buttonsPressed.Length; i++)
            {
                if (!buttonsPressed[i]) continue;
                foreach (MouseClickEvent clickEvent in mouseClickEvents.Where
                    (input => (int)input.Button == i || input.Button == Button.Other && i == input.ButtonNumber))
                {
                    clickEvent.DoActions();
                }
            }

            for (int i = 3; i < buttonsPressed.Length; i++)
            {
                if (!buttonsPressed[i]) continue;
                foreach (MouseClickEvent clickEvent in mouseClickEvents.Where
                    (input => input.Button == Button.Other &&  i == input.ButtonNumber))
                {
                    clickEvent.DoActions();
                }
            }

        }

        public void RegisterMouseClick(MouseClickEvent evt)
        {
            mouseClickEvents.Add(evt);
        }

        public void RemoveMouseClick(MouseClickEvent evt)
        {
            mouseClickEvents.Remove(evt);
        }
    }
}
