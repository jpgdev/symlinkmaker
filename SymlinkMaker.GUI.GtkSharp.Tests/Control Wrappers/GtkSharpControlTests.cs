using Moq;
using NUnit.Framework;
using SymlinkMaker.GUI.GTKSharp;
using Gtk;
using System;

namespace SymlinkMaker.GUI.GtkSharp.Tests
{
    [TestFixture]
    public class GtkSharpControlTests
    {
        #region Fields

        private Mock<Widget> _widgetMock;
        private GtkSharpControlMock _control;

        #endregion

        #region Fake GtkSharpControl implementing class

        private class GtkSharpControlMock : GtkSharpControl
        {
            public GtkSharpControlMock(Widget widget)
                : base(widget)
            {
            }
        }

        #endregion

        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _widgetMock = new Mock<Widget>() { CallBase = true };
            _control = new GtkSharpControlMock(_widgetMock.Object);
        }

        [TearDown]
        public void AfterEachTearDown()
        {
            _widgetMock.Reset();
        }

        #endregion

        #region Tests

        [Test]
        public void Constructor_WithoutAValidWidget_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf<ArgumentNullException>()
                .And.Property("Message").Contains("widget"),
                () => new GtkSharpControlMock(null)
            );
        }

        [Test]
        public void Tooltip_WhenSet_ShouldSetWidgetTooltipText()
        {
            const string newTooltip = "This is the new tooltip text";

            _control.Tooltip = newTooltip;

            Assert.AreEqual(newTooltip, _widgetMock.Object.TooltipText);
        }

        [Test]
        public void Tooltip_WhenGet_ShouldGetWidgetTooltipText()
        {
            const string newTooltip = "This is the new tooltip text";

            _widgetMock.Object.TooltipText = newTooltip;

            Assert.AreEqual(newTooltip, _control.Tooltip);
        }

        #endregion

    }
}


