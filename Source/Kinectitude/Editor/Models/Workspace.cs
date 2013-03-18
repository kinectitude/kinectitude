using Kinectitude.Core.Attributes;
using Kinectitude.Core.Components;
using Kinectitude.Core.Data;
using Kinectitude.Core.Functions;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Commands;
using Kinectitude.Editor.Models.Exceptions;
using Kinectitude.Editor.Models.Statements.Assignments;
using Kinectitude.Editor.Models.Statements.Base;
using Kinectitude.Editor.Models.Statements.Conditions;
using Kinectitude.Editor.Models.Statements.Events;
using Kinectitude.Editor.Models.Statements.Loops;
using Kinectitude.Editor.Models.Values;
using Kinectitude.Editor.Storage;
using Kinectitude.Editor.Storage.Kgl;
using Kinectitude.Editor.Views.Main;
using Kinectitude.Editor.Views.Utils;
using Kinectitude.Render;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Action = Kinectitude.Editor.Models.Statements.Actions.Action;

namespace Kinectitude.Editor.Models
{
    internal delegate Entity EntityFactory();

    internal sealed class Workspace : BaseModel
    {
        private static class Messages
        {
            public const string RevertProject = "Revert Project";
            public const string LoadProject = "Load Project";
            public const string NewProject = "New Project";
            public const string Exit = "Exit Application";
            public const string RevertMessage = "This will revert your project to its last saved state and cannot be undone. Are you sure you want to continue?";
            public const string UnsavedMessage = "Do you want to save changes to your project?";
            public const string FailedToLoad = "Failed to Load";
        }

        public static IValueMaker ValueMaker = new KglValueMaker();

        private const string PluginDirectory = "Plugins";

        private static readonly Lazy<Workspace> instance = new Lazy<Workspace>(() => new Workspace());

        public static Workspace Instance
        {
            get { return instance.Value; }
        }

        private object clippedItem;
        private Project project;

        public Project Project
        {
            get { return project; }
            set
            {
                if (project != value)
                {
                    project = value;
                    CommandHistory.Clear();

                    NotifyPropertyChanged("Project");
                }
            }
        }

        public object ClippedItem
        {
            get { return clippedItem; }
            set
            {
                if (clippedItem != value)
                {
                    clippedItem = value;
                    NotifyPropertyChanged("ClippedItem");
                }
            }
        }

        public ICommandHistory CommandHistory { get; set; }
        public IDialogService DialogService { get; set; }

        public EntityFactory ImageEntityFactory { get; private set; }
        public EntityFactory TextEntityFactory { get; private set; }
        public EntityFactory ShapeEntityFactory { get; private set; }
        public EntityFactory BlankEntityFactory { get; private set; }
        
        public ObservableCollection<Plugin> Plugins { get; private set; }
        public ObservableCollection<Plugin> Services { get; private set; }
        public ObservableCollection<Plugin> Managers { get; private set; }
        public ObservableCollection<Plugin> Components { get; private set; }
        public ObservableCollection<StatementFactory> Events { get; private set; }
        public ObservableCollection<StatementFactory> Actions { get; private set; }
        public ObservableCollection<StatementFactory> Statements { get; private set; }
        
        public ICommand NewProjectCommand { get; private set; }
        public ICommand LoadProjectCommand { get; private set; }
        public ICommand SaveProjectCommand { get; private set; }
        public ICommand RevertProjectCommand { get; private set; }

