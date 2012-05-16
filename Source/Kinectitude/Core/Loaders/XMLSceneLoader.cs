using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Kinectitude.Core.Base;
using Action = Kinectitude.Core.Base.Action;

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
                        //TODO do the prototype here.
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
                        Event evt = createEvent(Game, node);
                        evt.Initialize(Scene, entity);
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
                            Tuple<string, string> t = new Tuple<string,string>(value, param);
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
                    Action actionObj =
                        (Action)game.CreateFromReflection(action.Attribute("type").Value, 
                        new Type[] { },new object[] { });
                    actionObj.Event = evt;
                    if (null == cond)
                    {
                        evt.AddAction(actionObj);
                    }
                    else
                    {
                        cond.AddAction(actionObj);
                    }
                    foreach (XAttribute attrib in action.Attributes())
                    {
                        if ("type" == attrib.Name.ToString())
                        {
                            continue;
                        }
                        string value = attrib.Value;
                        string param = attrib.Name.ToString();
                        game.SetParam(actionObj, value, param, evt, this);
                    }
                }
                else if (action.Name == "condition")
                {
                    evt.AddAction(createCondition(game, evt, action));
                }
            }
        }

        //merges src and dst with results in dst.  dst is returned
        private Event createEvent(Game game, XElement node)
        {
            Event evt = (Event)game.CreateFromReflection((string)node.Attribute("type"),
                new Type[] { }, new object[] { });

            foreach (XAttribute attrib in node.Attributes())
            {
                if ("type" == attrib.Name.ToString())
                {
                    continue;
                }
                string value = attrib.Value;
                string param = attrib.Name.ToString();
                game.SetParam(evt, value, param);
            }
            addActions(game, node, evt);
            return evt;
        }

        private Condition createCondition(Game game, Event e, XElement node)
        {
            Condition c = Condition.CreateCondition((string)node.Attribute("if"), e, this);
            addActions(game, node, e, c);
            return c;
        }
    }
}
