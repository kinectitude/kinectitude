using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EditorModels.ViewModels;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Reflection;
using System.Xml;
using System.IO;

namespace EditorModels.Storage
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
            public static readonly XName Manager = Namespace + "Manager";
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
            public static readonly XName If = "If";

            public static readonly XName Project = "Project";
            public static readonly XName Root = "Root";
        }

        private static readonly XName[] GameProperties = new[] { Constants.Xmlns, Constants.Name, Constants.Width, Constants.Height, Constants.IsFullScreen, Constants.FirstScene };
        private static readonly XName[] SceneProperties = new[] { Constants.Name };
        private static readonly XName[] EntityProperties = new[] { Constants.Name, Constants.Prototype };

        private GameViewModel game;
        private readonly string fileName;
        private readonly Queue<Tuple<XElement, EntityViewModel>> entities;

        public XmlGameStorage(string fileName)
        {
            this.fileName = fileName;
            entities = new Queue<Tuple<XElement, EntityViewModel>>();
        }

        public GameViewModel LoadGame()
        {
            XDocument document = XDocument.Load(fileName);
            XElement gameElement = document.Root;

            XmlSchemaSet schemas = new XmlSchemaSet();
            Assembly asm = Assembly.GetExecutingAssembly();
            schemas.Add(null, new XmlTextReader(asm.GetManifestResourceStream("EditorModels.Storage.schema.xsd")));
            document.Validate(schemas, (o, e) => { throw new ArgumentException("Invalid Kinectitude XML file."); });

            game = new GameViewModel((string)gameElement.Attribute(Constants.Name))
            {
                FileName = fileName,
                Width = (int)gameElement.Attribute(Constants.Width),
                Height = (int)gameElement.Attribute(Constants.Height),
                IsFullScreen = (bool)gameElement.Attribute(Constants.IsFullScreen)
            };

            foreach (XElement usingElement in gameElement.Elements(Constants.Using))
            {
                UsingViewModel use = CreateUsing(usingElement);
                game.AddUsing(use);
            }

            foreach (XAttribute xmlAttribute in gameElement.Attributes().Where(x => !GameProperties.Contains(x.Name)))
            {
                AttributeViewModel attribute = CreateAttribute(xmlAttribute);
                game.AddAttribute(attribute);
            }

            foreach (XElement prototypeElement in gameElement.Elements(Constants.PrototypeElement))
            {
                EntityViewModel entity = CreateEntity(prototypeElement);
                entities.Enqueue(Tuple.Create<XElement, EntityViewModel>(prototypeElement, entity));
                game.AddPrototype(entity);
            }

            foreach (XElement sceneElement in gameElement.Elements(Constants.Scene))
            {
                SceneViewModel scene = CreateScene(sceneElement);
                game.AddScene(scene);
            }

            // Second Pass
            // Run through the XML again, to resolve references such as the game's first scene, or an entity's prototype

            game.FirstScene = game.GetScene((string)gameElement.Attribute(Constants.FirstScene));

            while (entities.Count > 0)
            {
                Tuple<XElement, EntityViewModel> tuple = entities.Dequeue();
                XElement element = tuple.Item1;
                EntityViewModel entity = tuple.Item2;

                string prototypeNames = (string)element.Attribute(Constants.Prototype);
                if (null != prototypeNames)
                {
                    string[] tokens = prototypeNames.Split(' ');
                    foreach (string token in tokens)
                    {
                        EntityViewModel prototype = game.GetPrototype(token);
                        if (null != prototype)
                        {
                            entity.AddPrototype(prototype);
                        }
                    }
                }
            }
            return game;
        }

        private UsingViewModel CreateUsing(XElement element)
        {
            UsingViewModel use = new UsingViewModel() { File = (string)element.Attribute(Constants.File) };

            foreach (XElement defineElement in element.Elements(Constants.Define))
            {
                DefineViewModel define = CreateDefine(defineElement);
                use.AddDefine(define);
            }

            return use;
        }

        private DefineViewModel CreateDefine(XElement element)
        {
            DefineViewModel define = new DefineViewModel((string)element.Attribute(Constants.Name), (string)element.Attribute(Constants.Class));
            return define;
        }

        private SceneViewModel CreateScene(XElement element)
        {
            SceneViewModel scene = new SceneViewModel((string)element.Attribute(Constants.Name));

            foreach (XAttribute xmlAttribute in element.Attributes().Where(x => !SceneProperties.Contains(x.Name)))
            {
                AttributeViewModel attribute = CreateAttribute(xmlAttribute);
                scene.AddAttribute(attribute);
            }

            foreach (XElement managerElement in element.Elements(Constants.Manager))
            {
                ManagerViewModel manager = CreateManager(managerElement);
                scene.AddManager(manager);
            }

            foreach (XElement entityElement in element.Elements(Constants.Entity))
            {
                EntityViewModel entity = CreateEntity(entityElement);
                entities.Enqueue(Tuple.Create<XElement, EntityViewModel>(entityElement, entity));
                scene.AddEntity(entity);
            }

            return scene;
        }

        private ManagerViewModel CreateManager(XElement element)
        {
            PluginViewModel plugin = game.GetPlugin((string)element.Attribute(Constants.Type));
            ManagerViewModel manager = new ManagerViewModel(plugin);

            foreach (XAttribute attribute in element.Attributes().Except(element.Attributes(Constants.Type)))
            {
                manager.SetProperty(attribute.Name.LocalName, (string)attribute);
            }
            return manager;
        }

        private EntityViewModel CreateEntity(XElement element)
        {
            EntityViewModel entity = new EntityViewModel() { Name = (string)element.Attribute(Constants.Name) };

            foreach (XAttribute xmlAttribute in element.Attributes().Where(x => !EntityProperties.Contains(x.Name)))
            {
                AttributeViewModel attribute = CreateAttribute(xmlAttribute);
                entity.AddAttribute(attribute);
            }

            foreach (XElement componentElement in element.Elements(Constants.Component))
            {
                ComponentViewModel component = CreateComponent(componentElement);
                entity.AddComponent(component);
            }

            foreach (XElement eventElement in element.Elements(Constants.Event))
            {
                EventViewModel evt = CreateEvent(eventElement);
                entity.AddEvent(evt);
            }
            return entity;
        }

        private ComponentViewModel CreateComponent(XElement element)
        {
            PluginViewModel plugin = game.GetPlugin((string)element.Attribute(Constants.Type));
            ComponentViewModel component = new ComponentViewModel(plugin);

            foreach (XAttribute attribute in element.Attributes().Except(element.Attributes(Constants.Type)))
            {
                component.SetProperty(attribute.Name.LocalName, (string)attribute);
            }
            return component;
        }

        private EventViewModel CreateEvent(XElement element)
        {
            PluginViewModel plugin = game.GetPlugin((string)element.Attribute(Constants.Type));
            EventViewModel evt = new EventViewModel(plugin);

            foreach (XAttribute attribute in element.Attributes().Except(element.Attributes(Constants.Type)))
            {
                evt.SetProperty(attribute.Name.LocalName, (string)attribute);
            }

            foreach (XElement actionElement in element.Elements())
            {
                AbstractActionViewModel action = null;

                if (actionElement.Name == Constants.Action)
                {
                    action = CreateAction(actionElement);
                }
                else if (actionElement.Name == Constants.Condition)
                {
                    action = CreateCondition(actionElement);
                }

                evt.AddAction(action);
            }
            return evt;
        }

        private ConditionViewModel CreateCondition(XElement element)
        {
            ConditionViewModel condition = new ConditionViewModel() { If = (string)element.Attribute(Constants.If) };

            foreach (XElement actionElement in element.Elements())
            {
                AbstractActionViewModel action = null;

                if (actionElement.Name == Constants.Action)
                {
                    action = CreateAction(actionElement);
                }
                else if (actionElement.Name == Constants.Condition)
                {
                    action = CreateCondition(actionElement);
                }

                condition.AddAction(action);
            }

            return condition;
        }

        private ActionViewModel CreateAction(XElement element)
        {
            PluginViewModel plugin = game.GetPlugin((string)element.Attribute(Constants.Type));
            ActionViewModel action = new ActionViewModel(plugin);

            foreach (XAttribute attribute in element.Attributes().Except(element.Attributes(Constants.Type)))
            {
                action.SetProperty(attribute.Name.LocalName, (string)attribute);
            }
            return action;
        }

        private AttributeViewModel CreateAttribute(XAttribute xmlAttribute)
        {
            AttributeViewModel attribute = new AttributeViewModel((string)xmlAttribute.Name.LocalName) { Value = (string)xmlAttribute.Value };

            return attribute;
        }

        public void SaveGame(GameViewModel game)
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
                new XAttribute(Constants.FirstScene, game.FirstScene)
            );

            foreach (UsingViewModel use in game.Usings)
            {
                XElement element = SerializeUsing(use);
                document.Add(element);
            }

            foreach (EntityViewModel entity in game.Prototypes)
            {
                XElement element = SerializeEntity(entity, Constants.PrototypeElement);
                document.Add(element);
            }

            foreach (AttributeViewModel attribute in game.Attributes)
            {
                XAttribute xmlAttribute = SerializeAttribute(attribute);
                document.Add(xmlAttribute);
            }

            foreach (SceneViewModel scene in game.Scenes)
            {
                XElement element = SerializeScene(scene);
                document.Add(element);
            }

            document.Save(gameFile);
        }

        private XElement SerializeUsing(UsingViewModel use)
        {
            XElement element = new XElement(Constants.Using, new XAttribute(Constants.File, use.File));

            foreach (DefineViewModel define in use.Defines)
            {
                XElement defineElement = SerializeDefine(define);
                element.Add(defineElement);
            }

            return element;
        }

        private XElement SerializeDefine(DefineViewModel define)
        {
            XElement element = new XElement(Constants.Define, new XAttribute(Constants.Name, define.Name), new XAttribute(Constants.Class, define.Class));
            return element;
        }

        private XElement SerializeScene(SceneViewModel scene)
        {
            XElement element = new XElement(Constants.Scene, new XAttribute(Constants.Name, scene.Name));

            foreach (AttributeViewModel attribute in scene.Attributes)
            {
                XAttribute xmlAttribute = SerializeAttribute(attribute);
                element.Add(xmlAttribute);
            }

            foreach (ManagerViewModel manager in scene.Managers)
            {
                XElement managerElement = SerializeManager(manager);
                element.Add(managerElement);
            }

            foreach (EntityViewModel entity in scene.Entities)
            {
                XElement entityElement = SerializeEntity(entity, Constants.Entity);
                element.Add(entityElement);
            }

            return element;
        }

        private XElement SerializeManager(ManagerViewModel manager)
        {
            XElement element = new XElement(Constants.Component, new XAttribute(Constants.Type, manager.Type));

            foreach (AbstractPropertyViewModel property in manager.Properties)
            {
                if (!property.IsInherited)
                {
                    XAttribute propertyAttribute = SerializeProperty(property);
                    element.Add(propertyAttribute);
                }
            }
            return element;
        }

        private XElement SerializeEntity(EntityViewModel entity, XName elementName)
        {
            XElement element = new XElement(elementName);

            if (null != entity.Name)
            {
                XAttribute name = new XAttribute(Constants.Name, entity.Name);
                element.Add(name);
            }

            if (entity.Prototypes.Count() > 0)
            {
                XAttribute prototype = new XAttribute(Constants.Prototype, string.Join(" ", entity.Prototypes.Select(x => x.Name)));
                element.Add(prototype);
            }

            foreach (AttributeViewModel attribute in entity.Attributes)
            {
                if (attribute.IsLocal)
                {
                    XAttribute xmlAttribute = SerializeAttribute(attribute);
                    element.Add(xmlAttribute);
                }
            }

            foreach (ComponentViewModel component in entity.Components)
            {
                if (component.IsRoot || component.HasLocalProperties)
                {
                    XElement componentElement = SerializeComponent(component);
                    element.Add(componentElement);
                }
            }

            foreach (AbstractEventViewModel evt in entity.Events)
            {
                if (evt.IsLocal)
                {
                    XElement eventElement = SerializeEvent(evt);
                    element.Add(eventElement);
                }
            }
            return element;
        }

        private XAttribute SerializeAttribute(AttributeViewModel attribute)
        {
            XAttribute xmlAttribute = new XAttribute(attribute.Key, attribute.Value);
            return xmlAttribute;
        }

        private XElement SerializeComponent(ComponentViewModel component)
        {
            XElement element = new XElement(Constants.Component, new XAttribute(Constants.Type, component.Type));

            foreach (AbstractPropertyViewModel property in component.Properties)
            {
                if (property.IsLocal)
                {
                    XAttribute propertyAttribute = SerializeProperty(property);
                    element.Add(propertyAttribute);
                }
            }
            return element;
        }

        private XElement SerializeEvent(AbstractEventViewModel evt)
        {
            XElement element = new XElement(Constants.Event, new XAttribute(Constants.Type, evt.Type));

            foreach (AbstractPropertyViewModel property in evt.Properties)
            {
                if (!property.IsInherited)
                {
                    XAttribute propertyAttribute = SerializeProperty(property);
                    element.Add(propertyAttribute);
                }
            }

            foreach (AbstractActionViewModel action in evt.Actions)
            {
                XElement actionElement = null;

                AbstractConditionViewModel condition = action as AbstractConditionViewModel;
                if (null != condition)
                {
                    actionElement = SerializeCondition(condition);
                }
                else
                {
                    actionElement = SerializeAction(action);
                }

                element.Add(actionElement);
            }

            return element;
        }

        private XElement SerializeCondition(AbstractConditionViewModel condition)
        {
            XElement element = new XElement(Constants.Condition, new XAttribute(Constants.If, condition.If));

            foreach (AbstractActionViewModel action in condition.Actions)
            {
                XElement actionElement = null;

                AbstractConditionViewModel nestedCondition = action as AbstractConditionViewModel;
                if (null != nestedCondition)
                {
                    actionElement = SerializeCondition(nestedCondition);
                }
                else
                {
                    actionElement = SerializeAction(action);
                }

                element.Add(actionElement);
            }

            return element;
        }

        private XElement SerializeAction(AbstractActionViewModel action)
        {
            XElement element = new XElement(Constants.Action, new XAttribute(Constants.Type, action.Type));

            foreach (AbstractPropertyViewModel property in action.Properties)
            {
                if (!property.IsInherited)
                {
                    XAttribute propertyAttribute = SerializeProperty(property);
                    element.Add(propertyAttribute);
                }
            }
            return element;
        }

        private XAttribute SerializeProperty(AbstractPropertyViewModel property)
        {
            XAttribute propertyAttribute = new XAttribute(property.Name, property.Value);
            return propertyAttribute;
        }
    }
}
