
using System.Collections.Generic;
namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface IEntityScope : IScope, IEntityNamespace, IPluginNamespace
    {
        IEnumerable<Entity> Prototypes { get; }
    }
}
