using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using Irony.Parsing.Construction;
using Irony.Interpreter.Ast;
using Kinectitude.Core.Data;
using System.Text.RegularExpressions;

namespace Kinectitude.Core.Language
{
    [Language("Kinectitude Game Language", "1.0.0", "Used to create Kinectitude games")]
    internal sealed class KinectitudeGrammar : Grammar
    {
        internal readonly IdentifierTerminal Identifier = TerminalFactory.CreateCSharpIdentifier("Identifier");
        internal readonly NumberLiteral Number = TerminalFactory.CreateCSharpNumber("Number");
        internal readonly Terminal Str = TerminalFactory.CreateCSharpString("Str");
        internal readonly Terminal ClassName = new RegexBasedTerminal("ClassName", @"@?[a-z_A-Z]\w+(?:\.@?[a-z_A-Z]\w+)*");

        #region kinectitude key words and parts
        internal readonly NonTerminal Game = new NonTerminal("Game", "Game");

        internal readonly NonTerminal Scene = new NonTerminal("Scene", "Scene");
        internal readonly NonTerminal Entity = new NonTerminal("Entity", "Entity");
        internal readonly NonTerminal Properties = new NonTerminal("Properties");
        internal readonly NonTerminal Names = new NonTerminal("Names", "Names");
        internal readonly NonTerminal Prototype = new NonTerminal("Prototype", "Prototype");
        internal readonly NonTerminal EntityDefinition = new NonTerminal("EntityDefinition", "EntityDefinition");
        internal readonly NonTerminal BasicDefinition = new NonTerminal("BasicDefinition", "BasicDefinition");
        internal readonly NonTerminal Manager = new NonTerminal("Manager", "Manager");
        internal readonly NonTerminal Component = new NonTerminal("Component", "Component");
        internal readonly NonTerminal Evt = new NonTerminal("Event", "Event");
        internal readonly NonTerminal EvtDefinition = new NonTerminal("EvtDefinition", "EvtDefinition");
        internal readonly NonTerminal Action = new NonTerminal("Action", "Action");
        internal readonly NonTerminal Condition = new NonTerminal("Condition", "Condition");
        internal readonly NonTerminal Actions = new NonTerminal("Actions", "Actions");
        internal readonly NonTerminal Uses = new NonTerminal("Uses", "Uses");
        internal readonly NonTerminal Classes = new NonTerminal("Classes", "Classes");
        internal readonly NonTerminal Definitions = new NonTerminal("Definitions", "Definitions");
        internal readonly NonTerminal Assignment = new NonTerminal("Assignment", "Assignment");
        internal readonly ConstantTerminal Constants = new ConstantTerminal("Constants", typeof(ConstantReader));
        internal readonly NonTerminal Else = new NonTerminal("Else", "Else");
        internal readonly NonTerminal Service = new NonTerminal("Service", "Service");
        internal readonly NonTerminal Loop = new NonTerminal("Loop", "Loop");
        internal readonly NonTerminal Function = new NonTerminal("Function", "Function");
        internal readonly NonTerminal Argument = new NonTerminal("Argument", "Argument");
        #endregion

        #region Key Types of KGL
        internal readonly NonTerminal TypeMatcher = new NonTerminal("TypeMatcher", "TypeMatcher");
        internal readonly NonTerminal IsType = new NonTerminal("IsType", "IsType");
        internal readonly NonTerminal IsExactType = new NonTerminal("IsExactType", "IsExactType");
        internal readonly NonTerminal ThreeVal = new NonTerminal("ThreeVal", "ThreeVal");
        internal readonly NonTerminal TwoVal = new NonTerminal("TwoVal", "TwoVal");
        internal readonly NonTerminal ParentVal = new NonTerminal("ParentVal", "ParentVal");
        #endregion

        #region expressions
        internal readonly NonTerminal Expr = new NonTerminal("Expr", "Expr");
        internal readonly NonTerminal BinOp = new NonTerminal("BinOp", "BinOp");
        internal readonly NonTerminal UniOp = new NonTerminal("UniOp", "UniOp");
        #endregion

