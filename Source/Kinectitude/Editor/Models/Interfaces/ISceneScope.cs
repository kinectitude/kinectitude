using System.Collections.Generic;

namespace Kinectitude.Editor.Models.Interfaces
{
    interface ISceneScope : IScope, IEntityNamespace, IPluginNamespace, IDataContainer
    {
        IEnumerable<Entity> Prototypes { get; }
    }
}
