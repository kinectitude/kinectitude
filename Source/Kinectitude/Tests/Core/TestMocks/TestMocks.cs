using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using Kinectitude.Core.Data;
using Kinectitude.Core.Loaders;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Tests.Core.TestMocks
{

    public class GameLoaderMock : GameLoader
    {
        internal SceneLoader mockScene = null;

        internal override SceneLoader GetSceneLoader(string name)
        {
            return mockScene;
        }
    }

    internal class SceneLoaderMock : SceneLoader
    {
        public string EntityCreated = "";

        public SceneLoaderMock(GameLoader gameLoader) : base(gameLoader) { }

        internal override void CreateEntity(string name)
        {
            EntityCreated = name;
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
        public bool Destroyed = false;
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
            Destroyed = true;
        }
    }

    public class ManagerMock : Manager<Component>
    {
        [Plugin("", "")]
        public string Value { get; set; }
    }

    [Requires(typeof(TransformComponent))]
    public class RequiresTransformComponent : Component
    {
        public bool Destroyed = false;
        public override void Destroy()
        {
            Destroyed = true;
        }
    }

    [Provides(typeof(TransformComponent))]
    public class IllegalProvidesComponent : Component
    {
        public override void Destroy() { }
    }

    [Provides(typeof(TransformComponent))]
    public class GoodProvidesComponent : TransformComponent
    {
        public override void Destroy() { }
    }

    public class MockServiceToRun : Service
    {
        public bool Started = false;
        public bool Stopped = false;

        public override void OnStart()
        {
            Started = true;
        }

        public override void OnStop()
        {
            Stopped = true;
        }

        public override bool AutoStart()
        {
            return true;
        }
    }

    public class MockServiceNotToRun : MockServiceToRun
    {
        public override bool AutoStart()
        {
            return false;
        }
    }

    public class ExpressionMock : IExpressionReader
    {
        public int NumNotified = 0;

        private string value;

        public ExpressionMock(string value)
        {
            this.value = value;
        }

        public string GetValue()
        {
            return value;
        }

        public void notifyOfChange(System.Action<string> callback)
        {
            NumNotified++;
        }
    }

    public class WriterMock : IValueWriter
    {
        public string Value { get; set; }
    }

    public class DoubleMock : IDoubleExpressionReader
    {
        private readonly double val;
        public DoubleMock(double value)
        {
            val = value;
        }
        public double GetValue()
        {
            return val;
        }
    }

}