using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Base
{
    internal interface IChangeable
    {
        bool ShouldCheck { get; set; }
    }
}
