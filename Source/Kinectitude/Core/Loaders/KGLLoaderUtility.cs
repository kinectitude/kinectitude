using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Language;
using Irony.Parsing;
using Kinectitude.Core.Data;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    internal sealed class KGLLoaderUtility : LoaderUtility
    {
        private readonly ParseTreeNode Root;
        private readonly KinectitudeGrammar grammar = new KinectitudeGrammar();

        internal KGLLoaderUtility(string fileName, GameLoader gameLoader)
        {
            Parser parser = new Parser(grammar);
            string src = System.IO.File.ReadAllText(fileName);
            ParseTree parseTree = parser.Parse(src, fileName);
            if (parseTree.HasErrors())
            {
                //TODO
            }
            Root = parseTree.Root;
        }

        public object EntityType { get { return KinectitudeGrammar.Entities; } }
        public object ActionType { get { return KinectitudeGrammar.Actions; } }
        public object ConditionType { get { return KinectitudeGrammar.Condition; } }
        public object ManagerType { get { return KinectitudeGrammar.Managers; } }
        public object EventType { get { return KinectitudeGrammar.Evts; } }
        public object ComponentType { get { return KinectitudeGrammar.Components; } }
        public object UsingType { get { return KinectitudeGrammar.Uses; } }
        public object DefineType { get { return KinectitudeGrammar.Definitions; } }
        public object PrototypeType { get { return KinectitudeGrammar.Prototypes; } }
        public object SceneType { get { return KinectitudeGrammar.Scenes; } }
        public object GetGame() { return Root; }

        public PropertyHolder GetProperties(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;

            PropertyHolder propertyHolder = new PropertyHolder();

            //there should only be one because they are nested.
            for (ParseTreeNode properties = node.ChildNodes.Where(child => child.ToString() == KinectitudeGrammar.Properties.Name)
                .First(); properties.ChildNodes.Count == 3; properties = properties.ChildNodes[2])
            {
                propertyHolder.AddValue(properties.ChildNodes[0].Token.Value.ToString(), properties.ChildNodes[1]);
            }

            return propertyHolder;
        }

        public string GetName(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            return node.ChildNodes.Where(child => child.ToString() == KinectitudeGrammar.Name.Name).
                First().Token.Value.ToString();
        }

        public IEnumerable<object> GetOfType(object from, object type)
        {
            NonTerminal gatherType = type as NonTerminal;
            List<ParseTreeNode> nodes = new List<ParseTreeNode>();
            //Since pluarals are nested there is only one child with the correct name)
            for (ParseTreeNode node = from as ParseTreeNode; node.ChildNodes.Count != 0;
                node = node.ChildNodes.Where(child => child.ToString() == gatherType.Name).First())
                nodes.Add(node);
            return nodes;
        }

        public string GetFile(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            return node.ChildNodes.Where(child => child.ToString() == KinectitudeGrammar.File.Name).First().Token.Value.ToString();
        }

        public bool IsAciton(object obj)
        {
            ParseTreeNode node = obj as ParseTreeNode;
            return node.ToString() == KinectitudeGrammar.Action.Name;
        }

        public IEnumerable<string> GetPrototypes(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            ParseTreeNode entityDef = node.ChildNodes.Where(child => child.ToString() == KinectitudeGrammar.Prototypes.Name).First();
            List<string> prototypes = new List<string>();

            for (ParseTreeNode names = entityDef.ChildNodes.Where(child => child.ToString() == KinectitudeGrammar.Names.Name)
                .First(); names != null && names.ChildNodes.Count != 0; names = names.ChildNodes[1])
            {
                prototypes.Add(names.Token.Value.ToString());
            }


            return prototypes;
        }

        public string GetType(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            return node.ChildNodes[0].Token.Value.ToString();
        }

        private ValueReader uniOpCreate(string op, ValueReader value)
        {
            if (grammar.Not.Name == op) return new NotOpReader(value);
            if (grammar.Minus.Name == op) return new NegOpReader(value);
            throw new NotImplementedException("Error with implementation of operator " + op);
        }

        private ValueReader binOpCreate(string op, ValueReader left, ValueReader right)
        {
            if (grammar.Eql.Name == op) return new EqlOpReader(left, right);
            if (grammar.Lt.Name == op) return new GtOpReader(right, left);
            if (grammar.Gt.Name == op) return new GtOpReader(left, right);
            if (grammar.Le.Name == op) return new GeOpReader(right, left);
            if (grammar.Ge.Name == op) return new GeOpReader(left, right);
            if (grammar.Neq.Name == op) return new NeqOpReader(left, right);
            if (grammar.Plus.Name == op) return new PlusOpReader(left, right);
            if (grammar.Minus.Name == op) return new MinusOpReader(left, right);
            if (grammar.Mult.Name == op) return new MultOpReader(left, right);
            if (grammar.Rem.Name == op) return new RemOpReader(left, right);
            if (grammar.Pow.Name == op) return new PowOpReader(left, right);
            if (grammar.And.Name == op) return new PowOpReader(left, right);
            if (grammar.Or.Name == op) return new OrOpReader(left, right);
            if (grammar.LeftShift.Name == op) return new LeftShiftOpReader(left, right);
            if (grammar.RightShift.Name == op) return new RightShiftOpReader(left, right);
            throw new NotImplementedException("Error with implementation of operator " + op);
        }

        private DataContainer getDataContainer(Scene scene, Entity entity, string name)
        {
            //TODO ask group about making the name of scene and game the keyword instead of game and scene.  Also allow this in game/scene?
            /*TODO if I have an entity created later, and it's in an action/evt/component should I be able to reference it?
             * Makes it difficult if I can't, but idk how to program this better if I can*/
            switch (name)
            {
                case "this":
                    return entity;
                case "scene":
                    return scene;
                case "game":
                    return scene.Game;
                default:
                    if(scene.EntityByName.ContainsKey(name)) return scene.EntityByName[name];
                    return null;
            }
        }

        private ValueReader makeParameterReader(Scene scene, Entity entity, string name, string component, string param)
        {
            DataContainer dataContainer = getDataContainer(scene, entity, name);
            object obj = dataContainer.GetComponentOrManager(component);
            return new ParameterValueReader(obj, param, scene);
        }

        private ValueReader makeDcReader(Scene scene, Entity entity, string name, string param)
        {
            DataContainer dataContainer = getDataContainer(scene, entity, name);
            return new DataContainerReader(dataContainer, param);
        }

        private ValueReader makeValueReader(ParseTreeNode node, Scene scene, Entity entity, Event evt)
        {
            string child0 = node.ChildNodes[0].ToString();
            
            if (child0 == KinectitudeGrammar.UniOp.Name)
            {
                ValueReader value = makeValueReader(node.ChildNodes[1], scene, entity, evt);
                return uniOpCreate(node.ChildNodes[0].ChildNodes[0].Term.Name, value);
            }
            else if (child0 == KinectitudeGrammar.Number.Name)
            {
                return new ConstantReader(node.ChildNodes[0].Token.Value);
            }
            else if (child0 == KinectitudeGrammar.ThreeVal.Name)
            {
                ParseTreeNodeList list = node.ChildNodes[0].ChildNodes;
                makeParameterReader(scene, entity, list[0].Token.ValueString, 
                    list[1].Token.ValueString, list[2].Token.ValueString);
            }
            else if (child0 == KinectitudeGrammar.TwoVal.Name)
            {
                ParseTreeNodeList list = node.ChildNodes[0].ChildNodes;
                if (null == getDataContainer(scene, entity, list[0].Token.ValueString)) 
                    return makeParameterReader(scene, entity, "this", list[0].Token.ValueString, list[1].Token.ValueString);
                return makeDcReader(scene, entity, list[0].Token.ValueString, list[1].Token.ValueString);
            }
            else if (child0 == KinectitudeGrammar.Name.Name)
            {
                return makeDcReader(scene, entity, "this", node.ChildNodes[0].ChildNodes[2].Token.ValueString);
            }
            else if (child0 == KinectitudeGrammar.ParentVal.Name)
            {
                //TODO
            }
            else if (node.ChildNodes.Count == 3 && node.ChildNodes[1].ToString() == KinectitudeGrammar.BinOp.Name)
            {
                ValueReader left = makeValueReader(node.ChildNodes[0], scene, entity, evt);
                ValueReader right = makeValueReader(node.ChildNodes[2], scene, entity, evt);
                return binOpCreate(node.ChildNodes[1].ChildNodes[0].Term.Name, left, right);
            }

            throw new NotImplementedException("Error with implementation of operator");
        }

        private TypeMatcher makeTypeMatcherHelper(ParseTreeNode node, Scene scene)
        {
            if (node.ToString() == KinectitudeGrammar.IsType.Name)
                return new PrototypeTypeMatcher(scene.GetOfPrototype(node.ChildNodes[0].Token.ToString(), false));
            
            if (node.ToString() == KinectitudeGrammar.IsExactType.Name)
                return new PrototypeTypeMatcher(scene.GetOfPrototype(node.ChildNodes[0].Token.ToString(), true));

            return new SingleTypeMatcher(scene.EntityByName[node.ChildNodes[0].Token.ToString()]);
        }

        private TypeMatcher makeTypeMatcher(ParseTreeNode node, Scene scene)
        {
            if (node.ChildNodes.Count == 1)
            {
                return makeTypeMatcherHelper(node.ChildNodes[0], scene);
            }
            else
            {
                List<TypeMatcher> readables = new List<TypeMatcher>();
                for (; node.ChildNodes.Count != 1; node = node.ChildNodes[2])
                {
                    readables.Add(makeTypeMatcherHelper(node.ChildNodes[0], scene));
                    node = node.ChildNodes[2];
                }
                readables.Add(makeTypeMatcherHelper(node.ChildNodes[0], scene));
                return new ListedTypeMatcher(readables);
            }
        }

        public IAssignable MakeAssignable(object obj, Scene scene, Entity entity, Event evt)
        {
            ParseTreeNode node = obj as ParseTreeNode;

            string type = node.ToString();

            if (type == KinectitudeGrammar.TypeMatcher.Name) return makeTypeMatcher(node, scene);
            return makeValueReader(node, scene, entity, evt);
        }

        public IEnumerable<Tuple<string, string>> GetDefines(object from)
        {
            List<Tuple<string, string>> definitions = new List<Tuple<string, string>>();
            ParseTreeNode node = from as ParseTreeNode;
            
            return definitions;
        }

        public object GetCondition(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            return node.ChildNodes[0];
        }
    }
}
