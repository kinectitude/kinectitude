using Kinectitude.Editor.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Kinectitude.Editor.Models
{
    internal class SceneTransaction : BaseModel
    {
        private string name;
        private Plugin managerToAdd;
        private PluginSelection selectedManager;

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

        public string Title
        {
            get { return "Scene Properties"; }
        }

        public Plugin ManagerToAdd
        {
            get { return managerToAdd; }
            set
            {
                if (managerToAdd != value)
                {
                    managerToAdd = value;
                    NotifyPropertyChanged("ManagerToAdd");
                }
            }
        }

        public PluginSelection SelectedManager
        {
            get { return selectedManager; }
            set
            {
                if (selectedManager != value)
                {
                    selectedManager = value;
                    NotifyPropertyChanged("SelectedManager");
                }
            }
        }

        public ObservableCollection<Plugin> AvailableManagers { get; private set; }
        public ObservableCollection<PluginSelection> RequiredManagers { get; private set; }
        public ObservableCollection<PluginSelection> SelectedManagers { get; private set; }

        public ICommand CommitCommand { get; private set; }
        public ICommand AddManagerCommand { get; private set; }
        public ICommand RemoveManagerCommand { get; private set; }

        public SceneTransaction(Scene scene)
        {
            Name = scene.Name;

            RequiredManagers = new ObservableCollection<PluginSelection>();
            SelectedManagers = new ObservableCollection<PluginSelection>();

            foreach (Manager manager in scene.Managers)
            {
                if (manager.IsRequired)
                {
                    RequiredManagers.Add(new PluginSelection(manager.Plugin, true));
                }
                else
                {
                    SelectedManagers.Add(new PluginSelection(manager.Plugin, false));
                }
            }

            AvailableManagers = new ObservableCollection<Plugin>();

            foreach (Plugin plugin in Workspace.Instance.Managers)
            {
                if (!RequiredManagers.Any(x => x.Plugin == plugin) && !SelectedManagers.Any(x => x.Plugin == plugin))
                {
                    AvailableManagers.Add(plugin);
                }
            }

            CommitCommand = new DelegateCommand(null, (parameter) =>
            {
                scene.Name = Name;

                IEnumerable<Manager> existingManagers = scene.Managers.ToArray();

                foreach (Manager manager in existingManagers)
                {
                    if (!SelectedManagers.Any(x => x.Plugin == manager.Plugin) && !RequiredManagers.Any(x => x.Plugin == manager.Plugin))
                    {
                        scene.RemoveManager(manager);
                    }
                }

                foreach (PluginSelection selection in SelectedManagers)
                {
                    if (!scene.HasManagerOfType(selection.Plugin))
                    {
                        scene.AddManager(new Manager(selection.Plugin));
                    }
                }
            });

            AddManagerCommand = new DelegateCommand(null, (parameter) =>
            {
                Plugin plugin = ManagerToAdd;

                if (null != plugin)
                {
                    AvailableManagers.Remove(plugin);
                    SelectedManagers.Add(new PluginSelection(plugin, false));
                }
            });

            RemoveManagerCommand = new DelegateCommand(null, (parameter) =>
            {
                PluginSelection pluginSelection = SelectedManager;

                if (null != selectedManager)
                {
                    SelectedManagers.Remove(pluginSelection);
                    AvailableManagers.Add(pluginSelection.Plugin);
                }
            });
        }
    }
}
