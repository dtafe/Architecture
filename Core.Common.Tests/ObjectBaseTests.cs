using Core.Common.Tests.Test_Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Tests
{
    [TestClass]
    public class ObjectBaseTests
    {
        public void test_clean_property_change()
        {
            TestClass objTest = new TestClass();
            bool propertyChange = false;
            objTest.PropertyChanged += (s, e) =>
                 { if (e.PropertyName == "CleanProp") propertyChange = true; };
            objTest.CleanProp = "test value";
            Assert.IsTrue(propertyChange, "The property should have triggered change notification.");
        }
        [TestMethod]
        public void test_dirty_set()
        {
            TestClass objTest = new TestClass();

            Assert.IsFalse(objTest.IsDirty, "Object should be clean.");

            objTest.DirtyProp = "test value";

            Assert.IsTrue(objTest.IsDirty, "Object should be dirty.");
        }

        [TestMethod]
        public void test_property_change_single_subscription()
        {
            TestClass objTest = new TestClass();
            int changeCounter = 0;
            PropertyChangedEventHandler handler1 = new PropertyChangedEventHandler((s, e) => { changeCounter++; });
            PropertyChangedEventHandler handler2 = new PropertyChangedEventHandler((s, e) => { changeCounter++; });

            objTest.PropertyChanged += handler1;
            objTest.PropertyChanged += handler1;    //should not duplicate
            objTest.PropertyChanged += handler1;    //should not duplicate
            objTest.PropertyChanged += handler2;
            objTest.PropertyChanged += handler2;   //should not duplicate

            objTest.CleanProp = "test value";

            Assert.IsTrue(changeCounter == 2, "Property change notifcation should only have 2 notifications.");

        }

        [TestMethod]
        public void test_object_validation()
        {
            TestClass objTest = new TestClass();

            Assert.IsFalse(objTest.IsValid, "Object should not be valid as one of its rules has not been set.");

            objTest.StringProp = "some value";

            Assert.IsTrue(objTest.IsValid, "Object should be valid as its property has been validated to be set.");

        }
    }
}
