using Moq;
using NUnit.Framework;
using SymlinkMaker.GUI.GtkSharp;
using Gtk;
using System;

namespace SymlinkMaker.GUI.GtkSharp.Tests
{
    [TestFixture]
    public class GtkSharpToggleButtonTests
    {
        private Mock<ToggleButton> _toggleButtonMock;

        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _toggleButtonMock = new Mock<ToggleButton>()
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
            var button = new GtkSharpToggleButton(_toggleButtonMock.Object);
            button.StatusChanged += (btn, eArgs) => called = true;

            _toggleButtonMock.Object.Toggle();

            Assert.IsTrue(called);
        }

        [Test]
        public void Dispose_ShouldUnsubscribeToToggledEvent()
        {
            var called = false;
            var button = new GtkSharpToggleButton(_toggleButtonMock.Object);
            button.StatusChanged += (btn, eArgs) => called = true;

            button.Dispose();
            _toggleButtonMock.Object.Toggle();

            Assert.IsFalse(called);
        }

        [Test]
        public void Toggle_ShouldCallBaseWidgetToggle()
        {
            var called = false;
            var button = new GtkSharpToggleButton(_toggleButtonMock.Object);
            _toggleButtonMock.Object.Toggled += (btn, eArgs) => called = true;

            button.Toggle();

            Assert.IsTrue(called);
        }

        [Test]
        public void IsActive_WhenSet_ShouldTriggerToggled()
        {
            var called = false;
            var button = new GtkSharpToggleButton(_toggleButtonMock.Object);
            _toggleButtonMock.Object.Toggled += (btn, eArgs) => called = true;

            button.IsActive = !button.IsActive;

            Assert.IsTrue(called);
        }

        [Test]
        public void Text_WhenGet_ShouldReturnWidgetText()
        {
            var button = new GtkSharpToggleButton(_toggleButtonMock.Object);
            button.IsActive = !button.IsActive;
            Assert.AreEqual(_toggleButtonMock.Object.Active, button.IsActive);
        }

    }
}

