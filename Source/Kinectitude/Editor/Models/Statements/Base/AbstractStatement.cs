using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Statements.Actions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Kinectitude.Editor.Models.Statements.Base
{
    internal abstract class AbstractStatement : GameModel<IStatementScope>
    {
        private readonly AbstractStatement inheritedStatement;

        public virtual IEnumerable<Plugin> Plugins
        {
            get { return Enumerable.Empty<Plugin>(); }
        }
        
        public bool IsReadOnly
        {
            get { return null != inheritedStatement; }
        }

        public bool IsEditable
        {
            get { return !IsReadOnly; }
        }

        public int Index
        {
            get { return null != Scope ? Scope.IndexOf(this) : -1; }
        }

        public bool ShouldCopyStatement
        {
            get { return null != Scope ? Scope.ShouldCopyStatement : false; }
        }

        public ICommand InsertBeforeCommand { get; private set; }

        public AbstractStatement(AbstractStatement inheritedStatement = null)
        {
            this.inheritedStatement = inheritedStatement;

            if (null != inheritedStatement)
            {
                inheritedStatement.PropertyChanged += OnInheritedPropertyChanged;
            }

            InsertBeforeCommand = new DelegateCommand((parameter) => IsEditable,
            (parameter) =>
            {
                if (IsEditable)
                {
                    var toInsert = parameter as AbstractStatement;
                    if (null == toInsert)
                    {
                        var factory = parameter as StatementFactory;
                        if (null != factory)
                        {
                            toInsert = factory.CreateStatement();
                        }
                    }

                    if (null != toInsert && ShouldCopyStatement)
                    {
                        toInsert = toInsert.DeepCopyStatement();
                    }

                    if (null != toInsert && toInsert.IsEditable && null != Scope)
                    {
                        int oldIdx = -1;
                        var oldParent = toInsert.Scope;
                        if (null != oldParent)
                        {
                            oldIdx = oldParent.IndexOf(toInsert);
                        }

                        Scope.InsertBefore(this, toInsert);

                        Workspace.Instance.CommandHistory.Log(
                            "insert/move statement",
                            () => Scope.InsertBefore(this, toInsert),
                            () =>
                            {
                                toInsert.RemoveFromParent();
                                if (null != oldParent)
                                {
                                    oldParent.InsertAt(oldIdx, toInsert);
                                }
                            }
                        );
                    }
                }
            });

            AddDependency<ScopeChanged>("Index");
        }

        public bool InheritsFrom(AbstractStatement statement)
        {
            return statement == inheritedStatement;
        }

        private void OnInheritedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        public void RemoveFromParent()
        {
            if (null != Scope)
            {
                Scope.RemoveStatement(this);
            }
        }

        public abstract AbstractStatement DeepCopyStatement();
        public abstract AbstractStatement CreateReadOnly();
    }
}
