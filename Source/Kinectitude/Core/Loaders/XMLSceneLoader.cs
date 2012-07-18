using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Loaders
{
    internal class XMLSceneLoader : SceneLoader
    {        
        internal static readonly XName EntityName = XMLGameLoader.Namespace + "Entity";
        internal static readonly XName ActionName = XMLGameLoader.Namespace + "Action";
        internal static readonly XName ConditionName = XMLGameLoader.Namespace + "Condition";
        internal static readonly XName ManagerName = XMLGameLoader.Namespace + "Manager";

        XMLGameLoader xmlGameLoader;

        internal XMLSceneLoader(XElement scene, XMLGameLoader gl):base(gl)
        {
            xmlGameLoader = gl;

            string sceneName = "";
            List<Tuple<string, string>> sceneValues = new List<Tuple<string, string>>();

            foreach (XAttribute attrib in scene.Attributes())
            {
                string attribName = attrib.Name.ToString();
                if ("Name" == attribName) sceneName = attrib.Value;
                else sceneValues.Add(new Tuple<string, string>(attribName, attrib.Value));
            }

            LoadedScene = new LoadedScene(sceneName, sceneValues, this, gl.Game);

            foreach (XElement e in scene.Elements().Where(input => ManagerName == input.Name))
            {
                string managerName = (string)e.Attribute("Type");
                LoadedScene.addManagerName(managerName);
                LoadedManager.GetLoadedManager(managerName, LoadedScene, gatherTuples(e));
            }

            foreach (XElement e in scene.Elements().Where(input => EntityName == input.Name))
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
                        foreach (string n in names) createWithPrototype(gl, n, parsedNode, Onid);
                    }
                    else
                    {
                        createWithPrototype(gl, name, parsedNode, Onid);
                    }
                }

                string entityName = null;
                List<Tuple<string, string>> entityValues = new List<Tuple<string, string>>();

                foreach (XAttribute attrib in parsedNode.Attributes())
                {
                    if ("Prototype" == attrib.Name) continue;
                    else if ("Name" == attrib.Name) entityName = attrib.Value;
                    else entityValues.Add(new Tuple<string, string>(attrib.Name.ToString(), attrib.Value));
                }

                LoadedEntity entity = new LoadedEntity(entityName, entityValues, Onid++);
                LoadedScene.addLoadedEntity(entity);
                
                entityParse(parsedNode, entity);
            }            
        }

        //Adds actions to an event or trigger
        internal void AddActions(Game game, XElement node, LoadedEvent evt, LoadedCondition cond = null)
        {
            foreach (XElement action in node.Elements())
            {
                if (action.Name == ActionName)
                {
                    LoadedAction loadedAction = new LoadedAction(action.Attribute("Type").Value, gatherTuples(action));

                    if (null != cond) cond.AddAction(loadedAction);
                    else evt.AddAction(loadedAction);
                }
                else if (action.Name == ConditionName)
                {
                    evt.AddAction(createCondition(game, evt, action));
                }
            }
        }

        protected override LoadedEntity PrototypeMaker(string name)
        {
            XElement prototype = xmlGameLoader.Prototypes[name];

            List<Tuple<string, string>> values = new List<Tuple<string, string>>();
            string entityName = null;

            foreach (XAttribute attrib in prototype.Attributes())
            {
                if ("Prototype" == attrib.Name)
                {
                    if (name.Contains(' '))
                    {
                        string[] names = name.Split(' ');
                        foreach (string n in names) addToAllTypes(n, Onid);
                    }
                    else
                    {
                        addToAllTypes(name, Onid);
                    }
                }
                else if ("Name" == attrib.Name)
                {
                    continue;
                }
                else
                {
                    values.Add(new Tuple<string,string>(attrib.Name.ToString(), attrib.Value));
                }
            }

            LoadedEntity entity = new LoadedEntity(entityName, values, Onid);
            entityParse(prototype, entity);
            return entity;
        }

        private void createWithPrototype(XMLGameLoader gl, string name, XElement parsedNode, int id)
        {
            XElement prototype = gl.Prototypes[name];
            XMLGameLoader.mergeXmlNodes(prototype, parsedNode);   
        }

        private static List<Tuple<string, string>> gatherTuples(XElement node)
        {
            List<Tuple<string, string>> values = new List<Tuple<string, string>>();

            foreach (XAttribute attrib in node.Attributes())
            {
                if ("Type" == attrib.Name) continue;
                string value = attrib.Value;
                string param = attrib.Name.ToString();
                values.Add(new Tuple<string, string>(param, value));
            }
            return values;
        }

        private void entityParse(XElement e, LoadedEntity entity)
        {
            foreach (XElement node in e.Elements().Where(element => element.Name == XMLGameLoader.ComponentName))
            {
                string stringType = (string)node.Attribute("Type");
                LoadedComponent lc = new LoadedComponent(stringType, gatherTuples(node));
                entity.AddLoadedComponent(lc);
            }

            foreach (XElement node in e.Elements().Where(element => element.Name == XMLGameLoader.EventName))
                createEvent(Game, node, entity);
        }

        private LoadedEvent createEvent(Game game, XElement node, LoadedEntity entity)
        {
            LoadedEvent loadedEvt = new LoadedEvent((string)node.Attribute("Type"), gatherTuples(node), entity);
            AddActions(game, node, loadedEvt);
            return loadedEvt;
        }

        private LoadedCondition createCondition(Game game, LoadedEvent e, XElement node)
        {
            LoadedCondition lc = new LoadedCondition((string)node.Attribute("If"));
            AddActions(game, node, e, lc);
            return lc;
        }
    }
}