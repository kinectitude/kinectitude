//-----------------------------------------------------------------------
// <copyright file="IStatementScope.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models.Statements.Base;

namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface IStatementScope : IScope, IPluginNamespace
    {
        bool ShouldCopyStatement { get; }

        int IndexOf(AbstractStatement statement);
        void RemoveStatement(AbstractStatement statement);
        void InsertAt(int idx, AbstractStatement statement);
        void InsertBefore(AbstractStatement statement, AbstractStatement toInsert);
    }
}
