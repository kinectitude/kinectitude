using Kinectitude.Core.Base;
using Kinectitude.Core.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kinectitude.Tests.Core
{
    public class Setup
    {
        public static Game StartGame(string testFile)
        {
            GameLoader gameLoader = new GameLoader(testFile, new Assembly[] { typeof(Setup).Assembly }, 1, 1, null);
            Game game = gameLoader.CreateGame();
            game.Start();
            return game;
        }

        public static void RunGame(string testFile)
        {
            GameLoader gameLoader = new GameLoader(testFile, new Assembly[] { typeof(Setup).Assembly }, 1, 1, null);
            Game game = gameLoader.CreateGame();
            game.Start();
            while (game.Running) game.OnUpdate(1 / 60f);
        }
    }
}
