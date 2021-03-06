//-----------------------------------------------------------------------
// <copyright file="ICommandHistory.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Kinectitude.Editor.Commands
{
    internal interface ICommandHistory : INotifyPropertyChanged
    {
        ObservableCollection<IUndoableCommand> UndoableCommands { get; }
        ObservableCollection<IUndoableCommand> RedoableCommands { get; }

        IUndoableCommand LastUndoableCommand { get; }
        IUndoableCommand LastRedoableCommand { get; }

        ICommand UndoCommand { get; }
        ICommand RedoCommand { get; }

        bool HasUnsavedChanges { get; }

        void Undo();
        void Redo();
        void Clear();

        void Log(IUndoableCommand command);
        void Log(string name, Action executeDelegate, Action unexecuteDelegate);
        void WithoutLogging(Action action);
        void Save();
    }
}
