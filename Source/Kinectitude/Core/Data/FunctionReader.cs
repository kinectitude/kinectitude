using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal abstract class FunctionReader : ValueReader
    {
        private readonly PreferedType PreferedType;
        private readonly List<ValueReader> Args;

        internal static FunctionReader getFunctionReader(string name, List<ValueReader> parameters)
        {
            FunctionHolder fh = FunctionHolder.getFunctionHolder(name);

            Tuple<Func<ValueReader[], object>, Type> callInfo = fh.GetExactMatch(parameters.Count);
            if(null != callInfo)
            {
                return new BasicFunctionReader(parameters, callInfo.Item1, callInfo.Item2);

            }
            Tuple<int, Func<ValueReader[],  ValueReader[], object>, Type> paramCall = fh.GetParamsMatch(parameters.Count);
            return new ParamsFunctionReader(paramCall.Item1, parameters, paramCall.Item3, paramCall.Item2);
        }

        protected FunctionReader(List<ValueReader> args, Type ret)
        {
            PreferedType = NativeReturnType(ret);
            Args = args;
        }

        internal override PreferedType PreferedRetType() { return PreferedType; }
        internal override ValueWriter ConvertToWriter() { return new NullWriter(this); }
        internal override void SetupNotifications()
        {
            foreach (ValueReader vr in Args) vr.NotifyOfChange(this);
        }
    }
}
