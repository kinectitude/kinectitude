using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using System.Windows.Forms;

namespace Kinectitude.Input
{
    public delegate void MouseEventHandler(MouseButtons button, float x, float y);
    public delegate void KeyEventHandler(Keys key, int code);

    public class InputService : Service
    {
        //private static readonly CooperativeLevel CooperativeLevel = CooperativeLevel.Nonexclusive | CooperativeLevel.Foreground;

        //private readonly Dictionary<Type, Device> Devices = new Dictionary<Type, Device>();

        //public Control Control { get; set; }
        //public SlimDX.DirectInput.DirectInput DirectInput { get; private set; }

        private Form form;

        public Form Form
        {
            get { return form; }
            set
            {
                if (null == form)
                {
                    form = value;
                    form.MouseMove += OnMouseMove;
                    form.MouseClick += OnMouseClick;
                    form.KeyDown += OnKeyDown;
                    form.KeyUp += OnKeyUp;
                }
            }
        }

        public event Kinectitude.Input.MouseEventHandler MouseMove;
        public event Kinectitude.Input.MouseEventHandler MouseClick;
        public event Kinectitude.Input.KeyEventHandler KeyDown;
        public event Kinectitude.Input.KeyEventHandler KeyUp;

        public override void OnStart() 
        {
            //DirectInput = new SlimDX.DirectInput.DirectInput();
        }

        public override void OnStop() 
        {
            //foreach (KeyValuePair<Type, Device> devicePair in Devices)
            //{
            //    devicePair.Value.Unacquire();
            //    devicePair.Value.Dispose();
            //}
            //DirectInput.Dispose();
        }

        public override bool AutoStart()
        {
            return true;
        }

        //public void InitDevice<T>(T device) where T : Device
        //{
        //    Devices[typeof(T)] = device;
        //    device.SetCooperativeLevel(Control, CooperativeLevel);
        //    device.Acquire();
        //}

        //public T RemoveDevice<T>() where T : Device
        //{
        //    T device =  Devices[typeof(T)] as T;
        //    if (null == device) return null;
        //    Devices.Remove(typeof(T));
        //    return device;
        //}

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (null != MouseMove)
            {
                MouseMove(MouseButtons.None, e.X, e.Y);
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (null != MouseClick)
            {
                MouseClick(e.Button, e.X, e.Y);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (null != KeyDown)
            {
                KeyDown(e.KeyCode, e.KeyValue);
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (null != KeyUp)
            {
                KeyUp(e.KeyCode, e.KeyValue);
            }
        }
    }
}