        #region operator terminals
        //ToTerm needs an instance, but reference to these are needed.
        internal readonly Terminal Becomes, Eql, Lt, Gt, Le, Ge, Neq, Plus, Minus, Mult, Div, Rem, Pow, And, Or, Not,
            LeftShift, RightShift, BPlus, BMinus, BMult, BDiv, BRem, BPow, BAnd, BOr, BLeftShift, BRightShift;
        #endregion

        internal enum OpCode { Becomes, Plus, Minus, Div, Mult, Rem, Pow, And, Or, Eql, Neq, Gt, Ge, Lt, Le, LeftShift, RightShift };

        public readonly Dictionary<BnfTerm, OpCode> OpLookup = new Dictionary<BnfTerm, OpCode>();

        public string GetName(ParseTreeNode node)
        {
            ParseTreeNode nameNode = node.ChildNodes.FirstOrDefault(child => child.Term == Identifier);
            return nameNode == null ? null : nameNode.Token.ValueString;
        }

        public IEnumerable<string> GetPrototypes(ParseTreeNode node)
        {
            node = node.ChildNodes.First(child => child.Term == EntityDefinition);
            List<ParseTreeNode> names = new List<ParseTreeNode>();
            getOfTypeHelper(node, Names, names);
            //Names should only have name as a child EVER.
            foreach (ParseTreeNode name in names) yield return name.ChildNodes[0].Token.ValueString;
        }

        public IEnumerable<ParseTreeNode> GetOfType(ParseTreeNode node, NonTerminal nonTerm)
        {
            List<ParseTreeNode> nodes = new List<ParseTreeNode>();

            if (nonTerm == Actions)
            {
                List<ParseTreeNode> firstType = new List<ParseTreeNode>();

                HashSet<NonTerminal> valids = new HashSet<NonTerminal>() { Action, Condition, Assignment, Loop };

                getOfTypeHelper(node, nonTerm, firstType);
                foreach (ParseTreeNode singular in firstType) getOfTypeHelper(singular, valids, nodes);
                return nodes;
            }

            //There should be one entity definition per entity
            if (nonTerm == Component || nonTerm == Evt) node = node.ChildNodes.First(child => child.Term == EntityDefinition);

            getOfTypeHelper(node, nonTerm, nodes);
            return nodes;
        }

        private void getOfTypeHelper(ParseTreeNode node, NonTerminal type, List<ParseTreeNode> nodes)
        {
            IEnumerable<ParseTreeNode> correctTypedNodes = node.ChildNodes.Where(child => child.Term == type);
            nodes.AddRange(correctTypedNodes);
            foreach (ParseTreeNode child in correctTypedNodes) getOfTypeHelper(child, type, nodes);
        }

        private void getOfTypeHelper(ParseTreeNode node, HashSet<NonTerminal> type, List<ParseTreeNode> nodes)
        {
            Func<ParseTreeNode, bool> predicate = (child => type.Contains(child.Term));

            if (node.Term == Loop)
            {
                predicate = (child => child.Term != Assignment && type.Contains(child.Term));
            }

            IEnumerable<ParseTreeNode> correctTypedNodes = node.ChildNodes.Where(predicate);
            nodes.AddRange(correctTypedNodes);
            foreach (ParseTreeNode child in correctTypedNodes) getOfTypeHelper(child, type, nodes);
        }

