using System;
using System.Linq;
using Kinectitude.Core.Base;
using Kinectitude.Core.Exceptions;

namespace Kinectitude.Core.Data
{
    internal abstract class ExpressionReader : IExpressionReader
    {
        internal static ExpressionReader CreateExpressionReader(string value, Event evt, Entity entity)
        {
            double d;
            if (value.Contains('\\') || '!' != value[0] && !value.Contains('.') || double.TryParse(value, out d))
            {
                value = value.Replace("\\\\", "\\").Replace("\\!", "!")
                    .Replace("\\.", ".").Replace("\\{", "{").Replace("\\}","}");
                return new ConstantExpressionReader(value);
            }
            string[] vals = value.Split('.');
            if ('!' == value[0])
            {
                if (evt == null)
                {
                    throw new IllegalPlacementException("!", "events or actions");
                }
            }
            switch(vals.Length)
            {
                case 1:
                    return new ParameterValueReader(evt, value.Substring(1));
                case 2:
                    return new EntityValueReader(vals[1], TypeMatcher.CreateTypeMatcher(vals[0], evt, entity), entity);
                case 3:
                    string[] part = new string [] { vals[1], vals[2] };
                    return new ComponentValueReader(part, TypeMatcher.CreateTypeMatcher(vals[0], evt, entity));
                default:
                    //TODO make this a better exception
                    throw new ArgumentException("Invalid reader");
            }

        }

        public abstract string GetValue();

        public abstract void notifyOfChange(Action<string> callback);

    }
}