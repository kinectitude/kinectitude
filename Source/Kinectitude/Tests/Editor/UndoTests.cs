using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models;

namespace Kinectitude.Tests.Editor
{
    [TestClass]
    public class UndoTests
    {
        public UndoTests()
        {
            Workspace.Instance.DialogService = MockDialogService.Instance;
        }

        [TestMethod]
        public void Test()
        {
            
        }
    }
}
