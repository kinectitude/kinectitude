using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal interface IChangeable
    {
        void Prepare();
        void Change();
    }
}
