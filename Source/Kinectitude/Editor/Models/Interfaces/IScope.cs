using System.Collections.Generic;

namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface IScope : INotify
    {
        IScope Parent { get; }
        IList<GameModel> Children { get; }
    }
}
