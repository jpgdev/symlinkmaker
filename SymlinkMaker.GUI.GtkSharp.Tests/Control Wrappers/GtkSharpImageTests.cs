using Moq;
using NUnit.Framework;
using SymlinkMaker.GUI.GTKSharp;
using Gtk;
using System;

namespace SymlinkMaker.GUI.GtkSharp.Tests
{
    [TestFixture]
    public class GtkSharpImageTests
    {
        // FIXME : In unit tests the Stetic.IconLoader.LoadIcon throws a 
        //         NullReferenceException in IconLoader at the line:
        //            Gdk.Pixmap pmap = new Gdk.Pixmap (Gdk.Screen.Default.RootWindow, sz, sz);
        //          
        //         So I can't instanciate a GtkSharpImage for now, since it calls
        //         it in the constructor

        #region Fields

        private const string DEFAULT_IMAGE_NAME = "yes";

        private Mock<Image> _imageMock;
        private Mock<IGtkIconNameConverter> _nameConverter;
        private GtkSharpImage _image;

        #endregion

        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _imageMock = new Mock<Image>()
            { 
                CallBase = true 
            };
            _imageMock.SetupAllProperties();

            _nameConverter = new Mock<IGtkIconNameConverter>();
            SetupNameConverter();

//            _image = new GtkSharpImage(
//                _imageMock.Object,
//                DEFAULT_IMAGE_NAME,
//                _nameConverter.Object
//            );
        }

        void SetupNameConverter()
        {
            _nameConverter.SetupAllProperties();

            // NAME -> GTK_NAME
            _nameConverter
                .Setup(c => c.GetGtkNameFromImageName("yes"))
                .Returns("gtk-yes");
            _nameConverter
                .Setup(c => c.GetGtkNameFromImageName("no"))
                .Returns("gtk-no");
        
            // GTK_NAME -> NAME
            _nameConverter
                .Setup(c => c.GetImageNameFromGtkName("gtk-yes"))
                .Returns("yes");
            _nameConverter
                .Setup(c => c.GetImageNameFromGtkName("gtk-no"))
                .Returns("no");
        }

        [TearDown]
        public void AfterEachTearDown()
        {
            _nameConverter.Reset();
            SetupNameConverter();

            _imageMock.Reset();
        }

        #endregion

        #region Tests

        [Test]
        public void Constructor_WithoutAName_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf<ArgumentNullException>()
                .And.Property("Message").Contains("name"),
                () => new GtkSharpImage(
                    _imageMock.Object,
                    null,
                    _nameConverter.Object
                )
            );
        }

        [Test]
        public void Constructor_WithoutIconNameConverter_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf<ArgumentNullException>()
                .And.Property("Message").Contains("iconNameConverter"),
                () => new GtkSharpImage(
                    _imageMock.Object,
                    "pathName",
                    null
                )
            );
        }

        [Test, Ignore]
        public void Constructor_ShouldSetImagePixbuf()
        {
            Assert.IsNotNull(_imageMock.Object.Pixbuf);
        }

        [Test, Ignore]
        public void Path_WhenGet_ShouldReturnImageName()
        {
            Assert.AreEqual(DEFAULT_IMAGE_NAME, _image.Path);
        }

        [Test, Ignore]
        public void Path_WhenSet_ShouldSetImagePixbuf()
        {
            _imageMock.Object.Pixbuf = null;

            _image.Path = "no";

            Assert.IsNotNull(_imageMock.Object.Pixbuf);
        }

        #endregion
    }
}

