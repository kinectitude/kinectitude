using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Exceptions
{
    public class InvalidAttributeException : Exception
    {
        internal InvalidAttributeException(string attribute, string referedName) :
            base("Attribute " + attribute + " does not exist in " + referedName) { }
    }
}
