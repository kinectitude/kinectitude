using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;

namespace Kinectitude.Tests.Core
{
    [TestClass]
    public class BadName
    {
        private const string badGame = "core/badName.kgl";

        bool bad;

        void changeBadDie()
        {
            bad = true;
            throw new Exception();
        }

        [TestMethod]
        [DeploymentItem(badGame)]
        public void BadNameTest()
        {

            bad = false;
            try{

                Setup.StartGame("badName.kgl", (new Action<String>(s => changeBadDie())));
            }catch(Exception){}
            Assert.IsTrue(bad);
        }
    }
}
