//-----------------------------------------------------------------------
// <copyright file="GameDataContainer.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.Changeables;
using Kinectitude.Editor.Models.Data.ValueReaders;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Models.Values;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.DataContainers
{
    internal sealed class GameDataContainer : BaseDataContainer
    {
        private readonly Value value;
        private Game game;

        public Game Game
        {
            get { return game; }
            private set
            {
                if (game != value)
                {
                    if (null != game)
                    {
                        game.Attributes.CollectionChanged -= OnGameAttributesChanged;

                        foreach (var attribute in game.Attributes)
                        {
                            UnfollowAttribute(attribute);
                        }
                    }

                    game = value;

                    if (null != game)
                    {
                        game.Attributes.CollectionChanged += OnGameAttributesChanged;

                        foreach (var attribute in game.Attributes)
                        {
                            UnfollowAttribute(attribute);
                        }
                    }

                    if (null != GameChanged)
                    {
                        GameChanged();
                    }

                    PublishAll();
                }
            }
        }

        public event System.Action GameChanged;

        public GameDataContainer(Value value)
        {
            this.value = value;

            Game = value.Game;
            value.AddHandler<ScopeChanged>(OnScopeChanged);
        }

        private void FollowAttribute(Attribute attribute)
        {
            attribute.PropertyChanged += OnAttributePropertyChanged;
            attribute.NameChanged += OnAttributeNameChanged;
        }

        private void UnfollowAttribute(Attribute attribute)
        {
            attribute.PropertyChanged -= OnAttributePropertyChanged;
            attribute.NameChanged -= OnAttributeNameChanged;
        }
        private void OnScopeChanged(ScopeChanged e)
        {
            Game = value.Game;
        }

        private void OnAttributePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                PublishAttributeChange(((Attribute)sender).Name);
            }
        }

        private void OnAttributeNameChanged(string oldName, string newName)
        {
            PublishAttributeChange(oldName);
            PublishAttributeChange(newName);
        }


        private void OnGameAttributesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Attribute attribute in e.NewItems)
                {
                    FollowAttribute(attribute);
                    PublishAttributeChange(attribute.Name);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Attribute attribute in e.OldItems)
                {
                    UnfollowAttribute(attribute);
                    PublishAttributeChange(attribute.Name);
                }
            }
        }

        protected override ValueReader CreateAttributeReader(string key)
        {
            return new GameValueReader(this, key);
        }

        protected override IChangeable CreateChangeable(string name)
        {
            return new ServiceChangeable(this, name);
        }
    }
}
