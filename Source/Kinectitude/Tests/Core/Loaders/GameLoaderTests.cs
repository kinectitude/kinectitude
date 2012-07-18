using System.Reflection;
using Kinectitude.Core.Loaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kinectitude.Tests.Core.Loaders
{
    [TestClass]
    public class GameLoaderTests
    {
        private const string xmlFile = "sample.xml";

        private static readonly GameLoader xmlGameLoader = GameLoader.GetGameLoader(xmlFile, new Assembly[] { });

        private static readonly GameLoader[] gameLoaders = { xmlGameLoader };

        static GameLoaderTests()
        {
            foreach (GameLoader gameLoader in gameLoaders) gameLoader.CreateGame();
        }

        [TestMethod]
        [DeploymentItem("Core\\" + xmlFile)]
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
        [DeploymentItem("Core\\" + xmlFile)]
        public void TestAvaliblePrototypes()
        {
            foreach (GameLoader gameLoader in gameLoaders)
            {
                Assert.IsTrue(gameLoader.AvaliblePrototypes.Count == 4, gameLoader.GetType().Name + " " + gameLoader.AvaliblePrototypes.Count);
            }
        }
    }
}
