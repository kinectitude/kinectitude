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

        public object EntityType { get { return grammar.Entity; } }
        public object ActionType { get { return grammar.Actions; } }
        public object ServiceType { get { return grammar.Service; } }
        public object ManagerType { get { return grammar.Manager; } }
        public object EventType { get { return grammar.Evt; } }
        public object ComponentType { get { return grammar.Component; } }
        public object UsingType { get { return grammar.Uses; } }
        public object DefineType { get { return grammar.Definitions; } }
        public object PrototypeType { get { return grammar.Prototype; } }
        public object SceneType { get { return grammar.Scene; } }
        public object Else { get { return grammar.Else; } }
        public object GetGame() { return Root; }

        internal KGLLoaderUtility(string fileName, GameLoader gameLoader)
        {
            Parser parser = new Parser(grammar);
            string src = File.ReadAllText(fileName);
            ParseTree parseTree = parser.Parse(src, fileName);
            //TODO find out what to do here to get the error to show
            if (parseTree.HasErrors()) Game.CurrentGame.Die("Can't construct game ");
            Root = parseTree.Root;
        }

        public PropertyHolder GetProperties(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;

            PropertyHolder propertyHolder = new PropertyHolder();

            if (grammar.Prototype == node.Term || grammar.Entity == node.Term)
            {
                foreach (ParseTreeNode entityDef in GetOfType(from, grammar.EntityDefinition))
                {
                    foreach (ParseTreeNode property in GetOfType(entityDef, grammar.Properties))
                        propertyHolder.AddValue(property.ChildNodes[0].Token.ValueString, property.ChildNodes[1]);
                }
            }
            else
            {
                foreach (ParseTreeNode property in GetOfType(from, grammar.Properties))
                    propertyHolder.AddValue(property.ChildNodes[0].Token.ValueString, property.ChildNodes[1]);
            }

            return propertyHolder;
        }

        public string GetName(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            ParseTreeNode nameNode = node.ChildNodes.FirstOrDefault(child => child.Term == grammar.Identifier);
            return nameNode == null ? null : nameNode.Token.ValueString;
        }

        public IEnumerable<object> GetOfType(object from, object type)
        {
            List<ParseTreeNode> nodes = new List<ParseTreeNode>();
            ParseTreeNode node = from as ParseTreeNode;
            NonTerminal nonTerm = type as NonTerminal;

            if (nonTerm == ActionType)
            {
                List<ParseTreeNode> firstType = new List<ParseTreeNode>();
                
                HashSet<NonTerminal> valids = new HashSet<NonTerminal>() { grammar.Action, grammar.Condition, 
                    grammar.Assignment, grammar.Loop};

                getOfTypeHelper(node, nonTerm, firstType);
                foreach (ParseTreeNode singular in firstType) getOfTypeHelper(singular, valids, nodes);
                return nodes;
            }

            //There should be one entity definition per entity
            if (nonTerm == ComponentType || nonTerm == EventType)
                node = node.ChildNodes.First(child => child.Term == grammar.EntityDefinition);

            getOfTypeHelper(node, nonTerm, nodes);
            return nodes;
        }

        public string GetFile(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            return node.ChildNodes.First(child => child.Term == grammar.ClassName).Token.ValueString;
        }

        public bool IsAciton(object obj)
        {
            ParseTreeNode node = obj as ParseTreeNode;
            return node.Term == grammar.Action;
        }

        public bool IsAssignment(object obj)
        {
            ParseTreeNode node = obj as ParseTreeNode;
            return node.Term == grammar.Assignment;
        }

        public bool IsCondition(object obj)
        {
            ParseTreeNode node = obj as ParseTreeNode;
            return node.Term == grammar.Condition;
        }

        public object GetBeforeAction(object obj)
        {
            ParseTreeNode node = obj as ParseTreeNode;
            //First assignment, expression, assignment, actions
            if (node.ChildNodes.Count == 4) return node.ChildNodes[0];
            return null;
        }

        public object GetAfterAction(object obj)
        {
            ParseTreeNode node = obj as ParseTreeNode;
            //First assignment, expression, assignment, actions
            if (node.ChildNodes.Count == 4) return node.ChildNodes[2];
            //expression, assignment, actions
            if (node.ChildNodes.Count == 3) return node.ChildNodes[1];
            return null;
        }

        public Tuple<object, object, object> GetAssignment(object obj)
        {
            ParseTreeNode node = obj as ParseTreeNode;
            switch (node.ChildNodes.Count)
            {
                case 2:
                    return new Tuple<object, object, object>(node.ChildNodes[0], null, node.ChildNodes[1]);
                case 3:
                    return new Tuple<object, object, object>(node.ChildNodes[0], node.ChildNodes[1], node.ChildNodes[2]);
            }
            Game.CurrentGame.Die("Error with assignment");
            return null;
        }

        public IEnumerable<string> GetPrototypes(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            node = node.ChildNodes.First(child => child.Term == grammar.EntityDefinition);
            List<ParseTreeNode> names = new List<ParseTreeNode>();
            getOfTypeHelper(node, grammar.Names, names);
            //Names should only have name as a child EVER.
            foreach (ParseTreeNode name in names) yield return name.ChildNodes[0].Token.ValueString;
        }

        public string GetType(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            if (node.Term == grammar.Actions)
            {
                node = node.ChildNodes.First(child => child.Term == grammar.Action);
            }
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
            if (grammar.Div == op) return new DivOpReader(left, right);
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
            object obj = getDataContainer(scene, entity, name).GetChangeable(component);
            return ParameterValueReader.GetParameterValueReader(obj, param, scene);
        }

        private ValueReader makeDcReader(Scene scene, Entity entity, string name, string param)
        {
            DataContainer dataContainer = getDataContainer(scene, entity, name);
            return DataContainerReader.GetDataContainerReader(dataContainer, param);
        }

        private ValueReader makeValueReader(ParseTreeNode node, Scene scene, Entity entity, Event evt)
        {
            BnfTerm type0;
            //From is used for assignments where the node is not an expr.  It can only be an exactvalue, but not constant
            ParseTreeNode from;
            if(node.Term == grammar.Expr){
                from = node.ChildNodes[0];
                type0 = from.Term;
            }else{
                from = node;
                type0 = node.Term;
            }

            if (type0 == grammar.UniOp)
            {
                ValueReader value = makeValueReader(node.ChildNodes[1], scene, entity, evt);
                return uniOpCreate(node.ChildNodes[0].ChildNodes[0].Term, value);
            }
            else if (type0 == grammar.Number)
            {
                return new ConstantReader(node.ChildNodes[0].Token.Value);
            }
            else if (type0 == grammar.ThreeVal)
            {
                ParseTreeNodeList list = from.ChildNodes;
                return makeParameterReader(scene, entity, list[0].Token.ValueString,
                    list[1].Token.ValueString, list[2].Token.ValueString);
            }
            else if (type0 == grammar.TwoVal)
            {
                ParseTreeNodeList list = from.ChildNodes;
                if (null == getDataContainer(scene, entity, list[0].Token.ValueString))
                    return makeParameterReader(scene, entity, "this", list[0].Token.ValueString, list[1].Token.ValueString);
                return makeDcReader(scene, entity, list[0].Token.ValueString, list[1].Token.ValueString);
            }
            else if (type0 == grammar.Identifier)
            {
                return makeDcReader(scene, entity, "this", from.Token.ValueString);
            }
            else if (type0 == grammar.Constants)
            {
                return from.Token.Value as ConstantReader;
            }
            else if (type0 == grammar.ParentVal)
            {
                switch (from.ChildNodes.Count)
                {
                    case 2:
                        return TypeMatcherDCReader.GetTypeMatcherDCValueReader(evt, from.ChildNodes[0].Token.ValueString,
                            from.ChildNodes[1].Token.ValueString, entity);
                    case 3:
                        return TypeMatcherProperyReader.GetTypeMatcherProperyReader(evt, from.ChildNodes[0].Token.ValueString,
                            from.ChildNodes[1].Token.ValueString, from.ChildNodes[2].Token.ValueString, entity);
                }
            }
            else if (node.ChildNodes.Count == 3 && node.ChildNodes[1].Term == grammar.BinOp)
            {
                ValueReader left = makeValueReader(node.ChildNodes[0], scene, entity, evt);
                ValueReader right = makeValueReader(node.ChildNodes[2], scene, entity, evt);
                return binOpCreate(node.ChildNodes[1].ChildNodes[0].Term, left, right);
            }
            else
            {
                return new ConstantReader(from.Token.Value);
            }
            Game.CurrentGame.Die("Error with implementation of operator");
            return null;
        }

        private TypeMatcher makeTypeMatcherHelper(ParseTreeNode node, Scene scene)
        {
            string strVal = node.ChildNodes[0].Token.ValueString;
            if (grammar.Constants.Constants.ContainsKey(strVal)) Game.CurrentGame.Die("Invalid use of the term " + strVal);

            if (node.Term == grammar.IsType)
                return new PrototypeTypeMatcher(scene.GetOfPrototype(strVal, false));

            return new PrototypeTypeMatcher(scene.GetOfPrototype(strVal, true));
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
            if (obj == null) return ConstantReader.TrueValue;
            ParseTreeNode node = obj as ParseTreeNode;
            if (node.Term == grammar.TypeMatcher) return makeTypeMatcher(node, scene);
            return makeValueReader(node, scene, entity, evt);
        }

        public ValueReader MakeAssignmentValue(ValueReader ls, object type, ValueReader rs)
        {
            if (type == null) return rs;
            ParseTreeNode binOpNode = type as ParseTreeNode;
            BnfTerm binOp = binOpNode.ChildNodes[0].Term;
            return binOpCreate(binOp, ls, rs);
        }

        public IEnumerable<Tuple<string, string>> GetDefines(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            List<ParseTreeNode> defines = new List<ParseTreeNode>();
            getOfTypeHelper(node, grammar.Definitions, defines);

            foreach (ParseTreeNode define in defines)
            {
                string className = define.ChildNodes.First(child => child.Term == grammar.ClassName).Token.ValueString;
                yield return (new Tuple<string, string>(GetName(define), className));
            }
        }

        public object GetCondition(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            return node.ChildNodes.FirstOrDefault(child => child.Term == grammar.Expr);
        }

        private void getOfTypeHelper(ParseTreeNode node, NonTerminal type, List<ParseTreeNode> nodes)
        {
            IEnumerable<ParseTreeNode> correctTypedNodes = node.ChildNodes.Where(child => child.Term == type);

            nodes.AddRange(correctTypedNodes);
            foreach (ParseTreeNode child in correctTypedNodes) getOfTypeHelper(child, type, nodes);
        }

        private void getOfTypeHelper(ParseTreeNode node, HashSet<NonTerminal> type, List<ParseTreeNode> nodes)
        {
            IEnumerable<ParseTreeNode> correctTypedNodes = node.ChildNodes.Where(child => type.Contains(child.Term));

            nodes.AddRange(correctTypedNodes);
            foreach (ParseTreeNode child in correctTypedNodes) getOfTypeHelper(child, type, nodes);
        }
    }
}
