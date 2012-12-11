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
        /*If they change the order in the function call, it will change the value here.
         * This is not what we want, so allocate an array ahead of time so one is not always created for each copy.
         */
        private readonly ValueReader[] ParamCopy;
        private readonly Func<ValueReader[], ValueReader[], object> Function;

        internal ParamsFunctionReader(int numArgs, List<ValueReader> args, Type ret, 
            Func<ValueReader[], ValueReader[], object> function): base(args, ret)
        {
            Args = args.GetRange(0, numArgs).ToArray();
            Params = args.GetRange(numArgs, args.Count - numArgs).ToArray();
            Function = function;
            ParamCopy = new ValueReader[Params.Length];
        }

        internal override double GetDoubleValue()
        {
            for (int i = 0; i < Params.Length; i++) ParamCopy[i] = Params[i];
            return ToNumber<double>(Function(Args, ParamCopy)); 
        }
        
        internal override float GetFloatValue() 
        {
            for (int i = 0; i < Params.Length; i++) ParamCopy[i] = Params[i];
            return ToNumber<float>(Function(Args, ParamCopy));
        }
        
        internal override int GetIntValue() 
        {
            for (int i = 0; i < Params.Length; i++) ParamCopy[i] = Params[i];
            return ToNumber<int>(Function(Args, ParamCopy));
        }
        
        internal override long GetLongValue() {
            for (int i = 0; i < Params.Length; i++) ParamCopy[i] = Params[i];
            return ToNumber<long>(Function(Args, ParamCopy));
        }
        
        internal override bool GetBoolValue()
        {
            for (int i = 0; i < Params.Length; i++) ParamCopy[i] = Params[i];
            return ToBool(Function(Args, ParamCopy));
        }
        
        internal override string GetStrValue()
        {
            for (int i = 0; i < Params.Length; i++) ParamCopy[i] = Params[i];
            return Function(Args, ParamCopy).ToString();
        }
    }
}
