using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.Changeables;
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
        private readonly Dictionary<string, ServiceChangeable> changeables;

        public Game Game
        {
            get { return value.Game; }
        }

        public GameDataContainer(Value value)
        {
            this.value = value;
            
            attributes = new Dictionary<string, GameValueReader>();
            changeables = new Dictionary<string, ServiceChangeable>();
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
            ServiceChangeable changeable;
            changeables.TryGetValue(name, out changeable);

            if (null == changeable)
            {
                changeable = new ServiceChangeable(this, name);
                changeables[name] = changeable;
            }

            return changeable;
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
