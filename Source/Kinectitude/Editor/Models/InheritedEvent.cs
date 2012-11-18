﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Kinectitude.Editor.Models.Interfaces;
using System.Text.RegularExpressions;
using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models
{
    internal sealed class InheritedEvent : AbstractEvent
    {
        private readonly AbstractEvent inheritedEvent;

        public override event DefineAddedEventHandler DefineAdded
        {
            add { inheritedEvent.DefineAdded += value; }
            remove { inheritedEvent.DefineAdded -= value; }
        }

        public override event DefinedNameChangedEventHandler DefineChanged
        {
            add { inheritedEvent.DefineChanged += value; }
            remove { inheritedEvent.DefineChanged -= value; }
        }

        public override Plugin Plugin
        {
            get { return inheritedEvent.Plugin; }
        }

        public override string Type
        {
            get { return inheritedEvent.Type; }
        }

        public override string Header
        {
            get { return inheritedEvent.Header; }
        }

        public IEnumerable<object> Tokens { get; private set; }

        public override string Description
        {
            get { return inheritedEvent.Description; }
        }

        public override bool IsLocal
        {
            get { return false; }
        }

        public override bool IsInherited
        {
            get { return true; }
        }

        public override IEnumerable<Plugin> Plugins
        {
            get { return inheritedEvent.Plugins; }
        }

        public InheritedEvent(AbstractEvent inheritedEvent)
        {
            this.inheritedEvent = inheritedEvent;

            inheritedEvent.Actions.CollectionChanged += OnInheritedEventActionsChanged;
            foreach (AbstractAction inheritedAction in inheritedEvent.Actions)
            {
                InheritAction(inheritedAction);
            }
            
            inheritedEvent.PropertyChanged += (sender, args) => NotifyPropertyChanged(args.PropertyName);

            foreach (AbstractProperty inheritedProperty in inheritedEvent.Properties)
            {
                InheritedProperty localProperty = new InheritedProperty(inheritedProperty);
                AddProperty(localProperty);
            }

            string[] splitHeader = Regex.Split(inheritedEvent.Header, "({.*?})");
            List<object> tokens = new List<object>();

            foreach (string token in splitHeader)
            {
                if (token.StartsWith("{"))
                {
                    string property = token.TrimStart('{').TrimEnd('}');
                    tokens.Add(GetProperty(property));
                }
                else if (token != string.Empty)
                {
                    tokens.Add(token);
                }
            }

            Tokens = tokens;
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        private void OnInheritedEventActionsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (AbstractAction action in args.NewItems)
                {
                    InheritAction(action);
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (AbstractAction action in args.OldItems)
                {
                    DisinheritAction(action);
                }
            }
        }

        private void InheritAction(AbstractAction inheritedAction)
        {
            AbstractAction localAction = Actions.FirstOrDefault(x => x.InheritsFrom(inheritedAction));
            if (null == localAction)
            {
                AbstractCondition inheritedCondition = inheritedAction as AbstractCondition;
                if (null != inheritedCondition)
                {
                    localAction = new InheritedCondition(inheritedCondition);
                }
                else
                {
                    localAction = new InheritedAction(inheritedAction);
                }
                
                AddAction(localAction);
            }
        }

        private void DisinheritAction(AbstractAction inheritedAction)
        {
            AbstractAction localAction = Actions.FirstOrDefault(x => x.InheritsFrom(inheritedAction));
            if (null != localAction)
            {
                PrivateRemoveAction(localAction);
            }
        }

        public override bool InheritsFrom(AbstractEvent evt)
        {
            return evt == inheritedEvent;
        }
    }
}
