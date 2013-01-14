using Irony.Parsing;
using Kinectitude.Core.Data;
using Kinectitude.Core.Language;
using Kinectitude.Core.Loaders;
using Kinectitude.Core.Loaders.Kgl;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Models.Statements;
using Kinectitude.Editor.Models.Statements.Assignments;
using Kinectitude.Editor.Models.Statements.Base;
using Kinectitude.Editor.Models.Statements.Conditions;
using Kinectitude.Editor.Models.Statements.Events;
using Kinectitude.Editor.Models.Statements.Loops;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Action = Kinectitude.Editor.Models.Statements.Actions.Action;
using Attribute = Kinectitude.Editor.Models.Attribute;

namespace Kinectitude.Editor.Storage.Kgl
{
    internal sealed class KglGameStorage : KGLBase, IGameStorage
    {
        private readonly FileInfo FileName;
        private readonly KglGameVisitor Visitor = new KglGameVisitor();

        private Game game;
        private ParseTreeNode root;
        string src;

        internal KglGameStorage(FileInfo fileName)
        {
            FileName = fileName;
        }

        private void addDefaults(Using uses, List<string> defaults, string type)
        {
            foreach (string defaultName in defaults) 
                uses.AddDefine(new Define(defaultName, "Kinectitude.Core." + type + "s." + defaultName + type));
        }

        private void initLangDefaults()
        {
            //TODO add functions here as well later
            Using defaults = new Using();
            List<string> defaultEvts = new List<string>()
            {
                "AttributeChanges",
                "SceneStarts",
                "TriggerOccurs",
                "OnCreate"
            };
            addDefaults(defaults, defaultEvts, "Event");
            List<string> defaultComponents = new List<string>()
            {
                "Transform"
            };
            addDefaults(defaults, defaultComponents, "Component");
            List<string> defaultActions = new List<string>()
            {
                "PushScene",
                "PopScene",
                "ChangeScene",
                "PopScene",
                "FireTrigger",
                "CreateEntity",
                "Destroy",
                "CreateTimer",
                "PauseTimers",
                "ResumeTimers",
                "Quit"
            };
            addDefaults(defaults, defaultActions, "Action");
            
            List<string> defaultManagers = new List<string>()
            {
                "Time"
            };

            addDefaults(defaults, defaultManagers, "Manager");
            game.AddUsing(defaults);
        }

        public Game LoadGame()
        {
            Parser parser = new Parser(grammar);
            src = File.ReadAllText(FileName.FullName);
            ParseTree parseTree = parser.Parse(src, FileName.FullName);
            //TODO find out what to do here to get the error to show
            if (parseTree.HasErrors()) { /*TODO*/ }
            root = parseTree.Root;
            game = new Game(FileName.Name.Replace(".kgl", ""));
            initLangDefaults();
            IEnumerable<ParseTreeNode> usings = grammar.GetOfType(root, grammar.Uses);

            foreach(ParseTreeNode node in usings)
            {
                Using use = CreateUsing(node);
                game.AddUsing(use);
            }

            //TODO this could be an expression set it later
            game.Width = 800;
            game.Height = 600;
            game.IsFullScreen = false;

            foreach (Tuple<string, object> attribute in GetProperties(root))
            {
                ParseTreeNode attributeNode = (ParseTreeNode)attribute.Item2;
                ValueReader reader = MakeAssignable(attributeNode, null, null, null) as ValueReader;
                game.AddAttribute(new Attribute(attribute.Item1) { Value = new Value(getStrVal(attributeNode), reader) });
                //TODO set width, height and is full screen here.  All values should be value readers
            }

            foreach (ParseTreeNode prototype in grammar.GetOfType(root, grammar.Prototype))
            {
                Entity entity = CreateEntity(prototype, null);
                game.AddPrototype(entity);
            }

            foreach (ParseTreeNode sceneNode in grammar.GetOfType(root, grammar.Scene))
            {
                Scene scene = CreateScene(sceneNode);
                game.AddScene(scene);
            }

            //TODO won't need explicit cast when using value reader (or similar for editor) also may be an expression?
            game.FirstScene = game.GetScene(game.GetAttribute("FirstScene").Value.Reader);

            return game;
        }

