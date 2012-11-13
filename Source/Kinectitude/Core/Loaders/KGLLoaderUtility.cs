using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Language;
using Irony.Parsing;
using Kinectitude.Core.Data;
using Kinectitude.Core.Base;
using System.IO;

namespace Kinectitude.Core.Loaders
{
    internal sealed class KGLLoaderUtility : LoaderUtility
    {
        private readonly ParseTreeNode Root;
        private readonly KinectitudeGrammar grammar = new KinectitudeGrammar();

        public object EntityType { get { return KinectitudeGrammar.Entity; } }
        public object ActionType { get { return KinectitudeGrammar.Actions; } }
        public object ConditionType { get { return KinectitudeGrammar.Condition; } }
        public object ManagerType { get { return KinectitudeGrammar.Manager; } }
        public object EventType { get { return KinectitudeGrammar.Evt; } }
        public object ComponentType { get { return KinectitudeGrammar.Component; } }
        public object UsingType { get { return KinectitudeGrammar.Uses; } }
        public object DefineType { get { return KinectitudeGrammar.Definitions; } }
        public object PrototypeType { get { return KinectitudeGrammar.Prototype; } }
        public object SceneType { get { return KinectitudeGrammar.Scene; } }
        public object GetGame() { return Root; }

        internal KGLLoaderUtility(string fileName, GameLoader gameLoader)
        {
            Parser parser = new Parser(grammar);
            string src = File.ReadAllText(fileName);
            ParseTree parseTree = parser.Parse(src, fileName);
            if (parseTree.HasErrors())
            {
                //TODO
            }
            Root = parseTree.Root;
        }

        public PropertyHolder GetProperties(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;

            PropertyHolder propertyHolder = new PropertyHolder();

            if (KinectitudeGrammar.Prototype == node.Term || KinectitudeGrammar.Entity == node.Term)
            {
                foreach (ParseTreeNode entityDef in GetOfType(from, KinectitudeGrammar.EntityDefinition))
                {
                    foreach (ParseTreeNode property in GetOfType(entityDef, KinectitudeGrammar.Properties))
                        propertyHolder.AddValue(property.ChildNodes[0].Token.ValueString, property.ChildNodes[1]);
                }
            }
            else
            {
                foreach (ParseTreeNode property in GetOfType(from, KinectitudeGrammar.Properties))
                    propertyHolder.AddValue(property.ChildNodes[0].Token.ValueString, property.ChildNodes[1]);
            }

            return propertyHolder;
        }

        public string GetName(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            ParseTreeNode nameNode = node.ChildNodes.FirstOrDefault(child => child.Term == KinectitudeGrammar.Name);
            if (nameNode == null) return null;
            return nameNode.Token.ValueString;
        }

        public IEnumerable<object> GetOfType(object from, object type)
        {
            List<ParseTreeNode> nodes = new List<ParseTreeNode>();
            ParseTreeNode node = from as ParseTreeNode;
            NonTerminal nonTerm = type as NonTerminal;
            if (nonTerm == SceneType || nonTerm == EntityType || nonTerm == ComponentType || nonTerm == EventType)
            {
                List<ParseTreeNode> firstType = new List<ParseTreeNode>();
                NonTerminal pluralType = nonTerm == SceneType ? KinectitudeGrammar.Scenes :
                    nonTerm == ComponentType || nonTerm == EventType? KinectitudeGrammar.EntityDefinition : 
                        KinectitudeGrammar.Entities;

                getOfTypeHelper(node, pluralType, firstType);
                foreach (ParseTreeNode singular in firstType) getOfTypeHelper(singular, nonTerm, nodes);
            }
            else if (nonTerm == ActionType)
            {
                List<ParseTreeNode> firstType = new List<ParseTreeNode>();
                HashSet<NonTerminal> valids = new HashSet<NonTerminal>();
                valids.Add(KinectitudeGrammar.Action);
                valids.Add(KinectitudeGrammar.Condition);
                getOfTypeHelper(node, nonTerm, firstType);
                foreach (ParseTreeNode singular in firstType) getOfTypeHelper(singular, valids, nodes);
            }
            else
            {
                getOfTypeHelper(node, nonTerm, nodes);
            }
            return nodes;
        }

        public string GetFile(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            return node.ChildNodes.First(child => child.Term == KinectitudeGrammar.ClassName).Token.ValueString;
        }

