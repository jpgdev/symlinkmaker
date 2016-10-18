using Moq;
using NUnit.Framework;
using SymlinkMaker.GUI.GTKSharp;
using Gtk;
using System;
using NUnit.Framework.Constraints;

namespace SymlinkMaker.GUI.GtkSharp.Tests
{
    [TestFixture]
    public class GtkSharpButtonTests
    {
        private Mock<Button> _buttonMock;
//        private Widget _buttonFake;
        //        private GtkSharpButton _button;

        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _buttonMock = new Mock<Button>();

//            _buttonFake = new Button();

//            _button = new GtkSharpButton(
//                _buttonMock.Object
//            );
        }

//        [TestFixtureTearDown]
//        public void AfterAllTearDown()
//        {
//        }
//
//        [SetUp]
//        public void BeforeEachSetUp()
//        {
//        }
//
//        [TearDown]
//        public void AfterEachTearDown()
//        {
//        }

        #endregion

        [Test, Ignore]
        public void Constructor_WithValidBaseButton_ShouldSubscribeToClickedEvent()
        {
            var called = false;
            var button = new GtkSharpButton(
                             _buttonMock.Object);

            button.Triggered += (btn, eArgs) => called = true;

            _buttonMock.Raise(
                b => b.Clicked += null, 
                _buttonMock.Object,
                EventArgs.Empty
            );

            Assert.IsTrue(called);
        }
    }
}

