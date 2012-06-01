using System;

namespace Kinectitude.Editor.Models.Base
{
    internal sealed class PluginNotLoadedException : Exception
    {
        private readonly string type;

        public PluginNotLoadedException(string type)
        {
            this.type = type;
        }
    }
}
