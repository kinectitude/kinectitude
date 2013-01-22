using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    public sealed class DataContainerReader : RepeatReader
    {
        internal readonly IDataContainer DataContainer;
        internal readonly string Param;

        protected override ValueReader Reader
        {
            get { return DataContainer[Param]; }
        }

        internal static DataContainerReader GetDataContainerReader(IDataContainer dataContainer, string param)
        {
            Func<DataContainerReader> create = new Func<DataContainerReader>(() => new DataContainerReader(dataContainer, param));
            return DoubleDictionary<IDataContainer, string, DataContainerReader>.GetItem(dataContainer, param, create);
        }

        internal static void DeleteDataContainer(IDataContainer dataContainer)
        {
            DoubleDictionary<IDataContainer, string, DataContainerReader>.DeleteDict(dataContainer);
        }

        private DataContainerReader(IDataContainer dataContainer, string param)
        {
            DataContainer = dataContainer;
            Param = param;
        }

        internal override void SetupNotifications()
        {
            DataContainer.NotifyOfChange(Param, this);
        }

        internal override ValueWriter ConvertToWriter()
        {
            DataContainerWriter dataContainerWriter = new DataContainerWriter(Param, this);
            dataContainerWriter.DataContainer = DataContainer;
            return dataContainerWriter;
        }
    }
}
