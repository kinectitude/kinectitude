using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.DataContainers;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Values
{
    internal sealed class Value : GameModel<IValueScope>
    {
        private readonly string initializer;
        private readonly ValueReader reader;

        private readonly ThisDataContainer thisContainer;
        private readonly SceneDataContainer sceneContainer;

        public string Initializer
        {
            get { return initializer; }
        }

        public ValueReader Reader
        {
            get { return reader; }
        }

        public Entity Entity
        {
            get { return GetAncestor<Entity>(); }
        }

        public Scene Scene
        {
            get { return GetAncestor<Scene>(); }
        }

        public Game Game
        {
            get { return GetAncestor<Game>(); }
        }

        public Value(string initializer)
        {
            thisContainer = new ThisDataContainer(this);
            sceneContainer = new SceneDataContainer(this);

            this.initializer = initializer;
            reader = Workspace.ValueMaker.CreateValueReader(initializer, sceneContainer, thisContainer);

            AddHandler<ScopeChanged>(OnScopeChanged);
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        private void OnScopeChanged(ScopeChanged e)
        {
            // TODO: Tell ThisDataContainer to update if needed
        }
    }
}
