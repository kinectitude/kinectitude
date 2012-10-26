using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Exceptions
{
    internal sealed class InvalidValueWriterException : Exception
    {
        internal InvalidValueWriterException()
            : base("ValueWriter can only be assigned to this.<attribute>, Scene.<attribute>, " +
                "Game.<attribute> Or <attribute>") { }
    }
}