        public void SaveGame(Game game)
        {
            string kgl = Visitor.Apply(game);
            File.WriteAllBytes(FileName.FullName, System.Text.Encoding.UTF8.GetBytes(kgl));
        }

        private Using CreateUsing(ParseTreeNode node)
        {
            Using use = new Using() { File = node.ChildNodes.First(child => child.Term == grammar.ClassName).Token.ValueString };

            IEnumerable<ParseTreeNode> defines = grammar.GetOfType(node, grammar.Definitions);

            foreach (ParseTreeNode define in defines)
            {
                string className = define.ChildNodes.First(child => child.Term == grammar.ClassName).Token.ValueString;
                use.AddDefine(new Define(grammar.GetName(define), className));
            }

            return use;
        }

        private Entity CreateEntity(ParseTreeNode node, Scene scene)
        {
            Entity entity = new Entity() { Name = grammar.GetName(node) };

            foreach (string name in grammar.GetPrototypes(node)) entity.AddPrototype(game.GetPrototype(name));

            foreach (Tuple<string, object> attribute in GetProperties(node))
            {
                ParseTreeNode attributeNode = (ParseTreeNode)attribute.Item2;
                ValueReader reader = MakeAssignable(attributeNode, scene, entity, null) as ValueReader;
                entity.AddAttribute(new Attribute(attribute.Item1) { Value = new Value(getStrVal(attributeNode), reader) });
            }

            foreach (ParseTreeNode componentNode in grammar.GetOfType(node, grammar.Component))
            {
                // Adding a component adds any other components that the new component requires.
                // It is necessary to check if the component we are attempting to add already
                // exists. If it does, we should not attempt to create a new component.

                Plugin plugin = game.GetPlugin(grammar.GetName(componentNode));

                Component component = entity.GetComponentByType(plugin.CoreType);
                if (null == component)
                {
                    component = new Component(plugin);
                    entity.AddComponent(component);
                }

                foreach (Tuple<string, object> property in GetProperties(componentNode))
                {
                    //TODO make this have a value reader when it is ready
                    ParseTreeNode propertyNode = (ParseTreeNode)property.Item2;
                    component.SetProperty(property.Item1, getStrVal(propertyNode));
                }
            }

            foreach (ParseTreeNode eventNode in grammar.GetOfType(node, grammar.Evt))
            {
                Event evt = CreateEvent(eventNode);
                entity.AddEvent(evt);
            }

            return entity;
        }

        private Action createAction(ParseTreeNode node)
        {
            Action action = new Action(game.GetPlugin(grammar.GetName(node)));
            foreach (Tuple<string, object> property in GetProperties(node))
            {
                ParseTreeNode propertyNode = (ParseTreeNode)property.Item2;
                //TODO value reader when ready
                action.SetProperty(property.Item1, getStrVal(propertyNode));
            }
            return action;
        }

