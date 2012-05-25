using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core
{
    public abstract class Component
    {
        public abstract void OnUpdate(double frameDelta);

        public Component() { }
    }
}
