using System;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class ParameterValueReader : ExpressionReader
    {

        Event evt;
        string param;

        internal ParameterValueReader(Event evt, string value)
        {
            this.evt = evt;
            param = value;
        }

        public override string GetValue()
        {
            return ClassFactory.GetStringParam(evt, param);
        }

        public override void notifyOfChange(Action<string> callback)
        {
            throw new NotImplementedException();
        }
    }
}
