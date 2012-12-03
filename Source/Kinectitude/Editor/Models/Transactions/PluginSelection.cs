using Kinectitude.Editor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
{
    internal sealed class PluginSelection
    {
        public Plugin Plugin { get; private set; }
        public bool IsRequired { get; private set; }

        public PluginSelection(Plugin plugin, bool required)
        {
            Plugin = plugin;
            IsRequired = required;
        }
    }
}
