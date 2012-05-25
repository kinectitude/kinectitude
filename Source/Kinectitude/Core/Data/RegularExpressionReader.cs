using System;
using Kinectitude.Core.Exceptions;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Data
{
    internal sealed class RegularExpressionReader : ExpressionReader
    {
        private readonly string value;

        public TypeMatcher ReadableSelector { get; private set; }

        internal RegularExpressionReader(string value, TypeMatcher readableSelector)
        {
            ReadableSelector = readableSelector;
            this.value = value;
        }
        
        public override string GetValue()
        {
            return ReadableSelector.DataContainer[value];
        }

        public override void notifyOfChange(Action<string, string> callback)
        {
            if (typeof(SingleTypeMatcher) == ReadableSelector.GetType())
            {
                ReadableSelector.DataContainer.notifyOfChange(value, callback);
            }
            else
            {
                //TODO notify them if the expression changes in any way?
                throw new InvalidTypeException();
            }
        }
    }
}
