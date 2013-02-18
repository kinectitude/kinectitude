using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.DataContainers;
using Kinectitude.Editor.Models.Data.ValueReaders;
using Kinectitude.Editor.Models.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.Changeables
{
    internal sealed class ServiceChangeable : BaseChangeable
    {
        private readonly GameDataContainer container;
        private readonly string type;
        private Game game;
        private Service service;

        private Game Game
        {
            get { return game; }
            set
            {
                if (game != value)
                {
                    if (null != game)
                    {
                        game.Services.CollectionChanged -= OnGameServicesChanged;
                        Service = null;

                        foreach (var service in game.Services)
                        {
                            UnfollowService(service);
                        }
                    }

                    game = value;

                    if (null != game)
                    {
                        game.Services.CollectionChanged += OnGameServicesChanged;
                        Service = game.GetServiceByDefinedName(type);

                        foreach (var service in game.Services)
                        {
                            FollowService(service);
                        }
                    }
                }
            }
        }

        public Service Service
        {
            get { return service; }
            set
            {
                if (service != value)
                {
                    service = value;
                    PublishComponentChange();
                }
            }
        }

        public ServiceChangeable(GameDataContainer container, string type) : base(container)
        {
            this.container = container;
            this.type = type;

            Game = container.Game;
            container.GameChanged += OnGameChanged;
        }

        private void OnGameChanged()
        {
            Game = container.Game;
        }

        private void UnfollowService(Service service)
        {
            service.PropertyChanged -= OnServicePropertyChanged;

            foreach (var property in service.Properties)
            {
                property.PropertyChanged -= OnPropertyPropertyChanged;
            }
        }

        private void FollowService(Service service)
        {
            service.PropertyChanged += OnServicePropertyChanged;

            foreach (var property in service.Properties)
            {
                property.PropertyChanged += OnPropertyPropertyChanged;
            }
        }

        private void OnServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Type")
            {
                var service = (Service)sender;
                if (service == Service)
                {
                    if (service.Type != type)
                    {
                        Service = null;
                    }
                }
                else if (service.Type == type)
                {
                    Service = service;
                }
            }
        }
        
        private void OnPropertyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                PublishPropertyChange(((Property)sender).Name);
            }
        }

        private void OnGameServicesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Service service in e.NewItems)
                {
                    if (Game.GetDefinedName(service.Plugin) == type)
                    {
                        Service = service;
                    }

                    FollowService(service);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Service service in e.OldItems)
                {
                    if (Game.GetDefinedName(service.Plugin) == type)
                    {
                        Service = null;
                    }

                    UnfollowService(service);
                }
            }
        }

        protected override ValueReader CreatePropertyReader(string name)
        {
            return new ServiceValueReader(this, name);
        }
    }
}
