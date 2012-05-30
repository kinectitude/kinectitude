using System;
using Kinectitude.Core.Exceptions;
using Kinectitude.Core.Data;
using Kinectitude.Core.Base;
using System.Collections.Generic;

namespace Kinectitude.Core.Data
{
    internal sealed class NormalExpressionReader : ExpressionReader
    {
        private readonly string value;

        internal TypeMatcher ReadableSelector { get; private set; }

        private Entity entity;
        private Tuple<DataContainer, string, Action<string>> change;

        private readonly List<Action<string>> callbacks = new List<Action<string>>();

        internal NormalExpressionReader(string value, TypeMatcher readableSelector, Entity entity)
        {
            ReadableSelector = readableSelector;
            this.value = value;
            this.entity = entity;
        }
        
        public override string GetValue()
        {
            return ReadableSelector[value];
        }

        internal void changedDataContainer(DataContainer dataContainer)
        {
            foreach (Action<string> callback in callbacks)
            {
                ReadableSelector.OldDataContainer.StopNotifications(value, callback);
                ReadableSelector.DataContainer.NotifyOfChange(value, callback);
                if (ReadableSelector[value] != ReadableSelector.OldDataContainer[value])
                {
                    callback(ReadableSelector[value]);
                }

                entity.Changes.Remove(change);
                change = new Tuple<DataContainer, string, Action<string>>(ReadableSelector.DataContainer, value, callback);
                entity.Changes.Add(change);
            }
        }

        public override void notifyOfChange(Action<string> callback)
        {
            ReadableSelector.NotifyOfChange(changedDataContainer);
            ReadableSelector.DataContainer.NotifyOfChange(value, callback);
            change = new Tuple<DataContainer, string, Action<string>>(ReadableSelector.DataContainer, value, callback);
            entity.Changes.Add(change);
        }
    }
}
