using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace IronlyLangTest
{
    class KinectGrammar : Grammar
    {
        //TODO known issue is that all terminals can have a space entity.component == entity . component
        private IdentifierTerminal identifier = TerminalFactory.CreateCSharpIdentifier("identifier");
        private RegexBasedTerminal name = new RegexBasedTerminal("name", "[a-zA-Z][a-zA-Z0-9_]*");

        private void createDefinitionsRules(NonTerminal many, NonTerminal single, string type, BnfTerm definedAs)
        {
            single.Rule = type + identifier + definedAs;
            many.Rule = single + many | Empty;
        }

        public KinectGrammar()
        {
            Terminal openBrace = ToTerm("{");
            Terminal closeBrace = ToTerm("}");
            Terminal equals = ToTerm("=");
            Terminal colon = ToTerm(":");
            Terminal eql = ToTerm("==");
            Terminal lt = ToTerm("<");
            Terminal gt = ToTerm(">");
            Terminal le = ToTerm("<=");
            Terminal ge = ToTerm(">=");
            Terminal ne = ToTerm("!=");
            Terminal plus = ToTerm("+");
            Terminal minus = ToTerm("-");
            Terminal mult = ToTerm("*");
            Terminal div = ToTerm("/");
            Terminal rem = ToTerm("%");
            Terminal pow = ToTerm("^");
            Terminal openBrac = ToTerm("(");
            Terminal closeBrac = ToTerm(")");
            Terminal and = ToTerm("&&");
            Terminal or = ToTerm("||");
            Terminal not = ToTerm("!");

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

            NonTerminal value = new NonTerminal("value");

            NonTerminal threeVal = new NonTerminal("threeVal");
            threeVal.Rule = name + "." + identifier + "." + identifier;

            NonTerminal twoVal = new NonTerminal("twoVal");
            twoVal.Rule = name + "." + identifier;


            NonTerminal isType = new NonTerminal("isType");
            isType.Rule = "$" + name;

            NonTerminal isExactType = new NonTerminal("isExactType");
            isExactType.Rule = "#" + name;

            NonTerminal parentVal = new NonTerminal("parentVal");
            parentVal.Rule = "@" + twoVal | "@" + threeVal | "@" + identifier;

            RegisterBracePair("(", ")");
            RegisterBracePair("{", "}");

            NonTerminal exactValue = new NonTerminal("exactValue");
            exactValue.Rule = name | twoVal | threeVal | parentVal;

            NonTerminal typeMatcher = new NonTerminal("typeMatcher");
            typeMatcher.Rule = isType | isExactType | isType + plus + typeMatcher | isExactType + plus + typeMatcher;

            #region math

            Terminal number = TerminalFactory.CreateCSharpNumber("number");
            NonTerminal mathExpr = new NonTerminal("expr");
            NonTerminal mathBinOp = new NonTerminal("binOp");
            NonTerminal mathTerm = new NonTerminal("term");

            mathExpr.Rule = mathTerm | mathExpr + mathBinOp + ReduceHere() + mathExpr;
            mathBinOp.Rule = plus | minus | div | mult | pow;
            mathTerm.Rule = number | "(" + mathExpr + ")" | exactValue | minus + mathExpr | plus + mathExpr;

            RegisterOperators(10, plus, minus);
            RegisterOperators(40, div, mult, rem);
            RegisterOperators(50, pow);
            #endregion

            #region string

            NonTerminal strExpr = new NonTerminal("strExpr");
            Terminal str = TerminalFactory.CreateCSharpString("str");
            strExpr.Rule = str | str + plus + ReduceHere() + strExpr | exactValue | exactValue + plus + strExpr;

            #endregion

            #region bool

            NonTerminal boolExpr = new NonTerminal("boolExpr");
            NonTerminal boolTerm = new NonTerminal("boolTerm");
            NonTerminal boolBinOp = new NonTerminal("boolBinOp");

            boolBinOp.Rule = and | or | "and" | "or";

            RegisterOperators(20, or, ToTerm("or"));
            RegisterOperators(30, and, ToTerm("and"));

            boolExpr.Rule = boolTerm | boolExpr + boolBinOp + boolExpr;

            //TODO think of a way to make this not ambigious with values
            boolTerm.Rule = exactValue + ReduceHere() | "true" | "false" | openBrac + boolExpr + closeBrac | not + boolExpr |
                mathExpr + eql + PreferShiftHere() + mathExpr | mathExpr + ne + PreferShiftHere() + mathExpr | mathExpr + gt + mathExpr |
                mathExpr + ge + mathExpr | mathExpr + lt + mathExpr | mathExpr + le + mathExpr |
                strExpr + eql + PreferShiftHere() + strExpr | strExpr + ne + PreferShiftHere() + strExpr;

            #endregion

            value.Rule = typeMatcher | mathExpr | boolExpr | strExpr;

            names.Rule = names + name | name;
            isPrototype.Rule = colon + names | Empty;

            basicDefinition.Rule = openBrace + properties + closeBrace;

            properties.Rule = properties + identifier + equals + value | Empty;
            entityDefinition.Rule = isPrototype + openBrace + properties + components + evts + closeBrace;

            createDefinitionsRules(prototypes, prototype, "Prototype", entityDefinition);
            createDefinitionsRules(managers, manager, "Manager", basicDefinition);
            createDefinitionsRules(components, component, "Component", basicDefinition);

            condition.Rule = "if" + openBrac + boolExpr + closeBrac + openBrace + optionalActions + closeBrace;

            action.Rule = "Action" + identifier + basicDefinition;
            actions.Rule = action + optionalActions | condition + optionalActions;
            optionalActions.Rule = actions | Empty;

            evtDefinition.Rule = openBrace + properties + actions + closeBrace;
            createDefinitionsRules(evts, evt, "Event", evtDefinition);

            entity.Rule = "Entity" + name + entityDefinition | "Entity" + entityDefinition;
            entities.Rule = entity + entities | entity;

            scenes.Rule = scene + scenes | scene;
            scene.Rule = "Scene" + name + openBrace + properties + managers + entities + closeBrace;

            NonTerminal className = new NonTerminal("className");
            className.Rule = identifier | identifier + "." + className;

            uses.Rule = files + uses | Empty;
            files.Rule = "using" + className + openBrace + definitions + closeBrace;
            define.Rule = "define" + identifier + "as" + className;
            definitions.Rule = define + definitions | define;

            game.Rule = uses + "Game" + name + openBrace + properties + prototypes + scenes + closeBrace;
            Root = game;

            MarkPunctuation("{", "}", "(", ")", ":");
            MarkTransient(exactValue);
        }
    }
}
