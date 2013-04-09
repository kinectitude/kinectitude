//-----------------------------------------------------------------------
// <copyright file="LoadSave.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

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

            int line = 1;
            int col = 1;
            for (int i = 0; i < shouldBe.Length; i++)
            {
                char ch = shouldBe[i];

                if (ch == '\n')
                {
                    line++;
                    col = 1;
                }

                if (after[i] != shouldBe[i])
                {
                    Assert.Fail("Files do not match at line " + line + " column " + col);
                }

                col++;
            }

            Assert.AreEqual(shouldBe, after);
        }
    }
}
