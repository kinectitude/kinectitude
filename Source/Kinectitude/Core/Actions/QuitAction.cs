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
