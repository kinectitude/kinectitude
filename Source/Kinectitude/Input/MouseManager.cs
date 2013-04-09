//-----------------------------------------------------------------------
// <copyright file="MouseManager.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using System.Windows.Forms;
using Kinectitude.Core.Attributes;
using Kinectitude.Render;
using System.Drawing;

namespace Kinectitude.Input
{
    [Plugin("Mouse Manager", "")]
    public class MouseManager : Manager<MouseFollowComponent>
    {
        //private static Mouse mouse;
        private RenderService renderService;
        private InputService inputService;
        private PointF currentPoint;

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
            //if (mouse == null)
            //{
            //    service = GetService<InputService>();
            //    mouse = new Mouse(service.DirectInput);
            //    service.InitDevice<Mouse>(mouse);
            //}
        }

        protected override void OnStart()
        {
            if (null == renderService)
            {
                renderService = GetService<RenderService>();
            }

            if (null == inputService)
            {
                inputService = GetService<InputService>();
                inputService.MouseMove += OnMouseMove;
                inputService.MouseClick += OnMouseClick;
            }
        }

        protected override void OnStop()
        {
            inputService.MouseMove -= OnMouseMove;
            inputService.MouseClick -= OnMouseClick;
        }

        private void OnMouseMove(MouseButtons button, float x, float y)
        {
            currentPoint = renderService.ConvertPixelsToDips(new PointF(x, y));
        }

        private void OnMouseClick(MouseButtons button, float x, float y)
        {
            foreach (var evt in mouseClickEvents.Where(e => e.Button == button))
            {
                evt.DoActions();
            }
        }

        public override void OnUpdate(float frameDelta)
        {
            foreach (MouseFollowComponent mfc in Children)
            {
                //mfc.UpdateDelta(state.X, state.Y);
                //int x = Cursor.Position.X;
                //int y = Cursor.Position.Y;
                //OffsetByWindow(ref x, ref y);
                //mfc.UpdatePosition(ScaleX(x), ScaleY(y));
                //mfc.OnUpdate(frameDelta);

                mfc.UpdatePosition(currentPoint.X, currentPoint.Y);
                mfc.OnUpdate(frameDelta);
            }

            //if (mouse.Acquire().IsFailure || mouse.Poll().IsFailure) return;
            //MouseState state = mouse.GetCurrentState();
            //if (Result.Last.IsFailure) return;

            //bool [] buttonsPressed = state.GetButtons();
            //for (int i = 0; i < 3 && i < buttonsPressed.Length; i++)
            //{
            //    if (!buttonsPressed[i]) continue;
            //    //foreach (MouseClickEvent clickEvent in mouseClickEvents.Where
            //    //    (input => (int)input.Button == i || input.Button == Button.Other && i == input.ButtonNumber))
            //    //{
            //    //    clickEvent.DoActions();
            //    //}
            //}

            //for (int i = 3; i < buttonsPressed.Length; i++)
            //{
            //    if (!buttonsPressed[i]) continue;
            //    foreach (MouseClickEvent clickEvent in mouseClickEvents.Where
            //        (input => input.Button == Button.Other &&  i == input.ButtonNumber))
            //    {
            //        clickEvent.DoActions();
            //    }
            //}
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
