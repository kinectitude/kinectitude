using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using Irony.Parsing.Construction;
using Irony.Interpreter.Ast;

namespace Kinectitude.Core.Language
{
    [Language("Kinectitude TEST Game Language", "1.0.0", "Used to create Kinectitude games")]
    public class KinectitudeGrammar : Grammar
    {
        //TODO known issue is that all terminals can have a space entity.component == entity . component
        private static readonly IdentifierTerminal Identifier = TerminalFactory.CreateCSharpIdentifier("Identifier");
        private static readonly RegexBasedTerminal Name = new RegexBasedTerminal("Name", "[a-zA-Z][a-zA-Z0-9_]*");
        private static readonly Terminal Number = TerminalFactory.CreateCSharpNumber("Number");
        private static readonly Terminal Str = TerminalFactory.CreateCSharpString("Str");
        private static readonly Terminal ClassName = new RegexBasedTerminal(@"@?[a-z_A-Z]\w+(?:\.@?[a-z_A-Z]\w+)*");

        #region kinectitude key words and parts
        internal static readonly NonTerminal Game = new NonTerminal("Game");

        internal static readonly NonTerminal Scenes = new NonTerminal("Scenes");
        internal static readonly NonTerminal Scene = new NonTerminal("Scene");
        internal static readonly NonTerminal Entities = new NonTerminal("Entities");
        internal static readonly NonTerminal Entity = new NonTerminal("Entity");
        internal static readonly NonTerminal Properties = new NonTerminal("Properties");
        internal static readonly NonTerminal Names = new NonTerminal("Names");
        internal static readonly NonTerminal IsPrototype = new NonTerminal("IsPrototype");
        internal static readonly NonTerminal Prototypes = new NonTerminal("Prototypes");
        internal static readonly NonTerminal EntityDefinition = new NonTerminal("EntityDefinition");
        internal static readonly NonTerminal Prototype = new NonTerminal("Prototype");
        internal static readonly NonTerminal BasicDefinition = new NonTerminal("BasicDefinition");
        internal static readonly NonTerminal Managers = new NonTerminal("Managers");
        internal static readonly NonTerminal Manager = new NonTerminal("Manager");
        internal static readonly NonTerminal Components = new NonTerminal("Components");
        internal static readonly NonTerminal Component = new NonTerminal("Component");
        internal static readonly NonTerminal Evts = new NonTerminal("events");
        internal static readonly NonTerminal Evt = new NonTerminal("event");
        internal static readonly NonTerminal EvtDefinition = new NonTerminal("EvtDefinition");
        internal static readonly NonTerminal OptionalActions = new NonTerminal("OptionalActions");
        internal static readonly NonTerminal Action = new NonTerminal("Action");
        internal static readonly NonTerminal Condition = new NonTerminal("Condition");
        internal static readonly NonTerminal Actions = new NonTerminal("Actions");
        internal static readonly NonTerminal Uses = new NonTerminal("Uses");
        internal static readonly NonTerminal File = new NonTerminal("File");
        internal static readonly NonTerminal Classes = new NonTerminal("Classes");
        internal static readonly NonTerminal Definitions = new NonTerminal("Definitions");
        #endregion

        private void createDefinitionsRules(NonTerminal many, NonTerminal single, string type, BnfTerm definedAs)
        {
            single.Rule = type + Identifier + definedAs;
            many.Rule = single + many | Empty;
        }

