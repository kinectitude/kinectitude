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
        public TestGrammar()
        {
            /*ConstantTerminal Constant = new ConstantTerminal("Constant", typeof(LiteralValueNode));
            Constant.Add("pi", Math.PI);
            var Module = new NonTerminal("Module");
            Module.Rule = ToTerm("lol") | Constant | Module + ToTerm("lol") | Module + Constant;
            base.Root = Module;*/
            NonTerminal game = new NonTerminal("Game");
            NonTerminal scene = new NonTerminal("Scene");
            NonTerminal entity = new NonTerminal("Entity");
            NonTerminal prototype = new NonTerminal("Prototype");
            NonTerminal manager = new NonTerminal("Manager");
            NonTerminal component = new NonTerminal("Component");
            NonTerminal action = new NonTerminal("Action");
            

            base.Root = game;
        }
    }
}
