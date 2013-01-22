using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    public interface IChanges
    {
        void Prepare();
        void Change();
    }
}
