using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
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

        public ICommand InsertBeforeCommand { get; private set; }

        public AbstractStatement(AbstractStatement inheritedStatement = null)
        {
            this.inheritedStatement = inheritedStatement;

            if (null != inheritedStatement)
            {
                inheritedStatement.PropertyChanged += OnInheritedPropertyChanged;
            }

            InsertBeforeCommand = new DelegateCommand(
                (parameter) => IsEditable,
                (parameter) =>
                {
                    if (IsEditable)
                    {
                        var toInsert = parameter as AbstractStatement;
                        if (null != toInsert)
                        {
                            if (null != Scope)
                            {
                                Scope.InsertBefore(this, toInsert);
                            }
                        }
                    }
                });
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
