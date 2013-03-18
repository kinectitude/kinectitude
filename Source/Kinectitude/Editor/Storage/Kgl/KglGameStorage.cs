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
using Kinectitude.Editor.Models.Values;
using System;
using System.Linq;
using System.IO;
using Action = Kinectitude.Editor.Models.Statements.Actions.Action;
using Attribute = Kinectitude.Editor.Models.Attribute;
using System.Collections.Generic;
using Kinectitude.Editor.Models.Exceptions;
using System.Text;

namespace Kinectitude.Editor.Storage.Kgl
{
    internal sealed class KglGameStorage : KGLBase, IGameStorage
    {
        public static void AddDefaultUsings(Game game)
        {
            //TODO add functions here as well later
            Using defaults = new Using();
            defaults.File = null;
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
                "Quit",
                "PointTowards"
            };
            addDefaults(defaults, defaultActions, "Action");

            List<string> defaultManagers = new List<string>()
            {
                "Time"
            };

            addDefaults(defaults, defaultManagers, "Manager");
            game.AddUsing(defaults);
        }

        private static void addDefaults(Using uses, List<string> defaults, string type)
        {
            foreach (string defaultName in defaults)
                uses.AddDefine(new Define(defaultName, "Kinectitude.Core." + type + "s." + defaultName + type));
        }

        private readonly FileInfo FileName;
        private readonly KglGameVisitor Visitor = new KglGameVisitor();

        private Game game;
        private ParseTreeNode root;
        string src;

        internal KglGameStorage(FileInfo fileName)
        {
            FileName = fileName;
        }

        public Game LoadGame()
        {
            Parser parser = new Parser(grammar);
            src = File.ReadAllText(FileName.FullName);
            ParseTree parseTree = parser.Parse(src, FileName.FullName);
            
            if (parseTree.HasErrors())
            {
                throw new StorageException(BuildErrorMessage(parseTree.ParserMessages, FileName.Name));
            }
            
            root = parseTree.Root;
            var nameProperty = GetProperties(root).First(x => x.Item1 == "Name");
            var name = getStrVal((ParseTreeNode)nameProperty.Item2);
            game = new Game(new Value(name).GetStringValue());
            AddDefaultUsings(game);
            IEnumerable<ParseTreeNode> usings = grammar.GetOfType(root, grammar.Uses);

            foreach(ParseTreeNode node in usings)
            {
                Using use = CreateUsing(node);
                game.AddUsing(use);
            }

            foreach (Tuple<string, object> attribute in GetProperties(root))
            {
                ParseTreeNode attributeNode = (ParseTreeNode)attribute.Item2;
                if (attribute.Item1 != "Name")
                {
                    game.AddAttribute(new Attribute(attribute.Item1) { Value = new Value(getStrVal(attributeNode)) });
                }
            }

            foreach (ParseTreeNode prototype in grammar.GetOfType(root, grammar.Prototype))
            {
                Entity entity = CreateEntity(prototype, null, true);
                game.AddPrototype(entity);
            }

            foreach (ParseTreeNode serviceNode in grammar.GetOfType(root, grammar.Service))
            {
                Service service = CreateService(serviceNode);
                game.AddService(service);
            }

            foreach (ParseTreeNode sceneNode in grammar.GetOfType(root, grammar.Scene))
            {
                Scene scene = CreateScene(sceneNode);
                game.AddScene(scene);
            }

            var firstScene = game.GetAttribute("FirstScene");
            game.FirstScene = game.GetScene(firstScene.Value.GetStringValue());
            game.RemoveAttribute(firstScene);

            return game;
        }

        public void SaveGame(Game game)
        {
            string kgl = Visitor.Apply(game);
            File.WriteAllBytes(FileName.FullName, System.Text.Encoding.UTF8.GetBytes(kgl));
        }

