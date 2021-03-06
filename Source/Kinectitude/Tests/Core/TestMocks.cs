//-----------------------------------------------------------------------
// <copyright file="TestMocks.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

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
    public enum TestEnum { value1, value2, value3 }

    [Plugin("Assertion", "")]
    public class AssertionAction : Action
    {
        public static readonly Dictionary<string, List<bool>> Assertions = new Dictionary<string, List<bool>>();
        [PluginProperty("ls", "")]
        public ValueReader LS { get; set; }
        [PluginProperty("rs", "")]
        public ValueReader RS { get; set; }
        [PluginProperty("assertion string", "")]
        public string Assertion { get; set; }
        [PluginProperty("same type needed", "")]
        public bool Type { get; set; }
        [PluginProperty("Run iff matches", "")]
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
        [PluginProperty("Matcher", "")]
        public TypeMatcher MatcherVal { get; set; }

        public override void OnInitialize()
        {
            GetManager<MatcherManager>().Events.Add(this);
        }
    }

    [Plugin("MockComponent", "")]
    public class MockComponent : Component
    {
        private int intVal;
        [PluginProperty("int", "", 10)]
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
        [PluginProperty("double", "", 10.1)]
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
        [PluginProperty("string", "", "ROFL")]
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
        [PluginProperty("bool", "", true)]
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
        TestEnum enumVal;
        [PluginProperty("enum", "", TestEnum.value3)]
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
        ValueReader readerVal;
        [PluginProperty("reader", "", true)]
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
        [PluginProperty("Matcher", "")]
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
        [PluginProperty("bool", "")]
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
        [PluginProperty("test", "test")]
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

        [PluginProperty("setVal", "")]
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

        [PluginProperty("setVal", "")]
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

        [PluginProperty("setVal", "")]
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

    [Plugin("FuncHolder", "function holder")]
    public class FuncHolder
    {
        [Plugin("function", "test")]
        public static int NumArgs() { return 0; }

        [Plugin("function", "test")]
        public static int NumArgs(ValueReader vr) { return 1; }

        [Plugin("function", "test")]
        public static int NumArgs(ValueReader vr, ValueReader vr2, ValueReader vr3) { return 3; }

        [Plugin("function", "test")]
        public static string NumArgs(params ValueReader [] vrs) { return "params 0"; }

        [Plugin("function", "test")]
        public static string NumArgs(ValueReader vr, ValueReader vr2, ValueReader vr3, ValueReader vr4, ValueReader vr5, params ValueReader [] vrs) { return "params 5"; }
    }
}
