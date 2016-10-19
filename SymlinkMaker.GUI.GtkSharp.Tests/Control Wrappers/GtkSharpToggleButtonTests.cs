using Moq;
using NUnit.Framework;
using SymlinkMaker.GUI.GTKSharp;
using Gtk;
using System;

namespace SymlinkMaker.GUI.GtkSharp.Tests
{
    [TestFixture]
    public class GtkSharpToggleActionTests
    {
        private Mock<ToggleAction> _toggleButtonMock;

        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _toggleButtonMock = new Mock<ToggleAction>("Name", "Label", "", "" )
            { 
                CallBase = true 
            };
        }

        [TearDown]
        public void AfterEachTearDown()
        {
            _toggleButtonMock.Reset();
        }

        #endregion

        [Test]
        public void Constructor_WithValidBaseButton_ShouldSubscribeToToggledEvent()
        {
            var called = false;
            var action = new GtkSharpToggleAction(_toggleButtonMock.Object);
            action.StatusChanged += (btn, eArgs) => called = true;

            _toggleButtonMock.Object.Toggle();

            Assert.IsTrue(called);
        }

        [Test]
        public void Dispose_ShouldUnsubscribeToToggledEvent()
        {
            var called = false;
            var action = new GtkSharpToggleAction(_toggleButtonMock.Object);
            action.StatusChanged += (btn, eArgs) => called = true;

            action.Dispose();
            _toggleButtonMock.Object.Toggle();

            Assert.IsFalse(called);
        }

        [Test]
        public void Toggle_ShouldCallBaseWidgetToggle()
        {
            var called = false;
            var action = new GtkSharpToggleAction(_toggleButtonMock.Object);
            _toggleButtonMock.Object.Toggled += (btn, eArgs) => called = true;

            action.Toggle();

            Assert.IsTrue(called);
        }

        [Test]
        public void IsActive_WhenSet_ShouldTriggerToggled()
        {
            var called = false;
            var action = new GtkSharpToggleAction(_toggleButtonMock.Object);
            _toggleButtonMock.Object.Toggled += (btn, eArgs) => called = true;

            action.IsActive = !action.IsActive;

            Assert.IsTrue(called);
        }

        [Test]
        public void Text_WhenGet_ShouldReturnWidgetText()
        {
            var action = new GtkSharpToggleAction(_toggleButtonMock.Object);
            action.IsActive = !action.IsActive;
            Assert.AreEqual(_toggleButtonMock.Object.Active, action.IsActive);
        }
    }
}

