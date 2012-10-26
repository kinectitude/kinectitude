using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class DataContainerReader : RepeatReader
    {
        internal readonly DataContainer DataContainer;
        internal readonly string Param;

        internal DataContainerReader(DataContainer dataContainer, string param)
        {
            DataContainer = dataContainer;
            Param = param;
            Reader = dataContainer[Param];
        }

        internal override void notifyOfChange(Action<ValueReader> change)
        {
            Callbacks.Add(change);
            DataContainer.NotifyOfChange(Param, change);
        }

        private void change(ValueReader value)
        {
            if (value != Reader)
            {
                foreach (Action<ValueReader> callback in Callbacks) callback(value);
            }
            Reader = value;
        }

        internal override ValueWriter ConvertToWriter()
        {
            DataContainerWriter dataContainerWriter = new DataContainerWriter(Param, this);
            dataContainerWriter.DataContainer = DataContainer;
            return dataContainerWriter;
        }
    }
}
