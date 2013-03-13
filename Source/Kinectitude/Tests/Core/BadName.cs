using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class BadName
    {
        bool bad;

        void changeBadDie()
        {
            bad = true;
            throw new Exception();
        }

        [TestMethod]
        public void BadNameTest()
        {

            bad = false;
            try{

                Setup.StartGame("Core/badName.kgl", (new Action<String>(s => changeBadDie())));
            }catch(Exception){}
            Assert.IsTrue(bad);
        }
    }
}
