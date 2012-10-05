using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Base;
using Kinectitude.Tests.Core.TestMocks;
using Kinectitude.Core.Loaders;

namespace Kinectitude.Tests.Core.Base
{
    [TestClass]
    public class SceneTests
    {
        static SceneTests()
        {
            try
            {
                ClassFactory.RegisterType("manager", typeof(ManagerMock));
            }
            catch (ArgumentException)
            {
                //this is incase another test case registered this type already
            }
        }

        [TestMethod]
        public void RunningGetManager()
        {
            LoadedEntity e = new LoadedEntity("", new System.Collections.Generic.List<Tuple<string, string>>(), 0, new List<string>(), new List<string>());
            GameLoaderMock glm = new GameLoaderMock();
            Scene s = new Scene(new SceneLoaderMock(glm, new LoaderUtilityMock()), glm.Game);
            Entity entity = e.Create(s);
            EventMock evt = new EventMock();
            evt.Entity = entity;
            GetManagerAction action = new GetManagerAction();
            action.Event = evt;
            //This used to crash.
            action.Run();
        }
    }
}
