using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Base
{

    internal enum EType { Create, Pause, Resume }

    internal class TimerEvt
    {
        internal string Name { get; private set; }
        internal EType Type { get; private set; }
        internal Timer Timer { get; private set; }

        internal TimerEvt(EType type, string name, Timer timer = null)
        {
            Name = name;
            Type = type;
            Timer = timer;
        }
    }
}
