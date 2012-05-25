using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Exceptions
{
    internal sealed class NoSuchPrototypeException : Exception
    {
        internal NoSuchPrototypeException(string prototype) : base("Prototype " + prototype + " does not exist") { }
    }
}
