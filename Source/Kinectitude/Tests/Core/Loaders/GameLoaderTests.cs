using System.Reflection;
using Kinectitude.Core.Loaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Schema;

namespace Kinectitude.Tests.Core.Loaders
{
    [TestClass]
    public class GameLoaderTests
    {
        private const string xmlFile = "sample.xml";

        private static readonly GameLoader gameLoader;

        static GameLoaderTests()
        {
            XMLLoaderUtility.schemas = new XmlSchemaSet();
            gameLoader = new GameLoader(xmlFile, new Assembly[] { });
            gameLoader.CreateGame();
        }

        [TestMethod]
        [DeploymentItem("Core\\" + xmlFile)]
        public void TestPrototypeIs()
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

        [TestMethod]
        [DeploymentItem("Core\\" + xmlFile)]
        public void TestAvaliblePrototypes()
        {
            Assert.IsTrue(gameLoader.AvaliblePrototypes.Count == 4, gameLoader.AvaliblePrototypes.Count.ToString());
        }
    }
}
