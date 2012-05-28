using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Models.Plugins;
using Kinectitude.Editor.Models.Properties;
using Action = Kinectitude.Editor.Models.Plugins.Action;
using Attribute = Kinectitude.Editor.Models.Base.Attribute;
using System.Xml.Schema;
using System.Xml;
using Kinectitude.Editor.Models.Base;

namespace Kinectitude.Editor.Storage
{
    public class XmlGameStorage : IGameStorage
    {
        private static class Constants
        {
            private static readonly XNamespace Namespace  = "http://www.kinectitude.com";

            public static readonly XName Game = Namespace + "Game";
            public static readonly XName Using = Namespace + "Using";
            public static readonly XName Alias = Namespace + "Alias";
            public static readonly XName Scene = Namespace + "Scene";
            public static readonly XName Entity = Namespace + "Entity";
            public static readonly XName Attribute = Namespace + "Attribute";
            public static readonly XName Component = Namespace + "Component";
            public static readonly XName Event = Namespace + "Event";
            public static readonly XName Condition = Namespace + "Condition";
            public static readonly XName Action = Namespace + "Action";

            public static readonly XName Name = "Name";
            public static readonly XName Width = "Width";
            public static readonly XName Height = "Height";
            public static readonly XName IsFullScreen = "IsFullScreen";
            public static readonly XName FirstScene = "FirstScene";
            public static readonly XName Prototype = "Prototype";
            public static readonly XName Type = "Type";
            public static readonly XName Key = "Key";
            public static readonly XName Value = "Value";
            public static readonly XName Path = "Path";
            public static readonly XName Class = "Class";

            public static readonly XName Project = "Project";
            public static readonly XName Root = "Root";
        }

        private readonly Game game;
        private readonly string fileName;
        private readonly IPluginNamespace pluginNamespace;
        private readonly Queue<Tuple<XElement, Entity>> entities;

        public XmlGameStorage(string fileName, IPluginNamespace pluginNamespace)
        {
            this.fileName = fileName;
            this.game = new Game(pluginNamespace);
            this.pluginNamespace = pluginNamespace;
            entities = new Queue<Tuple<XElement,Entity>>();
        }

        public Game LoadGame()
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(null, new XmlTextReader(Path.Combine("Storage", "schema.xsd")));

            XElement project = XElement.Load(fileName);
            XElement root = project.Element(Constants.Root);
            string gameFileName = Path.Combine(Path.GetDirectoryName(fileName), root.Value, "game.xml");

            XDocument document = XDocument.Load(gameFileName);
            XElement gameElement = document.Root;

            document.Validate(schemas, (o, e) => { throw new ArgumentException("Invalid Kinectitude XML file."); });

            // First Pass
            // Create the Game object top-down, including properties, attributes, and parent relationships.

            game.Name = (string)gameElement.Attribute(Constants.Name);
            game.Width = (int)gameElement.Attribute(Constants.Width);
            game.Height = (int)gameElement.Attribute(Constants.Height);

            foreach (XElement usingElement in gameElement.Elements(Constants.Using))
            {
                Using use = CreateUsing(usingElement);
                game.AddUsing(use);
            }

            foreach (XElement attributeElement in gameElement.Elements(Constants.Attribute))
            {
                Attribute attribute = CreateAttribute(attributeElement);
                game.AddAttribute(attribute);
            }

            foreach (XElement prototypeElement in gameElement.Elements(Constants.Entity))
            {
                Entity entity = CreateEntity(prototypeElement);
                entities.Enqueue(new Tuple<XElement, Entity>(prototypeElement, entity));
                game.AddEntity(entity);
            }

            foreach (XElement sceneElement in gameElement.Elements(Constants.Scene))
            {
                Scene scene = CreateScene(sceneElement);
                game.AddScene(scene);
            }

            // Second Pass
            // Run through the XML again, to resolve references such as the game's first scene, or an entity's prototype

