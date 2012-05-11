using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Base
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
