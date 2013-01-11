using Kinectitude.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
{
    internal sealed class ComponentChangeable : IChangeable
    {
        private readonly Entity entity;
        private readonly Plugin plugin;

        public ComponentChangeable(Entity entity, string component)
        {
            this.entity = entity;

            plugin = entity.GetPlugin(component);
        }

        public object this[string parameter]
        {
            get
            {
                //var component = 
                throw new NotImplementedException();
            }
        }

        public bool ShouldCheck
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
