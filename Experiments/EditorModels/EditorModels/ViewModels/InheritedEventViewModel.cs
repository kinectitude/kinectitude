using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using EditorModels.ViewModels.Interfaces;

namespace EditorModels.ViewModels
{
    internal sealed class InheritedEventViewModel : AbstractEventViewModel
    {
        private readonly AbstractEventViewModel inheritedEvent;

        public override event PluginAddedEventHandler PluginAdded
        {
            add { inheritedEvent.PluginAdded += value; }
            remove { inheritedEvent.PluginAdded -= value; }
        }

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

        public override string Type
        {
            get { return inheritedEvent.Type; }
        }

        public override bool IsLocal
        {
            get { return false; }
        }

        public override bool IsInherited
        {
            get { return true; }
        }

        public override IEnumerable<PluginViewModel> Plugins
        {
            get { return inheritedEvent.Plugins; }
        }

        public InheritedEventViewModel(AbstractEventViewModel inheritedEvent)
        {
            this.inheritedEvent = inheritedEvent;
            inheritedEvent.Actions.CollectionChanged += OnInheritedEventActionsChanged;
            inheritedEvent.PropertyChanged += (sender, args) => NotifyPropertyChanged(args.PropertyName);

            foreach (AbstractPropertyViewModel inheritedProperty in inheritedEvent.Properties)
            {
                InheritedPropertyViewModel localProperty = new InheritedPropertyViewModel(inheritedProperty);
                AddProperty(localProperty);
            }
        }

        private void OnInheritedEventActionsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (AbstractActionViewModel action in args.OldItems)
                {

                }
            }
        }

        public override bool InheritsFrom(AbstractEventViewModel evt)
        {
            return evt == inheritedEvent;
        }
    }
}
