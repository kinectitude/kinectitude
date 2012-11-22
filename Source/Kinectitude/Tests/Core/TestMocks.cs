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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Core.Managers;

namespace Kinectitude.Tests.Core
{
    public enum TestEnum { value1, value2 }

    [Plugin("Assertion", "")]
    public class AssertionAction : Action
    {
        public static readonly Dictionary<string, List<bool>> Assertions = new Dictionary<string, List<bool>>();
        [Plugin("ls", "")]
        public ValueReader LS { get; set; }
        [Plugin("rs", "")]
        public ValueReader RS { get; set; }
        [Plugin("assertion string", "")]
        public string Assertion { get; set; }
        [Plugin("same type needed", "")]
        public bool Type { get; set; }
        [Plugin("Run iff matches", "")]
        public ValueReader OnlyRunIf { get; set; }

        public static void CheckValue(string value, int expectedRuns = 1)
        {
            List<bool> assertionList;
            if (!Assertions.TryGetValue(value, out assertionList))
            {
                if(expectedRuns != 0) Assert.Fail("The assertion " + value + " was not run");
                return;
            }

            if (assertionList.Count != expectedRuns)
                Assert.Fail("The assertion " + value + " was run " + assertionList.Count + " but expected to be run " + expectedRuns);

            for (int i = 0; i < assertionList.Count; i++)
            {
                if (!assertionList[i]) Assert.Fail("The assertion " + value + " failed on run " + (i + 1));
            }
        }

        public override void Run()
        {
            if(OnlyRunIf.GetBoolValue())
            {
                List<bool> assertionList;
                if (!Assertions.TryGetValue(Assertion, out assertionList))
                {
                    assertionList = new List<bool>();
                    Assertions[Assertion] = assertionList;
                }

                if (Type) assertionList.Add(LS.HasSameVal(RS));
                else assertionList.Add(LS.GetStrValue() == RS.GetStrValue());
            }
        }

        public AssertionAction()
        {
            Type = true;
        }
    }

    public class MatcherManager : Manager<MockMatchComponent>
    {
        public readonly List<MockMatchHappensEvent> Events = new List<MockMatchHappensEvent>();
        public override void OnUpdate(float frameDelta)
        {
            foreach (MockMatchHappensEvent evt in Events)
            {
                foreach(MockMatchComponent child in Children)
                {
                    if (evt.MatcherVal.MatchAndSet(child.IEntity)) evt.DoActions();
                }
            }
        }
    }

    public class MockMatchComponent : Component
    {
        public override void Ready()
        {
            GetManager<MatcherManager>().Add(this);
        }

        public override void Destroy() { }
    }

    public class MockMatchHappensEvent : Event
    {
        [Plugin("Matcher", "")]
        public TypeMatcher MatcherVal { get; set; }

        public override void OnInitialize()
        {
            GetManager<MatcherManager>().Events.Add(this);
        }
    }

    [Plugin("MockComponent", "")]
    public class MockComponent : Component
    {
        private int intVal = 0;
        [Plugin("int", "")]
        public int IntVal { 
            get { return intVal; }
            set
            {
                if (intVal != value)
                {
                    intVal = value;
                    Change("IntVal");
                }
            }
        }
        double doubleVal = 0;
        [Plugin("double", "")]
        public double DoubleVal
        {
            get { return doubleVal; }
            set
            {
                if(doubleVal != value)
                {
                    doubleVal = value;
                    Change("DoubleVal");
                }
            }
        }
        string strVal = "";
        [Plugin("string", "")]
        public string StrVal
        {
            get { return strVal; }
            set
            {
                if (strVal != value)
                {
                    strVal = value;
                    Change("StrVal");
                }
            }
        }
        bool boolVal = false;
        [Plugin("bool", "")]
        public bool BoolVal
        {
            get { return boolVal; }
            set
            {
                if (boolVal != value)
                {
                    boolVal = value;
                    Change("BoolVal");
                }
            }
        }
        TestEnum enumVal = TestEnum.value1;
        [Plugin("enum", "")]
        public TestEnum EnumVal
        {
            get { return enumVal; }
            set
            {
                if(enumVal != value)
                {
                    enumVal = value;
                    Change("EnumVal");
                }
            }
        }
        ValueReader readerVal = null;
        [Plugin("reader", "")]
        public ValueReader ReaderVal
        {
            get { return readerVal; }
            set
            {
                if (readerVal != value)
                {
                    readerVal = value;
                    Change("ReaderVal");
                }
            }
        }

        TypeMatcher matcherVal = null;
        [Plugin("Matcher", "")]
        public TypeMatcher MatcherVal
        {
            get { return matcherVal; }
            set
            {
                if (matcherVal != value)
                {
                    matcherVal = value;
                    Change("MatcherVal");
                }
            }
        }
        public bool DestroyedCalled = false;
        public override void Destroy() { DestroyedCalled = true; }
    }

    //Have a second component to test with
    [Plugin("MockComponent2", "")]
    public class MockComponent2 : MockComponent { }

    [Plugin("Manager", "")]
    public class MockManager : Manager<SubscribeComponent>
    {
        [Plugin("bool", "")]
        public bool BoolVal { get; set; }
        public static int Added = 0;
        public override void OnUpdate(float frameDelta) { }
        public MockManager() { BoolVal = false; }
        protected override void OnAdd(SubscribeComponent item)
        {
            Added++;
        }
    }

    [Plugin("Component register", "")]
    public class SubscribeComponent : Component
    {

        public override void Destroy() { }
        public override void Ready()
        {
            GetManager<MockManager>().Add(this);
        }
    }

    [Plugin("MockRequiersComponent", "")]
    [Requires(typeof(TransformComponent))]
    public class MockRequiersComponent : Component
    {
        [Plugin("test", "test")]
        public bool ShouldCheckNulls { get; set; }
        public override void Ready()
        {
            if (ShouldCheckNulls)
            {
                Assert.IsNotNull(GetComponent<TransformComponent>());
                Assert.IsNotNull(GetManager<TimeManager>());
            }
        }

        public override void Destroy() { }
    }

    [Plugin("InvalidProvides", "")]
    [Provides(typeof(TransformComponent))]
    public class InvalidProvidesComponent : Component
    {
        public override void Destroy() { }
    }

    [Plugin("Service test", "")]
    public class ServiceNoAuto : Service
    {
        public static int started = 0;
        public static int stopped = 0;
        public static int setVal = 0;

        [Plugin("setVal", "")]
        public int SetVal
        {
            get { return setVal; }
            set { setVal = value; }
        }

        public override void OnStart() { started++; }
        public override void OnStop() { stopped++; }
        public override bool AutoStart() { return false; }
    }

    [Plugin("Service test", "")]
    public class ServiceAuto : Service
    {
        public static int setVal = 0;

        [Plugin("setVal", "")]
        public int SetVal
        {
            get { return setVal; }
            set { setVal = value; }
        }

        public static int started = 0;
        public static int stopped = 0;

        public override void OnStart() { started++; }
        public override void OnStop() { stopped++; }
        public override bool AutoStart() { return true; }
    }


    [Plugin("Service test", "")]
    public class ServiceAutoSelfStop : Service
    {
        public static int started = 0;
        public static int stopped = 0;

        public static int setVal = 0;

        [Plugin("setVal", "")]
        public int SetVal
        {
            get { return setVal; }
            set { setVal = value; }
        }

        public override void OnStart() 
        { 
            started++;
            Stop();
        }
        public override void OnStop() { stopped++; }
        public override bool AutoStart() { return true; }
    }
}