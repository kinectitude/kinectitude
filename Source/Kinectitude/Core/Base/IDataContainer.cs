using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Base
{
    public interface IDataContainer
    {
        int Id { get; }
        string Name { get; }
    }
}
