//-----------------------------------------------------------------------
// <copyright file="Functions.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class Functions
    {   
        [TestMethod]
        public void FunctionTest()
        {
            Setup.StartGame("Core/functions.kgl");
            AssertionAction.CheckValue("1");
            AssertionAction.CheckValue("params 0", 2);
            AssertionAction.CheckValue("3");
            AssertionAction.CheckValue("params 5");
            AssertionAction.CheckValue("min");
            AssertionAction.CheckValue("max");
            AssertionAction.CheckValue("bool");
            AssertionAction.CheckValue("number");
            AssertionAction.CheckValue("str");
            AssertionAction.CheckValue("ln");
            AssertionAction.CheckValue("log", 2);
            AssertionAction.CheckValue("absolute");
            AssertionAction.CheckValue("random", 3);
        }        
    }
}
