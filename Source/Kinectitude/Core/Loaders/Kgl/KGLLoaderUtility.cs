using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Language;
using Irony.Parsing;
using Kinectitude.Core.Data;
using Kinectitude.Core.Base;
using System.IO;

namespace Kinectitude.Core.Loaders.Kgl
{
    internal sealed class KGLLoaderUtility : KGLBase, LoaderUtility
    {
        private readonly ParseTreeNode Root;

        public object EntityType { get { return grammar.Entity; } }
        public object ActionType { get { return grammar.Actions; } }
        public object ServiceType { get { return grammar.Service; } }
        public object ManagerType { get { return grammar.Manager; } }
        public object EventType { get { return grammar.Evt; } }
        public object ComponentType { get { return grammar.Component; } }
        public object UsingType { get { return grammar.Uses; } }
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
            if (parseTree.HasErrors())
            {
                var builder = new StringBuilder();
                builder.Append("Can't construct game due to syntax errors:\n\n");

                foreach (var message in parseTree.ParserMessages)
                {
                    builder.AppendLine(message.Message);
                }

                Game.CurrentGame.Die(builder.ToString());
            }
            Root = parseTree.Root;
        }

        public PropertyHolder GetProperties(object from) { return base.GetProperties(from as ParseTreeNode); }

        public IEnumerable<object> GetOfType(object from, object type)
        { 
            return grammar.GetOfType(from as ParseTreeNode, type as NonTerminal);
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

        public bool IsFunction(object obj)
        {
            ParseTreeNode node = obj as ParseTreeNode;
            return node.Term == grammar.Function;
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

        public IEnumerable<string> GetPrototypes(object from) { return grammar.GetPrototypes(from as ParseTreeNode); }

        public string GetType(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            if (node.Term == grammar.Actions)
            {
                node = node.ChildNodes.First(child => child.Term == grammar.Action);
            }
            return node.ChildNodes[0].Token.ValueString;
        }

        public IEnumerable<Tuple<string, string>> GetDefines(object from)
        {
            ParseTreeNode node = from as ParseTreeNode;
            IEnumerable<ParseTreeNode> defines = grammar.GetOfType(node, grammar.Definitions);

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

        public string GetName(object from) { return grammar.GetName(from as ParseTreeNode); }
        public ValueReader MakeAssignmentValue(ValueReader ls, object type, ValueReader rs) { return base.MakeAssignmentValue(ls, type as ParseTreeNode, rs); }
        public object MakeAssignable(object obj, IScene scene, IDataContainer entity, Event evt) { return base.MakeAssignable(obj as ParseTreeNode, scene, entity, evt); }
    }
}
