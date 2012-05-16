using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Managers
{
    class TimeManager:Manager<Component>
    {

        public TimeManager(Game game):base(game) { }

        public override void OnUpdate(float t)
        {
            foreach (Component c in children)
            {
                c.OnUpdate(t);
            }
        }
    }
}
