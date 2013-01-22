using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Base
{
    internal interface IScene : IDataContainer
    {
        IDataContainer Game { get; }
        IDataContainer GetEntity(string name);
        HashSet<int> GetOfPrototype(string prototype, bool exact);
    }
}
