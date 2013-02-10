﻿using Kinectitude.Core.Attributes;
using Kinectitude.Core.Components;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Commands;
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
    internal sealed class Workspace : BaseModel
    {
        public static IValueMaker ValueMaker = new KglValueMaker();

        private const string PluginDirectory = "Plugins";

        private static readonly Lazy<Workspace> instance = new Lazy<Workspace>();

        public static Workspace Instance
        {
            get { return instance.Value; }
        }

        private readonly Lazy<CommandHistory> commandHistory;
        private readonly List<Entity> entityPresets;
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

        public ICommandHistory CommandHistory
        {
            get { return commandHistory.Value; }
        }

        public IEnumerable<Entity> EntityPresets
        {
            get { return entityPresets; }
        }

        public ObservableCollection<Plugin> Plugins { get; private set; }
        public ObservableCollection<Plugin> Managers { get; private set; }
        public ObservableCollection<Plugin> Components { get; private set; }
        public ObservableCollection<StatementFactory> Events { get; private set; }
        public ObservableCollection<StatementFactory> Actions { get; private set; }
        public ObservableCollection<StatementFactory> Statements { get; private set; }
        
        public ICommand NewProjectCommand { get; private set; }
        public ICommand LoadProjectCommand { get; private set; }
        public ICommand SaveProjectCommand { get; private set; }
        public ICommand SaveProjectAsCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }

        public Workspace()
        {
            Plugins = new ObservableCollection<Plugin>();

            Managers = new FilteredObservableCollection<Plugin>(Plugins, (plugin) => plugin.Type == PluginType.Manager);
            //Events = new FilteredObservableCollection<Plugin>(Plugins, (plugin) => plugin.Type == PluginType.Event);
            Components = new FilteredObservableCollection<Plugin>(Plugins, (plugin) => plugin.Type == PluginType.Component);

            Events = new ObservableCollection<StatementFactory>();
            Actions = new ObservableCollection<StatementFactory>();
            Statements = new ObservableCollection<StatementFactory>();

            commandHistory = new Lazy<CommandHistory>();

            NewProjectCommand = new DelegateCommand(null, (parameter) =>
            {
                Game game = new Game("Untitled Game") { Width = 800, Height = 600 };
                game.AddScene(new Scene("Scene 1"));

                //Project project = new Project() { GameRoot = "Data/" };
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
            });

            LoadProjectCommand = new DelegateCommand(null, (parameter) =>
            {
                DialogService.ShowLoadDialog((result, fileName) =>
                {
                    if (result == true)
                    {
                        LoadProject(fileName);
                    }
                });
            });

            SaveProjectCommand = new DelegateCommand(null, (parameter) =>
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

            SaveProjectAsCommand = new DelegateCommand(null, (parameter) =>
            {
                DialogService.ShowSaveDialog(
                    (result, fileName) =>
                    {
                        if (result == true)
                        {
                            Project.Title = fileName;
                            SaveProject();
                        }
                    }
                );
            });

            ExitCommand = new DelegateCommand(null, (parameter) => Exit());

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

            entityPresets = new List<Entity>();
        }

        public void Initialize()
        {
            Entity blankEntity = new Entity() { Name = "Blank Entity" };
            Component blankEntityTransform = new Component(GetPlugin(typeof(TransformComponent)));
            blankEntityTransform.SetProperty("Width", new Value(48, true));
            blankEntityTransform.SetProperty("Height", new Value(48, true));
            blankEntity.AddComponent(blankEntityTransform);

            entityPresets.Add(blankEntity);

            Entity imageEntity = new Entity() { Name = "Image Entity" };
            imageEntity.AddComponent(new Component(GetPlugin(typeof(ImageRenderComponent))));

            entityPresets.Add(imageEntity);

            Entity shapeEntity = new Entity() { Name = "Shape Entity" };
            Component shapeEntityTransform = new Component(GetPlugin(typeof(TransformComponent)));
            shapeEntityTransform.SetProperty("Width", new Value(48, true));
            shapeEntityTransform.SetProperty("Height", new Value(48, true));
            shapeEntity.AddComponent(blankEntityTransform);
            Component shapeEntityRender = new Component(GetPlugin(typeof(RenderComponent)));
            shapeEntityRender.SetProperty("Shape", new Value("Rectangle", true));
            shapeEntityRender.SetProperty("FillColor", new Value("Blue", true));
            shapeEntity.AddComponent(shapeEntityRender);

            entityPresets.Add(shapeEntity);

            Entity textEntity = new Entity() { Name = "Text Entity" };
            Component textEntityText = new Component(GetPlugin(typeof(TextRenderComponent)));
            textEntityText.SetProperty("FontSize", new Value(36, true));
            textEntityText.SetProperty("FontColor", new Value("Black", true));
            textEntityText.SetProperty("Value", new Value("Your Text Here", true));
            textEntity.AddComponent(textEntityText);

            entityPresets.Add(textEntity);

            Statements.Add(new StatementFactory("If", StatementType.ConditionGroup, () => new ConditionGroup()));
            Statements.Add(new StatementFactory("While Loop", StatementType.WhileLoop, () => new WhileLoop()));
            Statements.Add(new StatementFactory("For Loop", StatementType.ForLoop, () => new ForLoop()));
            Statements.Add(new StatementFactory("Change a Value", StatementType.Assignment, () => new Assignment()));
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        public void LoadProject(string fileName)
        {
            Project = ProjectStorage.LoadProject(fileName);
        }

        public void SaveProject()
        {
            ProjectStorage.SaveProject(Project);
        }

        private void RegisterPlugins(Assembly assembly)
        {
            var types = from type in assembly.GetTypes()
                        where System.Attribute.IsDefined(type, typeof(PluginAttribute)) &&
                        (
                            typeof(Kinectitude.Core.Base.Component) != type && typeof(Kinectitude.Core.Base.Component).IsAssignableFrom(type) ||
                            typeof(Kinectitude.Core.Base.Event) != type && typeof(Kinectitude.Core.Base.Event).IsAssignableFrom(type) ||
                            typeof(Kinectitude.Core.Base.Action) != type && typeof(Kinectitude.Core.Base.Action).IsAssignableFrom(type) ||
                            typeof(Kinectitude.Core.Base.IManager) != type && typeof(Kinectitude.Core.Base.IManager).IsAssignableFrom(type)
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
                Actions.Add(new StatementFactory(plugin.ShortName, StatementType.Action, () => new Action(plugin)));
            }
            else if (plugin.Type == PluginType.Event)
            {
                Events.Add(new StatementFactory(plugin.ShortName, StatementType.Event, () => new Event(plugin)));
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
    }
}
