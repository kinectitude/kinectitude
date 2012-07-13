using System;
using Kinectitude.Core.Base;
using System.Collections.Generic;

namespace Kinectitude.Core.Data
{
    internal sealed class ComponentValueReader : ExpressionReader
    {
        private readonly TypeMatcher matcher;
        private readonly string componentName;
        private readonly string getterName;
        private readonly string value;
        private List<Action<string>> callbacks = new List<Action<string>>();

        internal ComponentValueReader(string [] values, TypeMatcher readableSelector)
        {
            value = values[0] + '.' + values[1];
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

        private void dataSelectorChange(DataContainer current)
        {
            foreach (Action<string> callback in callbacks)
            {
                if (matcher.OldDataContainer != null)
                {
                    matcher.OldDataContainer.UnnotifyOfComponentChange(value, callback);
                    if (matcher.OldDataContainer[value] != matcher.DataContainer[value])
                        current.NotifyOfChange(value, callback);
                }
                else current.NotifyOfChange(value, callback);
            }
        }

        public override void notifyOfChange(Action<string> callback)
        {
            matcher.NotifyOfChange(dataSelectorChange);
            if (matcher.DataContainer != null)
                matcher.DataContainer.NotifyOfComponentChange(value, callback);
            callbacks.Add(callback);
        }
    }
}
