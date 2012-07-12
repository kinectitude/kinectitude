using System;
using Kinectitude.Core.Base;
using System.Collections.Generic;

namespace Kinectitude.Core.Data
{
    internal sealed class ManagerValueReader : ExpressionReader
    {
        private readonly Func<string> getValue;
        private readonly Scene scene;
        private List<Action<string>> callbacks = new List<Action<string>>();
        string value;

        internal ManagerValueReader(string[] values, Scene scene)
        {
            value = values[0] + '.' + values[1];
            IManager manager = scene.GetManager(values[0]);
            this.scene = scene;
            getValue = () => ClassFactory.GetStringParam(manager, values[1]);
        }

        public override string GetValue()
        {

            return getValue();
        }

        public override void notifyOfChange(Action<string> callback)
        {
            scene.NotifyOfComponentChange(value, callback);
            callbacks.Add(callback);
        }
    }
}
