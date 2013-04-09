//-----------------------------------------------------------------------
// <copyright file="QuitAction.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Actions
{
    [Plugin("quit game", "Quit game")]
    public class QuitAction : Action
    {
        public override void Run()
        {
            Game.CurrentGame.Quit();
        }
    }
}
