using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public class PluginNotLoadedException : Exception
    {
        private string type;

        public PluginNotLoadedException(string type)
        {
            this.type = type;
        }
    }
}
