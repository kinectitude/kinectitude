using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class ParamsFunctionReader : FunctionReader
    {
        private readonly ValueReader[] Args;
        private readonly ValueReader[] Params;
        private readonly Func<ValueReader[], ValueReader[], object> Function;

        internal ParamsFunctionReader(int numArgs, List<ValueReader> args, Type ret, 
            Func<ValueReader[], ValueReader[], object> function): base(args, ret)
        {
            Args = args.GetRange(0, numArgs).ToArray();
            Params = args.GetRange(numArgs, args.Count - numArgs).ToArray();
            Function = function;
        }

        internal override double GetDoubleValue() { return ToNumber<double>(Function(Args, Params)); }
        internal override float GetFloatValue() { return ToNumber<float>(Function(Args, Params)); }
        internal override int GetIntValue() { return ToNumber<int>(Function(Args, Params)); }
        internal override long GetLongValue() { return ToNumber<long>(Function(Args, Params)); }
        internal override bool GetBoolValue() { return ToBool(Function(Args, Params)); }
        internal override string GetStrValue() { return Function(Args, Params).ToString(); }
    }
}
