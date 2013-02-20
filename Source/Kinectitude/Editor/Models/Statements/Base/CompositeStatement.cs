using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Statements.Actions;
using Kinectitude.Editor.Models.Statements.Assignments;
using Kinectitude.Editor.Models.Statements.Conditions;
using Kinectitude.Editor.Models.Statements.Loops;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Kinectitude.Editor.Models.Statements.Base
{
    internal abstract class CompositeStatement : AbstractStatement, IStatementScope
    {
        private readonly CompositeStatement sourceStatement;

        public override IEnumerable<Plugin> Plugins
        {
            get { return Statements.SelectMany(x => x.Plugins).Distinct(); }
        }

        public ObservableCollection<AbstractStatement> Statements { get; private set; }

        public virtual IEnumerable<Type> AllowableStatements
        {
            get
            {
                return new[]
                {
                    typeof(AbstractAction),
                    typeof(AbstractAssignment),
                    typeof(AbstractConditionGroup),
                    typeof(AbstractForLoop),
                    typeof(AbstractWhileLoop)
                };
            }
        }

        public ICommand AddActionCommand { get; private set; }

        protected CompositeStatement(CompositeStatement sourceStatement = null) : base(sourceStatement)
        {
            this.sourceStatement = sourceStatement;

            Statements = new ObservableCollection<AbstractStatement>();

            if (null != sourceStatement)
            {
                sourceStatement.Statements.CollectionChanged += OnSourceStatementsChanged;

                foreach (AbstractStatement statement in sourceStatement.Statements)
                {
                    FollowStatement(statement);
                }
            }

            AddActionCommand = new DelegateCommand((parameter) => IsEditable,
            (parameter) =>
            {
                if (IsEditable)
                {
                    AbstractStatement statement = parameter as AbstractStatement;
                    if (null == statement)
                    {
                        StatementFactory factory = parameter as StatementFactory;
                        if (null != factory)
                        {
                            statement = factory.CreateStatement();
                        }
                    }

                    if (null != statement)
                    {
                        if (IsAllowed(statement))
                        {
                            int oldIdx = -1;
                            var oldParent = statement.Scope;
                            if (null != oldParent)
                            {
                                oldIdx = oldParent.IndexOf(statement);
                            }

                            AddStatement(statement);

                            Workspace.Instance.CommandHistory.Log(
                                "insert/move statement",
                                () => AddStatement(statement),
                                () =>
                                {
                                    statement.RemoveFromParent();
                                    if (null != oldParent)
                                    {
                                        oldParent.InsertAt(oldIdx, statement);
                                    }
                                }
                            );
                        }
                    }
                }
            });
        }

        public void AddStatement(AbstractStatement statement)
        {
            PrivateAddStatement(Statements.Count, statement);
        }

        private void PrivateAddStatement(int idx, AbstractStatement statement)
        {
            if (IsAllowed(statement))
            {
                statement.RemoveFromParent();
                statement.Scope = this;
                Statements.Insert(idx, statement);

                foreach (Plugin plugin in statement.Plugins)
                {
                    Notify(new PluginUsed(plugin));
                }
            }
        }

        private bool IsAllowed(AbstractStatement statement)
        {
            foreach (var type in AllowableStatements)
            {
                if (type.IsAssignableFrom(statement.GetType()))
                {
                    return true;
                }
            }

            return false;
        }

        private void PrivateRemoveStatement(AbstractStatement statement)
        {
            statement.Scope = null;
            Statements.Remove(statement);
        }

        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        private void OnSourceStatementsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (AbstractStatement sourceStatement in e.NewItems)
                {
                    FollowStatement(sourceStatement);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (AbstractStatement sourceStatement in e.OldItems)
                {
                    UnfollowStatement(sourceStatement);
                }
            }

            // TODO: Handle rearrange/move
        }

        private AbstractStatement FindReadOnly(AbstractStatement sourceStatement)
        {
            return Statements.FirstOrDefault(x => x.InheritsFrom(sourceStatement));
        }

        private void FollowStatement(AbstractStatement sourceStatement)
        {
            AbstractStatement readonlyStatement = FindReadOnly(sourceStatement);
            if (null == readonlyStatement)
            {
                readonlyStatement = sourceStatement.CreateReadOnly();
                AddStatement(readonlyStatement);
            }
        }

        private void UnfollowStatement(AbstractStatement sourceStatement)
        {
            AbstractStatement readonlyStatement = FindReadOnly(sourceStatement);
            if (null != readonlyStatement)
            {
                PrivateRemoveStatement(readonlyStatement);
            }
        }

        #region IStatementScope implementation

        public int IndexOf(AbstractStatement statement)
        {
            return Statements.IndexOf(statement);
        }

        public void RemoveStatement(AbstractStatement statement)
        {
            if (statement.IsEditable)
            {
                PrivateRemoveStatement(statement);
            }
        }

        public void InsertAt(int idx, AbstractStatement statement)
        {
            if (IsEditable)
            {
                if (idx != -1)
                {
                    if (IsAllowed(statement))
                    {
                        statement.RemoveFromParent();
                        PrivateAddStatement(idx, statement);
                    }
                }
            }
        }

        public void InsertBefore(AbstractStatement statement, AbstractStatement toInsert)
        {
            if (IsEditable)
            {
                InsertAt(Statements.IndexOf(statement), toInsert);
            }
        }

        public string GetDefinedName(Plugin plugin)
        {
            return null != Scope ? Scope.GetDefinedName(plugin) : plugin.ClassName;
        }

        public Plugin GetPlugin(string name)
        {
            return null != Scope ? Scope.GetPlugin(name) : Workspace.Instance.GetPlugin(name);
        }

        #endregion
    }
}
