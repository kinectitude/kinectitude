//-----------------------------------------------------------------------
// <copyright file="CommandHistory.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Base;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Kinectitude.Editor.Commands
{
    internal sealed class CommandHistory : BaseModel, ICommandHistory
    {
        private bool replay;

        public ObservableCollection<IUndoableCommand> UndoableCommands { get; private set; }
        public ObservableCollection<IUndoableCommand> RedoableCommands { get; private set; }

        public IUndoableCommand LastUndoableCommand
        {
            get { return UndoableCommands.LastOrDefault(); }
        }

        public IUndoableCommand LastRedoableCommand
        {
            get { return RedoableCommands.LastOrDefault(); }
        }

        public bool HasUnsavedChanges { get; private set; }

        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }

        public CommandHistory()
        {
            UndoableCommands = new ObservableCollection<IUndoableCommand>();
            RedoableCommands = new ObservableCollection<IUndoableCommand>();

            UndoCommand = new DelegateCommand(p => UndoableCommands.Count > 0, p => Undo());
            RedoCommand = new DelegateCommand(p => RedoableCommands.Count > 0, p => Redo());
        }

        public void Undo()
        {
            replay = true;

            if (UndoableCommands.Count > 0)
            {
                IUndoableCommand command = PopUndo();
                command.Unexecute();
                PushRedo(command);
                HasUnsavedChanges = true;
            }

            replay = false;
        }

        public void Redo()
        {
            replay = true;

            if (RedoableCommands.Count > 0)
            {
                IUndoableCommand command = PopRedo();
                command.Execute();
                PushUndo(command);
                HasUnsavedChanges = true;
            }

            replay = false;
        }

        public void Clear()
        {
            UndoableCommands.Clear();
            RedoableCommands.Clear();
            Save();

            NotifyPropertyChanged("LastUndoableCommand");
            NotifyPropertyChanged("LastRedoableCommand");
        }

        private void PushUndo(IUndoableCommand command)
        {
            UndoableCommands.Add(command);
            NotifyPropertyChanged("LastUndoableCommand");
        }

        private void PushRedo(IUndoableCommand command)
        {
            RedoableCommands.Add(command);
            NotifyPropertyChanged("LastRedoableCommand");
        }

        private IUndoableCommand PopUndo()
        {
            IUndoableCommand command = UndoableCommands.Last();
            UndoableCommands.RemoveAt(UndoableCommands.Count - 1);

            NotifyPropertyChanged("LastUndoableCommand");
            return command;
        }

        private IUndoableCommand PopRedo()
        {
            IUndoableCommand command = RedoableCommands.Last();
            RedoableCommands.RemoveAt(RedoableCommands.Count - 1);

            NotifyPropertyChanged("LastRedoableCommand");
            return command;
        }

        public void Log(IUndoableCommand command)
        {
            if (!replay)
            {
                PushUndo(command);
                RedoableCommands.Clear();
                HasUnsavedChanges = true;
            }
        }

        public void Log(string name, Action executeDelegate, Action unexecuteDelegate)
        {
            if (!replay)
            {
                Log(new DelegateUndoableCommand(name, executeDelegate, unexecuteDelegate));
            }
        }

        public void WithoutLogging(Action action)
        {
            bool oldReplay = replay;
            replay = true;

            if (null != action)
            {
                action();
            }

            replay = oldReplay;
        }

        public void Save()
        {
            HasUnsavedChanges = false;
        }
    }
}
