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
        private AbstractExpressionCondition ifCondition;
        private AbstractBasicCondition elseCondition;

        public AbstractExpressionCondition If
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

        public AbstractBasicCondition Else
        {
            get { return elseCondition; }
            set
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
            get { return new[] { typeof(AbstractExpressionCondition) }; }
        }

        public ICommand AddElseIfCommand { get; private set; }
        public ICommand AddElseCommand { get; private set; }

        protected AbstractConditionGroup(AbstractConditionGroup sourceGroup = null) : base(sourceGroup)
        {
            this.sourceGroup = sourceGroup;

            AddElseIfCommand = new DelegateCommand((parameter) => IsEditable,
            (parameter) =>
            {
                if (IsEditable)
                {
                    AddStatement(new ExpressionCondition());
                }
            });

            AddElseCommand = new DelegateCommand((parameter) => IsEditable && null == Else,
            (parameter) =>
            {
                if (IsEditable && null == Else)
                {
                    Else = new BasicCondition();
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
