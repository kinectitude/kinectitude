//-----------------------------------------------------------------------
// <copyright file="Scene.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
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
using Kinectitude.Editor.Views.Scenes;
using Kinectitude.Editor.Views.Scenes.Presenters;
using Kinectitude.Editor.Views.Utils;
using Kinectitude.Render;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Kinectitude.Editor.Models
{
    internal enum EntityPlacementMode
    {
        None,
        Image,
        Shape,
        Text,
        Blank
    }

    internal sealed class Scene : GameModel<ISceneScope>, IAttributeScope, IEntityScope, IManagerScope
    {
        private string name;
        private int nextAttribute;
        private EntityPlacementMode placementMode;
        private Attribute selectedAttribute;
        private double cursorX;
        private double cursorY;
        private double placeLeft;
        private double placeTop;
        
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    if (value.ContainsWhitespace())
                    {
                        throw new InvalidSceneNameException(value);
                    }

                    if (SceneNameExists(value))
                    {
                        throw new SceneNameExistsException(value);
                    }

                    string oldName = name;
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        value = null;
                    }

                    Workspace.Instance.CommandHistory.Log(
                        "rename scene to '" + value + "'",
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
                var manager = GetManagerByType(typeof(RenderManager));
                if (null != manager)
                {
                    var property = manager.GetProperty("Width");
                    if (null != property && property.HasOwnValue)
                    {
                        return property.GetDoubleValue();
                    }
                }

                if (null != Scope)
                {
                    return Scope.Width;
                }

                return ConstantReader.CacheOrCreate(800);
            }
        }

        public double Height
        {
            get
            {
                var manager = GetManagerByType(typeof(RenderManager));
                if (null != manager)
                {
                    var property = manager.GetProperty("Height");
                    if (null != property && property.HasOwnValue)
                    {
                        return property.GetDoubleValue();
                    }
                }

                if (null != Scope)
                {
                    return Scope.Height;
                }

                return ConstantReader.CacheOrCreate(600);
            }
        }

        public double CameraX
        {
            get
            {
                var manager = GetManagerByType(typeof(RenderManager));
                if (null != manager)
                {
                    var property = manager.GetProperty("CameraX");
                    if (null != property)
                    {
                        return property.GetDoubleValue();
                    }
                }

                return ConstantReader.CacheOrCreate(0);
            }
        }

        public double CameraY
        {
            get
            {
                var manager = GetManagerByType(typeof(RenderManager));
                if (null != manager)
                {
                    var property = manager.GetProperty("CameraY");
                    if (null != property)
                    {
                        return property.GetDoubleValue();
                    }
                }

                return ConstantReader.CacheOrCreate(0);
            }
        }

        public EntityPlacementMode PlacementMode
        {
            get { return placementMode; }
            set
            {
                if (placementMode != value)
                {
                    placementMode = value;
                    NotifyPropertyChanged("PlacementMode");
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

        public double CursorX
        {
            get { return cursorX; }
            set
            {
                if (cursorX != value)
                {
                    cursorX = value;
                    NotifyPropertyChanged("CursorX");
                }
            }
        }

        public double CursorY
        {
            get { return cursorY; }
            set
            {
                if (cursorY != value)
                {
                    cursorY = value;
                    NotifyPropertyChanged("CursorY");
                }
            }
        }

        [DependsOn("PlacementMode")]
        public bool IsPlacing
        {
            get { return PlacementMode != EntityPlacementMode.None; }
        }

        public IEnumerable<Plugin> Plugins
        {
            get { return Entities.SelectMany(x => x.Plugins).Union(Managers.Select(x => x.Plugin)).Distinct(); }
        }

        public ObservableCollection<Attribute> Attributes { get; private set; }
        public ObservableCollection<Manager> Managers { get; private set; }
        public ObservableCollection<Entity> Entities { get; private set; }
        public ObservableCollection<EntityPresenter> EntityPresenters { get; private set; }

        public ICommand RenameCommand { get; private set; }
        public ICommand AddAttributeCommand { get; private set; }
        public ICommand RemoveAttributeCommand { get; private set; }
        public ICommand SetEntityFactoryCommand { get; private set; }
        public ICommand AddEntityCommand { get; private set; }
        public ICommand RemoveEntityCommand { get; private set; }
        public ICommand PropertiesCommand { get; private set; }

        public ICommand CutCommand { get; private set; }
        public ICommand CopyCommand { get; private set; }
        public ICommand PasteCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand BeginDragCommand { get; private set; }
        public ICommand CommitDragCommand { get; private set; }
        public ICommand CreateFromPrototypeCommand { get; private set; }

        public ICommand CommitSendForwardCommand { get; private set; }
        public ICommand CommitSendBackwardCommand { get; private set; }
        public ICommand CommitSendToFrontCommand { get; private set; }
        public ICommand CommitSendToBackCommand { get; private set; }

        public ICommand CommitAlignLeftCommand { get; private set; }
        public ICommand CommitAlignCenterCommand { get; private set; }
        public ICommand CommitAlignRightCommand { get; private set; }
        public ICommand CommitAlignTopCommand { get; private set; }
        public ICommand CommitAlignMiddleCommand { get; private set; }
        public ICommand CommitAlignBottomCommand { get; private set; }

        public Scene(string name)
        {
            this.name = name;
            
            Attributes = new ObservableCollection<Attribute>();
            Managers = new ObservableCollection<Manager>();
            Entities = new ObservableCollection<Entity>();
            EntityPresenters = new TransformedObservableCollection<Entity, EntityPresenter>(Entities, x => x.Presenter);

            RenameCommand = new DelegateCommand(null, (parameter) =>
            {
                Workspace.Instance.DialogService.ShowDialog<NameDialog>(new SceneTransaction(this));
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

            AddEntityCommand = new DelegateCommand(null, (parameter) =>
            {
                var point = (Point)parameter;
                var entityFactory = EntityFactoryForPlacementMode();

                if (null != entityFactory)
                {
                    var entity = entityFactory();
                    var transform = entity.GetComponentByType(typeof(TransformComponent));
                    if (null != transform)
                    {
                        transform.SetProperty("X", new Value(point.X, true));
                        transform.SetProperty("Y", new Value(point.Y, true));
                    }

                    AddEntity(entity);
                    PlacementMode = EntityPlacementMode.None;

                    Workspace.Instance.CommandHistory.Log(
                        "add entity",
                        () => AddEntity(entity),
                        () => RemoveEntity(entity)
                    );
                }
            });

            RemoveEntityCommand = new DelegateCommand(null, (parameter) =>
            {
                Entity entity = parameter as Entity;
                RemoveEntity(entity);
            });

            PropertiesCommand = new DelegateCommand(null, (parameter) =>
            {
                Workspace.Instance.DialogService.ShowDialog<SceneDialog>(new SceneTransaction(this));
            });

            CutCommand = new DelegateCommand(null, (parameter) =>
            {
                var selected = (IEnumerable)parameter;
                if (null != selected)
                {
                    var clip = selected.Cast<EntityPresenter>().Select(x => x.Entity).ToArray();

                    foreach (var entity in clip)
                    {
                        RemoveEntity(entity);
                    }

                    Workspace.Instance.ClippedItem = clip;
                }
            });

            CopyCommand = new DelegateCommand(null, (parameter) =>
            {
                var selected = (IEnumerable)parameter;
                if (null != selected)
                {
                    var clip = selected.Cast<EntityPresenter>().Select(x => x.Entity.DeepCopy()).ToArray();
                    Workspace.Instance.ClippedItem = clip;
                }
            });

            PasteCommand = new DelegateCommand(null, (parameter) =>
            {
                var clippedEntities = Workspace.Instance.ClippedItem as IEnumerable<Entity>;
                if (null != clippedEntities)
                {
                    foreach (var entity in clippedEntities)
                    {
                        AddEntity(entity.DeepCopy());
                    }
                }
            });

            DeleteCommand = new DelegateCommand(null, (parameter) =>
            {
                var selected = (IEnumerable)parameter;
                if (null != selected)
                {
                    var entities = selected.Cast<EntityPresenter>().Select(x => x.Entity).ToArray();
                    foreach (var entity in entities)
                    {
                        RemoveEntity(entity);
                    }
                }
            });

            BeginDragCommand = new DelegateCommand(null, p =>
            {
                if (IsPlacing)
                {
                    placeLeft = CursorX;
                    placeTop = CursorY;
                }
                else
                {
                    var selected = (IEnumerable)p;
                    if (null != selected)
                    {
                        var presenters = selected.Cast<EntityPresenter>();
                        foreach (var presenter in presenters)
                        {
                            presenter.StartX = presenter.DisplayX;
                            presenter.StartY = presenter.DisplayY;
                        }
                    }
                }
            });

            CommitDragCommand = new DelegateCommand(null, p =>
            {
                if (IsPlacing)
                {
                    var point1 = new Point(placeLeft, placeTop);
                    var point2 = new Point(CursorX, CursorY);
                    var rect = new Rect(point1, point2);

                    var entityFactory = EntityFactoryForPlacementMode();
                    if (null != entityFactory)
                    {
                        var entity = entityFactory();
                        if (entity.HasComponentOfType(typeof(TransformComponent)))
                        {
                            entity.Presenter.Width = rect.Width;
                            entity.Presenter.Height = rect.Height;
                            entity.Presenter.X = rect.X;
                            entity.Presenter.Y = rect.Y;
                        }

                        AddEntity(entity);
                        PlacementMode = EntityPlacementMode.None;

                        Workspace.Instance.CommandHistory.Log(
                            "add entity",
                            () => AddEntity(entity),
                            () => RemoveEntity(entity)
                        );
                    }
                }
                else
                {
                    var selected = (IEnumerable)p;
                    if (null != selected)
                    {
                        var translations = selected.Cast<EntityPresenter>().Select(x => x.GetTranslation()).ToArray();
                        Workspace.Instance.CommandHistory.WithoutLogging(() =>
                        {
                            foreach (var translation in translations)
                            {
                                translation.Presenter.X = translation.EndX;
                                translation.Presenter.Y = translation.EndY;
                            }
                        });

                        Workspace.Instance.CommandHistory.Log(
                            "move entities",
                            () =>
                            {
                                foreach (var translation in translations)
                                {
                                    translation.Presenter.X = translation.EndX;
                                    translation.Presenter.Y = translation.EndY;
                                }
                            },
                            () =>
                            {
                                foreach (var translation in translations)
                                {
                                    translation.Presenter.X = translation.StartX;
                                    translation.Presenter.Y = translation.StartY;
                                }
                            }
                        );
                    }
                }
            });

            CreateFromPrototypeCommand = new DelegateCommand(null, p =>
            {
                var prototype = p as Entity;
                if (null != prototype)
                {
                    var entity = new Entity();
                    entity.AddPrototype(prototype);

                    var transform = entity.GetComponentByType(typeof(TransformComponent));
                    if (null != transform)
                    {
                        transform.SetProperty("X", new Value(CursorX, true));
                        transform.SetProperty("Y", new Value(CursorY, true));
                    }

                    AddEntity(entity);
                }
            });

            CommitSendForwardCommand = new DelegateCommand(null, p =>
            {
                var selected = (IEnumerable)p;
                if (null != selected)
                {
                    var depths = selected.Cast<EntityPresenter>().Select(x => x.GetDepth()).ToArray();
                    var maxIndex = depths.Max(x => x.StartIndex);

                    if (maxIndex < Entities.Count - 1)
                    {
                        Workspace.Instance.CommandHistory.WithoutLogging(() =>
                        {
                            foreach (var depth in depths)
                            {
                                RemoveEntity(depth.Presenter.Entity);
                                PrivateAddEntity(depth.StartIndex + 1, depth.Presenter.Entity);
                            }
                        });
                        
                        Workspace.Instance.CommandHistory.Log(
                            "send forward",
                            () =>
                            {
                                foreach (var depth in depths)
                                {
                                    RemoveEntity(depth.Presenter.Entity);
                                    PrivateAddEntity(depth.StartIndex + 1, depth.Presenter.Entity);
                                }
                            },
                            () =>
                            {
                                foreach (var depth in depths.Reverse())
                                {
                                    RemoveEntity(depth.Presenter.Entity);
                                    PrivateAddEntity(depth.StartIndex, depth.Presenter.Entity);
                                }
                            }
                        );
                    }
                }
            });

            CommitSendBackwardCommand = new DelegateCommand(null, p =>
            {
                var selected = (IEnumerable)p;
                if (null != selected)
                {
                    var depths = selected.Cast<EntityPresenter>().Select(x => x.GetDepth()).ToArray();
                    var minIndex = depths.Min(x => x.StartIndex);

                    if (minIndex > 0)
                    {
                        Workspace.Instance.CommandHistory.WithoutLogging(() =>
                        {
                            foreach (var depth in depths)
                            {
                                RemoveEntity(depth.Presenter.Entity);
                                PrivateAddEntity(depth.StartIndex - 1, depth.Presenter.Entity);
                            }
                        });

                        Workspace.Instance.CommandHistory.Log(
                            "send backward",
                            () =>
                            {
                                foreach (var depth in depths)
                                {
                                    RemoveEntity(depth.Presenter.Entity);
                                    PrivateAddEntity(depth.StartIndex - 1, depth.Presenter.Entity);
                                }
                            },
                            () =>
                            {
                                foreach (var depth in depths.Reverse())
                                {
                                    RemoveEntity(depth.Presenter.Entity);
                                    PrivateAddEntity(depth.StartIndex, depth.Presenter.Entity);
                                }
                            }
                        );
                    }
                }
            });

            CommitSendToFrontCommand = new DelegateCommand(null, p =>
            {
                var selected = (IEnumerable)p;
                if (null != selected)
                {
                    var depths = selected.Cast<EntityPresenter>().Select(x => x.GetDepth()).ToArray().OrderBy(x => x.StartIndex);

                    Workspace.Instance.CommandHistory.WithoutLogging(() =>
                    {
                        foreach (var depth in depths)
                        {
                            RemoveEntity(depth.Presenter.Entity);
                        }

                        foreach (var depth in depths)
                        {
                            PrivateAddEntity(Entities.Count, depth.Presenter.Entity);
                        }
                    });

                    Workspace.Instance.CommandHistory.Log(
                        "send to front",
                        () =>
                        {
                            foreach (var depth in depths)
                            {
                                RemoveEntity(depth.Presenter.Entity);
                            }

                            foreach (var depth in depths)
                            {
                                PrivateAddEntity(Entities.Count, depth.Presenter.Entity);
                            }
                        },
                        () =>
                        {
                            foreach (var depth in depths)
                            {
                                RemoveEntity(depth.Presenter.Entity);
                            }

                            foreach (var depth in depths)
                            {
                                PrivateAddEntity(depth.StartIndex, depth.Presenter.Entity);
                            }
                        }
                    );
                }
            });

            CommitSendToBackCommand = new DelegateCommand(null, p =>
            {
                var selected = (IEnumerable)p;
                if (null != selected)
                {
                    var depths = selected.Cast<EntityPresenter>().Select(x => x.GetDepth()).ToArray().OrderBy(x => x.StartIndex);

                    Workspace.Instance.CommandHistory.WithoutLogging(() =>
                    {
                        foreach (var depth in depths)
                        {
                            RemoveEntity(depth.Presenter.Entity);
                        }

                        int i = 0;
                        foreach (var depth in depths)
                        {
                            PrivateAddEntity(0, depth.Presenter.Entity);
                            i += 1;
                        }
                    });

                    Workspace.Instance.CommandHistory.Log(
                        "send to back",
                        () =>
                        {
                            foreach (var depth in depths)
                            {
                                RemoveEntity(depth.Presenter.Entity);
                            }

                            int i = 0;
                            foreach (var depth in depths)
                            {
                                PrivateAddEntity(0, depth.Presenter.Entity);
                                i += 1;
                            }
                        },
                        () =>
                        {
                            foreach (var depth in depths)
                            {
                                RemoveEntity(depth.Presenter.Entity);
                            }

                            foreach (var depth in depths)
                            {
                                PrivateAddEntity(depth.StartIndex, depth.Presenter.Entity);
                            }
                        }
                    );
                }
            });

            CommitAlignLeftCommand = new DelegateCommand(null, p =>
            {
                var selected = (IEnumerable)p;
                if (null != selected)
                {
                    var locations = selected.Cast<EntityPresenter>().Select(x => x.GetLocation()).ToArray();
                    var minLeft = locations.Min(x => x.Presenter.DisplayX);

                    Workspace.Instance.CommandHistory.WithoutLogging(() =>
                    {
                        foreach (var location in locations)
                        {
                            location.Presenter.X = minLeft;
                        }
                    });

                    Workspace.Instance.CommandHistory.Log(
                        "align left",
                        () =>
                        {
                            foreach (var location in locations)
                            {
                                location.Presenter.X = minLeft;
                            }
                        },
                        () =>
                        {
                            foreach (var location in locations)
                            {
                                location.Presenter.X = location.StartX;
                            }
                        }
                    );
                }
            });

            CommitAlignCenterCommand = new DelegateCommand(null, p =>
            {
                var selected = (IEnumerable)p;
                if (null != selected)
                {
                    var locations = selected.Cast<EntityPresenter>().Select(x => x.GetLocation()).ToArray();
                    var maxRight = locations.Max(x => x.Presenter.DisplayX + x.Presenter.ObservedWidth);
                    var minLeft = locations.Min(x => x.Presenter.DisplayX);
                    var centerX = (maxRight + minLeft) / 2.0d;

                    Workspace.Instance.CommandHistory.WithoutLogging(() =>
                    {
                        foreach (var location in locations)
                        {
                            location.Presenter.X = centerX - location.ObservedWidth / 2.0d;
                        }
                    });

                    Workspace.Instance.CommandHistory.Log(
                        "align center",
                        () =>
                        {
                            foreach (var location in locations)
                            {
                                location.Presenter.X = centerX - location.ObservedWidth / 2.0d;
                            }
                        },
                        () =>
                        {
                            foreach (var location in locations)
                            {
                                location.Presenter.X = location.StartX;
                            }
                        }
                    );
                }
            });

            CommitAlignRightCommand = new DelegateCommand(null, p =>
            {
                var selected = (IEnumerable)p;
                if (null != selected)
                {
                    var locations = selected.Cast<EntityPresenter>().Select(x => x.GetLocation()).ToArray();
                    var maxRight = locations.Max(x => x.Presenter.DisplayX + x.Presenter.ObservedWidth);

                    Workspace.Instance.CommandHistory.WithoutLogging(() =>
                    {
                        foreach (var location in locations)
                        {
                            location.Presenter.X = maxRight - location.ObservedWidth;
                        }
                    });

                    Workspace.Instance.CommandHistory.Log(
                        "align right",
                        () =>
                        {
                            foreach (var location in locations)
                            {
                                location.Presenter.X = maxRight - location.ObservedWidth;
                            }
                        },
                        () =>
                        {
                            foreach (var location in locations)
                            {
                                location.Presenter.X = location.StartX;
                            }
                        }
                    );
                }
            });

            CommitAlignTopCommand = new DelegateCommand(null, p =>
            {
                var selected = (IEnumerable)p;
                if (null != selected)
                {
                    var locations = selected.Cast<EntityPresenter>().Select(x => x.GetLocation()).ToArray();
                    var minTop = locations.Min(x => x.Presenter.DisplayY);

                    Workspace.Instance.CommandHistory.WithoutLogging(() =>
                    {
                        foreach (var location in locations)
                        {
                            location.Presenter.Y = minTop;
                        }
                    });

                    Workspace.Instance.CommandHistory.Log(
                        "align top",
                        () =>
                        {
                            foreach (var location in locations)
                            {
                                location.Presenter.Y = minTop;
                            }
                        },
                        () =>
                        {
                            foreach (var location in locations)
                            {
                                location.Presenter.Y = location.StartY;
                            }
                        }
                    );
                }
            });

            CommitAlignMiddleCommand = new DelegateCommand(null, p =>
            {
                var selected = (IEnumerable)p;
                if (null != selected)
                {
                    var locations = selected.Cast<EntityPresenter>().Select(x => x.GetLocation()).ToArray();
                    var maxBottom = locations.Max(x => x.Presenter.DisplayY + x.Presenter.ObservedHeight);
                    var minTop = locations.Min(x => x.Presenter.DisplayY);
                    var middleY = (maxBottom + minTop) / 2.0d;

                    Workspace.Instance.CommandHistory.WithoutLogging(() =>
                    {
                        foreach (var location in locations)
                        {
                            location.Presenter.Y = middleY - location.ObservedHeight / 2.0d;
                        }
                    });

                    Workspace.Instance.CommandHistory.Log(
                        "align middle",
                        () =>
                        {
                            foreach (var location in locations)
                            {
                                location.Presenter.Y = middleY - location.ObservedHeight / 2.0d;
                            }
                        },
                        () =>
                        {
                            foreach (var location in locations)
                            {
                                location.Presenter.Y = location.StartY;
                            }
                        }
                    );
                }
            });

            CommitAlignBottomCommand = new DelegateCommand(null, p =>
            {
                var selected = (IEnumerable)p;
                if (null != selected)
                {
                    var locations = selected.Cast<EntityPresenter>().Select(x => x.GetLocation()).ToArray();
                    var maxBottom = locations.Max(x => x.Presenter.DisplayY + x.Presenter.ObservedHeight);

                    Workspace.Instance.CommandHistory.WithoutLogging(() =>
                    {
                        foreach (var location in locations)
                        {
                            location.Presenter.Y = maxBottom - location.ObservedHeight;
                        }
                    });

                    Workspace.Instance.CommandHistory.Log(
                        "align bottom",
                        () =>
                        {
                            foreach (var location in locations)
                            {
                                location.Presenter.Y = maxBottom - location.ObservedHeight;
                            }
                        },
                        () =>
                        {
                            foreach (var location in locations)
                            {
                                location.Presenter.Y = location.StartY;
                            }
                        }
                    );
                }
            });

            AddDependency<EffectiveValueChanged>("Width", e => e.Plugin.CoreType == typeof(RenderManager) && e.PluginProperty.Name == "Width");
            AddDependency<EffectiveValueChanged>("Height", e => e.Plugin.CoreType == typeof(RenderManager) && e.PluginProperty.Name == "Height");
            AddDependency<EffectiveValueChanged>("CameraX", e => e.Plugin.CoreType == typeof(RenderManager) && e.PluginProperty.Name == "CameraX");
            AddDependency<EffectiveValueChanged>("CameraY", e => e.Plugin.CoreType == typeof(RenderManager) && e.PluginProperty.Name == "CameraY");
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        private EntityFactory EntityFactoryForPlacementMode()
        {
            EntityFactory entityFactory = null;
            switch (PlacementMode)
            {
                case EntityPlacementMode.Blank:
                    entityFactory = Workspace.Instance.BlankEntityFactory;
                    break;
                case EntityPlacementMode.Image:
                    entityFactory = Workspace.Instance.ImageEntityFactory;
                    break;
                case EntityPlacementMode.Shape:
                    entityFactory = Workspace.Instance.ShapeEntityFactory;
                    break;
                case EntityPlacementMode.Text:
                    entityFactory = Workspace.Instance.TextEntityFactory;
                    break;
            }
            return entityFactory;
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

        public void AddManager(Manager manager)
        {
            manager.Scope = this;
            Managers.Add(manager);

            Notify(new PluginUsed(manager.Plugin));
        }

        public void RemoveManager(Manager manager)
        {
            if (!manager.IsRequired)
            {
                manager.Scope = null;
                Managers.Remove(manager);
            }
        }

        public void ClearManagers()
        {
            var managers = Managers.ToArray();

            foreach (var manager in managers)
            {
                RemoveManager(manager);
            }
        }

        public void AddEntity(Entity entity)
        {
            PrivateAddEntity(Entities.Count, entity);
        }

        public void PrivateAddEntity(int index, Entity entity)
        {
            if (EntityNameExists(entity.Name))
            {
                throw new EntityNameExistsException(entity.Name);
            }

            if (HasDefinedName(entity.Name))
            {
                throw new NameDefinedException(entity.Name);
            }

            entity.Scope = this;
            Entities.Insert(index, entity);

            entity.Components.CollectionChanged += OnEntityComponentChanged;
            foreach (Component component in entity.Components)
            {
                ResolveComponentDependencies(component);
            }

            foreach (Plugin plugin in entity.Plugins)
            {
                Notify(new PluginUsed(plugin));
            }
        }

        public void RemoveEntity(Entity entity)
        {
            entity.Scope = null;
            Entities.Remove(entity);

            entity.Components.CollectionChanged -= OnEntityComponentChanged;

            Workspace.Instance.CommandHistory.Log(
                "remove entity",
                () => RemoveEntity(entity),
                () => AddEntity(entity)
            );
        }

        public Entity GetEntityByName(string name)
        {
            return Entities.FirstOrDefault(x => x.Name == name);
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

        private void ResolveComponentDependencies(Component component)
        {
            foreach (string require in component.Requires)
            {
                Plugin plugin = GetPlugin(require);
                if (plugin.Type == PluginType.Manager)
                {
                    if (!HasManagerOfType(plugin))
                    {
                        Manager manager = new Manager(plugin);
                        AddManager(manager);
                    }
                }
            }
        }

        private void OnEntityComponentChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Component component in args.NewItems)
                {
                    ResolveComponentDependencies(component);
                }
            }
        }

        public Manager GetManagerByType(Plugin type)
        {
            return Managers.FirstOrDefault(x => x.Plugin == type);
        }

        public Manager GetManagerByType(Type type)
        {
            return Managers.FirstOrDefault(x => x.Plugin.CoreType == type);
        }

        public Manager GetManagerByDefinedName(string name)
        {
            return Managers.FirstOrDefault(x => x.Type == name);
        }

        public bool HasManagerOfType(Plugin type)
        {
            return Managers.Any(x => x.Plugin == type);
        }

        public bool SceneNameExists(string name)
        {
            return null != Scope ? Scope.HasSceneWithName(name) : false;
        }

        #region IEntityScope implementation

        public IEnumerable<Entity> Prototypes
        {
            get { return null != Scope ? Scope.Prototypes : null; }
        }

        public bool EntityNameExists(string name)
        {
            return name != null && Entities.Any(x => x.Name != null && x.Name == name) || null != Scope && Scope.EntityNameExists(name);
        }

        void IEntityScope.RemoveEntity(Entity entity)
        {
            RemoveEntity(entity);
        }

        int IEntityScope.IndexOf(Entity entity)
        {
            return Entities.IndexOf(entity);
        }

        #endregion

        #region IPluginNamespace implementation

        public bool HasDefinedName(string name)
        {
            return null != Scope ? Scope.HasDefinedName(name) : false;
        }

        public Plugin GetPlugin(string type)
        {
            return null != Scope ? Scope.GetPlugin(type) : Workspace.Instance.GetPlugin(type);
        }

        public string GetDefinedName(Plugin plugin)
        {
            return null != Scope ? Scope.GetDefinedName(plugin) : plugin.ClassName;
        }

        #endregion

        #region IAttributeScope implementation

        Entity IAttributeScope.Entity
        {
            get { return null; }
        }

        Scene IAttributeScope.Scene
        {
            get { return this; }
        }

        Game IAttributeScope.Game
        {
            get { return null; }
        }

        public event AttributeEventHandler InheritedAttributeAdded { add { } remove { } }
        public event AttributeEventHandler InheritedAttributeRemoved { add { } remove { } }
        public event AttributeEventHandler InheritedAttributeChanged { add { } remove { } }

        public bool HasInheritedAttribute(string key)
        {
            return false;
        }

        public bool HasInheritedNonDefaultAttribute(string key)
        {
            return false;
        }

        public Value GetInheritedValue(string key)
        {
            return Attribute.DefaultValue;
        }

        public bool HasLocalAttribute(string key)
        {
            return Attributes.Any(x => x.Name == key);
        }

        #endregion

        #region IManagerScope implementation

        public bool RequiresManager(Manager manager)
        {
            foreach (Entity entity in Entities)
            {
                foreach (Component component in entity.Components)
                {
                    if (component.Requires.Contains(manager.Plugin.ClassName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}
