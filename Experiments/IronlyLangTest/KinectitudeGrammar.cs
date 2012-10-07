using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using Irony.Parsing.Construction;
using Irony.Interpreter.Ast;

namespace IronlyLangTest
{
    [Language("Kinectitude Game Language", "1.0a", "Used to create Kinectitude games")]
    public class KinectitudeGrammar : Grammar
    {
        //TODO known issue is that all terminals can have a space entity.component == entity . component
        private readonly IdentifierTerminal Identifier = TerminalFactory.CreateCSharpIdentifier("Identifier");
        private readonly RegexBasedTerminal Name = new RegexBasedTerminal("Name", "[a-zA-Z][a-zA-Z0-9_]*");
        private readonly Terminal Number = TerminalFactory.CreateCSharpNumber("Number");
        private readonly Terminal Str = TerminalFactory.CreateCSharpString("Str");
        private readonly Terminal ClassName = new RegexBasedTerminal(@"@?[a-z_A-Z]\w+(?:\.@?[a-z_A-Z]\w+)*");

        private void createDefinitionsRules(NonTerminal many, NonTerminal single, string type, BnfTerm definedAs)
        {
            single.Rule = type + Identifier + definedAs;
            many.Rule =  single + many | Empty;
        }

        public KinectitudeGrammar()
        {

            #region operator terminals
            Terminal openBrace = ToTerm("{");
            Terminal closeBrace = ToTerm("}");
            Terminal becomes = ToTerm("=");
            Terminal colon = ToTerm(":");
            Terminal eql = ToTerm("==");
            Terminal lt = ToTerm("<");
            Terminal gt = ToTerm(">");
            Terminal le = ToTerm("<=");
            Terminal ge = ToTerm(">=");
            Terminal neq = ToTerm("!=");
            Terminal plus = ToTerm("+");
            Terminal minus = ToTerm("-");
            Terminal mult = ToTerm("*");
            Terminal div = ToTerm("/");
            Terminal rem = ToTerm("%");
            Terminal pow = ToTerm("^");
            Terminal openBrac = ToTerm("(");
            Terminal closeBrac = ToTerm(")");
            Terminal and = new RegexBasedTerminal(@"(&&)|and");
            Terminal or = new RegexBasedTerminal(@"(\|\|)|or");
            Terminal not = new RegexBasedTerminal(@"\!|(not)");
            Terminal leftShift = ToTerm("<<");
            Terminal rightShift = ToTerm(">>");
            Terminal comma = ToTerm(",");
            Terminal plusEq = ToTerm("+=");
            Terminal minusEq = ToTerm("-=");
            Terminal multEq = ToTerm("*=");
            Terminal divEq = ToTerm("/=");
            Terminal remEq = ToTerm("%=");
            Terminal powEq = ToTerm("^=");
            Terminal rshiftEq = ToTerm(">>=");
            Terminal lshiftEq = ToTerm("<<=");
            #endregion

            #region kinectitude langauge part definition

            #region kinectitude key words and parts
            NonTerminal game = new NonTerminal("game");
            NonTerminal scenes = new NonTerminal("scenes");
            NonTerminal scene = new NonTerminal("scene");
            NonTerminal entities = new NonTerminal("entities");
            NonTerminal entity = new NonTerminal("entity");
            NonTerminal properties = new NonTerminal("properties");
            NonTerminal names = new NonTerminal("names");
            NonTerminal isPrototype = new NonTerminal("isPrototype");
            NonTerminal prototypes = new NonTerminal("prototypes");
            NonTerminal entityDefinition = new NonTerminal("entityDefinition");
            NonTerminal prototype = new NonTerminal("prototype");
            NonTerminal basicDefinition = new NonTerminal("basicDefinition");
            NonTerminal managers = new NonTerminal("managers");
            NonTerminal manager = new NonTerminal("manager");
            NonTerminal components = new NonTerminal("components");
            NonTerminal component = new NonTerminal("component");
            NonTerminal evts = new NonTerminal("events");
            NonTerminal evt = new NonTerminal("event");
            NonTerminal evtDefinition = new NonTerminal("evtDefinition");
            NonTerminal optionalActions = new NonTerminal("optionalActions");
            NonTerminal action = new NonTerminal("action");
            NonTerminal condition = new NonTerminal("condition");
            NonTerminal actions = new NonTerminal("actions");
            NonTerminal uses = new NonTerminal("uses");
            NonTerminal file = new NonTerminal("file");
            NonTerminal classes = new NonTerminal("classes");
            NonTerminal definitions = new NonTerminal("definitions");
            NonTerminal define = new NonTerminal("define");
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
            NonTerminal legalSetValue = new NonTerminal("legalSetValue");
            #endregion

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
            uniOp.Rule = not;
            term.Rule = Number | Str | openBrac + expr + closeBrac | exactValue | minus + expr | plus + expr;
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

            names.Rule = names + Name | Name;
            isPrototype.Rule = colon + names | Empty;

            basicDefinition.Rule = openBrac + properties + closeBrac;

            properties.Rule = Identifier + becomes + value | Identifier + becomes + value + comma + properties | Empty;
            entityDefinition.Rule = isPrototype + basicDefinition + openBrace + components + evts + closeBrace;

            /*assignments.Rule = exactValue + becomes + value | exactValue + plusEq + value | 
                exactValue + minusEq + value | exactValue + multEq + value | exactValue + remEq + value | 
                exactValue + divEq + value | */

            createDefinitionsRules(prototypes, prototype, "Prototype", entityDefinition);
            createDefinitionsRules(managers, manager, "Manager", basicDefinition);
            createDefinitionsRules(components, component, "Component", basicDefinition);

            condition.Rule = "if" + openBrac + expr + closeBrac + openBrace + optionalActions + closeBrace;

            action.Rule = Identifier + basicDefinition;
            actions.Rule = action + optionalActions | condition + optionalActions;
            optionalActions.Rule = actions | Empty;

            evtDefinition.Rule = basicDefinition + openBrace + actions + closeBrace;
            createDefinitionsRules(evts, evt, "Event", evtDefinition);

            entity.Rule = "Entity" + Name + entityDefinition | "Entity" + entityDefinition;
            entities.Rule = entity + entities | entity;

            scenes.Rule = scene + scenes | scene;
            scene.Rule = "Scene" + Name + basicDefinition + openBrace + managers + entities + closeBrace;

            uses.Rule = file + uses | Empty;
            file.Rule = "using" + ClassName + openBrace + definitions + closeBrace;
            define.Rule = "define" + Identifier + "as" + ClassName;
            definitions.Rule = define + definitions | define;

            game.Rule = uses + "Game" + Name + basicDefinition + openBrace + prototypes + scenes + closeBrace;
            #endregion

            Root = game;
            //Removes from the tree, we don't care about having these there
            MarkPunctuation("{", "}", "(", ")", ":", "$", "@", "#", "Game", "using", "define", "Scene", "Entity", 
                 ",", "if", "Component", "Manager", "Prototype", "=", ".", "as");

            MarkTransient(basicDefinition, value, basicDefinition, isPrototype);
        }

        public static void Main()
        {
            KinectitudeGrammar kGrammar = new KinectitudeGrammar();
            Parser parser = new Parser(kGrammar);
            string fileName = @"D:\School\Assignments\Kinectitude\tmpconversion.txt";
            string source = System.IO.File.ReadAllText(fileName);
            ParseTree tree = parser.Parse(source, fileName);
        }
    }
}