        public bool IsAciton(object obj)
        {
            ParseTreeNode node = obj as ParseTreeNode;
            return node.Term == KinectitudeGrammar.Action;
        }

        public IEnumerable<string> GetPrototypes(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            List<string> prototypeNames = new List<string>();
            node = node.ChildNodes.First(child => child.Term == KinectitudeGrammar.EntityDefinition);
            List<ParseTreeNode> names = new List<ParseTreeNode>();
            getOfTypeHelper(node, KinectitudeGrammar.Names, names);
            //Names should only have name as a child EVER.
            foreach (ParseTreeNode name in names)prototypeNames.Add(name.ChildNodes[0].Token.ValueString);
            return prototypeNames;
        }

        public string GetType(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            if (node.Term == KinectitudeGrammar.Actions) node = node.ChildNodes.First(child => child.Term == KinectitudeGrammar.Action);
            return node.ChildNodes[0].Token.ValueString;
        }

        private ValueReader uniOpCreate(BnfTerm op, ValueReader value)
        {
            if (grammar.Not == op) return new NotOpReader(value);
            if (grammar.Minus == op) return new NegOpReader(value);
            throw new NotImplementedException("Error with implementation of operator " + op.Name);
        }

        private ValueReader binOpCreate(BnfTerm op, ValueReader left, ValueReader right)
        {
            if (grammar.Eql == op) return new EqlOpReader(left, right);
            if (grammar.Lt == op) return new GtOpReader(right, left);
            if (grammar.Gt == op) return new GtOpReader(left, right);
            if (grammar.Le == op) return new GeOpReader(right, left);
            if (grammar.Ge == op) return new GeOpReader(left, right);
            if (grammar.Neq == op) return new NeqOpReader(left, right);
            if (grammar.Plus == op) return new PlusOpReader(left, right);
            if (grammar.Minus == op) return new MinusOpReader(left, right);
            if (grammar.Mult == op) return new MultOpReader(left, right);
            if (grammar.Rem == op) return new RemOpReader(left, right);
            if (grammar.Pow == op) return new PowOpReader(left, right);
            if (grammar.And == op) return new AndOpReader(left, right);
            if (grammar.Or == op) return new OrOpReader(left, right);
            if (grammar.LeftShift == op) return new LeftShiftOpReader(left, right);
            if (grammar.RightShift == op) return new RightShiftOpReader(left, right);
            if(grammar.Div == op) return new DivOpReader(left, right);
            throw new NotImplementedException("Error with implementation of operator " + op.Name);
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
                    if (scene.EntityByName.ContainsKey(name)) return scene.EntityByName[name];
                    return null;
            }
        }

        private ValueReader makeParameterReader(Scene scene, Entity entity, string name, string component, string param)
        {
            object obj = getDataContainer(scene, entity, name).GetComponentOrManager(component);
            return ParameterValueReader.getParameterValueReader(obj, param, scene);
        }

        private ValueReader makeDcReader(Scene scene, Entity entity, string name, string param)
        {
            DataContainer dataContainer = getDataContainer(scene, entity, name);
            return DataContainerReader.getDataContainerReader(dataContainer, param);
        }

