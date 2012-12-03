﻿using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Loaders
{
    internal sealed class LoadedLoop : LoadedActionable
    {
        private readonly LoadedBaseAction After;

        internal LoadedLoop(object whileVal, LoaderUtility loader, LoadedBaseAction after) : base(whileVal, loader)
        {
            After = after;
        }

        internal override void Ready()
        {
            if (After == null) return;
            Actions.Add(After);
        }

        internal override Action Create(Event evt)
        {
            ValueReader reader = LoaderUtil.MakeAssignable(ConditianlExpression, evt.Entity.Scene, evt.Entity, evt) as ValueReader;

            Loop loop = new Loop(reader);
            loop.Event = evt;

            foreach (LoadedBaseAction loadedAction in Actions)
            {
                Action action = loadedAction.Create(evt);
                loop.AddAction(action);
            }

            return loop;
        }
    }
}