using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface IValueScope : IScope
    {
        Entity Entity { get; }
        Scene Scene { get; }
        Game Game { get; }
    }
}
