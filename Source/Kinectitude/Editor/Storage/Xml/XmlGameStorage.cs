using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Kinectitude.Editor.Models;
using Action = Kinectitude.Editor.Models.Action;
using Attribute = Kinectitude.Editor.Models.Attribute;
using Kinectitude.Editor.Models.Statements;

namespace Kinectitude.Editor.Storage.Xml
{
    internal sealed class XmlGameStorage : IGameStorage
    {
        private static readonly XName[] GameProperties = new[] { XmlConstants.Xmlns, XmlConstants.Name, XmlConstants.Width, XmlConstants.Height, XmlConstants.IsFullScreen, XmlConstants.FirstScene };
        private static readonly XName[] SceneProperties = new[] { XmlConstants.Name };
        private static readonly XName[] EntityProperties = new[] { XmlConstants.Name, XmlConstants.Prototype };

        private Game game;
        private readonly FileInfo file;
        private readonly Queue<Tuple<XElement, Entity>> entities;

        public XmlGameStorage(FileInfo file)
        {
            this.file = file;
            entities = new Queue<Tuple<XElement, Entity>>();
        }

        public Game LoadGame()
        {
            XDocument document = XDocument.Load(file.ToString());
            XElement gameElement = document.Root;

            XmlSchemaSet schemas = new XmlSchemaSet();
            Assembly asm = Assembly.GetExecutingAssembly();

            schemas.Add(null, new XmlTextReader(asm.GetManifestResourceStream("Kinectitude.Editor.Storage.Xml.schema.xsd")));
            document.Validate(schemas, (o, e) => { throw new ArgumentException("Invalid Kinectitude XML file."); });

            game = new Game((string)gameElement.Attribute(XmlConstants.Name))
            {
                Width = (int)gameElement.Attribute(XmlConstants.Width),
                Height = (int)gameElement.Attribute(XmlConstants.Height),
                IsFullScreen = (bool)gameElement.Attribute(XmlConstants.IsFullScreen)
            };

            foreach (XElement usingElement in gameElement.Elements(XmlConstants.Using))
            {
                Using use = CreateUsing(usingElement);
                game.AddUsing(use);
            }

            foreach (XAttribute xmlAttribute in gameElement.Attributes().Where(x => !GameProperties.Contains(x.Name)))
            {
                Attribute attribute = CreateAttribute(xmlAttribute);
                game.AddAttribute(attribute);
            }

            foreach (XElement prototypeElement in gameElement.Elements(XmlConstants.PrototypeElement))
            {
                Entity entity = CreateEntity(prototypeElement);
                entities.Enqueue(Tuple.Create<XElement, Entity>(prototypeElement, entity));
                game.AddPrototype(entity);
            }

            foreach (XElement sceneElement in gameElement.Elements(XmlConstants.Scene))
            {
                Scene scene = CreateScene(sceneElement);
                game.AddScene(scene);
            }

            // Second Pass
            // Run through the XML again, to resolve references such as the game's first scene, or an entity's prototype

            game.FirstScene = game.GetScene((string)gameElement.Attribute(XmlConstants.FirstScene));

            while (entities.Count > 0)
            {
                Tuple<XElement, Entity> tuple = entities.Dequeue();
                XElement element = tuple.Item1;
                Entity entity = tuple.Item2;

                string prototypeNames = (string)element.Attribute(XmlConstants.Prototype);
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
            Using use = new Using() { File = (string)element.Attribute(XmlConstants.File) };

            foreach (XElement defineElement in element.Elements(XmlConstants.Define))
            {
                Define define = CreateDefine(defineElement);
                use.AddDefine(define);
            }

            return use;
        }

        private Define CreateDefine(XElement element)
        {
            return new Define((string)element.Attribute(XmlConstants.Name), (string)element.Attribute(XmlConstants.Class));
        }

        private Scene CreateScene(XElement element)
        {
            Scene scene = new Scene((string)element.Attribute(XmlConstants.Name));

            foreach (XAttribute xmlAttribute in element.Attributes().Where(x => !SceneProperties.Contains(x.Name)))
            {
                Attribute attribute = CreateAttribute(xmlAttribute);
                scene.AddAttribute(attribute);
            }

            foreach (XElement managerElement in element.Elements(XmlConstants.Manager))
            {
                Manager manager = CreateManager(managerElement);
                scene.AddManager(manager);
            }

            foreach (XElement entityElement in element.Elements(XmlConstants.Entity))
            {
                Entity entity = CreateEntity(entityElement);
                entities.Enqueue(Tuple.Create<XElement, Entity>(entityElement, entity));
                scene.AddEntity(entity);
            }

            return scene;
        }

        private Manager CreateManager(XElement element)
        {
            Manager manager = new Manager(game.GetPlugin((string)element.Attribute(XmlConstants.Type)));

            foreach (XAttribute attribute in element.Attributes().Except(element.Attributes(XmlConstants.Type)))
            {
                manager.SetProperty(attribute.Name.LocalName, (string)attribute);
            }

            return manager;
        }

        private Entity CreateEntity(XElement element)
        {
            Entity entity = new Entity() { Name = (string)element.Attribute(XmlConstants.Name) };

            foreach (XAttribute xmlAttribute in element.Attributes().Where(x => !EntityProperties.Contains(x.Name)))
            {
                Attribute attribute = CreateAttribute(xmlAttribute);
                entity.AddAttribute(attribute);
            }

            foreach (XElement componentElement in element.Elements(XmlConstants.Component))
            {
                // Adding a component adds any other components that the new component requires.
                // It is necessary to check if the component we are attempting to add already
                // exists. If it does, we should not attempt to create a new component.

                Plugin plugin = game.GetPlugin((string)componentElement.Attribute(XmlConstants.Type));
                
                Component component = entity.GetComponentByType(plugin.CoreType);
                if (null == component)
                {
                    component = new Component(plugin);
                    entity.AddComponent(component);
                }

                PopulateComponent(componentElement, component);
            }

            foreach (XElement eventElement in element.Elements(XmlConstants.Event))
            {
                Event evt = CreateEvent(eventElement);
                entity.AddEvent(evt);
            }

            return entity;
        }

        private void PopulateComponent(XElement element, Component component)
        {
            foreach (XAttribute attribute in element.Attributes().Except(element.Attributes(XmlConstants.Type)))
            {
                component.SetProperty(attribute.Name.LocalName, (string)attribute);
            }
        }

        private Event CreateEvent(XElement element)
        {
            Event evt = new Event(game.GetPlugin((string)element.Attribute(XmlConstants.Type)));

            foreach (XAttribute attribute in element.Attributes().Except(element.Attributes(XmlConstants.Type)))
            {
                evt.SetProperty(attribute.Name.LocalName, (string)attribute);
            }

            foreach (XElement actionElement in element.Elements())
            {
                AbstractStatement statement = null;

                if (actionElement.Name == XmlConstants.Action)
                {
                    statement = CreateAction(actionElement);
                }
                else if (actionElement.Name == XmlConstants.Condition)
                {
                    statement = CreateCondition(actionElement);
                }

                evt.AddStatement(statement);
            }

            return evt;
        }

        private Condition CreateCondition(XElement element)
        {
            Condition condition = new Condition() { If = (string)element.Attribute(XmlConstants.If) };

            foreach (XElement actionElement in element.Elements())
            {
                AbstractStatement statement = null;

                if (actionElement.Name == XmlConstants.Action)
                {
                    statement = CreateAction(actionElement);
                }
                else if (actionElement.Name == XmlConstants.Condition)
                {
                    statement = CreateCondition(actionElement);
                }

                condition.AddStatement(statement);
            }

            return condition;
        }

        private Action CreateAction(XElement element)
        {
            Action action = new Action(game.GetPlugin((string)element.Attribute(XmlConstants.Type)));

            foreach (XAttribute attribute in element.Attributes().Except(element.Attributes(XmlConstants.Type)))
            {
                action.SetProperty(attribute.Name.LocalName, (string)attribute);
            }

            return action;
        }

        private Attribute CreateAttribute(XAttribute xmlAttribute)
        {
            return new Attribute((string)xmlAttribute.Name.LocalName) { Value = (string)xmlAttribute.Value };
        }

        public void SaveGame(Game game)
        {
            XDocument document = new XDocument(new XmlGameVisitor().Apply(game));
            document.Save(file.ToString());
        }
    }
}