            game.FirstScene = game.GetScene((string)gameElement.Attribute(Constants.FirstScene));

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
                        Entity prototype = game.GetPrototype(token);
                        if (null != prototype)
                        {
                            entity.AddPrototype(prototype);
                        }
                    }
                }
            }
            return game;
        }

        private Using CreateUsing(XElement element)
        {
            Using use = new Using
            {
                Path = (string)element.Attribute(Constants.Path)
            };

            foreach (XElement aliasElement in element.Elements(Constants.Alias))
            {
                Alias alias = CreateAlias(aliasElement);
                use.AddAlias(alias);
            }

            return use;
        }

        private Alias CreateAlias(XElement element)
        {
            Alias alias = new Alias
            {
                Name = (string)element.Attribute(Constants.Name),
                Class = (string)element.Attribute(Constants.Class)
            };

            return alias;
        }

        private Scene CreateScene(XElement element)
        {
            Scene scene = new Scene
            {
                Name = (string)element.Attribute(Constants.Name)
            };

            foreach (XElement attributeElement in element.Elements(Constants.Attribute))
            {
                Attribute attribute = CreateAttribute(attributeElement);
                scene.AddAttribute(attribute);
            }

            foreach (XElement entityElement in element.Elements(Constants.Entity))
            {
                Entity entity = CreateEntity(entityElement);
                entities.Enqueue(new Tuple<XElement,Entity>(entityElement, entity));
                scene.AddEntity(entity);
            }

            return scene;
        }

        private Entity CreateEntity(XElement element)
        {
            Entity entity = new Entity
            {
                Name = (string)element.Attribute(Constants.Name)
            };

            foreach (XElement attributeElement in element.Elements(Constants.Attribute))
            {
                Attribute attribute = CreateAttribute(attributeElement);
                entity.AddAttribute(attribute);
            }

            foreach (XElement componentElement in element.Elements(Constants.Component))
            {
                Component component = CreateComponent(componentElement);
                entity.AddComponent(component);
            }

            foreach (XElement eventElement in element.Elements(Constants.Event))
            {
                Event evt = CreateEvent(eventElement);
                entity.AddEvent(evt);
            }
            return entity;
        }

        private Component CreateComponent(XElement element)
        {
            PluginDescriptor descriptor = game.GetPluginDescriptor((string)element.Attribute(Constants.Type));
            Component component = new Component(descriptor);

            foreach (XAttribute attribute in element.Attributes().Except(element.Attributes(Constants.Type)))
            {
                component.SetProperty(attribute.Name.LocalName, (string)attribute);
            }
            return component;
        }

        private Event CreateEvent(XElement element)
        {
            PluginDescriptor descriptor = game.GetPluginDescriptor((string)element.Attribute(Constants.Type));
            Event evt = new Event(descriptor);

            foreach (XAttribute attribute in element.Attributes().Except(element.Attributes(Constants.Type)))
            {
                evt.SetProperty(attribute.Name.LocalName, (string)attribute);
            }

            foreach (XElement actionElement in element.Elements(Constants.Action))
            {
                Action action = CreateAction(actionElement);
                evt.AddAction(action);
            }
            return evt;
        }

        private Action CreateAction(XElement element)
        {
            PluginDescriptor descriptor = game.GetPluginDescriptor((string)element.Attribute(Constants.Type));
            Action action = new Action(descriptor);

            foreach (XAttribute attribute in element.Attributes().Except(element.Attributes(Constants.Type)))
            {
                action.SetProperty(attribute.Name.LocalName, (string)attribute);
            }
            return action;
        }

        private Attribute CreateAttribute(XElement element)
        {
            string key = (string)element.Attribute(Constants.Key);
            string value = (string)element.Attribute(Constants.Value);
            Attribute attribute = new Attribute(key, value);
            return attribute;
        }

        public void SaveGame(Game game)
        {
            // Check if the project file exists

            string projectFolder = Path.GetDirectoryName(fileName);
            string assetFolder = Path.GetFileNameWithoutExtension(fileName);
            string gameFile = Path.Combine(projectFolder, assetFolder, "game.xml");

            if (!File.Exists(fileName))
            {
                XElement project = new XElement
                (
                    Constants.Project,
                    new XElement(Constants.Root, assetFolder)
                );
                project.Save(fileName);
            }

            FileInfo file = new FileInfo(gameFile);
            file.Directory.Create();

            // Check if the project folder exists

            XElement document = new XElement
            (
                Constants.Game,
                new XAttribute(Constants.Name, game.Name),
                new XAttribute(Constants.Width, game.Width),
                new XAttribute(Constants.Height, game.Height),
                new XAttribute(Constants.IsFullScreen, game.IsFullScreen),
                new XAttribute(Constants.FirstScene, game.FirstScene.Name)
            );

            foreach (Using use in game.Usings)
            {
                XElement element = SerializeUsing(use);
                document.Add(element);
            }

            foreach (Entity entity in game.Entities)
            {
                XElement element = SerializeEntity(entity);
                document.Add(element);
            }

            foreach (Attribute attribute in game.Attributes)
            {
                XElement element = SerializeAttribute(attribute);
                document.Add(element);
            }

            foreach (Scene scene in game.Scenes)
            {
                XElement element = SerializeScene(scene);
                document.Add(element);
            }

            document.Save(gameFile);
        }

        private XElement SerializeUsing(Using use)
        {
            XElement element = new XElement
            (
                Constants.Using,
                new XAttribute(Constants.Path, use.Path)
            );

            foreach (Alias alias in use.Aliases)
            {
                XElement aliasElement = SerializeAlias(alias);
                element.Add(aliasElement);
            }

            return element;
        }

        private XElement SerializeAlias(Alias alias)
        {
            XElement element = new XElement
            (
                Constants.Alias,
                new XAttribute(Constants.Name, alias.Name),
                new XAttribute(Constants.Class, alias.Class)
            );
            return element;
        }

        private XElement SerializeScene(Scene scene)
        {
            XElement element = new XElement
            (
                Constants.Scene,
                new XAttribute(Constants.Name, scene.Name)
            );

            foreach (Attribute attribute in scene.Attributes)
            {
                XElement attributeElement = SerializeAttribute(attribute);
                element.Add(attributeElement);
            }

            foreach (Entity entity in scene.Entities)
            {
                XElement entityElement = SerializeEntity(entity);
                element.Add(entityElement);
            }

            return element;
        }

        private XElement SerializeEntity(Entity entity)
        {
            XElement element = new XElement(Constants.Entity);

            if (null != entity.Name)
            {
                XAttribute name = new XAttribute(Constants.Name, entity.Name);
                element.Add(name);
            }

            if (entity.Prototypes.Count() > 0)
            {
                var prototypeNames = from prototypeEntity in entity.Prototypes select prototypeEntity.Name;
                XAttribute prototype = new XAttribute(Constants.Prototype, string.Join(" ", prototypeNames));
                element.Add(prototype);
            }

            foreach (Attribute attribute in entity.Attributes)
            {
                XElement attributeElement = SerializeAttribute(attribute);
                element.Add(attributeElement);
            }

            foreach (Component component in entity.Components)
            {
                XElement componentElement = SerializeComponent(component);
                element.Add(componentElement);
            }

            foreach (Event evt in entity.Events)
            {
                XElement eventElement = SerializeEvent(evt);
                element.Add(eventElement);
            }
            return element;
        }

        private XElement SerializeAttribute(Attribute attribute)
        {
            XElement element = new XElement
            (
                Constants.Attribute,
                new XAttribute(Constants.Key, attribute.Key),
                new XAttribute(Constants.Value, attribute.Value.ToString())
            );
            return element;
        }

        private XElement SerializeComponent(Component component)
        {
            XElement element = new XElement(
                Constants.Component,
                new XAttribute(Constants.Type, component.Descriptor.DisplayName)
            );

            foreach (Property property in component.Properties)
            {
                XAttribute propertyAttribute = new XAttribute(property.Descriptor.DisplayName, property.ToString());
                element.Add(propertyAttribute);
            }
            return element;
        }

        private XElement SerializeEvent(Event evt)
        {
            XElement element = new XElement
            (
                Constants.Event,
                new XAttribute(Constants.Type, evt.Descriptor.DisplayName)
            );

            foreach (Property property in evt.Properties)
            {
                XAttribute propertyAttribute = new XAttribute(property.Descriptor.DisplayName, property.ToString());
                element.Add(propertyAttribute);
            }

            foreach (Action action in evt.Actions)
            {
                XElement actionElement = SerializeAction(action);
                element.Add(actionElement);
            }
            return element;
        }

        private XElement SerializeAction(Action action)
        {
            XElement element = new XElement
            (
                Constants.Action,
                new XAttribute(Constants.Type, action.Descriptor.DisplayName)
            );

            foreach (Property property in action.Properties)
            {
                XAttribute propertyAttribute = new XAttribute(property.Descriptor.DisplayName, property.ToString());
                element.Add(propertyAttribute);
            }
            return element;
        }
    }
}
