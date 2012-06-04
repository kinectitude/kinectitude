using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Kinectitude.Core.Loaders
{
    internal class XMLGameLoader : GameLoader
    {
        internal const string EventName = "Event";
        internal const string TriggerName = "Trigger";
        internal const string ComponentName = "Component";
        internal const string UsingName = "Using";
        internal const string DefineName = "Define";
        internal const string PrototypeName = "Prototype";
        internal const string SceneName = "Scene";

        internal Dictionary<string, XElement> Prototypes { get; private set; }
        
        private Dictionary<string, XElement> scenes = new Dictionary<string, XElement>();

        private void mergePrototpye(XElement newPrototype, string myName, string mergeWith)
        {
            XElement prototype = Prototypes[mergeWith];
            XMLGameLoader.mergeXmlNodes(prototype, newPrototype);
            foreach (string isPrototype in PrototypeIs[mergeWith])
            {
                if (!PrototypeIs[myName].Contains(isPrototype))
                {
                    PrototypeIs[myName].Add(isPrototype);
                }
            }
        }

        internal XMLGameLoader(string fileName)
        {
            Prototypes = new Dictionary<string, XElement>();
            XElement root = XElement.Load(Path.Combine(Environment.CurrentDirectory, fileName));
            IEnumerable<XAttribute> rootAttribs = root.Attributes();
            foreach (XAttribute attrib in rootAttribs)
            {
                if (attrib.Name.ToString() == "FirstScene")
                {
                    FirstScene = attrib.Value;
                }
                Game[attrib.Name.ToString()] = attrib.Value;
            }

            foreach (XElement node in root.Elements().Where(input => UsingName == input.Name))
            {
                string loadName = (string)node.Attribute("File");
                foreach (XElement defined in node.Elements().Where(input => DefineName == input.Name))
                {
                    LoadReflection(loadName, (string)defined.Attribute("Name"),
                        (string)defined.Attribute("Class"));
                }
            }

            foreach (XElement node in root.Elements().Where(input => PrototypeName == input.Name))
            {
                string myName = (string)node.Attribute("Name");
                PrototypeIs[myName] = new List<string>();
                PrototypeIs[myName].Add(myName);
                if (null != node.Attribute("Prototype"))
                {
                    string name = (string)node.Attribute("Prototype");
                    name = name.Trim();
                    if (name.Contains(' '))
                    {
                        string[] names = name.Split(' ');
                        foreach (string n in names)
                        {
                            mergePrototpye(node, myName, n);
                        }
                    }
                    else
                    {
                        mergePrototpye(node, myName, name);
                    }
                }
                Prototypes.Add(myName, node);
                AvaliblePrototypes.Add(myName);
            }

            foreach (XElement node in root.Elements().Where(input => SceneName == input.Name))
            {
                scenes[(string)node.Attribute("Name")] = node;
            }
        }

        internal override SceneLoader GetSceneLoader(string name)
        {
            return new XMLSceneLoader(scenes[name], this);
        }

        internal static XElement mergeXmlNodes(XElement src, XElement dst)
        {
            //TODO merge prototype attribute and insert into the dictionary's list
            Dictionary<string, XElement> nodes = new Dictionary<string, XElement>();
            //keep track of all the nodes in the dst because they will need to be merged if they are also in src
            foreach (XElement node in dst.Elements())
            {
                if (EventName == node.Name  || TriggerName == node.Name)
                {
                    continue;
                }
                nodes.Add((string)node.Attribute("Type"), node);
            }
            foreach (XAttribute attr in src.Attributes())
            {
                if ("Name" == attr.Name || "Type" == attr.Name)
                {
                    continue;
                }
                if (null == dst.Attribute(attr.Name))
                {
                    dst.SetAttributeValue(attr.Name, attr.Value);

                }
            }
            foreach (XElement node in src.Elements(ComponentName))
            {
                string key = (string)node.Attribute("Type");
                if (nodes.ContainsKey(key))
                {
                    mergeXmlNodes(node, nodes[key]);
                }
                else
                {
                    XElement copy = new XElement(node);
                    dst.AddFirst(copy);
                }
            }
            foreach (XElement node in src.Elements(EventName))
            {
                string key = (string)node.Attribute("Type");
                XElement copy = new XElement(node);
                dst.Add(copy);
            }
            return dst;
        }
    }
}
