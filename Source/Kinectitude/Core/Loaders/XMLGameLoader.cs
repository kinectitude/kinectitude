using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Kinectitude.Core.Loaders
{
    internal class XMLGameLoader : GameLoader
    {
        internal Dictionary<string, XElement> Prototypes { get; private set; }
        internal Dictionary<string, List<string>> PrototypeIs { get; private set; }
        private Dictionary<string, XElement> scenes = new Dictionary<string, XElement>();

        private void mergePrototpye(XElement newPrototype, string myName, string mergeWith)
        {
            XElement prototype = Prototypes[mergeWith];
            XMLGameLoader.mergeXmlNodes(prototype, newPrototype);
            foreach (string isPrototype in PrototypeIs[mergeWith])
            {
                if (!PrototypeIs[myName].Contains(mergeWith))
                {
                    PrototypeIs[myName].Add(mergeWith);
                }
            }
        }

        internal XMLGameLoader(string fileName)
        {
            Prototypes = new Dictionary<string, XElement>();
            PrototypeIs = new Dictionary<string, List<string>>();
            XElement root = XElement.Load(Path.Combine(Environment.CurrentDirectory, fileName));
            IEnumerable<XAttribute> rootAttribs = root.Attributes();
            foreach (XAttribute attrib in rootAttribs)
            {
                Game[attrib.Name.ToString()] = attrib.Value;
            }
            
            foreach (XElement node in root.Elements())
            {
                if ("define" == node.Name)
                {
                    LoadReflection((string)node.Attribute("file"), (string)node.Attribute("part"),
                        (string)node.Attribute("fullName"));
                }
                else if ("scene" == node.Name)
                {
                    scenes[(string)node.Attribute("name")] = node;
                }
                else if ("prototype" == node.Name)
                {
                    string myName = (string)node.Attribute("name");
                    PrototypeIs[myName] = new List<string>();
                    PrototypeIs[myName].Add(myName);
                    if (null != node.Attribute("prototype"))
                    {
                        string name = (string)node.Attribute("prototype");
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
                }
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
                if ("event" == node.Name  || "trigger" == node.Name)
                {
                    continue;
                }
                nodes.Add((string)node.Attribute("type"), node);
            }
            foreach (XAttribute attr in src.Attributes())
            {
                if ("name" == attr.Name || "type" == attr.Name)
                {
                    continue;
                }
                if (null == dst.Attribute(attr.Name))
                {
                    dst.SetAttributeValue(attr.Name, attr.Value);

                }
            }
            foreach (XElement node in src.Elements("component"))
            {
                string key = (string)node.Attribute("type");
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
            foreach (XElement node in src.Elements("event"))
            {
                string key = (string)node.Attribute("type");
                XElement copy = new XElement(node);
                dst.Add(copy);
            }
            return dst;
        }
    }
}
