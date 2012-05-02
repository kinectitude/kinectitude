using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
{
    public class PluginNotLoadedException : Exception
    {
        private readonly string type;

        public PluginNotLoadedException(string type)
        {
            this.type = type;
        }
    }
}