        private Workspace()
        {
            Plugins = new ObservableCollection<Plugin>();

            Services = new FilteredObservableCollection<Plugin>(Plugins, p => p.Type == PluginType.Service);
            Managers = new FilteredObservableCollection<Plugin>(Plugins, p => p.Type == PluginType.Manager);
            Components = new FilteredObservableCollection<Plugin>(Plugins, p => p.Type == PluginType.Component);

            Events = new ObservableCollection<StatementFactory>();
            Actions = new ObservableCollection<StatementFactory>();
            Statements = new ObservableCollection<StatementFactory>();

            NewProjectCommand = new DelegateCommand(null, p =>
            {
                var create = true;

                if (null != Project && CommandHistory.HasUnsavedChanges)
                {
                    DialogService.Warn(Messages.NewProject, Messages.UnsavedMessage, MessageBoxButton.YesNoCancel, r =>
                    {
                        if (r == MessageBoxResult.Yes)
                        {
                            SaveProject();
                        }
                        else if (r == MessageBoxResult.Cancel)
                        {
                            create = false;
                        }
                    });
                }

                if (create)
                {
                    Game game = new Game("Untitled Game");
                    KglGameStorage.AddDefaultUsings(game);

                    var service = new Service(GetPlugin(typeof(RenderService)));
                    service.SetProperty("Width", new Value(800, true));
                    service.SetProperty("Height", new Value(600, true));
                    game.AddService(service);

                    var scene = new Scene("Scene1");
                    
                    var manager = new Manager(GetPlugin(typeof(RenderManager)));
                    scene.AddManager(manager);

                    game.AddScene(scene);
                    game.FirstScene = scene;

                    Project project = new Project();
                    project.Game = game;

                    DialogService.ShowDialog<ProjectDialog>(project, (result) =>
                    {
                        if (result == true)
                        {
                            ProjectStorage.CreateProject(project);
                            Project = project;
                        }
                    });
                }
            });

            LoadProjectCommand = new DelegateCommand(null, p =>
            {
                var load = true;

                if (null != Project && CommandHistory.HasUnsavedChanges)
                {
                    DialogService.Warn(Messages.LoadProject, Messages.UnsavedMessage, MessageBoxButton.YesNoCancel, r =>
                    {
                        if (r == MessageBoxResult.Yes)
                        {
                            SaveProject();
                        }
                        else if (r == MessageBoxResult.Cancel)
                        {
                            load = false;
                        }
                    });
                }

                if (load)
                {
                    DialogService.ShowLoadDialog((result, fileName) =>
                    {
                        if (result == true)
                        {
                            try
                            {
                                LoadProject(fileName);
                            }
                            catch (EditorException e)
                            {
                                DialogService.Warn(Messages.FailedToLoad, e.Message, MessageBoxButton.OK);
                            }
                        }
                    });
                }
            });

            SaveProjectCommand = new DelegateCommand(p => null != project, p =>
            {
                if (null == Project.Title)
                {
                    DialogService.ShowSaveDialog(
                        (result, fileName) =>
                        {
                            if (result == true)
                            {
                                Project.Title = fileName;
                            }
                        }
                    );
                }

                if (null != Project.Title)
                {
                    SaveProject();
                }
            });

            RevertProjectCommand = new DelegateCommand(p => null != Project, p => RevertProject());

            Assembly core = typeof(Kinectitude.Core.Base.Component).Assembly;
            RegisterPlugins(core);

            DirectoryInfo path = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, PluginDirectory));
            if (path.Exists)
            {
                FileInfo[] files = path.GetFiles("*.dll");
                foreach (FileInfo file in files)
                {
                    Assembly asm = Assembly.LoadFrom(file.FullName);
                    RegisterPlugins(asm);
                }
            }

