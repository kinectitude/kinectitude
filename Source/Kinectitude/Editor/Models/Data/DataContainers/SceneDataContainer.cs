using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.ValueReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.DataContainers
{
    internal sealed class SceneDataContainer : IScene
    {
        private readonly Value value;
        private readonly Dictionary<string, SceneValueReader> attributes;
        private readonly Dictionary<string, EntityDataContainer> namedEntities;
        private readonly GameDataContainer gameContainer;

        public Scene Scene
        {
            get { return value.Scene; }
        }

        public SceneDataContainer(Value value)
        {
            this.value = value;
            attributes = new Dictionary<string, SceneValueReader>();
            namedEntities = new Dictionary<string, EntityDataContainer>();
            gameContainer = new GameDataContainer(value);
        }

        #region IDataContainer implementation

        ValueReader IDataContainer.this[string key]
        {
            get
            {
                SceneValueReader reader;
                attributes.TryGetValue(key, out reader);

                if (null == reader)
                {
                    reader = new SceneValueReader(this, key);
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

        #region IScene implementation

        IDataContainer IScene.Game
        {
            get { return gameContainer; }
        }

        IDataContainer IScene.GetEntity(string name)
        {
            EntityDataContainer container;
            namedEntities.TryGetValue(name, out container);

            if (null == container)
            {
                container = new EntityDataContainer(null, null);
                namedEntities[name] = container;
            }

            return container;
        }

        HashSet<int> IScene.GetOfPrototype(string prototype, bool exact)
        {
            return new HashSet<int>();
        }

        #endregion
    }
}
