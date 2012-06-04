using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Loaders;
using System.Reflection;

namespace Kinectitude.Tests.Core.Loaders
{
    [TestClass]
    public class GameLoaderTests
    {
        private const string sampleFile = "sample.xml";

        private static readonly GameLoader xmlGameLoader = GameLoader.GetGameLoader(sampleFile, new Assembly[] { });

        private static readonly GameLoader[] gameLoaders = { xmlGameLoader };

        [TestMethod]
        [DeploymentItem("Core\\" + sampleFile)]
        public void TestPrototypeIs()
        {
            foreach (GameLoader gameLoader in gameLoaders)
            {
                //Test if it inherits from a type
                Assert.IsTrue(gameLoader.PrototypeIs["prototype1"].Count == 1, gameLoader.GetType().Name);
                Assert.IsTrue(gameLoader.PrototypeIs["prototype1"][0] == "prototype1", gameLoader.GetType().Name);

                Assert.IsTrue(gameLoader.PrototypeIs["prototype2"].Count == 2, gameLoader.GetType().Name);
                Assert.IsTrue(gameLoader.PrototypeIs["prototype2"].Contains("prototype2"), gameLoader.GetType().Name);
                Assert.IsTrue(gameLoader.PrototypeIs["prototype2"].Contains("prototype1"), gameLoader.GetType().Name);

                Assert.IsTrue(gameLoader.PrototypeIs["prototype3"].Count == 3, gameLoader.GetType().Name);
                Assert.IsTrue(gameLoader.PrototypeIs["prototype3"].Contains("prototype3"), gameLoader.GetType().Name);
                Assert.IsTrue(gameLoader.PrototypeIs["prototype3"].Contains("prototype2"), gameLoader.GetType().Name);
                Assert.IsTrue(gameLoader.PrototypeIs["prototype3"].Contains("prototype1"), gameLoader.GetType().Name);

                Assert.IsTrue(gameLoader.PrototypeIs["prototype4"].Count == 3, gameLoader.GetType().Name);
                Assert.IsTrue(gameLoader.PrototypeIs["prototype4"].Contains("prototype4"), gameLoader.GetType().Name);
                Assert.IsTrue(gameLoader.PrototypeIs["prototype4"].Contains("prototype2"), gameLoader.GetType().Name);
                Assert.IsTrue(gameLoader.PrototypeIs["prototype4"].Contains("prototype1"), gameLoader.GetType().Name);
            }
        }

        [TestMethod]
        [DeploymentItem("Core\\" + sampleFile)]
        public void TestAvaliblePrototypes()
        {
            foreach (GameLoader gameLoader in gameLoaders)
            {
                Assert.IsTrue(gameLoader.AvaliblePrototypes.Count == 4, gameLoader.GetType().Name);
            }
        }
    }
}
