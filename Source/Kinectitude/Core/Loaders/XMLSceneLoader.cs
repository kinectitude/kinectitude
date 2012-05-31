using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    internal class XMLSceneLoader : SceneLoader
    {

        private int onid = 0;
        XMLGameLoader xmlGameLoader;

        internal XMLSceneLoader(XElement scene, XMLGameLoader gl):base(gl)
        {
            xmlGameLoader = gl;
            Scene = new Scene(this, gl.Game);
            foreach (XAttribute attrib in scene.Attributes())
            {
                string attribName = attrib.Name.ToString();
                if("Name" == attribName)
                {
                    Scene.Name = attrib.Value;
                }
                Scene[attribName] = attrib.Value;
            }
            foreach (XElement e in scene.Elements().Where(input => "Entity" == input.Name))
            {
                //so that I don't mess the original up when I merge;
                XElement parsedNode = new XElement(e);
                //insert the prototype into the original
                if (null != e.Attribute("Prototype"))
                {
                    string name = (string)e.Attribute("Prototype");
                    name = name.Trim();
                    if (name.Contains(' '))
                    {
                        string[] names = name.Split(' ');
                        foreach (string n in names)
                        {
                            createWithPrototype(gl, n, parsedNode, onid);
                        }
                    }
                    else
                    {
                        createWithPrototype(gl, name, parsedNode, onid);
                    }
                }
                Entity entity = new Entity(onid);
                
                foreach (XAttribute attrib in parsedNode.Attributes())
                {
                    if ("Prototype" == attrib.Name)
                    {
                        continue;
                    }
                    else if ("Name" == attrib.Name)
                    {
                        entity.Name = attrib.Value;
                        EntityByName.Add(attrib.Value, entity);
                    }
                    else
                    {
                        entity[attrib.Name.ToString()] = attrib.Value;
                    }
                }
                onid += 1;
                entity.Scene = Scene;
                entityParse(parsedNode, entity);
                entity.Ready();
            }            
        }

        //Adds actions to an event or trigger
        internal void AddActions(Game game, XElement node, Event evt, Condition cond = null)
        {
            foreach (XElement action in node.Elements())
            {
                if (action.Name == "Action")
                {

                    List<Tuple<string, string>> values = new List<Tuple<string,string>>();

                    foreach (XAttribute attrib in action.Attributes())
                    {
                        if ("Type" == attrib.Name.ToString())
                        {
                            continue;
                        }
                        values.Add(new Tuple<string, string>(attrib.Name.ToString(), attrib.Value));
                    }
                    Scene.CreateAction(evt, action.Attribute("Type").Value, values, cond);
                }
                else if (action.Name == "Condition")
                {
                    evt.AddAction(createCondition(game, evt, action));
                }
            }
        }

        internal override void CreateEntity(string name)
        {
            Entity entity = new Entity(onid);
            
            XElement prototype = xmlGameLoader.Prototypes[name];

            foreach (XAttribute attrib in prototype.Attributes())
            {
                if ("Prototype" == attrib.Name)
                {
                    if (name.Contains(' '))
                    {
                        string[] names = name.Split(' ');
                        foreach (string n in names)
                        {
                            addToAllTypes(n, onid);
                        }
                    }
                    else
                    {
                        addToAllTypes(name, onid);
                    }
                }
                else if ("Name" == attrib.Name)
                {
                    entity.Name = attrib.Value;
                    EntityByName.Add(attrib.Value, entity);
                }
                else
                {
                    entity[attrib.Name.ToString()] = attrib.Value;
                }
            }
            onid += 1;
            entity.Scene = Scene;
            entityParse(prototype, entity);
            entity.Ready();
        }

        private static void addToHashSet(int value, string name, Dictionary<string, HashSet<int>> dictionary)
        {
            HashSet<int> addTo;
            if (!dictionary.ContainsKey(name))
            {
                addTo = new HashSet<int>();
                dictionary[name] = addTo;
            }
            else
            {
                addTo = dictionary[name];
            }
            addTo.Add(value);
        }

        private void addToAllTypes(string name, int id)
        {
            HashSet<int> addTo;

            foreach (string prototypeIs in xmlGameLoader.PrototypeIs[name])
            {
                if (!IsType.ContainsKey(prototypeIs))
                {
                    addTo = new HashSet<int>();
                    IsType.Add(prototypeIs, addTo);
                }
                else
                {
                    addTo = IsType[prototypeIs];
                }
                addTo.Add(id);
            }
        }

        private void createWithPrototype(XMLGameLoader gl, string name, XElement parsedNode, int id)
        {
            XElement prototype = gl.Prototypes[name];
            XMLGameLoader.mergeXmlNodes(prototype, parsedNode);

            addToHashSet(id, name, IsType);
            addToHashSet(id, name, IsExactType);

            addToAllTypes(name, id);

        }

        private void entityParse(XElement e, Entity entity)
        {
            foreach (XElement node in e.Elements())
            {
                switch (node.Name.ToString())
                {
                    case "#comment":
                        continue;
                    case "Event":
                        Event evt = createEvent(Game, node, entity);
                        evt.Initialize();
                        break;
                    case "Component":
                        string stringType = (string)node.Attribute("Type");

                        List<Tuple<string, string>> values = new List<Tuple<string, string>>();

                        foreach (XAttribute attrib in node.Attributes())
                        {
                            if ("Type" == attrib.Name)
                            {
                                continue;
                            }
                            string value = attrib.Value;
                            string param = attrib.Name.ToString();
                            Tuple<string, string> t = new Tuple<string, string>(param, value);
                            values.Add(t);
                        }
                        Scene.CreateComponent(entity, stringType, values);
                        break;
                    default:
                        throw new Exception("Unknown type");
                }
            }
        }

        private Event createEvent(Game game, XElement node, Entity entity)
        {
            Event evt = ClassFactory.Create<Event>((string)node.Attribute("Type"));
            evt.Entity = entity;

            foreach (XAttribute attrib in node.Attributes())
            {
                if ("Type" == attrib.Name.ToString())
                {
                    continue;
                }
                string value = attrib.Value;
                string param = attrib.Name.ToString();
                ClassFactory.SetParam(evt, param, value, evt, evt.Entity);
            }
            AddActions(game, node, evt);
            return evt;
        }

        private Condition createCondition(Game game, Event e, XElement node)
        {
            Condition c = Condition.CreateCondition((string)node.Attribute("If"), e, e.Entity);
            AddActions(game, node, e, c);
            return c;
        }
    }
}
