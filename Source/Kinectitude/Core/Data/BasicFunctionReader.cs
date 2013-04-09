//-----------------------------------------------------------------------
// <copyright file="BasicFunctionReader.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

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
