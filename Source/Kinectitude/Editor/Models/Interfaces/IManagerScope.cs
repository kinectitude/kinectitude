//-----------------------------------------------------------------------
// <copyright file="IManagerScope.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------


namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface IManagerScope : IScope, IPluginNamespace
    {
        bool RequiresManager(Manager manager);
    }
}
