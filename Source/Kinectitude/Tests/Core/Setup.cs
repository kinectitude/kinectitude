using Kinectitude.Core.Base;
using Kinectitude.Core.Loaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kinectitude.Tests.Core
{
    public class Setup
    {

        internal static Game StartGame(string testFile, Action<string> die = null)
        {
            if (die == null) die = new Action<string>(str => Assert.Fail(str));
            GameLoader gameLoader = new GameLoader(testFile, new Assembly[] { typeof(Setup).Assembly }, 1, 1, null, die);
            Game game = gameLoader.CreateGame();
            game.Start();
            return game;
        }

        public static void RunGame(string testFile, Action<string> die = null)
        {
            if (die == null) die = new Action<string>(str => Assert.Fail(str));
            GameLoader gameLoader = new GameLoader(testFile, new Assembly[] { typeof(Setup).Assembly }, 1, 1, null, die);
            Game game = gameLoader.CreateGame();
            game.Start();
            while (game.Running) game.OnUpdate(1 / 60f);
        }
    }
}
