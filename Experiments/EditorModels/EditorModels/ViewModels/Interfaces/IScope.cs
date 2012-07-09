using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.ViewModels.Interfaces
{
    internal delegate void ScopeChangedEventHandler();

    internal interface IScope
    {
        event ScopeChangedEventHandler ScopeChanged;
    }
}
