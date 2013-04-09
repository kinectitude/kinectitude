//-----------------------------------------------------------------------
// <copyright file="ISceneScope.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Kinectitude.Editor.Models.Interfaces
{
    interface ISceneScope : IScope, IEntityNamespace, IPluginNamespace
    {
        double Width { get; }
        double Height { get; }

        IEnumerable<Entity> Prototypes { get; }

        bool HasSceneWithName(string name);
    }
}
