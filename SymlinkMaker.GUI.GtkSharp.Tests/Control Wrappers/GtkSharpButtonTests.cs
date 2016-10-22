using Moq;
using NUnit.Framework;
using SymlinkMaker.GUI.GtkSharp;
using Gtk;
using System;

namespace SymlinkMaker.GUI.GtkSharp.Tests
{
    [TestFixture]
    public class GtkSharpButtonTests
    {
        private Mock<Button> _buttonMock;

        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _buttonMock = new Mock<Button>()
            { 
                CallBase = true 
            };
        }

        [TearDown]
        public void AfterEachTearDown()
        {
            _buttonMock.Reset();
        }

        #endregion

        [Test]
        public void Constructor_WithValidBaseButton_ShouldSubscribeToClickedEvent()
        {
            var called = false;
            var button = new GtkSharpButton(_buttonMock.Object);
            button.Triggered += (btn, eArgs) => called = true;

            _buttonMock.Object.Click();

            Assert.IsTrue(called);
        }

        [Test]
        public void Dispose_ShouldUnsubscribeToClickedEvent()
        {
            var called = false;
            var button = new GtkSharpButton(_buttonMock.Object);
            button.Triggered += (btn, eArgs) => called = true;

            button.Dispose();
            _buttonMock.Object.Click();

            Assert.IsFalse(called);
        }

        [Test]
        public void Trigger_ShouldCallBaseWidgetClick()
        {
            var called = false;
            var button = new GtkSharpButton(_buttonMock.Object);
            _buttonMock.Object.Clicked += (btn, eArgs) => called = true;

            button.Trigger();

            Assert.IsTrue(called);
        }
    }
}

