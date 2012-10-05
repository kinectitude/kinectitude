using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using Kinectitude.Core.Data;
using Kinectitude.Core.Loaders;
using Action = Kinectitude.Core.Base.Action;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Collections;

namespace Kinectitude.Tests.Core.TestMocks
{

    public class GameLoaderMock : GameLoader
    {
        internal SceneLoader mockScene = null;

        public GameLoaderMock() : base("test.xml", new Assembly[] { }, 1, 1, new Func<Tuple<int,int>>(() => new Tuple<int,int>(0,0))) { }

        public void addType(string name)
        {
            PrototypeIs.Add(name, new List<string>() { name });
        }

    }

    internal class SceneLoaderMock : SceneLoader
    {
        GameLoaderMock glm;
        LoaderUtility lu;

        public SceneLoaderMock(GameLoader gameLoader, LoaderUtility lu) : base("a", null, lu, gameLoader) 
        {
            glm = gameLoader as GameLoaderMock;
            this.lu = lu;
        }

        public void callPrototypeMaker(string name)
        {
            if (glm != null) glm.addType(name);
            glm.Prototypes[name] = 10;
            PrototypeMaker(name);
        }

    }

    internal class LoaderUtilityMock : LoaderUtility
    {

        bool loaded = false;

        internal override object Load()
        {
            loaded = true;
            return null;
        }

        public List<Tuple<string, string>> Values { get; set; }

        internal override List<Tuple<string, string>> GetValues(object from, HashSet<string> ignore)
        {
            return Values;
        }

        public Dictionary<string, string> Properties { get; set; }

        internal override Dictionary<string, string> GetProperties(object from, HashSet<string> specialWords)
        {
            return Properties;
        }

        internal override IEnumerable<object> GetOfType(object entity, object type)
        {
            return new EnumerableMock<object>();
        }

        internal override IEnumerable<object> GetAll(object evt)
        {
            return null;
        }

        internal override bool IsAciton(object obj)
        {
            return true;
        }

        internal override void CreateWithPrototype(GameLoader gl, string name, ref object entity, int id) { }

        internal override void MergePrototpye(ref object newPrototype, string myName, string mergeWith) { }

        public LoaderUtilityMock()
        {
            Lang = new EnglishLanguageKeywords();
        }
    }

    public class EnumerableMock<T> : IEnumerable<T>
    {

        public IEnumerator<T> GetEnumerator()
        {
            return new EMock<T>();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class EMock<T> : IEnumerator<T>
    {

        public T Current
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose() { }

        public bool MoveNext(){return false;}

        public void Reset() { }

        object IEnumerator.Current
        {
            get { throw new NotImplementedException(); }
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
        public bool HasRun { get; set; }
        public override void Run() { HasRun = true; }
    }

    public class EventMock : Event
    {
        [Plugin("", "")]
        public ITypeMatcher TypeMatcher { get; set; }
        [Plugin("", "")]
        public IExpressionReader Expression { get; set; }
        [Plugin("", "")]
        public string Sexpr { get; set; }
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

        public bool Updated = false;

        public override void OnUpdate(float frameDelta) { Updated = true; }
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

    public class ExpressionEventMock : Event
    {
        [Plugin("", "")]
        public IDoubleExpressionReader Dexpr { get; set; }

        [Plugin("", "")]
        public double Dbl { get; set; }


        public override void OnInitialize() { }
    }

    public class GetManagerAction : Action
    {
        public override void Run()
        {
            ManagerMock m = GetManager<ManagerMock>();
        }
    }

}