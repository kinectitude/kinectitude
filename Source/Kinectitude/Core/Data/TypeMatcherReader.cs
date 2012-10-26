using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class TypeMatcherReader : RepeatReader
    {
        internal readonly TypeMatcher Matcher;
        private readonly string Property;

        internal TypeMatcherReader(TypeMatcher matcher, string property)
        {
            if (Matcher.DataContainer != null) Reader = Matcher.DataContainer[property];
            Matcher = matcher;
            Property = property;
            Matcher.NotifyOfChange(change);
        }

        internal override void notifyOfChange(Action<ValueReader> change)
        {
            Callbacks.Add(change);
            Matcher.DataContainer.NotifyOfChange(Property, change);
        }

        private void change(ValueReader value)
        {
            if (value != Reader)
            {
                foreach (Action<ValueReader> callback in Callbacks) callback(value);
            }
            Reader = value;
        }

        private void change(DataContainer dataContainer)
        {
            change(dataContainer[Property]);
            if (null != Writer)
            {
                DataContainerWriter writer = Writer as DataContainerWriter;
                writer.DataContainer = dataContainer;
            }
        }

        internal override ValueWriter ConvertToWriter()
        {
            DataContainerWriter dataContainerWriter = new DataContainerWriter(Property, this);
            if(null != Matcher) dataContainerWriter.DataContainer = Matcher.DataContainer;
            return dataContainerWriter;
        }
    }
}
