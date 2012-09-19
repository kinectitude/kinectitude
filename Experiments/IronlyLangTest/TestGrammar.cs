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
    [Language("test", "1.0", "Sample test grammar")]
    public class TestGrammar : Grammar
    {
        //TODO known issue is that all terminals can have a space entity.component == entity . component
        private IdentifierTerminal identifier = TerminalFactory.CreateCSharpIdentifier("identifier");
        private RegexBasedTerminal name = new RegexBasedTerminal("name", "[a-zA-Z][a-zA-Z0-9_]*");

        private void createDefinitionsRules(NonTerminal many, NonTerminal single, string type, BnfTerm definedAs)
        {
            single.Rule = type + identifier + definedAs;
            many.Rule =  single + many | Empty;
        }

        public TestGrammar()
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
            Terminal pow = ToTerm("**");
            Terminal openBrac = ToTerm("(");
            Terminal closeBrac = ToTerm(")");
            Terminal and = ToTerm("&&");
            Terminal or = ToTerm("||");

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

            MarkPunctuation(colon, eql, lt, gt, le, ge, ne, openBrac, closeBrac, plus, minus, div, mult, rem, pow, and, or);
            RegisterBracePair("(", ")");
            RegisterBracePair("{", "}");

            NonTerminal exactValue = new NonTerminal("exactValue");
            exactValue.Rule = name | twoVal | threeVal | parentVal;

            NonTerminal typeMatcher = new NonTerminal("typeMatcher");
            typeMatcher.Rule = isType | isExactType | isType + plus + typeMatcher | isExactType + plus + typeMatcher;

            #region math

            Terminal number = TerminalFactory.CreateCSharpNumber("number");
            NonTerminal mathExpr = new NonTerminal("mathExpr");
            NonTerminal mathTerm = new NonTerminal("mathTerm");
            NonTerminal mathFactor = new NonTerminal("mathFactor");

            mathExpr.Rule = mathTerm | mathExpr + plus + mathTerm | mathExpr + minus + mathTerm;
            mathTerm.Rule = mathFactor | mathTerm + mult + mathFactor | mathTerm + div + mathFactor | mathTerm + rem + mathFactor;
            //TODO think of a way to make this not ambigious with values
            mathFactor.Rule = "n:" + exactValue | number | "-" + number | openBrac + mathExpr + closeBrac;
            
            #endregion

            #region string

            NonTerminal strExpr = new NonTerminal("strExpr");
            Terminal str = TerminalFactory.CreateCSharpString("str");
            strExpr.Rule = str | str + plus + strExpr | exactValue | exactValue + plus + strExpr;

            #endregion
            
            #region bool

            NonTerminal boolExpr = new NonTerminal("boolExpr");
            NonTerminal boolTerm = new NonTerminal("boolTerm");
            NonTerminal boolFactor = new NonTerminal("boolFactor");
            NonTerminal test = new NonTerminal("test");

            boolExpr.Rule = boolTerm | boolExpr + or + boolTerm | boolExpr + "or" + boolTerm;
            boolTerm.Rule = boolFactor | boolTerm + and + boolFactor | boolTerm + "and" + boolFactor;
            
            test.Rule = mathExpr + lt + mathExpr | mathExpr + gt + mathExpr | mathExpr + eql + mathExpr |
                mathExpr + le + mathExpr | mathExpr + ge + mathExpr | mathExpr + ne + mathExpr |
                //TODO this should be allowed look into why it causes shift red conflicts
                /*boolExpr + eql + boolExpr | boolExpr + ne + boolExpr |*/ strExpr + eql + strExpr | strExpr + ne + strExpr;

            //TODO think of a way to make this not ambigious with values
            boolFactor.Rule = "b:" + exactValue | "true" | "false" | openBrac + boolExpr + closeBrac | test;

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
            //TODO this may need better logic?
            define.Rule = "define" + identifier + "as" + className;
            definitions.Rule = define + definitions | define;

            game.Rule = uses + "Game" + name + openBrace  + properties + prototypes + scenes + closeBrace;
            Root = game;
        }
    }
}
