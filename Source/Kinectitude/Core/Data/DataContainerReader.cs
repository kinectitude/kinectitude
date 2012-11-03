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

        private static readonly Dictionary<string, DataContainerReader> readers =
            new Dictionary<string, DataContainerReader>();


        internal static DataContainerReader getDataContainerReader(DataContainer dataContainer, string param)
        {
            string key;
            if (dataContainer.GetType() == typeof(Game)) key = param;
            else if (dataContainer.GetType() == typeof(Scene)) key = dataContainer.Name + "~" + param;
            else key = dataContainer.Id + "~" + (dataContainer as Entity).Scene.Name + "~" + param;
            DataContainerReader reader;
            if (!readers.TryGetValue(key, out reader))
            {
                readers[key] = reader = new DataContainerReader(dataContainer, param);
            }
            return reader;
        }

        private DataContainerReader(DataContainer dataContainer, string param)
        {
            DataContainer = dataContainer;
            Param = param;
            Reader = dataContainer[Param];
            DataContainer.NotifyOfChange(Param, changeContainer);
        }

        internal override void notifyOfChange(Action<ValueReader> change)
        {
            Callbacks.Add(change);
        }

        private void changeContainer(ValueReader value)
        {
            ValueReader old = Reader;
            Reader = value; 
            if (!old.hasSameVal(value))
            {
                foreach (Action<ValueReader> callback in Callbacks) callback(value);
            }
        }

        internal override ValueWriter ConvertToWriter()
        {
            DataContainerWriter dataContainerWriter = new DataContainerWriter(Param, this);
            dataContainerWriter.DataContainer = DataContainer;
            return dataContainerWriter;
        }
    }
}
