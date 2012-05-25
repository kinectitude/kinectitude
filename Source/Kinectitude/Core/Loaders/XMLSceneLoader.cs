using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Kinectitude.Core.Base;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    internal class XMLSceneLoader : SceneLoader
    {
        private void createWithPrototype(XMLGameLoader gl, string name, XElement parsedNode, int id)
        {
            XElement prototype = gl.Prototypes[name];
            XMLGameLoader.mergeXmlNodes(prototype, parsedNode);
            HashSet<int> addTo;
            if (!IsType.ContainsKey(name))
            {
                addTo = new HashSet<int>();
                IsType[name] = addTo;
            }
            else
            {
                addTo = IsType[name];
            }
            addTo.Add(id);
            if (!IsExactType.ContainsKey(name))
            {
                addTo = new HashSet<int>();
                IsExactType[name] = addTo;
            }
            else
            {
                addTo = IsExactType[name];
            }
            addTo.Add(id);
            foreach (string prototypeIs in gl.PrototypeIs[name])
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

        public XMLSceneLoader(XElement scene, XMLGameLoader gl):base(gl)
        {
            Scene = new Scene(this, gl.Game);
            foreach (XAttribute attrib in scene.Attributes())
            {
                string attribName = attrib.Name.ToString();
                if("name" == attribName)
                {
                    Scene.Name = attrib.Value;
                }
                Scene[attribName] = attrib.Value;
            }
            int onid = 0;
            foreach (XElement e in scene.Elements())
            {
                //so that I don't mess the original up when I merge;
                XElement parsedNode = new XElement(e);
                if (e.Name== "entity")
                {
                    //insert the prototype into the original
                    if (null != e.Attribute("prototype"))
                    {
                        string name = (string)e.Attribute("prototype");
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
                        if ("prototype" == attrib.Name)
                        {
                            continue;
                        }
                        else if ("name" == attrib.Name)
                        {
                            entity.Name = attrib.Value;
                            EntityByName.Add(attrib.Value, entity);
                        }
                        else
                        {
                            entity[attrib.Name.ToString()] = attrib.Value;
                        }
                    }
                    entity.Scene = Scene;
                    entityParse(parsedNode, entity);
                    entity.Ready();
                }
            }            
        }

        private void entityParse(XElement e, Entity entity)
        {
            foreach (XElement node in e.Elements())
            {
                switch (node.Name.ToString())
                {
                    case "#comment":
                        continue;
                    case "event":
                        Event evt = createEvent(Game, node, entity);
                        evt.Initialize();
                        break;
                    case "component":
                        string stringType = (string)node.Attribute("type");

                        List<Tuple<string, string>> values = new List<Tuple<string,string>>();

                        foreach (XAttribute attrib in node.Attributes())
                        {
                            if ("type" == attrib.Name)
                            {
                                continue;
                            }
                            string value = attrib.Value;
                            string param = attrib.Name.ToString();
                            Tuple<string, string> t = new Tuple<string,string>(param, value);
                            values.Add(t);
                        }
                        Scene.CreateComponent(entity, stringType, values);
                        break;
                    default:
                        throw new Exception("Unknown type");
                }
            }
        }

        //Adds actions to an event or trigger
        internal void addActions(Game game, XElement node, Event evt, Condition cond = null)
        {
            foreach (XElement action in node.Elements())
            {
                if (action.Name == "action")
                {

                    List<Tuple<string, string>> values = new List<Tuple<string,string>>();

                    foreach (XAttribute attrib in action.Attributes())
                    {
                        if ("type" == attrib.Name.ToString())
                        {
                            continue;
                        }
                        values.Add(new Tuple<string, string>(attrib.Name.ToString(), attrib.Value));
                    }
                    Scene.CreateAction(evt, action.Attribute("type").Value, values, cond);
                }
                else if (action.Name == "condition")
                {
                    evt.AddAction(createCondition(game, evt, action));
                }
            }
        }

        private Event createEvent(Game game, XElement node, Entity entity)
        {
            Event evt = ClassFactory.Create<Event>((string)node.Attribute("type"));
            evt.Entity = entity;

            foreach (XAttribute attrib in node.Attributes())
            {
                if ("type" == attrib.Name.ToString())
                {
                    continue;
                }
                string value = attrib.Value;
                string param = attrib.Name.ToString();
                ClassFactory.SetParam(evt, param, value, evt, evt.Entity);
            }
            addActions(game, node, evt);
            return evt;
        }

        private Condition createCondition(Game game, Event e, XElement node)
        {
            Condition c = Condition.CreateCondition((string)node.Attribute("if"), e, e.Entity);
            addActions(game, node, e, c);
            return c;
        }
    }
}
