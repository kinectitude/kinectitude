using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Data;

namespace Kinectitude.Tests.Core.Data
{
    [TestClass]
    public class StringDataTests
    {
        [TestMethod]
        public void ConstantString()
        {
            ExpressionReader reader = ExpressionReader.CreateExpressionReader("constant", null, null);
            Assert.IsTrue(reader.GetValue() == "constant");
        }
    }
}
