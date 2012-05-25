﻿using System.Collections.Generic;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Loaders
{
    internal abstract class SceneLoader
    {
        internal Dictionary<string, HashSet<int>> IsType { get; private set; }
        internal Dictionary<string, HashSet<int>> IsExactType { get; private set; }
        internal Dictionary<int, Entity> EntityById { get; private set; }
        internal Dictionary<string, Entity> EntityByName { get; private set; }
        internal protected Scene Scene { get; protected set; }
        internal Game Game { get; private set; }
        protected GameLoader GameLoader { get; private set; }
        
        protected SceneLoader(GameLoader gameLoader)
        {
            IsType = new Dictionary<string, HashSet<int>>();
            IsExactType = new Dictionary<string, HashSet<int>>();
            EntityById = new Dictionary<int, Entity>();
            EntityByName = new Dictionary<string, Entity>();
            GameLoader = gameLoader;
            Game = GameLoader.Game;
        }
    }
}