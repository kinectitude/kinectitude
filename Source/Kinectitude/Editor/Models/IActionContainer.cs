using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Action = Kinectitude.Editor.Models.Plugins.Action;

namespace Kinectitude.Editor.Models
{
    public interface IActionContainer
    {
        IEnumerable<IAction> Actions { get; }

        void AddAction(IAction action);
        void RemoveAction(IAction action);
    }
}
