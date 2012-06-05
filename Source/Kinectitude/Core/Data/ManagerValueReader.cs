using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class ManagerValueReader : ExpressionReader
    {
        private readonly Func<string> getValue;

        internal ManagerValueReader(string[] values, Scene scene)
        {
            IManager manager = scene.GetManager(values[0]);
            getValue = () => ClassFactory.GetStringParam(manager, values[1]);
        }

        public override string GetValue()
        {

            return getValue();
        }

        public override void notifyOfChange(Action<string> callback)
        {
            throw new NotImplementedException();
        }
    }
}
