using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Base
{
    internal interface IScene
    {
        IDataContainer Game { get; }

        IDataContainer GetEntity(string name);
    }
}
