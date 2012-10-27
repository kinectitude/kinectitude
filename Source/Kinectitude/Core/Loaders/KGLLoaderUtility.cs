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
        public object ActionType { get { return KinectitudeGrammar.Action; } }
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

            foreach(ParseTreeNode property in GetOfType(from, KinectitudeGrammar.Properties))
                propertyHolder.AddValue(property.ChildNodes[0].Token.Value.ToString(), property.ChildNodes[1]);

            foreach (ParseTreeNode entityDef in GetOfType(from, KinectitudeGrammar.EntityDefinition))
            {
                foreach (ParseTreeNode property in GetOfType(entityDef, KinectitudeGrammar.Properties))
                    propertyHolder.AddValue(property.ChildNodes[0].Token.Value.ToString(), property.ChildNodes[1]);
            }

            return propertyHolder;
        }

        public string GetName(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            return node.ChildNodes.First(child => child.Term.ToString() == KinectitudeGrammar.Name.Name).Token.Value.ToString();
        }

        public IEnumerable<object> GetOfType(object from, object type)
        {
            List<ParseTreeNode> nodes = new List<ParseTreeNode>();
            ParseTreeNode node = from as ParseTreeNode;
            NonTerminal nonTerm = type as NonTerminal;
            if (nonTerm == SceneType || nonTerm == EntityType)
            {
                List<ParseTreeNode> plurals = new List<ParseTreeNode>();
                NonTerminal pluralType = nonTerm == SceneType ? KinectitudeGrammar.Scenes : KinectitudeGrammar.Entities;
                getOfTypeHelper(node, pluralType, plurals);
                foreach (ParseTreeNode singular in plurals) getOfTypeHelper(singular, nonTerm, nodes);
            }
            else{
                getOfTypeHelper(node, nonTerm, nodes);
            }
            return nodes;
        }

        public string GetFile(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            return node.ChildNodes.First(child => child.Term.ToString() == KinectitudeGrammar.ClassName.Name).Token.Value.ToString();
        }

        public bool IsAciton(object obj)
        {
            ParseTreeNode node = obj as ParseTreeNode;
            return node.Term.ToString() == KinectitudeGrammar.Action.Name;
        }

        public IEnumerable<string> GetPrototypes(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            List<string> prototypeNames = new List<string>();
            List<ParseTreeNode> prototypes = new List<ParseTreeNode>();
            
            //TODO
            return prototypeNames;
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
            else
            {
                return new ConstantReader(node.ChildNodes[0].Token.Value);
            }
            throw new NotImplementedException("Error with implementation of operator");
        }

        private TypeMatcher makeTypeMatcherHelper(ParseTreeNode node, Scene scene)
        {
            if (node.Term.ToString() == KinectitudeGrammar.IsType.Name)
                return new PrototypeTypeMatcher(scene.GetOfPrototype(node.ChildNodes[0].Token.ToString(), false));
            
            if (node.Term.ToString() == KinectitudeGrammar.IsExactType.Name)
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

            string type = node.Term.ToString();

            if (type == KinectitudeGrammar.TypeMatcher.Name) return makeTypeMatcher(node, scene);
            return makeValueReader(node, scene, entity, evt);
        }

        public IEnumerable<Tuple<string, string>> GetDefines(object from)
        {
            List<Tuple<string, string>> definitions = new List<Tuple<string, string>>();
            ParseTreeNode node = from as ParseTreeNode;
            List<ParseTreeNode> defines = new List<ParseTreeNode>();
            getOfTypeHelper(node, KinectitudeGrammar.Definitions, defines);
            
            foreach(ParseTreeNode define in defines){
                string className = define.ChildNodes.First(child => 
                    child.Term.ToString() == KinectitudeGrammar.ClassName.Name).Token.Value.ToString();

                definitions.Add(new Tuple<string,string>(GetName(define), className));
            }
            return definitions;
        }

        public object GetCondition(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            return node.ChildNodes.First(child => child.Term.ToString() == KinectitudeGrammar.Expr.Name);
        }

        private void getOfTypeHelper(ParseTreeNode node, NonTerminal type, List<ParseTreeNode> nodes, bool needsChildren = true)
        {
            int minCount = needsChildren ? 1 : 0;
            IEnumerable<ParseTreeNode> correctTypedNodes =node.ChildNodes.Where(child => 
                child.Term.ToString() == type.Name && child.ChildNodes.Count >= minCount);

            nodes.AddRange(correctTypedNodes);
            foreach (ParseTreeNode child in correctTypedNodes) getOfTypeHelper(child, type, nodes);
        }
    }
}
