using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace Editor.Storage
{
    public class XmlGameStorage : IGameStorage
    {
        private static class Constants
        {
            public const string Scene = "Scene";
            public const string Name = "Name";
            public const string Width = "Width";
            public const string Height = "Height";
            public const string FirstScene = "FirstScene";
            public const string Game = "Game";
            public const string Entity = "Entity";
            public const string Prototype = "Prototype";
            public const string Component = "Component";
            public const string Event = "Event";
            public const string Action = "Action";
            public const string Type = "Type";
            public const string Attribute = "Attribute";
            public const string Key = "Key";
            public const string Value = "Value";
            public const string Root = "Root";
        }

        private readonly string fileName;
        private readonly IPluginFactory factory;
        private readonly Dictionary<string, Entity> prototypes;
        private readonly Queue<Tuple<XElement, Entity>> entities;

        public XmlGameStorage(string fileName, IPluginFactory factory)
        {
            this.fileName = fileName;
            this.factory = factory;
            prototypes = new Dictionary<string, Entity>();
            entities = new Queue<Tuple<XElement,Entity>>();
        }

        public Game LoadGame()
        {
            XElement project = XElement.Load(fileName);
            XElement root = project.Element(Constants.Root);
            string gameFileName = Path.Combine(Path.GetDirectoryName(fileName), root.Value, "game.xml");
            XElement document = XElement.Load(gameFileName);

            // First Pass
            // Create the Game object top-down, including properties, attributes, and parent relationships.

            Game game = new Game
            {
                Name = (string)document.Attribute(Constants.Name),
                Width = (int)document.Attribute(Constants.Width),
                Height = (int)document.Attribute(Constants.Height)
            };

            foreach (XElement attributeElement in document.Elements(Constants.Attribute))
            {
                BaseAttribute attribute = createAttribute(attributeElement);
                game.AddAttribute(attribute);
            }

            foreach (XElement prototypeElement in document.Elements(Constants.Entity))
            {
                Entity entity = createEntity(prototypeElement);
                prototypes.Add(entity.Name, entity);
                entities.Enqueue(new Tuple<XElement, Entity>(prototypeElement, entity));
                game.AddEntity(entity);
            }

            foreach (XElement sceneElement in document.Elements(Constants.Scene))
            {
                Scene scene = createScene(sceneElement);
                game.AddScene(scene);
            }

            // Second Pass
            // Run through the XML again, to resolve references such as the game's first scene, or an entity's prototype

            game.FirstScene = game.Scenes.FirstOrDefault(x => x.Name == (string)document.Attribute(Constants.FirstScene));

            while (entities.Count > 0)
            {
                Tuple<XElement, Entity> tuple = entities.Dequeue();
                XElement element = tuple.Item1;
                Entity entity = tuple.Item2;
                
                string prototypeNames = (string)element.Attribute(Constants.Prototype);
                if (null != prototypeNames)
                {
                    string[] tokens = prototypeNames.Split(' ');
                    foreach (string token in tokens)
                    {
                        if (prototypes.ContainsKey(token))
                        {
                            entity.AddPrototype(prototypes[token]);
                        }
                    }
                }
            }
            return game;
        }

        private Scene createScene(XElement element)
        {
            Scene scene = new Scene
            {
                Name = (string)element.Attribute(Constants.Name)
            };

            foreach (XElement attributeElement in element.Elements(Constants.Attribute))
            {
                BaseAttribute attribute = createAttribute(attributeElement);
                scene.AddAttribute(attribute);
            }

            foreach (XElement entityElement in element.Elements(Constants.Entity))
            {
                Entity entity = createEntity(entityElement);
                entities.Enqueue(new Tuple<XElement,Entity>(entityElement, entity));
                scene.AddEntity(entity);
            }

            foreach (XElement eventElement in element.Elements(Constants.Event))
            {
                Event evt = createEvent(eventElement);
                scene.AddEvent(evt);
            }
            return scene;
        }

        private Entity createEntity(XElement element)
        {
            Entity entity = new Entity
            {
                Name = (string)element.Attribute(Constants.Name)
            };

            foreach (XElement attributeElement in element.Elements(Constants.Attribute))
            {
                BaseAttribute attribute = createAttribute(attributeElement);
                entity.AddAttribute(attribute);
            }

            foreach (XElement componentElement in element.Elements(Constants.Component))
            {
                Component component = createComponent(componentElement);
                entity.AddComponent(component);
            }

            foreach (XElement eventElement in element.Elements(Constants.Event))
            {
                Event evt = createEvent(eventElement);
                entity.AddEvent(evt);
            }
            return entity;
        }

        private Component createComponent(XElement element)
        {
            string type = (string)element.Attribute(Constants.Type);
            Component component = factory.CreateComponent(type);

            foreach (XAttribute attribute in element.Attributes())
            {
                BaseProperty property = component.GetProperty(attribute.Name.LocalName);
                if (null != property)
                {
                    property.TryParse((string)attribute);
                }
            }
            return component;
        }

        private Event createEvent(XElement element)
        {
            string type = (string)element.Attribute(Constants.Type);
            Event evt = factory.CreateEvent(type);

            foreach (XAttribute attribute in element.Attributes())
            {
                BaseProperty property = evt.GetProperty(attribute.Name.LocalName);
                if (null != property)
                {
                    property.TryParse((string)attribute);
                }
            }

            foreach (XElement actionElement in element.Elements(Constants.Action))
            {
                Action action = createAction(actionElement);
                evt.AddAction(action);
            }
            return evt;
        }

        private Action createAction(XElement element)
        {
            string type = (string)element.Attribute(Constants.Type);
            Action action = factory.CreateAction(type);

            foreach (XAttribute attribute in element.Attributes())
            {
                BaseProperty property = action.GetProperty(attribute.Name.LocalName);
                if (null != property)
                {
                    property.TryParse((string)attribute);
                }
            }
            return action;
        }

        private BaseAttribute createAttribute(XElement element)
        {
            string key = (string)element.Attribute(Constants.Key);
            string value = (string)element.Attribute(Constants.Value);
            BaseAttribute attribute = BaseAttribute.CreateAttribute(key, value);
            return attribute;
        }

        public void SaveGame(Game game)
        {
            XElement document = new XElement
            (
                Constants.Game,
                new XAttribute(Constants.Name, game.Name),
                new XAttribute(Constants.Width, game.Width),
                new XAttribute(Constants.Height, game.Height),
                new XAttribute(Constants.FirstScene, game.FirstScene.Name)
            );

            foreach (BaseAttribute attribute in game.Attributes)
            {
                XElement element = serializeAttribute(attribute);
                document.Add(element);
            }

            foreach (Entity entity in game.Prototypes)
            {
                XElement element = serializeEntity(entity);
                document.Add(element);
            }

            foreach (Scene scene in game.Scenes)
            {
                XElement element = serializeScene(scene);
                document.Add(element);
            }

            document.Save(fileName);
        }

        private XElement serializeScene(Scene scene)
        {
            XElement element = new XElement
            (
                Constants.Scene,
                new XAttribute(Constants.Name, scene.Name)
            );

            foreach (BaseAttribute attribute in scene.Attributes)
            {
                XElement attributeElement = serializeAttribute(attribute);
                element.Add(attributeElement);
            }

            foreach (Entity entity in scene.Entities)
            {
                XElement entityElement = serializeEntity(entity);
                element.Add(entityElement);
            }

            foreach (Event evt in scene.Events)
            {
                XElement eventElement = serializeEvent(evt);
                element.Add(eventElement);
            }
            return element;
        }

        private XElement serializeEntity(Entity entity)
        {
            XElement element = new XElement(Constants.Entity);

            if (null != entity.Name)
            {
                XAttribute name = new XAttribute(Constants.Name, entity.Name);
                element.Add(name);
            }

            if (entity.Prototypes.Count > 0)
            {
                var prototypeNames = from prototypeEntity in entity.Prototypes select prototypeEntity.Name;
                XAttribute prototype = new XAttribute(Constants.Prototype, string.Join(" ", prototypeNames));
                element.Add(prototype);
            }

            foreach (BaseAttribute attribute in entity.Attributes)
            {
                XElement attributeElement = serializeAttribute(attribute);
                element.Add(attributeElement);
            }

            foreach (Component component in entity.Components)
            {
                XElement componentElement = serializeComponent(component);
                element.Add(componentElement);
            }

            foreach (Event evt in entity.Events)
            {
                XElement eventElement = serializeEvent(evt);
                element.Add(eventElement);
            }
            return element;
        }

        private XElement serializeAttribute(BaseAttribute attribute)
        {
            XElement element = new XElement
            (
                Constants.Attribute,
                new XAttribute(Constants.Key, attribute.Key),
                new XAttribute(Constants.Value, attribute.StringValue)
            );
            return element;
        }

        private XElement serializeComponent(Component component)
        {
            XElement element = new XElement(
                Constants.Component,
                new XAttribute(Constants.Type, component.Descriptor.Name)
            );

            foreach (BaseProperty property in component.Properties)
            {
                XAttribute propertyAttribute = new XAttribute(property.Descriptor.Key, property.StringValue);
                element.Add(propertyAttribute);
            }
            return element;
        }

        private XElement serializeEvent(Event evt)
        {
            XElement element = new XElement
            (
                Constants.Event,
                new XAttribute(Constants.Type, evt.Descriptor.Name)
            );

            foreach (BaseProperty property in evt.Properties)
            {
                XAttribute propertyAttribute = new XAttribute(property.Descriptor.Key, property.StringValue);
                element.Add(propertyAttribute);
            }

            foreach (Action action in evt.Actions)
            {
                XElement actionElement = serializeAction(action);
                element.Add(actionElement);
            }
            return element;
        }

        private XElement serializeAction(Action action)
        {
            XElement element = new XElement
            (
                Constants.Action,
                new XAttribute(Constants.Type, action.Descriptor.Name)
            );

            foreach (BaseProperty property in action.Properties)
            {
                XAttribute propertyAttribute = new XAttribute(property.Descriptor.Key, property.StringValue);
                element.Add(propertyAttribute);
            }
            return element;
        }
    }
}
