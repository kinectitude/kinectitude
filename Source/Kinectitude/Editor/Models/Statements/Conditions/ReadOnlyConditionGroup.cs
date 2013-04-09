//-----------------------------------------------------------------------
// <copyright file="ReadOnlyConditionGroup.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

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
            
            If = new ReadOnlyExpressionCondition(sourceGroup.If);

            sourceGroup.PropertyChanged += OnSourcePropertyChanged;
            if (null != sourceGroup.Else)
            {
                Else = new ReadOnlyBasicCondition(sourceGroup.Else);
            }
        }

        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Else")
            {
                if (null != sourceGroup.Else)
                {
                    Else = new ReadOnlyBasicCondition(sourceGroup.Else);
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
