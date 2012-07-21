using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Action = Kinectitude.Core.Base.Action;
using System.Xml.Schema;
using System.IO;
using System.Xml;

namespace Kinectitude.Core.Loaders
{
    internal class XMLLoaderUtility : LoaderUtility
    {
        private static readonly XNamespace Namespace = "http://www.kinectitude.com/2012/v1";
        
#if TEST
        internal static XmlSchemaSet schemas = new XmlSchemaSet();
#else
        private static readonly XmlSchemaSet schemas = new XmlSchemaSet();
#endif

        private readonly GameLoader gameLoader;

        internal XMLLoaderUtility(string fileName, GameLoader gameLoader) : base()
        {
            //TODO lang
            Lang = LanguageKeywords.GetLanguage("en");
            EntityType = Namespace + Lang.Entity;
            ActionType = Namespace + Lang.Action;
            ConditionType = Namespace + Lang.Condition;
            ManagerType = Namespace + Lang.Manager;
            EventType = Namespace + Lang.Event;
            TriggerType = Namespace + Lang.Trigger;
            ComponentType = Namespace + Lang.Component;
            UsingType = Namespace + Lang.Using;
            DefineType = Namespace + Lang.Define;
            PrototypeType = Namespace + Lang.Prototype;
            SceneType = Namespace + Lang.Scene;
            Stream schemaStream =
                typeof(XMLLoaderUtility).Assembly.GetManifestResourceStream("Kinectitude.Core.Loaders.schema.xsd");
            schemas.Add(null, new XmlTextReader(schemaStream));
            FileName = fileName;
            this.gameLoader = gameLoader;
        }

        internal override object Load()
        {
            XDocument document = XDocument.Load(Path.Combine(Environment.CurrentDirectory, FileName));
            XElement root = document.Root;
            document.Validate(schemas, (o, e) => { throw new ArgumentException("Invalid Kinectitude XML file."); });
            return root;
        }

        internal override void CreateWithPrototype(GameLoader gl, string name, ref object entity, int id)
        {
            XElement prototype = gameLoader.Prototypes[name] as XElement;
            mergeXmlNodes(prototype, entity as XElement);
        }

        internal override List<Tuple<string, string>> GetValues(object from, HashSet<string> ignore)
        {
            List<Tuple<string, string>> values = new List<Tuple<string, string>>();

            XElement node = from as XElement;

            foreach (XAttribute attrib in node.Attributes().Where(input => !ignore.Contains(input.Name.ToString())))
            {
                string value = attrib.Value;
                string param = attrib.Name.ToString();
                values.Add(new Tuple<string, string>(param, value));
            }
            return values;
        }

        internal override Dictionary<string, string> GetProperties(object from, HashSet<string> specialWords)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            XElement node = from as XElement;

            foreach (XAttribute attrib in node.Attributes().Where(input => specialWords.Contains(input.Name.ToString())))
            {
                string value = attrib.Value;
                string param = attrib.Name.ToString();
                values[param] = value;
            }
            return values;
        }

        internal override IEnumerable<object> GetOfType(object scene, object type)
        {
            XElement sceneNode = scene as XElement;
            return sceneNode.Elements().Where(input => type as XName == input.Name);
        }

        internal override IEnumerable<object> GetAll(object evt)
        {
            XElement eventNode = evt as XElement;
            return eventNode.Elements();
        }

        internal override bool IsAciton(object obj)
        {
            XElement node = obj as XElement;
            return node.Name == ActionType as XName;
        }

        private XElement mergeXmlNodes(XElement src, XElement dst)
        {
            //TODO merge prototype attribute and insert into the dictionary's list
            Dictionary<string, XElement> nodes = new Dictionary<string, XElement>();
            //keep track of all the nodes in the dst because they will need to be merged if they are also in src
            foreach (XElement node in dst.Elements())
            {
                if (EventType as XName == node.Name || TriggerType as XName == node.Name) continue;
                nodes.Add((string)node.Attribute(Lang.Type), node);
            }

            foreach (XAttribute attr in src.Attributes())
            {
                if (Lang.Name == attr.Name || Lang.Type == attr.Name)
                {
                    continue;
                }
                if (null == dst.Attribute(attr.Name))
                {
                    dst.SetAttributeValue(attr.Name, attr.Value);

                }
            }
            foreach (XElement node in src.Elements(ComponentType as XName))
            {
                string key = (string)node.Attribute(Lang.Type);
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
            foreach (XElement node in src.Elements(EventType as XName))
            {
                string key = (string)node.Attribute(Lang.Type);
                XElement copy = new XElement(node);
                dst.Add(copy);
            }
            return dst;
        }

        internal override void MergePrototpye(ref object newPrototype, string myName, string mergeWith)
        {
            XElement prototype = gameLoader.Prototypes[mergeWith] as XElement;
            XElement newXml = newPrototype as XElement;
            mergeXmlNodes(prototype, newXml);
        }
    }
}