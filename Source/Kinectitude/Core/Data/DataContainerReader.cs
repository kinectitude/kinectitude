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

        protected override ValueReader Reader
        {
            get { return DataContainer[Param]; }
        }

        internal static DataContainerReader getDataContainerReader(DataContainer dataContainer, string param)
        {
            Func<DataContainerReader> create = new Func<DataContainerReader>(() => new DataContainerReader(dataContainer, param));
            return DoubleDictionary<DataContainer, string, DataContainerReader>.getItem(dataContainer, param, create);
        }

        private DataContainerReader(DataContainer dataContainer, string param)
        {
            DataContainer = dataContainer;
            Param = param;
            DataContainer.NotifyOfChange(Param, Change);
        }

        internal override ValueWriter ConvertToWriter()
        {
            DataContainerWriter dataContainerWriter = new DataContainerWriter(Param, this);
            dataContainerWriter.DataContainer = DataContainer;
            return dataContainerWriter;
        }
    }
}
