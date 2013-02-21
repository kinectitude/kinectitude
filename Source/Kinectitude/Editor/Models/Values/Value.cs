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

        public IDataContainer ThisContainer
        {
            get { return thisContainer; }
        }

        public IDataContainer SceneContainer
        {
            get { return sceneContainer; }
        }

        public Value(string initializer)
        {
            thisContainer = new ThisDataContainer(this);
            sceneContainer = new SceneDataContainer(this);

            this.initializer = initializer;
            reader = Workspace.ValueMaker.CreateValueReader(initializer, sceneContainer, thisContainer);
        }

        public Value(object value, bool constant)
        {
            initializer = "";

            if (null != value)
            {
                initializer = value.ToString();

                var type = value.GetType();
                if (type == typeof(string) || type.IsEnum)
                {
                    if (type.IsEnum)
                    {
                        value = value.ToString();
                    }

                    initializer = "\"" + initializer + "\"";
                }
            }

            reader = new ConstantReader(value);
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        public double GetDoubleValue()
        {
            return Reader.GetDoubleValue();
        }

        public float GetFloatValue()
        {
            return Reader.GetFloatValue();
        }

        public int GetIntValue()
        {
            return Reader.GetIntValue();
        }

        public long GetLongValue()
        {
            return Reader.GetLongValue();
        }

        public bool GetBoolValue()
        {
            return Reader.GetBoolValue();
        }

        public string GetStringValue()
        {
            return Reader.GetStrValue();
        }

        public T GetEnumValue<T>() where T : struct, IConvertible
        {
            var result = default(T);

            var str = GetStringValue();
            if (!string.IsNullOrEmpty(str))
            {
                Enum.TryParse<T>(str, out result);
            }

            return result;
        }

        public void Subscribe(IChanges callback)
        {
            Reader.NotifyOfChange(callback);
        }

        public void Unsubscribe(IChanges callback)
        {
            // TODO implement this
        }
    }
}
