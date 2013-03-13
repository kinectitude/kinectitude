using System.Collections.Generic;

namespace Kinectitude.Editor.Models.Interfaces
{
    interface ISceneScope : IScope, IEntityNamespace, IPluginNamespace
    {
        double Width { get; }
        double Height { get; }

        IEnumerable<Entity> Prototypes { get; }

        bool HasSceneWithName(string name);
    }
}
