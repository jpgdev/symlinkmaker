using Moq;
using NUnit.Framework;
using SymlinkMaker.GUI.GTKSharp;
using Gtk;
using System;

namespace SymlinkMaker.GUI.GtkSharp.Tests
{
    [TestFixture]
    public class GtkSharpTextEntryTests
    {
        #region Fields

        private const string DEFAULT_VALUE = "DEFAULT_VALUE";

        private Mock<Entry> _entryMock;
        private GtkSharpTextEntry _textEntry;

        #endregion

        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _entryMock = new Mock<Entry>()
            { 
                CallBase = true 
            };
            _textEntry = new GtkSharpTextEntry(_entryMock.Object);
        }

        [TearDown]
        public void AfterEachTearDown()
        {
            _entryMock.Object.Text = DEFAULT_VALUE;
            _entryMock.Reset();
        }

        #endregion

        #region Tests

        [Test]
        public void Constructor_WithValidBaseEntry_ShouldSubscribeToChangedEvent()
        {
            var called = false;
            _textEntry.TextChanged += (btn, eArgs) => called = true;

            _entryMock.Object.Text = "new text";

            Assert.IsTrue(called);
        }

        [Test]
        public void Dispose_ShouldUnsubscribeToChangedEvent()
        {
            var called = false;
            _textEntry.TextChanged += (btn, eArgs) => called = true;

            _textEntry.Dispose();

            _entryMock.Object.Text = "new text";

            Assert.IsFalse(called);
        }

        [Test]
        public void Text_WhenSet_ShouldTriggerTextChanged()
        {
            var called = false;
            _entryMock.Object.Changed += (btn, eArgs) => called = true;

            _textEntry.Text = "new text";

            Assert.IsTrue(called);
        }

        [Test]
        public void Text_WhenGet_ShouldReturnWidgetText()
        {
            _textEntry.Text = "RANDOM NEW TEXT";
            Assert.AreEqual(_entryMock.Object.Text, _textEntry.Text);
        }

        #endregion
    }
}

