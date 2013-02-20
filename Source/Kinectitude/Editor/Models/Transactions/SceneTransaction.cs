using Kinectitude.Editor.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Kinectitude.Editor.Models.Transactions
{
    internal class SceneTransaction : BaseModel
    {
        private readonly Scene scene;
        private readonly string oldName;
        private readonly IEnumerable<Manager> oldManagers;
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
            this.scene = scene;

            oldName = scene.Name;
            oldManagers = scene.Managers.ToArray();

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
                Commit();

                Workspace.Instance.CommandHistory.Log(
                    "change scene properties",
                    () => Commit(),
                    () => Rollback()
                );
            });

            AddManagerCommand = new DelegateCommand(null, (parameter) =>
            {
                Plugin plugin = ManagerToAdd;

                if (null != plugin)
                {
                    AddManager(plugin);
                }
            });

            RemoveManagerCommand = new DelegateCommand(null, (parameter) =>
            {
                PluginSelection pluginSelection = SelectedManager;

                if (null != selectedManager)
                {
                    RemoveManager(selectedManager);
                }
            });
        }

        public void AddManager(Plugin plugin)
        {
            AvailableManagers.Remove(plugin);
            SelectedManagers.Add(new PluginSelection(plugin, false));
        }

        public void RemoveManager(PluginSelection selection)
        {
            SelectedManagers.Remove(selection);
            AvailableManagers.Add(selection.Plugin);
        }

        public void Commit()
        {
            Workspace.Instance.CommandHistory.WithoutLogging(() => scene.Name = Name);

            foreach (Manager manager in oldManagers)
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
        }

        public void Rollback()
        {
            Workspace.Instance.CommandHistory.WithoutLogging(() => scene.Name = oldName);
            scene.ClearManagers();

            foreach (var manager in oldManagers)
            {
                scene.AddManager(manager);
            }
        }
    }
}
