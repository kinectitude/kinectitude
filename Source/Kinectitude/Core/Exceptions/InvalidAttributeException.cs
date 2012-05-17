using System;

namespace Kinectitude.Core.Exceptions
{
    public class InvalidAttributeException : Exception
    {
        internal InvalidAttributeException(string attribute, string referedName) :
            base("Attribute " + attribute + " does not exist in " + referedName) { }
    }
}
