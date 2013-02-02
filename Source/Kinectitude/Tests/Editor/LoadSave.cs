using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Storage.Kgl;
using System.IO;
using Kinectitude.Editor.Models;

namespace Kinectitude.Tests.Editor.loadSave
{
    [TestClass]
    public class LoadSave
    {
        [TestMethod]
        [DeploymentItem("Editor/read.kgl")]
        [DeploymentItem("Editor/original.kgl")]
        [DeploymentItem("Plugins")]
        public void LoadAndSave()
        {
            Directory.CreateDirectory("Plugins");
            File.Move("Kinectitude.Physics.dll", "Plugins/Kinectitude.Physics.dll");
            File.Move("Kinectitude.Render.dll", "Plugins/Kinectitude.Render.dll");
            KglGameStorage storage = new KglGameStorage(new FileInfo("read.kgl"));
            Game game = storage.LoadGame();
            storage.SaveGame(game);
            string after = File.ReadAllText("read.kgl");
            string shouldBe = File.ReadAllText("original.kgl");
            Assert.AreEqual(shouldBe, after);
        }
    }
}
