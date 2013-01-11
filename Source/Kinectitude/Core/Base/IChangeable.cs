using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Base
{
    internal interface IChangeable
    {
        object this[string parameter] { get; }

        bool ShouldCheck { get; set; }
    }
}
