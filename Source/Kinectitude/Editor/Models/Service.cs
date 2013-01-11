using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
{
    internal sealed class Service : GameModel<IScope>
    {
        private readonly Plugin plugin;

        public Plugin Plugin
        {
            get { return plugin; }
        }

        public Service(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public override void Accept(Storage.IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        public Property GetProperty(string name)
        {
            return null;
        }
    }
}
