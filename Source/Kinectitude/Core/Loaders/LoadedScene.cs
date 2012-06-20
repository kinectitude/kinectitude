using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    internal class LoadedScene : LoadedObject
    {
        internal Dictionary<string, HashSet<int>> IsType { get; private set; }
        internal Dictionary<string, HashSet<int>> IsExactType { get; private set; }

        internal Dictionary<int, Entity> EntityById { get; private set; }
        internal Dictionary<string, Entity> EntityByName { get; private set; }

        private readonly List<LoadedEntity> loadedEntities = new List<LoadedEntity>();

        internal LoadedScene(List<Tuple<string, string>> values) : base(values) { }
    }
}