            CommandHistory = new CommandHistory();
            DialogService = new DialogService();
        }

        public void Initialize()
        {
            BlankEntityFactory = () =>
            {
                var entity = new Entity();
                var transform = new Component(GetPlugin(typeof(TransformComponent)));
                transform.SetProperty("Width", new Value(48, true));
                transform.SetProperty("Height", new Value(48, true));
                entity.AddComponent(transform);
                return entity;
            };

            ImageEntityFactory = () =>
            {
                var entity = new Entity();
                entity.AddComponent(new Component(GetPlugin(typeof(ImageRenderComponent))));
                return entity;
            };

            ShapeEntityFactory = () =>
            {
                var entity = new Entity();
                var transform = new Component(GetPlugin(typeof(TransformComponent)));
                transform.SetProperty("Width", new Value(48, true));
                transform.SetProperty("Height", new Value(48, true));
                entity.AddComponent(transform);
                var render = new Component(GetPlugin(typeof(RenderComponent)));
                render.SetProperty("Shape", new Value("Rectangle", true));
                render.SetProperty("FillColor", new Value("Blue", true));
                entity.AddComponent(render);
                return entity;
            };

            TextEntityFactory = () =>
            {
                var entity = new Entity();
                var text = new Component(GetPlugin(typeof(TextRenderComponent)));
                text.SetProperty("FontSize", new Value(36, true));
                text.SetProperty("FontColor", new Value("Black", true));
                text.SetProperty("Value", new Value("Your Text Here", true));
                entity.AddComponent(text);
                return entity;
            };

            Statements.Add(new StatementFactory("If", StatementType.ConditionGroup, () => new ConditionGroup()));
            Statements.Add(new StatementFactory("While Loop", StatementType.WhileLoop, () => new WhileLoop()));
            Statements.Add(new StatementFactory("For Loop", StatementType.ForLoop, () => new ForLoop()));
            Statements.Add(new StatementFactory("Change a Value", StatementType.Assignment, () => new Assignment()));

            foreach (MethodInfo mi in typeof(Kinectitude.Core.Functions.Math).GetMethods().Where(mi => System.Attribute.IsDefined(mi, typeof(PluginAttribute))))
            {
                FunctionHolder.AddFunction(mi.Name, mi);
            }

            foreach (MethodInfo mi in typeof(Conversions).GetMethods().Where(mi => System.Attribute.IsDefined(mi, typeof(PluginAttribute))))
            {
                FunctionHolder.AddFunction(mi.Name, mi);
            }
        }

        public void RevertProject()
        {
            if (null != Project)
            {
                DialogService.Warn( Messages.RevertProject, Messages.RevertMessage, MessageBoxButton.YesNo, r =>
                {
                    if (r == MessageBoxResult.Yes)
                    {
                        LoadProject(Project.FileName);
                    }
                });
            }
        }

        public void LoadProject(string fileName)
        {
            CommandHistory.WithoutLogging(() => Project = ProjectStorage.LoadProject(fileName));
        }

        public void SaveProject()
        {
            ProjectStorage.SaveProject(Project);
            CommandHistory.Save();
        }

        private void RegisterPlugins(Assembly assembly)
        {
            var types = from type in assembly.GetTypes()
                        where System.Attribute.IsDefined(type, typeof(PluginAttribute)) &&
                        (
                            typeof(Kinectitude.Core.Base.Component) != type && typeof(Kinectitude.Core.Base.Component).IsAssignableFrom(type) ||
                            typeof(Kinectitude.Core.Base.Event) != type && typeof(Kinectitude.Core.Base.Event).IsAssignableFrom(type) ||
                            typeof(Kinectitude.Core.Base.Action) != type && typeof(Kinectitude.Core.Base.Action).IsAssignableFrom(type) ||
                            typeof(Kinectitude.Core.Base.IManager) != type && typeof(Kinectitude.Core.Base.IManager).IsAssignableFrom(type) ||
                            typeof(Kinectitude.Core.Base.Service) != type && typeof(Kinectitude.Core.Base.Service).IsAssignableFrom(type)
                        )
                        select new Plugin(type);

            foreach (Plugin plugin in types)
            {
                AddPlugin(plugin);
            }
        }

        public void AddPlugin(Plugin plugin)
        {
            Plugins.Add(plugin);

            if (plugin.Type == PluginType.Action)
            {
                Actions.Add(new StatementFactory(plugin.Description, StatementType.Action, () => new Action(plugin)));
            }
            else if (plugin.Type == PluginType.Event)
            {
                Events.Add(new StatementFactory(plugin.Description, StatementType.Event, () => new Event(plugin)));
            }
        }

        public void RemovePlugin(Plugin plugin)
        {
            Plugins.Remove(plugin);
        }

        public Plugin GetPlugin(string name)
        {
            return Plugins.FirstOrDefault(x => x.ClassName == name);
        }

        public Plugin GetPlugin(Type type)
        {
            return GetPlugin(type.FullName);
        }

        public void WarnOnClose(MessageBoxCallback onClose)
        {
            if (null != Project && CommandHistory.HasUnsavedChanges)
            {
                DialogService.Warn(Messages.Exit, Messages.UnsavedMessage, MessageBoxButton.YesNoCancel, r =>
                {
                    if (r == MessageBoxResult.Yes)
                    {
                        SaveProject();
                    }

                    if (null != onClose)
                    {
                        onClose(r);
                    }
                });
            }
        }
    }
}
