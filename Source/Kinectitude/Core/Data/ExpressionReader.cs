﻿using System;
using System.Linq;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Core.Exceptions;

namespace Kinectitude.Core.Data
{
    internal abstract class ExpressionReader : IExpressionReader
    {
        internal static ExpressionReader CreateExpressionReader(string value, Event evt, Entity entity)
        {
            double d;
            if (!value.Contains('.') || double.TryParse(value, out d))
            {
                return new ConstantExpressionReader(value);
            }
            string[] vals = value.Split('.');
            if ('!' == value[0])
            {
                if (evt == null)
                {
                    throw new IllegalPlacementException("!", "events or actions");
                }
                return new RegularExpressionReader(vals[1], TypeMatcher.CreateTypeMatcher(value, evt, entity));
            }
            return new RegularExpressionReader(vals[1], TypeMatcher.CreateTypeMatcher(vals[0], evt, entity));
        }

        public abstract string GetValue();

        public abstract void notifyOfChange(Action<string, string> callback);

    }
}