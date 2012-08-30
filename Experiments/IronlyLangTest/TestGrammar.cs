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
        private IdentifierTerminal identifier = TerminalFactory.CreateCSharpIdentifier("identifier");
        private RegexBasedTerminal name = new RegexBasedTerminal("name", "[a-zA-Z][a-zA-Z0-9_]*");
        private RegexBasedTerminal anything = new RegexBasedTerminal("value", @"[^ {}]+");

        private void createDefinitionsRules(NonTerminal many, NonTerminal single, string type, BnfTerm definedAs)
        {
            single.Rule = type + identifier + definedAs;
            many.Rule =  single + many | Empty;
        }

        public TestGrammar()
        {
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
            Terminal openBrac = ToTerm("(");
            Terminal closeBrac = ToTerm(")");

            MarkPunctuation(colon, eql, lt, gt, le, ge, ne, openBrac, closeBrac);
            RegisterBracePair("(", ")");
            RegisterBracePair("{", "}");

            names.Rule = names + name | name;
            isPrototype.Rule = colon + names | Empty;

            basicDefinition.Rule = openBrace + properties + closeBrace;

            //TODO make this better
            properties.Rule = properties + identifier + equals + anything | Empty;
            entityDefinition.Rule = isPrototype + openBrace + properties + components + evts + closeBrace;


            createDefinitionsRules(prototypes, prototype, "Prototype", entityDefinition);
            createDefinitionsRules(managers, manager, "Manager", basicDefinition);
            createDefinitionsRules(components, component, "Component", basicDefinition);

            //TODO make this is temp
            condition.Rule = "if" + openBrac + name + closeBrac + openBrace + optionalActions + closeBrace;

            action.Rule = "Action" + identifier + basicDefinition;
            actions.Rule = action + optionalActions | condition + optionalActions;
            optionalActions.Rule = actions | Empty;

            evtDefinition.Rule = openBrace + properties + actions + closeBrace;
            createDefinitionsRules(evts, evt, "Event", evtDefinition);

            entity.Rule = "Entity" + name + entityDefinition | "Entity" + entityDefinition;
            entities.Rule = entity + entities | entity;

            scenes.Rule = scene + scenes | scene;
            scene.Rule = "Scene" + name + openBrace + properties + managers + entities + closeBrace;

            uses.Rule = files + uses | Empty;
            files.Rule = "using" + anything + openBrace + definitions + closeBrace;
            //TODO this may need better logic?
            define.Rule = "define" + identifier + "as" + anything;
            definitions.Rule = define + definitions | define;

            game.Rule = uses + "Game" + name + openBrace  + properties + prototypes + scenes + closeBrace;
            base.Root = game;
        }
    }
}
