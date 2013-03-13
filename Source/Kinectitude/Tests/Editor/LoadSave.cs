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
        public void LoadAndSave()
        {
            KglGameStorage storage = new KglGameStorage(new FileInfo("Editor/read.kgl"));

            Game game = storage.LoadGame();
            storage.SaveGame(game);
            string after = File.ReadAllText("Editor/read.kgl");
            string shouldBe = File.ReadAllText("Editor/original.kgl");
            Assert.AreEqual(shouldBe, after);
        }
    }
}
