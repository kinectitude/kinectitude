using Kinectitude.Editor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Tests.Editor
{
    internal static class CommandHelper
    {
        public static void TestCommand(Action action, Action postconditions)
        {
            MockDialogService.Instance.Start();
            Workspace.Instance.CommandHistory.Clear();
            action();

            if (null != postconditions)
            {
                postconditions();
            }
        }

        public static void TestUndoableCommand(Action preconditions, Action action, Action postconditions, int ignoreCommands = 0)
        {
            MockDialogService.Instance.Start();
            Workspace.Instance.CommandHistory.Clear();

            if (null != preconditions)
            {
                preconditions();
            }

            action();
            AssertAfterLog(ignoreCommands);

            if (null != postconditions)
            {
                postconditions();
            }

            Workspace.Instance.CommandHistory.Undo();
            AssertAfterUndo(ignoreCommands);

            if (null != preconditions)
            {
                preconditions();
            }

            Workspace.Instance.CommandHistory.Redo();
            AssertAfterRedo(ignoreCommands);

            if (null != postconditions)
            {
                postconditions();
            }
        }

        private static void AssertAfterLog(int ignoreCommands)
        {
            Assert.AreEqual(1, Workspace.Instance.CommandHistory.UndoableCommands.Count - ignoreCommands);
            Assert.AreEqual(0, Workspace.Instance.CommandHistory.RedoableCommands.Count);
            Assert.IsTrue(ignoreCommands > 0 || null != Workspace.Instance.CommandHistory.LastUndoableCommand);
            Assert.IsNull(Workspace.Instance.CommandHistory.LastRedoableCommand);
        }

        private static void AssertAfterUndo(int ignoreCommands)
        {
            Assert.AreEqual(0, Workspace.Instance.CommandHistory.UndoableCommands.Count - ignoreCommands);
            Assert.AreEqual(1, Workspace.Instance.CommandHistory.RedoableCommands.Count);
            Assert.IsNotNull(Workspace.Instance.CommandHistory.LastRedoableCommand);
            Assert.IsTrue(ignoreCommands > 0 || null == Workspace.Instance.CommandHistory.LastUndoableCommand);
        }

        private static void AssertAfterRedo(int ignoreCommands)
        {
            AssertAfterLog(ignoreCommands);
        }
    }
}
