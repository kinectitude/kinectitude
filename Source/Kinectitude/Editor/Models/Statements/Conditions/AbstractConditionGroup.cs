using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Statements.Base;
using Kinectitude.Editor.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Kinectitude.Editor.Models.Statements.Conditions
{
    internal abstract class AbstractConditionGroup : CompositeStatement
    {
        private readonly AbstractConditionGroup sourceGroup;
        private AbstractCondition ifCondition;
        private AbstractCondition elseCondition;

        public AbstractCondition If
        {
            get { return ifCondition; }
            protected set
            {
                if (ifCondition != value)
                {
                    if (null != ifCondition)
                    {
                        ifCondition.Scope = null;
                    }

                    ifCondition = value;

                    if (null != ifCondition)
                    {
                        ifCondition.Scope = this;
                    }

                    NotifyPropertyChanged("If");
                }
            }
        }

        public AbstractCondition Else
        {
            get { return elseCondition; }
            protected set
            {
                if (elseCondition != value)
                {
                    if (null != elseCondition)
                    {
                        elseCondition.Scope = null;
                    }

                    elseCondition = value;

                    if (null != elseCondition)
                    {
                        elseCondition.Scope = this;
                    }

                    NotifyPropertyChanged("Else");
                }
            }
        }

        public override IEnumerable<Type> AllowableStatements
        {
            get { return new[] { typeof(AbstractCondition) }; }
        }

        public ICommand SetElseCommand { get; private set; }

        protected AbstractConditionGroup(AbstractConditionGroup sourceGroup = null) : base(sourceGroup)
        {
            this.sourceGroup = sourceGroup;

            SetElseCommand = new DelegateCommand((parameter) => IsEditable,
            (parameter) =>
            {
                if (IsEditable)
                {
                    AbstractCondition condition = parameter as AbstractCondition;
                    if (null == condition)
                    {
                        StatementFactory factory = parameter as StatementFactory;
                        if (null != factory)
                        {
                            condition = factory.CreateStatement() as AbstractCondition;
                        }
                    }

                    if (null != condition)
                    {
                        condition.RemoveFromParent();
                        Else = condition;
                    }
                }
            });
        }

        public sealed override AbstractStatement DeepCopyStatement()
        {
            ConditionGroup copy = new ConditionGroup() { If = this.If.DeepCopyCondition() };

            foreach (AbstractStatement statement in Statements)
            {
                copy.AddStatement(statement.DeepCopyStatement());
            }

            if (null != Else)
            {
                copy.Else = Else.DeepCopyCondition();
            }

            return copy;
        }

        public override AbstractStatement CreateReadOnly()
        {
            return new ReadOnlyConditionGroup(this);
        }
    }
}
