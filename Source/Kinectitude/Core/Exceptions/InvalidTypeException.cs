using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Exceptions
{
    internal sealed class InvalidTypeException : Exception
    {
        internal InvalidTypeException() :
            base("Changes can only be subscribed to for entities, or constants") { }
    }
}
