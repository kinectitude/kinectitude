using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Exceptions
{
    internal sealed class IllegalPlacementException : Exception
    {
        internal IllegalPlacementException(string type, string allowedIn)
            : base(type + ", is only allowed in " + allowedIn)
        {
        }
    }
}
