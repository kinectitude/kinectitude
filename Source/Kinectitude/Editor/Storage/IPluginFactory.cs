using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Models.Plugins;
using Action = Kinectitude.Editor.Models.Plugins.Action;

namespace Kinectitude.Editor.Storage
{
    public interface IPluginFactory : IPluginNamespace
    {
        //Component CreateComponent(string type);
        //Event CreateEvent(string type);
        //Action CreateAction(string type);
    }
}