        private string BuildErrorMessage(Irony.LogMessageList messages, string file)
        {
            var builder = new StringBuilder(string.Format("'{0}' is not a valid KGL game file.", file)).Append(Environment.NewLine).Append(Environment.NewLine);

            foreach (var message in messages)
            {
                builder.AppendLine(message.Message + " at line " + message.Location.Line + " column " + message.Location.Column);
            }

            return builder.ToString();
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

        private Entity CreateEntity(ParseTreeNode node, Scene scene, bool prototype)
        {
            Entity entity = new Entity() { Name = grammar.GetName(node), IsPrototype = prototype };

            foreach (string name in grammar.GetPrototypes(node)) entity.AddPrototype(game.GetPrototype(name));

            foreach (Tuple<string, object> attribute in GetProperties(node))
            {
                ParseTreeNode attributeNode = (ParseTreeNode)attribute.Item2;
                entity.AddAttribute(new Attribute(attribute.Item1) { Value = new Value(getStrVal(attributeNode)) });
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
                    ParseTreeNode propertyNode = (ParseTreeNode)property.Item2;
                    component.SetProperty(property.Item1, new Value(getStrVal(propertyNode)));
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
                action.SetProperty(property.Item1, new Value(getStrVal(propertyNode)));
            }
            return action;
        }

        private void CreateStatement(ParseTreeNode statementNode, Event evt, CompositeStatement compositeStatement = null)
        {
            AbstractStatement statement = null;
            if (statementNode.Term == grammar.Action)
            {
                statement = createAction(statementNode);
            }
            else if (statementNode.Term == grammar.Condition)
            {
                ConditionGroup conditionGroup = new ConditionGroup();
                conditionGroup.If.Expression = getStrVal(statementNode.ChildNodes.First(child => child.Term == grammar.Expr));
                foreach (ParseTreeNode child in grammar.GetOfType(statementNode, grammar.Actions)) CreateStatement(child, evt, conditionGroup.If);
                foreach (ParseTreeNode elseNode in grammar.GetOfType(statementNode, grammar.Else))
                {
                    string expr = getStrVal(elseNode.ChildNodes.FirstOrDefault(child => child.Term == grammar.Expr));
                    AbstractCondition cond;

                    if (null == expr)
                    {
                        conditionGroup.Else = new BasicCondition();
                        cond = conditionGroup.Else;
                    }
                    else
                    {
                        cond = new ExpressionCondition() { Expression = expr };
                        conditionGroup.AddStatement(cond);
                    }

                    foreach (ParseTreeNode child in grammar.GetOfType(elseNode, grammar.Actions)) CreateStatement(child, evt, cond);
                }
                statement = conditionGroup;
            }
            else if (statementNode.Term == grammar.Assignment)
            {
                Assignment assignment = new Assignment();
                assignment.Key = getStrVal(statementNode.ChildNodes[0]);
                assignment.Value = getStrVal(statementNode.ChildNodes.Last());
                
                assignment.Operator = statementNode.ChildNodes.Count == 2 ? AssignmentOperator.Assign :
                    AbstractAssignment.AssignmentValues[grammar.OpLookup[statementNode.ChildNodes[1].Term]];

                statement = assignment;
            }
            else
            {
                CompositeStatement loop = null;
                switch (statementNode.ChildNodes.Count)
                {
                    case 2:
                        WhileLoop wl = new WhileLoop();
                        wl.Expression = getStrVal(statementNode.ChildNodes[0]);
                        statement = loop = wl;
                        break;
                    case 3:
                        ForLoop fl = new ForLoop();
                        fl.Expression = getStrVal(statementNode.ChildNodes[0]);
                        //First assignment, expression, assignment, actions
                        fl.PostExpression = getStrVal(statementNode.ChildNodes[1]);
                        statement = loop = fl;
                        break;
                    case 4:
                        ForLoop forLoop = new ForLoop();
                        forLoop.Expression = getStrVal(statementNode.ChildNodes[1]);
                        //First assignment, expression, assignment, actions
                        forLoop.PostExpression = getStrVal(statementNode.ChildNodes[2]);
                        forLoop.PreExpression = getStrVal(statementNode.ChildNodes[0]);
                        statement = loop = forLoop;
                        break;
                }
                foreach (ParseTreeNode child in grammar.GetOfType(statementNode, grammar.Actions))
                {
                    // TODO: This is a hack to skip the Actions found in the first line of the for() statement.
                    if (!statementNode.ChildNodes.Contains(child))
                    {
                        CreateStatement(child, evt, loop);
                    }
                }
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
                evt.SetProperty(property.Item1, new Value(getStrVal(propertyNode)));
            }
            foreach (ParseTreeNode actionNode in grammar.GetOfType(node, grammar.Actions)) CreateStatement(actionNode, evt, null);
            return evt;
        }

        private Scene CreateScene(ParseTreeNode node)
        {
            Scene scene = new Scene(grammar.GetName(node));

            foreach (Tuple<string, object> attribute in GetProperties(node))
            {
                ParseTreeNode attributeNode = (ParseTreeNode)attribute.Item2;
                scene.AddAttribute(new Attribute(attribute.Item1) { Value = new Value(getStrVal(attributeNode)) });
            }

            foreach (ParseTreeNode managerNode in grammar.GetOfType(node, grammar.Manager))
            {
                Manager manager = CreateManager(managerNode);
                scene.AddManager(manager);
            }

            foreach (ParseTreeNode entityNode in grammar.GetOfType(node, grammar.Entity))
            {
                Entity entity = CreateEntity(entityNode, scene, false);
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
                manager.SetProperty(property.Item1, new Value(getStrVal(propertyNode)));
            }
            return manager;
        }


        private Service CreateService(ParseTreeNode node)
        {
            Service service = new Service(game.GetPlugin(grammar.GetName(node)));
            foreach (Tuple<string, object> property in GetProperties(node))
            {
                ParseTreeNode propertyNode = (ParseTreeNode)property.Item2;
                service.SetProperty(property.Item1, new Value(getStrVal(propertyNode)));
            }
            return service;
        }

        private string getStrVal(ParseTreeNode node)
        {
            if (node == null) return null;
            int pos = node.Span.Location.Position;
            int length = node.Span.Length;
            var ret = src.Substring(pos, length);
            return ret;
        }
    }
}
