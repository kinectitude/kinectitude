//-----------------------------------------------------------------------
// <copyright file="DelegateUndoableCommand.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Kinectitude.Editor.Commands
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
