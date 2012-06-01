using System.Collections.Generic;

namespace Kinectitude.Editor.Models.Base
{
    internal interface IActionContainer
    {
        IEnumerable<IAction> Actions { get; }

        void AddAction(IAction action);
        void RemoveAction(IAction action);
    }
}
