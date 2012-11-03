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

namespace Kinectitude.Tests.Core.TestMocks
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
            if (!Assertions.TryGetValue(value, out assertionList)) Assert.Fail("The assertion " + value + " was not run");

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

                if (Type) assertionList.Add(LS.hasSameVal(RS));
                else assertionList.Add(LS.GetStrValue() == RS.GetStrValue());
            }
        }

        public AssertionAction()
        {
            Type = true;
        }
    }

    [Plugin("MockComponent", "")]
    public class MockComponent : Component
    {
        [Plugin("int", "")]
        public int IntVal { get; set; }
        [Plugin("double", "")]
        public double DoubleVal { get; set; }
        [Plugin("string", "")]
        public string StrVal { get; set; }
        [Plugin("boool", "")]
        public bool BoolVal { get; set; }
        [Plugin("enum", "")]
        public TestEnum EnumVal { get; set; }
        [Plugin("Reader", "")]
        public ValueReader ReaderVal { get; set; }
        [Plugin("Writer", "")]
        public ValueWriter WriterVal { get; set; }
        [Plugin("Matcher", "")]
        public TypeMatcher Matcher { get; set; }
        public bool DestroyedCalled = false;
        public MockComponent()
        {
            DoubleVal = IntVal = 0;
            StrVal = "";
            BoolVal = false;
            EnumVal = TestEnum.value2;
            ReaderVal = null;
            WriterVal = null;
            Matcher = null;
        }
        public override void Destroy() { DestroyedCalled = true; }
    }

    //Have a second component to test with
    [Plugin("MockComponent2", "")]
    public class MockComponent2 : MockComponent { }

    [Plugin("MockRequiersComponent", "")]
    [Requires(typeof(TransformComponent))]
    public class MockRequiersComponent : Component
    {
        public override void Destroy() { }
    }
}