        private ValueReader makeValueReader(ParseTreeNode node, Scene scene, Entity entity, Event evt)
        {
            BnfTerm child0 = node.ChildNodes[0].Term;
            
            if (child0 == KinectitudeGrammar.UniOp)
            {
                ValueReader value = makeValueReader(node.ChildNodes[1], scene, entity, evt);
                return uniOpCreate(node.ChildNodes[0].ChildNodes[0].Term, value);
            }
            else if (child0 == KinectitudeGrammar.Number)
            {
                return new ConstantReader(node.ChildNodes[0].Token.Value);
            }
            else if (child0 == KinectitudeGrammar.ThreeVal)
            {
                ParseTreeNodeList list = node.ChildNodes[0].ChildNodes;
                return makeParameterReader(scene, entity, list[0].Token.ValueString, 
                    list[1].Token.ValueString, list[2].Token.ValueString);
            }
            else if (child0 == KinectitudeGrammar.TwoVal)
            {
                ParseTreeNodeList list = node.ChildNodes[0].ChildNodes;
                if (null == getDataContainer(scene, entity, list[0].Token.ValueString)) 
                    return makeParameterReader(scene, entity, "this", list[0].Token.ValueString, list[1].Token.ValueString);
                return makeDcReader(scene, entity, list[0].Token.ValueString, list[1].Token.ValueString);
            }
            else if (child0 == KinectitudeGrammar.Name)
            {
                return makeDcReader(scene, entity, "this", node.ChildNodes[0].Token.ValueString);
            }
            else if (child0 == KinectitudeGrammar.ParentVal)
            {
                ParseTreeNode make = node.ChildNodes[0];
                switch (make.ChildNodes.Count)
                {
                    case 2:
                        return TypeMatcherDCReader.getTypeMatcherDCValueReader(evt, make.ChildNodes[0].Token.ValueString, 
                            make.ChildNodes[1].Token.ValueString, entity); 
                    case 3:
                        return TypeMatcherProperyReader.getTypeMatcherProperyReader(evt, make.ChildNodes[0].Token.ValueString,
                            make.ChildNodes[1].Token.ValueString, make.ChildNodes[2].Token.ValueString, entity);
                }
            }
            else if (node.ChildNodes.Count == 3 && node.ChildNodes[1].Term == KinectitudeGrammar.BinOp)
            {
                ValueReader left = makeValueReader(node.ChildNodes[0], scene, entity, evt);
                ValueReader right = makeValueReader(node.ChildNodes[2], scene, entity, evt);
                return binOpCreate(node.ChildNodes[1].ChildNodes[0].Term, left, right);
            }
            else
            {
                return new ConstantReader(node.ChildNodes[0].Token.Value);
            }
            throw new NotImplementedException("Error with implementation of operator");
        }

        private TypeMatcher makeTypeMatcherHelper(ParseTreeNode node, Scene scene)
        {
            if (node.Term == KinectitudeGrammar.IsType)
                return new PrototypeTypeMatcher(scene.GetOfPrototype(node.ChildNodes[0].Token.ValueString, false));
            
            return new PrototypeTypeMatcher(scene.GetOfPrototype(node.ChildNodes[0].Token.ValueString, true));
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
                    readables.Add(makeTypeMatcherHelper(node.ChildNodes[0], scene));
                readables.Add(makeTypeMatcherHelper(node.ChildNodes[0], scene));
                return new ListedTypeMatcher(readables);
            }
        }

        public object MakeAssignable(object obj, Scene scene, Entity entity, Event evt)
        {
            ParseTreeNode node = obj as ParseTreeNode;
            if (node.Term == KinectitudeGrammar.TypeMatcher) return makeTypeMatcher(node, scene);
            return makeValueReader(node, scene, entity, evt);
        }

        public IEnumerable<Tuple<string, string>> GetDefines(object from)
        {
            List<Tuple<string, string>> definitions = new List<Tuple<string, string>>();
            ParseTreeNode node = from as ParseTreeNode;
            List<ParseTreeNode> defines = new List<ParseTreeNode>();
            getOfTypeHelper(node, KinectitudeGrammar.Definitions, defines);
            
            foreach(ParseTreeNode define in defines){
                string className = define.ChildNodes.First(child => child.Term == KinectitudeGrammar.ClassName).Token.ValueString;

                definitions.Add(new Tuple<string,string>(GetName(define), className));
            }
            return definitions;
        }

        public object GetCondition(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            return node.ChildNodes.First(child => child.Term == KinectitudeGrammar.Expr);
        }

        private void getOfTypeHelper(ParseTreeNode node, NonTerminal type, List<ParseTreeNode> nodes, bool needsChildren = true)
        {
            int minCount = needsChildren ? 1 : 0;
            IEnumerable<ParseTreeNode> correctTypedNodes =node.ChildNodes.Where(child => 
                child.Term == type && child.ChildNodes.Count >= minCount);

            nodes.AddRange(correctTypedNodes);
            foreach (ParseTreeNode child in correctTypedNodes) getOfTypeHelper(child, type, nodes);
        }

        private void getOfTypeHelper(ParseTreeNode node, HashSet<NonTerminal> type, List<ParseTreeNode> nodes, bool needsChildren = true)
        {
            int minCount = needsChildren ? 1 : 0;
            IEnumerable<ParseTreeNode> correctTypedNodes = node.ChildNodes.Where(child =>
                type.Contains(child.Term)  && child.ChildNodes.Count >= minCount);

            nodes.AddRange(correctTypedNodes);
            foreach (ParseTreeNode child in correctTypedNodes) getOfTypeHelper(child, type, nodes);
        }
    }
}
