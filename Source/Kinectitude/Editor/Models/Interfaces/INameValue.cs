using Kinectitude.Editor.Models.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface INameValue
    {
        string Name { get; }
        Value Value { get; }

        bool IsEditable { get; }
        bool HasOwnValue { get; }
        bool HasFileChooser { get; }

        ICommand ClearValueCommand { get; }
    }
}
