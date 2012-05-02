using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Plugins
{
    public class Action : Plugin, IAction
    {
        private IActionContainer parent;

        public IActionContainer Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public Action(PluginDescriptor descriptor) : base(descriptor) { }
    }
}
