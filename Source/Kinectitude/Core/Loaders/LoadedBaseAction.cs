﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Loaders
{
    internal abstract class LoadedBaseAction : LoadedObject
    {
        protected LoadedBaseAction(PropertyHolder values, LoaderUtility loaderUtil) : base(values, loaderUtil) { }
        internal abstract Action Create(Event evt);
    }
}
