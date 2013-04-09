//-----------------------------------------------------------------------
// <copyright file="PluginSelection.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------


namespace Kinectitude.Editor.Models.Transactions
{
    internal sealed class PluginSelection
    {
        public Plugin Plugin { get; private set; }
        public bool IsRequired { get; private set; }

        public PluginSelection(Plugin plugin, bool required)
        {
            Plugin = plugin;
            IsRequired = required;
        }
    }
}
