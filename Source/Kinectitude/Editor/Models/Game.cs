//-----------------------------------------------------------------------
// <copyright file="Game.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Data.DataContainers;
using Kinectitude.Editor.Models.Exceptions;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Transactions;
using Kinectitude.Editor.Models.Values;
using Kinectitude.Editor.Storage;
using Kinectitude.Editor.Views.Dialogs;
using Kinectitude.Editor.Views.Utils;
using Kinectitude.Render;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Kinectitude.Editor.Models
{
    internal sealed class Game : GameModel<IScope>, IAttributeScope, IEntityScope, ISceneScope, IServiceScope
    {
        private string name;
        private Scene firstScene;
        private int nextAttribute;
        private int nextScene;
        private int nextPrototype;
        private Attribute selectedAttribute;
        private Plugin serviceToAdd;
        private Service selectedService;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    string oldName = name;
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        value = null;
                    }

                    Workspace.Instance.CommandHistory.Log(
                        "rename game to '" + value + "'",
                        () => Name = value,
                        () => Name = oldName
                    );

                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        [DependsOn("Name")]
        public string DisplayName
        {
            get { return Name; }
        }

        public double Width
        {
            get
            {
                var service = GetServiceByType(typeof(RenderService));
                if (null != service)
                {
                    var property = service.GetProperty("Width");
                    if (null != property)
                    {
                        return property.Value.GetDoubleValue();
                    }
                }

                return ConstantReader.CacheOrCreate(800);
            }
        }

        public double Height
        {
            get
            {
                var service = GetServiceByType(typeof(RenderService));
                if (null != service)
                {
                    var property = service.GetProperty("Height");
                    if (null != property)
                    {
                        return property.Value.GetDoubleValue();
                    }
                }

                return ConstantReader.CacheOrCreate(600);
            }
        }

        public Scene FirstScene
        {
            get { return firstScene; }
            set
            {
                if (firstScene != value)
                {
                    Scene oldFirstScene = firstScene;

                    Workspace.Instance.CommandHistory.Log(
                        "set first scene to '" + value.Name + "'",
                        () => FirstScene = value,
                        () => FirstScene = oldFirstScene
                    );

                    firstScene = value;
                    NotifyPropertyChanged("FirstScene");
                }
            }
        }

        public Attribute SelectedAttribute
        {
            get { return selectedAttribute; }
            set
            {
                if (selectedAttribute != value)
                {
                    selectedAttribute = value;
                    NotifyPropertyChanged("SelectedAttribute");
                }
            }
        }

        public Plugin ServiceToAdd
        {
            get { return serviceToAdd; }
            set
            {
                if (serviceToAdd != value)
                {
                    serviceToAdd = value;
                    NotifyPropertyChanged("ServiceToAdd");
                }
            }
        }

        public Service SelectedService
        {
            get { return selectedService; }
            set
            {
                if (selectedService != value)
                {
                    selectedService = value;
                    NotifyPropertyChanged("SelectedService");
                }
            }
        }

        public ObservableCollection<Using> Usings { get; private set; }
        public ObservableCollection<Plugin> AvailableServices { get; private set; }
        public ObservableCollection<Service> Services { get; private set; }
        public ObservableCollection<Entity> Prototypes { get; private set; }
        public ObservableCollection<Scene> Scenes { get; private set; }
        public ObservableCollection<Attribute> Attributes { get; private set; }

        public ICommand RenameCommand { get; private set; }
        public ICommand AddPrototypeCommand { get; private set; }
        public ICommand AddAttributeCommand { get; private set; }
        public ICommand RemoveAttributeCommand { get; private set; }
        public ICommand AddSceneCommand { get; private set; }
        public ICommand RemoveItemCommand { get; private set; }
        public ICommand AddServiceCommand { get; private set; }
        public ICommand RemoveServiceCommand { get; private set; }

        public Game(string name)
        {
            this.name = name;
            
            Usings = new ObservableCollection<Using>();
            Services = new ObservableCollection<Service>();
            Prototypes = new ObservableCollection<Entity>();
            Scenes = new ObservableCollection<Scene>();
            Attributes = new ObservableCollection<Attribute>();

            AvailableServices = new ObservableCollection<Plugin>();
            foreach (var plugin in Workspace.Instance.Services)
            {
                AvailableServices.Add(plugin);
            }

            AddHandler<PluginUsed>(OnPluginUsed);

            RenameCommand = new DelegateCommand(null, (parameter) =>
            {
                Workspace.Instance.DialogService.ShowDialog<NameDialog>(this);
            });

            AddPrototypeCommand = new DelegateCommand(null, (parameter) =>
            {
                Entity prototype = CreatePrototype();
                EntityTransaction transaction = new EntityTransaction(Prototypes, prototype);
                    
                Workspace.Instance.DialogService.ShowDialog<EntityDialog>(transaction, (result) =>
                {
                    if (result == true)
                    {
                        AddPrototype(prototype);

                        Workspace.Instance.CommandHistory.Log(
                            "add prototype '" + prototype.Name + "'",
                            () => AddPrototype(prototype),
                            () => RemovePrototype(prototype)
                        );
                    }
                });
            });

            AddSceneCommand = new DelegateCommand(null, (parameter) =>
            {
                Scene scene = CreateScene();

                Workspace.Instance.DialogService.ShowDialog<NameDialog>(new SceneTransaction(scene), (result) =>
                {
                    if (result == true)
                    {
                        AddScene(scene);
                    }
                });
            });

            AddAttributeCommand = new DelegateCommand(null, (parameter) =>
            {
                var attribute = CreateAttribute();

                Workspace.Instance.CommandHistory.Log(
                    "add attribute '" + attribute.Name + "'",
                    () => AddAttribute(attribute),
                    () => RemoveAttribute(attribute)
                );
            });

            RemoveAttributeCommand = new DelegateCommand(null, (parameter) =>
            {
                if (null != selectedAttribute)
                {
                    var attribute = selectedAttribute;
                    RemoveAttribute(attribute);

                    Workspace.Instance.CommandHistory.Log(
                        "remove attribute '" + attribute.Name + "'",
                        () => RemoveAttribute(attribute),
                        () => AddAttribute(attribute)
                    );
                }
            });

            RemoveItemCommand = new DelegateCommand(null, (parameter) =>
            {
                Entity entity = parameter as Entity;
                if (null != entity)
                {
                    entity.RemoveFromScope();
                }
                else
                {
                    Scene scene = parameter as Scene;
                    if (null != scene)
                    {
                        RemoveScene(scene);
                    }
                }
            });

            AddServiceCommand = new DelegateCommand(null, p =>
            {
                if (null != ServiceToAdd)
                {
                    var service = new Service(ServiceToAdd);
                    AddService(service);

                    Workspace.Instance.CommandHistory.Log(
                        "add service",
                        () => AddService(service),
                        () => RemoveService(service)
                    );
                }
            });

            RemoveServiceCommand = new DelegateCommand(null, p =>
            {
                if (null != SelectedService)
                {
                    var service = SelectedService;
                    RemoveService(service);

                    Workspace.Instance.CommandHistory.Log(
                        "remove service",
                        () => RemoveService(service),
                        () => AddService(service)
                    );
                }
            });

            AddDependency<EffectiveValueChanged>("Width", e => e.Plugin.CoreType == typeof(RenderService) && e.PluginProperty.Name == "Width");
            AddDependency<EffectiveValueChanged>("Height", e => e.Plugin.CoreType == typeof(RenderService) && e.PluginProperty.Name == "Height");
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void AddUsing(Using use)
        {
            use.Scope = this;
            Usings.Add(use);

            foreach (var define in use.Defines)
            {
                Notify(new DefineAdded(define));
            }
        }

        public void RemoveUsing(Using use)
        {
            use.Scope = null;
            Usings.Remove(use);

            foreach (var define in use.Defines)
            {
                Notify(new DefineRemoved(define));
            }
        }

        private bool HasPrototypeWithName(string name)
        {
            return Prototypes.Any(x => x.Name == name);
        }

        public Entity CreatePrototype()
        {
            return new Entity() { Name = GetNextPrototypeName(), IsPrototype = true };
        }

        public void AddPrototype(Entity prototype)
        {
            if (null == prototype.Name)
            {
                throw new InvalidPrototypeNameException();
            }

            if (HasPrototypeWithName(prototype.Name))
            {
                throw new PrototypeExistsException(prototype.Name);
            }

            prototype.Scope = this;

            foreach (Plugin plugin in prototype.Plugins)
            {
                DefinePlugin(plugin);
            }

            Prototypes.Add(prototype);
        }

        public void RemovePrototype(Entity prototype)
        {
            prototype.Scope = null;
            prototype.IsPrototype = false;
            Prototypes.Remove(prototype);

            Workspace.Instance.CommandHistory.Log(
                "remove prototype '" + prototype.Name + "'",
                () => RemovePrototype(prototype),
                () => AddPrototype(prototype)
            );
        }

        public Entity GetPrototype(string name)
        {
            return Prototypes.FirstOrDefault(x => x.Name == name);
        }

        public void AddScene(Scene scene)
        {
            if (HasSceneWithName(scene.Name))
            {
                throw new SceneNameExistsException(scene.Name);
            }

            scene.Scope = this;

            foreach (Plugin plugin in scene.Plugins)
            {
                DefinePlugin(plugin);
            }

            if (Scenes.Count == 0)
            {
                FirstScene = scene;
            }

            Scenes.Add(scene);

            Workspace.Instance.CommandHistory.Log(
                "add scene '" + scene.Name + "'",
                () => AddScene(scene),
                () => RemoveScene(scene)
            );
        }

        public void RemoveScene(Scene scene)
        {
            scene.Scope = null;
            Scenes.Remove(scene);

            Workspace.Instance.CommandHistory.Log(
                "remove scene '" + scene.Name + "'",
                () => RemoveScene(scene),
                () => AddScene(scene)
            );
        }

        public Scene CreateScene()
        {
            return new Scene(GetNextSceneName());
        }

        public Scene GetScene(string name)
        {
            return Scenes.FirstOrDefault(x => x.Name == name);
        }

        public Attribute GetAttribute(string name)
        {
            return Attributes.FirstOrDefault(x => x.Name == name);
        }

        public void AddAttribute(Attribute attribute)
        {
            if (HasLocalAttribute(attribute.Name))
            {
                throw new AttributeNameExistsException(attribute.Name);
            }

            attribute.Scope = this;
            Attributes.Add(attribute);
        }

        public void RemoveAttribute(Attribute attribute)
        {
            attribute.Scope = null;
            Attributes.Remove(attribute);
        }

        public Attribute CreateAttribute()
        {
            Attribute attribute = new Attribute(GetNextAttributeKey());
            AddAttribute(attribute);
            return attribute;
        }

        private string GetNextAttributeKey()
        {
            string ret = "attribute" + nextAttribute;

            while (Attributes.Any(x => x.Name == ret))
            {
                nextAttribute++;
                ret = "attribute" + nextAttribute;
            }

            return ret;
        }

        private string GetNextSceneName()
        {
            string ret = "Scene" + nextScene;

            while (Scenes.Any(x => x.Name == ret))
            {
                nextScene++;
                ret = "Scene " + nextScene;
            }

            return ret;
        }

        private string GetNextPrototypeName()
        {
            string ret = "Prototype" + nextPrototype;

            while (Prototypes.Any(x => x.Name == ret))
            {
                nextPrototype++;
                ret = "Prototype" + nextPrototype;
            }

            return ret;
        }

        private void OnPluginUsed(PluginUsed n)
        {
            DefinePlugin(n.Plugin);
        }

        private bool HasDefineWithName(string name)
        {
            return Usings.Any(x => x.HasDefineWithName(name));
        }

        private bool HasDefineWithClass(string name)
        {
            return Usings.Any(x => x.HasDefineWithClass(name));
        }

        private Using GetUsing(string file)
        {
            return Usings.FirstOrDefault(x => x.File == file);
        }

        private void DefinePlugin(Plugin plugin)
        {
            if (!HasDefineWithClass(plugin.ClassName))
            {
                string name = plugin.ShortName;
                if (HasDefineWithName(name))
                {
                    int sequenceNumber = 0;
                    while (HasDefineWithName(plugin.ShortName + sequenceNumber))
                    {
                        sequenceNumber++;
                    }
                    name = plugin.ShortName + sequenceNumber;
                }

                Using use = GetUsing(plugin.File);
                if (null == use)
                {
                    use = new Using() { File = plugin.File };
                    AddUsing(use);
                }

                Define define = new Define(name, plugin.ClassName);
                use.AddDefine(define);
            }
        }

        public bool HasServiceWithType(Plugin type)
        {
            return Services.Any(x => x.Plugin == type);
        }

        public Service GetServiceByType(Plugin type)
        {
            return Services.FirstOrDefault(x => x.Plugin == type);
        }

        public Service GetServiceByType(Type type)
        {
            return Services.FirstOrDefault(x => x.Plugin.CoreType == type);
        }

        public Service GetServiceByDefinedName(string name)
        {
            return Services.FirstOrDefault(x => x.Type == name);
        }

        public void AddService(Service service)
        {
            if (!HasServiceWithType(service.Plugin))
            {
                service.Scope = this;
                Services.Add(service);
                AvailableServices.Remove(service.Plugin);
                Notify(new PluginUsed(service.Plugin));
            }
        }

        public void RemoveService(Service service)
        {
            if (HasServiceWithType(service.Plugin))
            {
                service.Scope = null;
                Services.Remove(service);
                AvailableServices.Add(service.Plugin);
            }
        }

        #region IEntityScope implementation

        IEnumerable<Entity> IEntityScope.Prototypes
        {
            get { return Prototypes; }
        }

        public bool EntityNameExists(string name)
        {
            return HasPrototypeWithName(name);
        }

        void IEntityScope.RemoveEntity(Entity entity)
        {
            RemovePrototype(entity);
        }

        int IEntityScope.IndexOf(Entity entity)
        {
            return Prototypes.IndexOf(entity);
        }

        #endregion

        #region ISceneScope implementation

        IEnumerable<Entity> ISceneScope.Prototypes
        {
            get { return Prototypes; }
        }

        public bool HasSceneWithName(string name)
        {
            return Scenes.Any(x => x.Name == name);
        }

        #endregion

        #region IAttributeScope implementation

        Entity IAttributeScope.Entity
        {
            get { return null; }
        }

        Scene IAttributeScope.Scene
        {
            get { return null; }
        }

        Game IAttributeScope.Game
        {
            get { return this; }
        }

        public event AttributeEventHandler InheritedAttributeAdded { add { } remove { } }
        public event AttributeEventHandler InheritedAttributeRemoved { add { } remove { } }
        public event AttributeEventHandler InheritedAttributeChanged { add { } remove { } }

        public Value GetInheritedValue(string key)
        {
            return Attribute.DefaultValue;
        }

        public bool HasInheritedAttribute(string key)
        {
            return false;
        }

        public bool HasInheritedNonDefaultAttribute(string key)
        {
            return false;
        }

        public bool HasLocalAttribute(string key)
        {
            return Attributes.Any(x => x.Name == key);
        }

        #endregion

        #region IPluginNamespace implementation

        public bool HasDefinedName(string name)
        {
            return Usings.Any(x => x.Defines.Any(y => y.Name == name));
        }

        public string GetDefinedName(Plugin plugin)
        {
            foreach (Using use in Usings)
            {
                Define define = use.GetDefineByClass(plugin.ClassName);
                if (null != define)
                {
                    return define.Name;
                }
            }

            return plugin.ClassName;
        }

        public Plugin GetPlugin(string name)
        {
            foreach (Using use in Usings)
            {
                Define define = use.GetDefineByName(name);
                if (null != define)
                {
                    name = define.Class;
                    break;
                }
            }

            return Workspace.Instance.GetPlugin(name);
        }

        #endregion
    }
}
