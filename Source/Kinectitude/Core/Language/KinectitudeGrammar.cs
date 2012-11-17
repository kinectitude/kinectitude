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
        //TODO known issue is that all terminals can have a space entity.component == entity . component
        internal static readonly IdentifierTerminal Identifier = TerminalFactory.CreateCSharpIdentifier("Identifier");
        internal static readonly RegexBasedTerminal Name = new RegexBasedTerminal("Name", "[a-zA-Z][a-zA-Z0-9_]*");
        internal static readonly Terminal Number = TerminalFactory.CreateCSharpNumber("Number");
        internal static readonly Terminal Str = TerminalFactory.CreateCSharpString("Str");
        internal static readonly Terminal ClassName = new RegexBasedTerminal("ClassName", @"@?[a-z_A-Z]\w+(?:\.@?[a-z_A-Z]\w+)*");

        #region kinectitude key words and parts
        internal static readonly NonTerminal Game = new NonTerminal("Game", "Game");

        internal static readonly NonTerminal Scenes = new NonTerminal("Scenes", "Scenes");
        internal static readonly NonTerminal Scene = new NonTerminal("Scene", "Scene");
        internal static readonly NonTerminal Entities = new NonTerminal("Entities", "Entities");
        internal static readonly NonTerminal Entity = new NonTerminal("Entity", "Entity");
        internal static readonly NonTerminal Properties = new NonTerminal("Properties");
        internal static readonly NonTerminal Names = new NonTerminal("Names", "Names");
        internal static readonly NonTerminal Prototype = new NonTerminal("Prototype", "Prototype");
        internal static readonly NonTerminal EntityDefinition = new NonTerminal("EntityDefinition", "EntityDefinition");
        internal static readonly NonTerminal BasicDefinition = new NonTerminal("BasicDefinition", "BasicDefinition");
        internal static readonly NonTerminal Manager = new NonTerminal("Manager", "Manager");
        internal static readonly NonTerminal Component = new NonTerminal("Component", "Component");
        internal static readonly NonTerminal Evt = new NonTerminal("Event", "Event");
        internal static readonly NonTerminal EvtDefinition = new NonTerminal("EvtDefinition", "EvtDefinition");
        internal static readonly NonTerminal Action = new NonTerminal("Action", "Action");
        internal static readonly NonTerminal Condition = new NonTerminal("Condition", "Condition");
        internal static readonly NonTerminal Actions = new NonTerminal("Actions", "Actions");
        internal static readonly NonTerminal Uses = new NonTerminal("Uses", "Uses");
        internal static readonly NonTerminal Classes = new NonTerminal("Classes", "Classes");
        internal static readonly NonTerminal Definitions = new NonTerminal("Definitions", "Definitions");
        internal static readonly NonTerminal Assignment = new NonTerminal("Assignment", "Assignment");
        #endregion

        #region Key Types of KGL
        internal static readonly NonTerminal TypeMatcher = new NonTerminal("TypeMatcher", "TypeMatcher");
        internal static readonly NonTerminal IsType = new NonTerminal("IsType", "IsType");
        internal static readonly NonTerminal IsExactType = new NonTerminal("IsExactType", "IsExactType");
        internal static readonly NonTerminal ThreeVal = new NonTerminal("ThreeVal", "ThreeVal");
        internal static readonly NonTerminal TwoVal = new NonTerminal("TwoVal", "TwoVal");
        internal static readonly NonTerminal ParentVal = new NonTerminal("ParentVal", "ParentVal");
        #endregion

        #region expressions
        internal static readonly NonTerminal Expr = new NonTerminal("Expr", "Expr");
        internal static readonly NonTerminal BinOp = new NonTerminal("BinOp", "BinOp");
        internal static readonly NonTerminal UniOp = new NonTerminal("UniOp", "UniOp");
        #endregion

        #region operator terminals
        //ToTerm needs an instance, but reference to these are needed.
        internal Terminal Becomes, Eql, Lt, Gt, Le, Ge, Neq, Plus, Minus, Mult, Div, Rem, Pow, And, Or, Not,
            LeftShift, RightShift, PlusEq, MinusEq, MultEq, DivEq, RemEq, PowEq, RshiftEq, LshiftEq;
        #endregion

        internal readonly Dictionary<string, ConstantReader> Constants = new Dictionary<string, ConstantReader>();

        public KinectitudeGrammar()
        {
            #region constants
            Constants.Add("true", ConstantReader.TrueValue);
            Constants.Add("false", ConstantReader.FalseValue);
            Constants.Add("Pi", new ConstantReader(Math.PI));
            Constants.Add("E", new ConstantReader(Math.E));
            #endregion
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
            PlusEq = ToTerm("+=", "PlusEq");
            MinusEq = ToTerm("-=", "MinusEq");
            MultEq = ToTerm("*=", "MultEq");
            DivEq = ToTerm("/=", "DivEq");
            RemEq = ToTerm("%=", "RemEq");
            PowEq = ToTerm("^=", "PowEq");
            RshiftEq = ToTerm(">>=", "RshiftEq");
            LshiftEq = ToTerm("<<=", "LshiftEq");
            #endregion

            #region values
            NonTerminal value = new NonTerminal("value");
            NonTerminal exactValue = new NonTerminal("exactValue", "exactValue");
            #endregion

            #region value rules
            ThreeVal.Rule = Name + "." + Identifier + "." + Identifier;
            TwoVal.Rule = Name + "." + Identifier;
            IsType.Rule = "$" + Name;
            RegisterBracePair("(", ")");
            RegisterBracePair("{", "}");
            IsExactType.Rule = "#" + Name;
            ParentVal.Rule =  "@" + Identifier + "." + Identifier | "@" + Identifier + "." + Identifier + "." + Identifier;
            exactValue.Rule = Name | TwoVal | ThreeVal | ParentVal;
            TypeMatcher.Rule = IsType | IsExactType| IsType + Plus + TypeMatcher | IsExactType + Plus + TypeMatcher;
            #endregion

            #region expressions
            NonTerminal term = new NonTerminal("term");
            Expr.Rule = Expr + BinOp + Expr | UniOp + Expr | term;
            BinOp.Rule = Plus | Minus | Div | Mult | Rem | Pow | And | Or | Eql | Neq | Gt | Ge | Lt | Le | LeftShift | RightShift;
            UniOp.Rule = Not | Minus;
            term.Rule = Number | Str | openBrac + Expr + closeBrac | exactValue;
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

            #endregion

            #region game creation rules
            NonTerminal IsPrototype = new NonTerminal("IsPrototype", "IsPrototype");

            AddTermsReportGroup("constants", "true", "false", "Pi", "E");
            value.Rule =  TypeMatcher | Expr;

            Names.Rule = Name + comma + Names | Name;
            IsPrototype.Rule = colon + Names | Empty;

            BasicDefinition.Rule = openBrac + Properties + closeBrac;

            Properties.Rule = Identifier + Becomes + value | Identifier + Becomes + value + comma + Properties | Empty;
            EntityDefinition.Rule = IsPrototype + BasicDefinition + openBrace + Component + Evt + closeBrace;

            Prototype.Rule = "Prototype" + Name + EntityDefinition + Prototype | Empty;
            Manager.Rule = "Manager" + ClassName + BasicDefinition + Manager | Empty;
            Component.Rule = "Component" + ClassName + BasicDefinition + Component | Empty;

            NonTerminal OptionalActions = new NonTerminal("OptionalActions", "OptionalActions");
            Condition.Rule = "if" + openBrac + Expr + closeBrac + openBrace + Actions + closeBrace;

            Action.Rule = Identifier + BasicDefinition;
            Actions.Rule = Action + OptionalActions | Condition + OptionalActions;
            OptionalActions.Rule = Actions | Empty;

            Evt.Rule = "Event" + Name + BasicDefinition + openBrace + Actions + closeBrace + Evt | Empty;

            Entity.Rule = "Entity" + Name + EntityDefinition | "Entity" + EntityDefinition;
            Entities.Rule = Entity + Entities | Entity;

            Scenes.Rule = Scene + Scenes | Scene;
            Scene.Rule = "Scene" + Name + BasicDefinition + openBrace + Manager + Entities + closeBrace;

            Uses.Rule = "using" + ClassName + openBrace + Definitions + closeBrace + Uses | Empty;
            Definitions.Rule = "define" + Name + "as" + ClassName + Definitions | "define" + Name + "as" + ClassName;

            Game.Rule = Uses + "Game"  + BasicDefinition + openBrace + Prototype + Scenes + closeBrace;
            #endregion

            Root = Game;
            //Removes from the tree, we don't care about having these there
            MarkPunctuation("{", "}", "(", ")", ":", "$", "@", "#", "Game", "using", "define", "Scene", "Entity",
                 ",", "if", "Component", "Manager", "Prototype", "=", ".", "as", "Event");

            MarkTransient(BasicDefinition, value, IsPrototype, term, exactValue, OptionalActions);
        }
    }
}
