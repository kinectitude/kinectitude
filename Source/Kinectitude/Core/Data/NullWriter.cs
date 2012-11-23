using Kinectitude.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    /// <summary>
    /// Used to keep the game from crashing when an invalid writer is used.
    /// </summary>
    internal sealed class NullWriter : ValueWriter
    {
        public NullWriter(ValueReader reader) : base(reader) { }
        public override void SetValue(ValueReader value) 
        {
            Game.CurrentGame.Die("The target of a write must be a single attribute or property name");
        }
    }
}
