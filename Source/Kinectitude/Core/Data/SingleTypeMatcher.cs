using System;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class SingleTypeMatcher : TypeMatcher
    {
        private readonly Scene Scene;
        private readonly string Name;
        internal SingleTypeMatcher(string name, Scene scene) 
        {
            Scene = scene;
            Name = name;
        }

        public override bool MatchAndSet(IEntity entity)
        {
            Entity match;
            if(Scene.EntityByName.TryGetValue(Name, out match) && entity == match)
            {
                Entity = entity as Entity;
                return true;
            }
            return false;
        }

        internal override void NotifyOfChange(Action<DataContainer> action) { }
    }
}