        public KinectitudeGrammar()
        {
            Number.DefaultFloatType = TypeCode.Double;
            Number.DefaultIntTypes =  new TypeCode [] { TypeCode.Int64 };

            CommentTerminal lineComment = new CommentTerminal("Line Comment", "//", "\r\n", "\n");
            CommentTerminal blockComment = new CommentTerminal("Block Comment", "/*", "*/");
            NonGrammarTerminals.Add(lineComment);
            NonGrammarTerminals.Add(blockComment);

            #region constants
            Constants.Add("true", ConstantReader.TrueValue);
            Constants.Add("false",ConstantReader.FalseValue);
            Constants.Add("True", ConstantReader.TrueValue);
            Constants.Add("False", ConstantReader.FalseValue);
            Constants.Add("Pi", new ConstantReader(Math.PI));
            Constants.Add("E", new ConstantReader(Math.E));
            #endregion

            Constants.Priority = Identifier.Priority + 1;

            #region operator terminals
            Terminal openBrace = ToTerm("{");
            Terminal closeBrace = ToTerm("}");
            Terminal colon = ToTerm(":");
            Terminal comma = ToTerm(",");
            Terminal openBrac = ToTerm("(");
            Terminal closeBrac = ToTerm(")");
            Becomes = ToTerm("=", "Becomes");
            Eql = ToTerm("==", "Eql");
            Lt = ToTerm("<", "Lt");
            Gt = ToTerm(">", "Gt");
            Le = ToTerm("<=", "Le");
            Ge = ToTerm(">=", "Ge");
            Neq = ToTerm("!=", "ne");
            Plus = ToTerm("+", "Plus");
            Minus = ToTerm("-", "Minus");
            Mult = ToTerm("*", "Mult");
            Div = ToTerm("/", "Div");
            Rem = ToTerm("%", "Rem");
            Pow = ToTerm("**", "Pow");
            And = new RegexBasedTerminal("And", @"(&&)|(and)");
            Or = new RegexBasedTerminal("Or", @"(\|\|)|(or)");
            Not = new RegexBasedTerminal("Not", @"!|(not)");
            LeftShift = ToTerm("<<", "leftShitf");
            RightShift = ToTerm(">>", "RightShift");
            BPlus = ToTerm("+=", "Plus Equals");
            BMinus = ToTerm("-=", "Minus Equals");
            BMult = ToTerm("*=", "Mult Equals");
            BDiv = ToTerm("/=", "Div Equals");
            BRem = ToTerm("%=", "Rem Equals");
            BPow = ToTerm("^=", "Pow Equals");
            BAnd = ToTerm("&=", "And Equals");
            BOr = ToTerm("|=", "Or Equals");
            BLeftShift = ToTerm("<<=", "LShift Equals");
            BRightShift = ToTerm(">>=", "RShift Equals");

            NonTerminal becomesExpr = new NonTerminal("Becomes expr");
            becomesExpr.Rule = BPlus | BMinus | BDiv | BMult | BRem | BPow | BAnd | BOr | BLeftShift | BRightShift | Becomes;

            OpLookup[Plus] = OpLookup[BPlus] = OpCode.Plus;
            OpLookup[Minus] = OpLookup[BMinus] = OpCode.Minus;
            OpLookup[Div] = OpLookup[BDiv] = OpCode.Div;
            OpLookup[Mult] = OpLookup[BMult] = OpCode.Mult;
            OpLookup[Rem] = OpLookup[BRem] = OpCode.Rem;
            OpLookup[Pow] = OpLookup[BPow] = OpCode.Pow;
            OpLookup[And] = OpLookup[BAnd] = OpCode.And;
            OpLookup[Or] = OpLookup[BOr] = OpCode.Or;
            OpLookup[LeftShift] = OpLookup[BLeftShift] = OpCode.LeftShift;
            OpLookup[RightShift] = OpLookup[BRightShift] = OpCode.RightShift;
            OpLookup[Becomes] = OpCode.Becomes;

            OpLookup[Lt] = OpCode.Lt;
            OpLookup[Le] = OpCode.Le;
            OpLookup[Gt] = OpCode.Gt;
            OpLookup[Ge] = OpCode.Ge;
            OpLookup[Neq] = OpCode.Neq;
            OpLookup[Eql] = OpCode.Eql;
            #endregion

            #region values
            NonTerminal value = new NonTerminal("value");
            NonTerminal exactValue = new NonTerminal("exactValue", "exactValue");
            #endregion

            #region value rules
            ThreeVal.Rule = Identifier + "." + Identifier + "." + Identifier;
            TwoVal.Rule = Identifier + "." + Identifier;
            IsType.Rule = "$" + Identifier;
            RegisterBracePair("(", ")");
            RegisterBracePair("{", "}");
            IsExactType.Rule = "#" + Identifier;
            ParentVal.Rule =  "^" + Identifier + "." + Identifier | "^" + Identifier + "." + Identifier + "." + Identifier;
            exactValue.Rule = Identifier | TwoVal | ThreeVal | ParentVal;
            TypeMatcher.Rule = IsType | IsExactType| IsType + Plus + TypeMatcher | IsExactType + Plus + TypeMatcher;
            Argument.Rule = Expr | Expr + "," + Argument;
            Function.Rule = Identifier + ToTerm("(") +")" | Identifier + "(" + Argument + ")";
            #endregion

            #region expressions
            NonTerminal term = new NonTerminal("term");
            Expr.Rule = Expr + BinOp + Expr | UniOp + Expr | term;
            BinOp.Rule = Plus | Minus | Div | Mult | Rem | Pow | And | Or | Eql | Neq | Gt | Ge | Lt | Le | LeftShift | RightShift;
            UniOp.Rule = Not | Minus;
            term.Rule = Number | Str | openBrac + Expr + closeBrac | exactValue | Constants | Function;
            #endregion

            #region operator precedence
            /* NOTE: Order is taken from C++/C# order with power added in.
             * Higher number = more important
             * Increments are by 10 to allow easy adding of new terms
             * Power is not in C++/C#, but has been added where is seems to fit
             */
            RegisterOperators(10, Associativity.Right, Becomes);
            RegisterOperators(20, Associativity.Left, Or);
            RegisterOperators(30, Associativity.Left, And);
            RegisterOperators(40, Associativity.Left, Eql, Neq);
            RegisterOperators(50, Associativity.Left, Eql, Neq);
            RegisterOperators(60, Associativity.Left, Ge, Le, Lt, Gt);
            RegisterOperators(70, Associativity.Left, LeftShift, RightShift);
            RegisterOperators(70, Associativity.Left, Plus, Minus);
            RegisterOperators(80, Associativity.Left, Div, Mult, Rem);
            RegisterOperators(90, Associativity.Left, Pow);
            RegisterOperators(100, Associativity.Left, Not);

            #endregion

            #region game creation rules
            NonTerminal IsPrototype = new NonTerminal("IsPrototype", "IsPrototype");

            value.Rule = TypeMatcher | Expr;

            Names.Rule = Identifier + comma + Names | Identifier;
            IsPrototype.Rule = colon + Names | Empty;


            NonTerminal optionalProperties = new NonTerminal("optionalProperties", "optionalProperties");
            optionalProperties.Rule = Properties | Empty;
            Properties.Rule = Identifier + Becomes + value | Identifier + Becomes + value + comma + optionalProperties;

            BasicDefinition.Rule = openBrac + optionalProperties + closeBrac;

            NonTerminal optionalComponent = new NonTerminal("optionalComponent", "optionalComponent");
            optionalComponent.Rule = Component | Empty;
            Component.Rule = "Component" + Identifier + BasicDefinition + optionalComponent;

            


            NonTerminal optionalPrototype = new NonTerminal("optionalPrototype", "optionalPrototype");
            optionalPrototype.Rule = Prototype | Empty;
            Prototype.Rule = "Prototype" + Identifier + EntityDefinition + optionalPrototype;

            NonTerminal optionalManager = new NonTerminal("optionalManager", "optionalManager");
            optionalManager.Rule = Manager | Empty;
            Manager.Rule = "Manager" + Identifier + BasicDefinition + optionalManager;


            NonTerminal optionalService = new NonTerminal("optionalService", "optionalService");
            optionalService.Rule = Service | Empty;
            Service.Rule = "Service" + Identifier + BasicDefinition + optionalService;

            Else.Rule = ToTerm("else") + "if" + openBrac + Expr + closeBrac + openBrace + Actions + closeBrace + Else |
                ToTerm("else") + "if" + openBrac + Expr + closeBrac + openBrace + Actions + closeBrace |
                 "else" + openBrace + Actions + closeBrace + Else |
                 "else" + openBrace + Actions + closeBrace;

            Condition.Rule = "if" + openBrac + Expr + closeBrac + openBrace + Actions + closeBrace + Else |
                "if" + openBrac + Expr + closeBrac + openBrace + Actions + closeBrace;

            NonTerminal optionalAssignment = new NonTerminal("optionalAssignment", "optionalAssignment");
            optionalAssignment.Rule = Assignment | Empty;
            Loop.Rule = "while" + openBrac + Expr + closeBrac + openBrace + Actions + closeBrace |
                "for" + openBrac + optionalAssignment + ";" + Expr + ";" + Assignment + closeBrac + openBrace + Actions + closeBrace;

            NonTerminal optionalActions = new NonTerminal("OptionalActions", "OptionalActions");

            Action.Rule = Identifier + BasicDefinition;

            Assignment.Rule = exactValue + becomesExpr + Expr;

            Actions.Rule = Condition + optionalActions | Action + optionalActions | Assignment + optionalActions | Loop + optionalActions;
            optionalActions.Rule = Actions | Empty;

            NonTerminal optionalEvt = new NonTerminal("optionalEvt", "optionalEvt");
            optionalEvt.Rule = Evt | Empty;
            Evt.Rule = "Event" + Identifier + BasicDefinition + openBrace + optionalActions + closeBrace + optionalEvt;

            EntityDefinition.Rule = IsPrototype + BasicDefinition + openBrace + optionalComponent + optionalEvt + closeBrace;

            NonTerminal optionalEntity = new NonTerminal("optionalEntity", "optionalEntity");
            optionalEntity.Rule = Entity | Empty;

            NonTerminal optionalIdentifier = new NonTerminal("optionalIdentifier", "optionalIdentifier");
            optionalIdentifier.Rule = Identifier | Empty;
            Entity.Rule = "Entity" + optionalIdentifier + EntityDefinition + optionalEntity;

            NonTerminal optionalScene = new NonTerminal("optionalScene", "optionalScene");
            optionalScene.Rule = Scene | Empty;
            Scene.Rule = "Scene" + Identifier + BasicDefinition + openBrace + optionalManager + optionalEntity + closeBrace + optionalScene;

            NonTerminal optionalDefinitions = new NonTerminal("optionalDefinitions", "optionalDefinitions");
            optionalDefinitions.Rule = Definitions | Empty;
            Definitions.Rule = "define" + Identifier + "as" + ClassName + optionalDefinitions;

            NonTerminal optionalUses = new NonTerminal("optionalUses", "optionalUses");
            optionalUses.Rule = Uses | Empty;
            Uses.Rule = "using" + ClassName + openBrace + Definitions + closeBrace + optionalUses;

            Game.Rule = optionalUses + "Game" + BasicDefinition + openBrace + optionalPrototype + optionalService + Scene + closeBrace;
            #endregion

            Root = Game;
            //Removes from the tree, we don't care about having these there
            MarkPunctuation("{", "}", "(", ")", ":", "$", "@", "#", "^", "Game", "using", "define", "Scene", "Entity",
                 ",", "if", "Component", "Manager", "Prototype", "=", ".", "as", "Event", "else", "Service", ";", "while", "for");
            MarkReservedWords("using", "define", "if", "else", "while", "for");
            MarkTransient(BasicDefinition, value, IsPrototype, term, exactValue, optionalActions, optionalEvt, optionalComponent, 
                optionalProperties, optionalManager, optionalPrototype, optionalService, optionalUses, optionalDefinitions,
                optionalEntity, optionalScene, optionalIdentifier, optionalAssignment, becomesExpr);
        }
    }
}
