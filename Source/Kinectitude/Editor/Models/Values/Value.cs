using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Values
{
    internal abstract class Value : GameModel<IScope>
    {
        //private bool hasValue;
        private object currentValue;

        public bool HasValue
        {
            get { return null != currentValue; }
            //set
            //{
            //    if (hasValue != value)
            //    {
            //        hasValue = value;
            //        NotifyPropertyChanged("HasValue");
            //    }
            //}
        }

        public object CurrentValue
        {
            get { return currentValue; }
            set
            {
                if (currentValue != value)
                {
                    currentValue = value;
                    NotifyPropertyChanged("CurrentValue");
                }
            }
        }

        protected Value() { }

        public override void Accept(Storage.IGameVisitor visitor)
        {
            
        }
    }
}
