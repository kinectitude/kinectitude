
using System.Collections.Generic;
namespace Kinectitude.Editor.Models.Interfaces
{
    interface ISceneScope : IScope, IEntityNamespace, IPluginNamespace
    {
        IEnumerable<Entity> Prototypes { get; }
    }
}
