using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kinectitude.Input
{
    public interface IKeyChange
    {
        KeyState KeyState { get; }
        Keys Key { get; set; }
        void DoActions();
    }
}
