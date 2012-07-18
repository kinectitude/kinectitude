using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.Commands
{
    internal class DelegateUndoableCommand : IUndoableCommand
    {
        private readonly Action executeDelegate;
        private readonly Action unexecuteDelegate;

        public string Name
        {
            get;
            private set;
        }

        public DelegateUndoableCommand(string name, Action executeDelegate, Action unexecuteDelegate)
        {
            Name = name;

            this.executeDelegate = executeDelegate;
            this.unexecuteDelegate = unexecuteDelegate;
        }

        public void Execute()
        {
            if (null != executeDelegate)
            {
                executeDelegate();
            }
        }

        public void Unexecute()
        {
            if (null != unexecuteDelegate)
            {
                unexecuteDelegate();
            }
        }
    }
}
