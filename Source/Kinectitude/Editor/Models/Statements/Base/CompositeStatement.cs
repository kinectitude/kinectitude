using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Kinectitude.Editor.Models.Statements
{
    internal abstract class CompositeStatement : AbstractStatement, IStatementScope
    {
        private readonly CompositeStatement inheritedStatement;

        public override IEnumerable<Plugin> Plugins
        {
            get { return Statements.SelectMany(x => x.Plugins).Distinct(); }
        }

        public ObservableCollection<AbstractStatement> Statements { get; private set; }

        public ICommand AddActionCommand { get; private set; }

        protected CompositeStatement(CompositeStatement inheritedStatement = null) : base(inheritedStatement)
        {
            this.inheritedStatement = inheritedStatement;

            Statements = new ObservableCollection<AbstractStatement>();

            if (null != inheritedStatement)
            {
                inheritedStatement.Statements.CollectionChanged += OnInheritedStatementsChanged;

                foreach (AbstractStatement statement in inheritedStatement.Statements)
                {
                    InheritStatement(statement);
                }
            }

            AddActionCommand = new DelegateCommand(
                (parameter) => IsLocal,
                (parameter) =>
                {
                    if (IsLocal)
                    {
                        StatementFactory factory = parameter as StatementFactory;
                        if (null != factory)
                        {
                            AddStatement(factory.CreateStatement());
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
            statement.Scope = this;
            Statements.Insert(idx, statement);

            foreach (Plugin plugin in statement.Plugins)
            {
                Notify(new PluginUsed(plugin));
            }
        }

        public void RemoveStatement(AbstractStatement statement)
        {
            if (statement.IsLocal)
            {
                PrivateRemoveStatement(statement);
            }
        }

        private void PrivateRemoveStatement(AbstractStatement statement)
        {
            statement.Scope = null;
            Statements.Remove(statement);
        }

        private void OnInheritedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        private void OnInheritedStatementsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (AbstractStatement inheritedStatement in e.NewItems)
                {
                    InheritStatement(inheritedStatement);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (AbstractStatement inheritedStatement in e.OldItems)
                {
                    DisinheritStatement(inheritedStatement);
                }
            }

            // TODO: Handle rearrange/move
        }

        private AbstractStatement FindInheritor(AbstractStatement inheritedStatement)
        {
            return Statements.FirstOrDefault(x => x.InheritsFrom(inheritedStatement));
        }

        private void InheritStatement(AbstractStatement inheritedStatement)
        {
            AbstractStatement inheritor = FindInheritor(inheritedStatement);
            if (null == inheritor)
            {
                inheritor = inheritedStatement.CreateInheritor();
                AddStatement(inheritor);
            }
        }

        private void DisinheritStatement(AbstractStatement inheritedStatement)
        {
            AbstractStatement inheritor = FindInheritor(inheritedStatement);
            if (null != inheritor)
            {
                PrivateRemoveStatement(inheritor);
            }
        }

        #region IStatementScope implementation

        public void InsertBefore(AbstractStatement statement, AbstractStatement toInsert)
        {
            if (IsLocal)
            {
                int idx = Statements.IndexOf(statement);
                if (idx != -1)
                {
                    if (Statements.Contains(toInsert))
                    {
                        RemoveStatement(toInsert);
                    }

                    PrivateAddStatement(idx, toInsert);
                }
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