        public KinectitudeGrammar()
        {

            #region operator terminals
            Terminal openBrace = ToTerm("{");
            Terminal closeBrace = ToTerm("}");
            Terminal becomes = ToTerm("=", "becomes");
            Terminal colon = ToTerm(":");
            Terminal eql = ToTerm("==", "eql");
            Terminal lt = ToTerm("<", "lt");
            Terminal gt = ToTerm(">", "gt");
            Terminal le = ToTerm("<=", "le");
            Terminal ge = ToTerm(">=", "ge");
            Terminal neq = ToTerm("!=", "ne");
            Terminal plus = ToTerm("+", "plus");
            Terminal minus = ToTerm("-", "minus");
            Terminal mult = ToTerm("*", "mult");
            Terminal div = ToTerm("/", "div");
            Terminal rem = ToTerm("%", "rem");
            Terminal pow = ToTerm("^", "pow");
            Terminal openBrac = ToTerm("(");
            Terminal closeBrac = ToTerm(")");
            Terminal and = new RegexBasedTerminal(@"(&&)|(and)", "and");
            Terminal or = new RegexBasedTerminal(@"(\|\|)|(or)", "or");
            Terminal not = new RegexBasedTerminal(@"\!|(not)", "not");
            Terminal leftShift = ToTerm("<<", "leftShitf");
            Terminal rightShift = ToTerm(">>", "rightShift");
            Terminal comma = ToTerm(",");
            Terminal plusEq = ToTerm("+=", "plusEq");
            Terminal minusEq = ToTerm("-=", "minusEq");
            Terminal multEq = ToTerm("*=", "multEq");
            Terminal divEq = ToTerm("/=", "divEq");
            Terminal remEq = ToTerm("%=", "remEq");
            Terminal powEq = ToTerm("^=", "powEq");
            Terminal rshiftEq = ToTerm(">>=", "rshiftEq");
            Terminal lshiftEq = ToTerm("<<=", "lshiftEq");
            #endregion

            #region values
            NonTerminal value = new NonTerminal("value");
            NonTerminal threeVal = new NonTerminal("threeVal");
            NonTerminal twoVal = new NonTerminal("twoVal");
            NonTerminal isType = new NonTerminal("isType");
            NonTerminal isExactType = new NonTerminal("isExactType");
            NonTerminal parentVal = new NonTerminal("parentVal");
            NonTerminal exactValue = new NonTerminal("exactValue");
            NonTerminal typeMatcher = new NonTerminal("typeMatcher");
            //NonTerminal legalSetValue = new NonTerminal("legalSetValue");
            #endregion

            #region value rules
            threeVal.Rule = Name + "." + Identifier + "." + Identifier;
            twoVal.Rule = Name + "." + Identifier;
            isType.Rule = "$" + Name;
            RegisterBracePair("(", ")");
            RegisterBracePair("{", "}");
            isExactType.Rule = "#" + Name;
            parentVal.Rule = "@" + twoVal | "@" + threeVal | "@" + Identifier;
            exactValue.Rule = Name | twoVal | threeVal | parentVal;
            typeMatcher.Rule = isType | isExactType | isType + plus + typeMatcher | isExactType + plus + typeMatcher;
            #endregion

            #region expressions
            NonTerminal expr = new NonTerminal("expr");
            NonTerminal binOp = new NonTerminal("binOp");
            NonTerminal uniOp = new NonTerminal("uniOp");
            NonTerminal term = new NonTerminal("term");

            expr.Rule = term | expr + binOp + expr | uniOp + expr;
            binOp.Rule = plus | minus | div | mult | pow | and | or | eql | neq | gt | ge | lt | le;
            uniOp.Rule = not | minus | plus;
            term.Rule = Number | Str | openBrac + expr + closeBrac | exactValue;
            #endregion

            #region operator precedence
            /* NOTE: Order is taken from C++/C# order with power added in.
             * Higher number = more important
             * Increments are by 10 to allow easy adding of new terms
             * Power is not in C++/C#, but has been added where is seems to fit
             */
            RegisterOperators(10, Associativity.Right, becomes);
            RegisterOperators(20, Associativity.Left, or);
            RegisterOperators(30, Associativity.Left, and);
            RegisterOperators(40, Associativity.Left, eql, neq);
            RegisterOperators(50, Associativity.Left, eql, neq);
            RegisterOperators(60, Associativity.Left, ge, le, lt, gt);
            RegisterOperators(70, Associativity.Left, leftShift, rightShift);
            RegisterOperators(70, Associativity.Left, plus, minus);
            RegisterOperators(80, Associativity.Left, div, mult, rem);
            RegisterOperators(90, Associativity.Left, pow);

            #endregion

            #region game creation rules

            value.Rule = typeMatcher | expr;

            Names.Rule = Names + Name | Name;
            IsPrototype.Rule = colon + Names | Empty;

            BasicDefinition.Rule = openBrac + Properties + closeBrac;

            Properties.Rule = Identifier + becomes + value | Identifier + becomes + value + comma + Properties | Empty;
            EntityDefinition.Rule = IsPrototype + BasicDefinition + openBrace + Components + Evts + closeBrace;

            /*assignments.Rule = exactValue + becomes + value | exactValue + plusEq + value | 
                exactValue + minusEq + value | exactValue + multEq + value | exactValue + remEq + value | 
                exactValue + divEq + value | */

            createDefinitionsRules(Prototypes, Prototype, "Prototype", EntityDefinition);
            createDefinitionsRules(Managers, Manager, "Manager", BasicDefinition);
            createDefinitionsRules(Components, Component, "Component", BasicDefinition);

            Condition.Rule = "if" + openBrac + expr + closeBrac + openBrace + OptionalActions + closeBrace;

            Action.Rule = Identifier + BasicDefinition;
            Actions.Rule = Action + OptionalActions | Condition + OptionalActions;
            OptionalActions.Rule = Actions | Empty;

            EvtDefinition.Rule = BasicDefinition + openBrace + Actions + closeBrace;
            createDefinitionsRules(Evts, Evt, "Event", EvtDefinition);

            Entity.Rule = "Entity" + Name + EntityDefinition | "Entity" + EntityDefinition;
            Entities.Rule = Entity + Entities | Entity;

            Scenes.Rule = Scene + Scenes | Scene;
            Scene.Rule = "Scene" + Name + BasicDefinition + openBrace + Managers + Entities + closeBrace;

            Uses.Rule = "using" + ClassName + openBrace + Definitions + closeBrace + Uses | Empty;
            Definitions.Rule = "define" + Identifier + "as" + ClassName + Definitions | "define" + Identifier + "as" + ClassName;

            Game.Rule = Uses + "Game" + Name + BasicDefinition + openBrace + Prototypes + Scenes + closeBrace;
            #endregion

            Root = Game;
            //Removes from the tree, we don't care about having these there
            MarkPunctuation("{", "}", "(", ")", ":", "$", "@", "#", "Game", "using", "define", "Scene", "Entity",
                 ",", "if", "Component", "Manager", "Prototype", "=", ".", "as");

            MarkTransient(BasicDefinition, value, BasicDefinition, IsPrototype, term);
            //LanguageFlags = LanguageFlags.CreateAst;
        }
    }
}
