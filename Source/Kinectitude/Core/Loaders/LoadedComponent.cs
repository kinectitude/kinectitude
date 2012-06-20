using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Loaders
{
    internal class LoadedComponent : LoadedObject
    {
        internal Type ComponentType;
        internal string Name;

        internal LoadedComponent(List<Tuple<string, string>> values) : base(values) { }

    }
}
