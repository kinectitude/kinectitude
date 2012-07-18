using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using SlimDX.DirectInput;
using System.Windows.Forms;

namespace Kinectitude.DirectInput
{
    public class DirectInputService : Service
    {
        private static readonly CooperativeLevel CooperativeLevel = CooperativeLevel.Nonexclusive | CooperativeLevel.Foreground;

        private readonly Dictionary<Type, Device> Devices = new Dictionary<Type, Device>();

        public Control Control { get; set; }
        public SlimDX.DirectInput.DirectInput DirectInput { get; private set; }

        public override void OnStart() 
        {
            DirectInput = new SlimDX.DirectInput.DirectInput();
        }

        public override void OnStop() 
        {
            foreach (KeyValuePair<Type, Device> devicePair in Devices)
            {
                devicePair.Value.Unacquire();
                devicePair.Value.Dispose();
            }
            DirectInput.Dispose();
        }

        public override bool AutoStart()
        {
            return true;
        }

        public void InitDevice<T>(T device) where T : Device
        {
            Devices[typeof(T)] = device;
            device.SetCooperativeLevel(Control, CooperativeLevel);
            device.Acquire();
        }

        public T RemoveDevice<T>() where T : Device
        {
            T device =  Devices[typeof(T)] as T;
            if (null == device) return null;
            Devices.Remove(typeof(T));
            return device;
        }
    }
}