        private void createStatement(ParseTreeNode statementNode, Event evt, CompositeStatement compositeStatement = null)
        {
            AbstractStatement statement = null;
            if (statementNode.Term == grammar.Action)
            {
                statement = createAction(statementNode);
            }
            else if (statementNode.Term == grammar.Condition)
            {
                Condition cond = new Condition() { If = getStrVal(statementNode)};
                foreach (ParseTreeNode child in grammar.GetOfType(statementNode, grammar.Actions)) createStatement(child, evt, cond);
                statement = cond;
                //TODO else
            }
            else if (statementNode.Term == grammar.Assignment)
            {
                Assignment assignment = new Assignment();
                assignment.Key = getStrVal(statementNode.ChildNodes[0]);
                assignment.Value = getStrVal(statementNode.ChildNodes.Last());
                
                assignment.Operator = statementNode.ChildNodes.Count == 2 ? AssignmentOperator.Assign :
                    AbstractAssignment.assingmentValues[grammar.OpLookup[statementNode.ChildNodes[1].Term]];

                statement = assignment;
            }
            else
            {
                string expression = grammar.GetOfType(statementNode, grammar.Expr).First().Token.ValueString;
                CompositeStatement loop = null;
                switch (statementNode.ChildNodes.Count)
                {
                    case 2:
                        WhileLoop wl = new WhileLoop();
                        wl.Expression = expression;
                        statement = loop = wl;
                        break;
                    case 3:
                        ForLoop fl = new ForLoop();
                        fl.Expression = expression;
                        //First assignment, expression, assignment, actions
                        fl.PostExpression = statementNode.ChildNodes[1].Token.ValueString;
                        statement = loop = fl;
                        break;
                    case 4:
                        ForLoop forLoop = new ForLoop();
                        forLoop.Expression = expression;
                        //First assignment, expression, assignment, actions
                        forLoop.PostExpression = statementNode.ChildNodes[2].Token.ValueString;
                        forLoop.PreExpression = statementNode.ChildNodes[0].Token.ValueString;
                        statement = loop = forLoop;
                        break;
                }
                foreach (ParseTreeNode child in grammar.GetOfType(statementNode, grammar.Actions)) createStatement(child, evt, loop);
            }
            if (compositeStatement == null) evt.AddStatement(statement);
            else compositeStatement.AddStatement(statement);
        }

        private Event CreateEvent(ParseTreeNode node)
        {
            Event evt = new Event(game.GetPlugin(grammar.GetName(node)));
            foreach (Tuple<string, object> property in GetProperties(node))
            {
                ParseTreeNode propertyNode = (ParseTreeNode)property.Item2;
                //TODO value reader when ready
                evt.SetProperty(property.Item1, getStrVal(propertyNode));
            }
            foreach (ParseTreeNode actionNode in grammar.GetOfType(node, grammar.Actions)) createStatement(node.ChildNodes[2].ChildNodes[0], evt, null);
            return evt;
        }

        private Scene CreateScene(ParseTreeNode node)
        {
            Scene scene = new Scene(grammar.GetName(node));

            foreach (Tuple<string, object> attribute in GetProperties(node))
            {
                ParseTreeNode attributeNode = (ParseTreeNode)attribute.Item2;
                ValueReader reader = MakeAssignable(attributeNode, scene, null, null) as ValueReader;
                scene.AddAttribute(new Attribute(attribute.Item1) { Value = new Value(getStrVal(attributeNode), reader) });
            }

            foreach (ParseTreeNode managerNode in grammar.GetOfType(node, grammar.Manager))
            {
                Manager manager = CreateManager(managerNode);
                scene.AddManager(manager);
            }

            foreach (ParseTreeNode entityNode in grammar.GetOfType(node, grammar.Entity))
            {
                Entity entity = CreateEntity(entityNode, scene);
                scene.AddEntity(entity);
            }

            return scene;
        }

        private Manager CreateManager(ParseTreeNode node)
        {
            Manager manager = new Manager(game.GetPlugin(grammar.GetName(node)));
            foreach (Tuple<string, object> property in GetProperties(node))
            {
                ParseTreeNode propertyNode = (ParseTreeNode)property.Item2;
                //TODO value reader when ready
                manager.SetProperty(property.Item1, getStrVal(propertyNode));
            }
            return manager;
        }

        private string getStrVal(ParseTreeNode node)
        {
            int pos = node.Span.Location.Position;
            int length = node.Span.Length;
            return src.Substring(pos, length);
        }

        private Tuple<string, object> getAttribute(ParseTreeNode node)
        {
            return new Tuple<string, object>(node.ChildNodes[0].Token.ValueString, getStrVal(node.ChildNodes[1]));
        }
    }
}
