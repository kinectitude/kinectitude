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
        private IdentifierTerminal identifier = TerminalFactory.CreateCSharpIdentifier("identifier");
        private RegexBasedTerminal name = new RegexBasedTerminal("name", "[a-zA-Z][a-zA-Z0-9_]*");
        private Terminal number = TerminalFactory.CreateCSharpNumber("number");
        private Terminal str = TerminalFactory.CreateCSharpString("str");

        private void createDefinitionsRules(NonTerminal many, NonTerminal single, string type, BnfTerm definedAs)
        {
            single.Rule = type + identifier + definedAs;
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
            Terminal RightShift = ToTerm(">>");
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
            NonTerminal files = new NonTerminal("files");
            NonTerminal classes = new NonTerminal("classes");
            NonTerminal definitions = new NonTerminal("definitions");
            NonTerminal define = new NonTerminal("define");
            NonTerminal className = new NonTerminal("className");
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
            #endregion

            #endregion

            #region value rules
            threeVal.Rule = name + "." + identifier + "." + identifier;
            twoVal.Rule = name + "." + identifier;
            isType.Rule = "$" + name;
            RegisterBracePair("(", ")");
            RegisterBracePair("{", "}");
            isExactType.Rule = "#" + name;
            parentVal.Rule = "@" + twoVal | "@" + threeVal | "@" + identifier;
            exactValue.Rule = name | twoVal | threeVal | parentVal;
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
            term.Rule = number | str | openBrac + expr + closeBrac | exactValue | minus + expr | plus + expr;
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
            RegisterOperators(70, Associativity.Left, leftShift, RightShift);
            RegisterOperators(70, Associativity.Left, plus, minus);
            RegisterOperators(80, Associativity.Left, div, mult, rem);
            RegisterOperators(90, Associativity.Left, pow);

            #endregion

            #region game creation rules

            value.Rule = typeMatcher | expr;

            names.Rule = names + name | name;
            isPrototype.Rule = colon + names | Empty;

            basicDefinition.Rule = openBrace + properties + closeBrace;

            properties.Rule = properties + identifier + becomes + value | Empty;
            entityDefinition.Rule = isPrototype + openBrace + properties + components + evts + closeBrace;

            createDefinitionsRules(prototypes, prototype, "Prototype", entityDefinition);
            createDefinitionsRules(managers, manager, "Manager", basicDefinition);
            createDefinitionsRules(components, component, "Component", basicDefinition);

            condition.Rule = "if" + openBrac + expr + closeBrac + openBrace + optionalActions + closeBrace;

            action.Rule = "Action" + identifier + basicDefinition;
            actions.Rule = action + optionalActions | condition + optionalActions;
            optionalActions.Rule = actions | Empty;

            evtDefinition.Rule = openBrace + properties + actions + closeBrace;
            createDefinitionsRules(evts, evt, "Event", evtDefinition);

            entity.Rule = "Entity" + name + entityDefinition | "Entity" + entityDefinition;
            entities.Rule = entity + entities | entity;

            scenes.Rule = scene + scenes | scene;
            scene.Rule = "Scene" + name + openBrace + properties + managers + entities + closeBrace;

            className.Rule = identifier | identifier + "." + className;

            uses.Rule = files + uses | Empty;
            files.Rule = "using" + className + openBrace + definitions + closeBrace;
            define.Rule = "define" + identifier + "as" + className;
            definitions.Rule = define + definitions | define;

            game.Rule = uses + "Game" + name + openBrace  + properties + prototypes + scenes + closeBrace;
            #endregion

            Root = game;
            //Removes from the tree, we don't care about having these there
            MarkPunctuation("{", "}", "(", ")", ":", "$", "@", "#", "Game", "using", "define", "Scene", "Entity", 
                "Action", "if", "Component", "Manager", "Prototype");
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
