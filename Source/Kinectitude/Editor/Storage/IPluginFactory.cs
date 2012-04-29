using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public interface IPluginFactory
    {
        Component CreateComponent(string type);
        Event CreateEvent(string type);
        Action CreateAction(string type);
    }
}
