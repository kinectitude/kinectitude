using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Editor.Commands.Base
{
    public interface IUndoableCommand
    {
        string Name { get; }

        void Execute();
        void Unexecute();
    }
}
