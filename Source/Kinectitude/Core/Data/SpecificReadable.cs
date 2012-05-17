﻿using System;
using System.Linq;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    public abstract class SpecificReadable
    {
        internal static SpecificReadable CreateSpecificReadable(string value, Event evt, Scene scene)
        {
            double d;
            if (!value.Contains('.') || double.TryParse(value, out d))
            {
                return new ConstantReadable(value);
            }
            string[] vals = value.Split('.');
            if ('!' == value[0])
            {
                if (evt == null)
                {
                    throw new ArgumentException("Can't parse " + value);
                }
                return new SpecificContainerReadable(vals[1], ReadableData.CreateReadableData(value, evt, scene));
            }
            Entity entity = evt.Entity;
            return new SpecificContainerReadable(vals[1], ReadableData.CreateReadableData(vals[0], evt, scene));
        }

        public abstract string GetValue();
    }
}