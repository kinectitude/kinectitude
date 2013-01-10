using System;

namespace Kinectitude.Editor.Models.Statements.Base
{
    internal enum StatementType
    {
        Event,
        Action,
        Assignment,
        Condition,
        WhileLoop,
        ForLoop
    }

    internal sealed class StatementFactory
    {
        private readonly string name;
        private readonly StatementType type;
        private readonly Func<AbstractStatement> function;
        
        public string Name
        {
            get { return name; }
        }

        public StatementType Type
        {
            get { return type; }
        }

        public StatementFactory(string name, StatementType type, Func<AbstractStatement> function)
        {
            this.name = name;
            this.type = type;
            this.function = function;
        }

        public AbstractStatement CreateStatement()
        {
            return function();
        }
    }
}
