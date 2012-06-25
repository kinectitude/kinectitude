using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.Commands
{
    internal interface IUndoableCommand
    {
        void Execute();
        void Unexecute();
    }
}
