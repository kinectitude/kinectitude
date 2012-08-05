using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;

namespace Kinectitude.DirectInput
{
    public interface IKeyChange
    {
        KeyState KeyState { get;}
        Key Button { get; set; }
        void DoActions();
    }
}
