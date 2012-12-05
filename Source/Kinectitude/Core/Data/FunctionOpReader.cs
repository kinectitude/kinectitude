using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class FunctionOpReader : ValueReader
    {
        internal override double GetDoubleValue()
        {
            throw new NotImplementedException();
        }

        internal override float GetFloatValue()
        {
            throw new NotImplementedException();
        }

        internal override int GetIntValue()
        {
            throw new NotImplementedException();
        }

        internal override long GetLongValue()
        {
            throw new NotImplementedException();
        }

        internal override bool GetBoolValue()
        {
            throw new NotImplementedException();
        }

        internal override string GetStrValue()
        {
            throw new NotImplementedException();
        }

        internal override PreferedType PreferedRetType()
        {
            throw new NotImplementedException();
        }

        internal override ValueWriter ConvertToWriter()
        {
            throw new NotImplementedException();
        }
    }
}
