using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class BasicFunctionReader : FunctionReader
    {
        private readonly ValueReader[] Args;
        private readonly Func<ValueReader[], object> Function;

        internal BasicFunctionReader(List<ValueReader> args, Func<ValueReader[], object> function, Type ret) : base(args, ret)
        {
            Args = args.ToArray();
            Function = function;
        }

        internal override double GetDoubleValue() { return ToNumber<double>(Function(Args)); }
        internal override float GetFloatValue() { return ToNumber<float>(Function(Args)); }
        internal override int GetIntValue() { return ToNumber<int>(Function(Args)); }
        internal override long GetLongValue() { return ToNumber<long>(Function(Args)); }
        internal override bool GetBoolValue() { return ToBool(Function(Args)); }
        internal override string GetStrValue() { return Function(Args).ToString(); }
    }
}
