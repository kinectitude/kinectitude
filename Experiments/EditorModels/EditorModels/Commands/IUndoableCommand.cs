using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.Commands
{
    internal interface IUndoableCommand
    {
        string Name { get; }

        void Execute();
        void Unexecute();
    }
}
