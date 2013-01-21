using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.ValueReaders;
using Kinectitude.Editor.Models.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.DataContainers
{
    internal sealed class GameDataContainer : IDataContainer
    {
        private readonly Value value;
        private readonly Dictionary<string, GameValueReader> attributes;

        public Game Game
        {
            get { return value.Game; }
        }

        public GameDataContainer(Value value)
        {
            this.value = value;
            this.attributes = new Dictionary<string, GameValueReader>();
        }

        #region IDataContainer implementation

        ValueReader IDataContainer.this[string key]
        {
            get
            {
                GameValueReader reader;
                attributes.TryGetValue(key, out reader);

                if (null == reader)
                {
                    reader = new GameValueReader(this, key);
                    attributes[key] = reader;
                }

                return reader;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        IChangeable IDataContainer.GetChangeable(string name)
        {
            throw new NotImplementedException();
        }

        void IDataContainer.NotifyOfChange(string key, IChanges callback)
        {
            throw new NotImplementedException();
        }

        void IDataContainer.NotifyOfComponentChange(Tuple<IChangeable, string> what, IChanges callback)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
