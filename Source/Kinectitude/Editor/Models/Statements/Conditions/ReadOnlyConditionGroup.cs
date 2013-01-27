using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Statements.Conditions
{
    internal sealed class ReadOnlyConditionGroup : AbstractConditionGroup
    {
        private readonly AbstractConditionGroup sourceGroup;
        
        public ReadOnlyConditionGroup(AbstractConditionGroup sourceGroup) : base(sourceGroup)
        {
            this.sourceGroup = sourceGroup;
            
            If = new ReadOnlyCondition(sourceGroup.If);

            sourceGroup.PropertyChanged += OnSourcePropertyChanged;
            if (null != sourceGroup.Else)
            {
                Else = new ReadOnlyCondition(sourceGroup.Else);
            }
        }

        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Else")
            {
                if (null != sourceGroup.Else)
                {
                    Else = new ReadOnlyCondition(sourceGroup.Else);
                }
                else
                {
                    Else = null;
                }
            }
        }

        public override void Accept(Storage.IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
