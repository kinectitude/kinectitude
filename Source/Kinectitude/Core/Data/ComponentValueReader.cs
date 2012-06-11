using System;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class ComponentValueReader : ExpressionReader
    {
        private readonly TypeMatcher matcher;
        private readonly string componentName;
        private readonly string getterName;

        internal ComponentValueReader(string [] values, TypeMatcher readableSelector)
        {
            matcher = readableSelector;
            componentName = values[0];
            getterName = values[1];
        }

        public override string GetValue()
        {
            Entity entity = matcher.DataContainer as Entity;
            Component component = entity.GetComponent(componentName);
            return ClassFactory.GetStringParam(component, getterName);
        }

        public override void notifyOfChange(Action<string> callback)
        {
            throw new NotImplementedException();
        }
    }
}
