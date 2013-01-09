﻿using Kinectitude.Editor.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace Kinectitude.Editor.Models.Transactions
{
    internal sealed class EntityTransaction : BaseModel
    {
        private string name;
        private Entity prototypeToAdd;
        private Entity selectedPrototype;
        private Plugin componentToAdd;
        private PluginSelection selectedComponent;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public Entity PrototypeToAdd
        {
            get { return prototypeToAdd; }
            set
            {
                if (prototypeToAdd != value)
                {
                    prototypeToAdd = value;
                    NotifyPropertyChanged("PrototypeToAdd");
                }
            }
        }

        public Entity SelectedPrototype
        {
            get { return selectedPrototype; }
            set
            {
                if (selectedPrototype != value)
                {
                    selectedPrototype = value;
                    NotifyPropertyChanged("SelectedPrototype");
                }
            }
        }

        public Plugin ComponentToAdd
        {
            get { return componentToAdd; }
            set
            {
                if (componentToAdd != value)
                {
                    componentToAdd = value;
                    NotifyPropertyChanged("ComponentToAdd");
                }
            }
        }

        public PluginSelection SelectedComponent
        {
            get { return selectedComponent; }
            set
            {
                if (selectedComponent != value)
                {
                    selectedComponent = value;
                    NotifyPropertyChanged("SelectedComponent");
                }
            }
        }

        public ObservableCollection<Entity> AvailablePrototypes { get; private set; }
        public ObservableCollection<Entity> SelectedPrototypes { get; private set; }
        public ObservableCollection<Plugin> AvailableComponents { get; private set; }
        public ObservableCollection<PluginSelection> InheritedComponents { get; private set; }
        public ObservableCollection<PluginSelection> SelectedComponents { get; private set; }

        public ICommand CommitCommand { get; private set; }
        public ICommand AddPrototypeCommand { get; private set; }
        public ICommand RemovePrototypeCommand { get; private set; }
        public ICommand MovePrototypeUpCommand { get; private set; }
        public ICommand MovePrototypeDownCommand { get; private set; }
        public ICommand AddComponentCommand { get; private set; }
        public ICommand RemoveComponentCommand { get; private set; }

        public EntityTransaction(IEnumerable<Entity> prototypes, Entity entity)
        {
            Name = entity.Name;

            // TODO: Disallow circular prototyping

            AvailablePrototypes = new ObservableCollection<Entity>();

            foreach (Entity prototype in prototypes)
            {
                if (prototype != entity && !entity.Prototypes.Contains(prototype))
                {
                    AvailablePrototypes.Add(prototype);
                }
            }

            SelectedPrototypes = new ObservableCollection<Entity>(entity.Prototypes);
            SelectedPrototypes.CollectionChanged += SelectedPrototypes_CollectionChanged;

            InheritedComponents = new ObservableCollection<PluginSelection>();
            SelectedComponents = new ObservableCollection<PluginSelection>();

            foreach (Component component in entity.Components)
            {
                if (component.IsRoot)
                {
                    SelectedComponents.Add(new PluginSelection(component.Plugin, false));
                }
                else
                {
                    InheritedComponents.Add(new PluginSelection(component.Plugin, true));
                }
            }

            AvailableComponents = new ObservableCollection<Plugin>();

            foreach (Plugin plugin in Workspace.Instance.Components)
            {
                if (!SelectedComponents.Any(x => x.Plugin == plugin) && !InheritedComponents.Any(x => x.Plugin == plugin))
                {
                    AvailableComponents.Add(plugin);
                }
            }

            CommitCommand = new DelegateCommand(null, (parameter) =>
            {
                entity.Name = Name;

                entity.ClearPrototypes();

                foreach (Entity prototype in SelectedPrototypes)
                {
                    entity.AddPrototype(prototype);
                }

                // remove all components that are in the entity and not the selected components. roots only.

                IEnumerable<Component> existingComponents = entity.Components.ToArray();

                foreach (Component existingComponent in existingComponents)
                {
                    if (!SelectedComponents.Any(x => x.Plugin == existingComponent.Plugin) && !InheritedComponents.Any(x => x.Plugin == existingComponent.Plugin))
                    {
                        entity.RemoveComponent(existingComponent);
                    }
                }

                // add all components that are in the selected components and not the entity

                foreach (PluginSelection selection in SelectedComponents)
                {
                    if (!entity.HasComponentOfType(selection.Plugin.ClassName))
                    {
                        entity.AddComponent(new Component(selection.Plugin));
                    }
                }
            });

            AddPrototypeCommand = new DelegateCommand(null, (parameter) =>
            {
                Entity prototype = PrototypeToAdd;

                if (null != prototype)
                {
                    AvailablePrototypes.Remove(prototype);
                    SelectedPrototypes.Add(prototype);
                }
            });

            RemovePrototypeCommand = new DelegateCommand(null, (parameter) =>
            {
                Entity prototype = SelectedPrototype;

                if (null != prototype)
                {
                    SelectedPrototypes.Remove(prototype);
                    AvailablePrototypes.Add(prototype);
                }
            });

            MovePrototypeUpCommand = new DelegateCommand(null, (parameter) =>
            {
                int idx = SelectedPrototypes.IndexOf(SelectedPrototype);

                if (idx > 0)
                {
                    SelectedPrototypes.Move(idx, idx - 1);
                }
            });

            MovePrototypeDownCommand = new DelegateCommand(null, (parameter) =>
            {
                int idx = SelectedPrototypes.IndexOf(SelectedPrototype);

                if (idx >= 0 && idx < SelectedPrototypes.Count - 1)
                {
                    SelectedPrototypes.Move(idx, idx + 1);
                }
            });

            AddComponentCommand = new DelegateCommand(null, (parameter) =>
            {
                Plugin plugin = ComponentToAdd;

                if (null != plugin)
                {
                    AvailableComponents.Remove(plugin);
                    SelectedComponents.Add(new PluginSelection(plugin, false));
                }
            });

            RemoveComponentCommand = new DelegateCommand(null, (parameter) =>
            {
                PluginSelection selection = SelectedComponent;

                if (null != selection)
                {
                    SelectedComponents.Remove(selection);
                    AvailableComponents.Add(selection.Plugin);
                }
            });
        }

        void SelectedPrototypes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IEnumerable<PluginSelection> previousComponents = InheritedComponents.ToArray();

            InheritedComponents.Clear();

            foreach (Entity prototype in SelectedPrototypes)
            {
                foreach (Component component in prototype.Components)
                {
                    if (!InheritedComponents.Any(x => x.Plugin == component.Plugin))
                    {
                        InheritedComponents.Add(new PluginSelection(component.Plugin, true));
                        AvailableComponents.Remove(component.Plugin);

                        PluginSelection selected = SelectedComponents.FirstOrDefault(x => x.Plugin == component.Plugin);
                        if (null != selected)
                        {
                            SelectedComponents.Remove(selected);
                        }
                    }
                }
            }

            foreach (PluginSelection selection in previousComponents)
            {
                if (!InheritedComponents.Contains(selection))
                {
                    AvailableComponents.Add(selection.Plugin);
                }
            }
        }
    }
}
