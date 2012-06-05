using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Kinectitude.Editor.Models.Base;
using Kinectitude.Editor.Models.Plugins;
using Kinectitude.Editor.Models.Properties;
using Action = Kinectitude.Editor.Models.Plugins.Action;
using Attribute = Kinectitude.Editor.Models.Base.Attribute;

namespace Kinectitude.Editor.Storage
{
    internal sealed class XmlGameStorage : IGameStorage
    {
        private static class Constants
        {
            private static readonly XNamespace Namespace = "http://www.kinectitude.com/2012/v1";

            public static readonly XName Game = Namespace + "Game";
            public static readonly XName Using = Namespace + "Using";
            public static readonly XName Define = Namespace + "Define";
            public static readonly XName Scene = Namespace + "Scene";
            public static readonly XName PrototypeElement = Namespace + "Prototype";
            public static readonly XName Entity = Namespace + "Entity";
            public static readonly XName Component = Namespace + "Component";
            public static readonly XName Event = Namespace + "Event";
            public static readonly XName Condition = Namespace + "Condition";
            public static readonly XName Action = Namespace + "Action";

            public static readonly XName Xmlns = "xmlns";
            public static readonly XName Name = "Name";
            public static readonly XName Width = "Width";
            public static readonly XName Height = "Height";
            public static readonly XName IsFullScreen = "IsFullScreen";
            public static readonly XName FirstScene = "FirstScene";
            public static readonly XName Prototype = "Prototype";
            public static readonly XName Type = "Type";
            public static readonly XName File = "File";
            public static readonly XName Class = "Class";

            public static readonly XName Project = "Project";
            public static readonly XName Root = "Root";
        }

        private static readonly XName[] gameProperties = new[] { Constants.Xmlns, Constants.Name, Constants.Width, Constants.Height, Constants.IsFullScreen, Constants.FirstScene };
        private static readonly XName[] sceneProperties = new[] { Constants.Name };
        private static readonly XName[] entityProperties = new[] { Constants.Name, Constants.Prototype };

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
            game.IsFullScreen = (bool)gameElement.Attribute(Constants.IsFullScreen);

            foreach (XElement usingElement in gameElement.Elements(Constants.Using))
            {
                Using use = CreateUsing(usingElement);
                game.AddUsing(use);
            }

            foreach (XAttribute xmlAttribute in gameElement.Attributes().Where(x => !gameProperties.Contains(x.Name)))
            {
                Attribute attribute = CreateAttribute(xmlAttribute);
                game.AddAttribute(attribute);
            }

            foreach (XElement prototypeElement in gameElement.Elements(Constants.PrototypeElement))
            {
                Entity entity = CreateEntity(prototypeElement);
                entities.Enqueue(Tuple.Create<XElement, Entity>(prototypeElement, entity));
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
                File = (string)element.Attribute(Constants.File)
            };

            foreach (XElement defineElement in element.Elements(Constants.Define))
            {
                Define define = CreateDefine(defineElement);
                use.AddDefine(define);
            }

            return use;
        }

        private Define CreateDefine(XElement element)
        {
            Define define = new Define
            {
                Name = (string)element.Attribute(Constants.Name),
                Class = (string)element.Attribute(Constants.Class)
            };

            return define;
        }

        private Scene CreateScene(XElement element)
        {
            Scene scene = new Scene
            {
                Name = (string)element.Attribute(Constants.Name)
            };

            foreach (XAttribute xmlAttribute in element.Attributes().Where(x => !sceneProperties.Contains(x.Name)))
            {
                Attribute attribute = CreateAttribute(xmlAttribute);
                scene.AddAttribute(attribute);
            }

            foreach (XElement entityElement in element.Elements(Constants.Entity))
            {
                Entity entity = CreateEntity(entityElement);
                entities.Enqueue(Tuple.Create<XElement,Entity>(entityElement, entity));
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

            foreach (XAttribute xmlAttribute in element.Attributes().Where(x => !entityProperties.Contains(x.Name)))
            {
                Attribute attribute = CreateAttribute(xmlAttribute);
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

        private Attribute CreateAttribute(XAttribute xmlAttribute)
        {
            string key = (string)xmlAttribute.Name.LocalName;
            string value = (string)xmlAttribute.Value;
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
                XElement element = SerializeEntity(entity, Constants.PrototypeElement);
                document.Add(element);
            }

            foreach (Attribute attribute in game.Attributes)
            {
                XAttribute xmlAttribute = SerializeAttribute(attribute);
                document.Add(xmlAttribute);
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
                new XAttribute(Constants.File, use.File)
            );

            foreach (Define define in use.Defines)
            {
                XElement defineElement = SerializeDefine(define);
                element.Add(defineElement);
            }

            return element;
        }

        private XElement SerializeDefine(Define define)
        {
            XElement element = new XElement
            (
                Constants.Define,
                new XAttribute(Constants.Name, define.Name),
                new XAttribute(Constants.Class, define.Class)
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
                XAttribute xmlAttribute = SerializeAttribute(attribute);
                element.Add(xmlAttribute);
            }

            foreach (Entity entity in scene.Entities)
            {
                XElement entityElement = SerializeEntity(entity, Constants.Entity);
                element.Add(entityElement);
            }

            return element;
        }

        private XElement SerializeEntity(Entity entity, XName elementName)
        {
            XElement element = new XElement(elementName);

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
                XAttribute xmlAttribute = SerializeAttribute(attribute);
                element.Add(xmlAttribute);
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

        private XAttribute SerializeAttribute(Attribute attribute)
        {
            XAttribute xmlAttribute = new XAttribute(attribute.Key, attribute.Value.ToString());
            return xmlAttribute;
        }

        private XElement SerializeComponent(Component component)
        {
            XElement element = new XElement(
                Constants.Component,
                new XAttribute(Constants.Type, component.Descriptor.ClassName)
            );

            foreach (Property property in component.Properties)
            {
                XAttribute propertyAttribute = new XAttribute(property.Descriptor.Name, property.ToString());
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
