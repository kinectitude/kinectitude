using System;
using Kinectitude.Core.Base;
using Kinectitude.Core.Loaders;
using Action = Kinectitude.Core.Base.Action;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Data;

namespace Kinectitude.Tests.Core.TestMocks
{

    public class GameLoaderMock : GameLoader
    {
        internal SceneLoader mockScene;

        internal override SceneLoader GetSceneLoader(string name)
        {
            return mockScene;
        }
    }

    internal class SceneLoaderMock : SceneLoader
    {
        public bool entityCreated = false;

        public SceneLoaderMock(GameLoader gameLoader) : base(gameLoader) { }

        internal override void CreateEntity(string name)
        {
            entityCreated = true;
        }
    }

    public class ActionMock : Action
    {
        [Plugin("", "")]
        public IExpressionReader Expression { get; set; }
        [Plugin("", "")]
        public ITypeMatcher TypeMatcher { get; set; }
        [Plugin("", "")]
        public IValueWriter Writer { get; set; }
        public bool hasRun { get; private set; }
        public override void Run() { hasRun = true; }
    }


    public class EventMock : Event
    {
        [Plugin("", "")]
        public ITypeMatcher TypeMatcher { get; set; }
        [Plugin("", "")]
        public IExpressionReader Expression { get; set; }
        public bool hasInit = false;
        public override void OnInitialize() { hasInit = true; }
    }

    public class ComponentMock : Component
    {
        public bool destroyed = false;
        [Plugin("", "")]
        public int I { get; set; }
        [Plugin("", "")]
        public bool B { get; set; }
        [Plugin("", "")]
        public float F { get; set; }
        [Plugin("", "")]
        public double D { get; set; }

        public ComponentMock()
        {
            I = 10;
            B = true;
            F = 10.1f;
            D = 11.2;
        }

        public override void Destroy()
        {
            destroyed = true;
        }
    }

}
