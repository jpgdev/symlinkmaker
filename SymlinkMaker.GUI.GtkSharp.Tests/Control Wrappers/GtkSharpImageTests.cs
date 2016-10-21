using Moq;
using NUnit.Framework;
using SymlinkMaker.GUI.GtkSharp;
using Gtk;
using System;
using Gdk;
using Image = Gtk.Image;

namespace SymlinkMaker.GUI.GtkSharp.Tests
{
    [TestFixture]
    public class GtkSharpImageTests
    {
        #region Fields

        private const string DEFAULT_IMAGE_NAME = "yes";


        private Pixbuf _gtkYesPixbuf;
        private Pixbuf _gtkNoPixbuf;
        private Mock<Image> _imageMock;
        private Mock<IGtkIconNameConverter> _nameConverter;
        private Mock<IIconLoader<Pixbuf>> _iconLoader;
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

            _gtkYesPixbuf = new Pixbuf(
                new byte[] { 255, 0, 0, 255 },
                true, 1, 1, 1, 4);

            _gtkNoPixbuf = new Pixbuf(
                new byte[] { 0, 255, 0, 255 },
                true, 1, 1, 1, 4);
            
            _iconLoader = new Mock<IIconLoader<Pixbuf>>();
            SetupIconLoader();

            _nameConverter = new Mock<IGtkIconNameConverter>();
            SetupNameConverter();

            _image = new GtkSharpImage(
                _imageMock.Object,
                DEFAULT_IMAGE_NAME,
                _nameConverter.Object,
                _iconLoader.Object
            );
        }

        private void SetupNameConverter()
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

        private void SetupIconLoader()
        {
            _iconLoader.SetupAllProperties();

            _iconLoader
                .Setup(c => c.Load("gtk-yes"))
                .Returns(_gtkYesPixbuf);
            
            _iconLoader
                .Setup(c => c.Load("gtk-no"))
                .Returns(_gtkNoPixbuf);
        }

        [TearDown]
        public void AfterEachTearDown()
        {
            _iconLoader.ResetCalls();
            _nameConverter.ResetCalls();
            _imageMock.ResetCalls();
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
                    _nameConverter.Object,
                    _iconLoader.Object
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
                    null,
                    _iconLoader.Object
                )
            );
        }

        [Test]
        public void Constructor_WithoutIconLoader_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf<ArgumentNullException>()
                .And.Property("Message").Contains("iconLoader"),
                () => new GtkSharpImage(
                    _imageMock.Object,
                    "pathName",
                    _nameConverter.Object,
                    null
                )
            );
        }

        [Test, Ignore("There is a bug when assigning my 'fake' pixbuf, but it actually tries to set it")]
        public void Constructor_ShouldSetImagePixbuf()
        {
            Assert.IsNotNull(_imageMock.Object.Pixbuf);
        }

        [Test]
        public void Path_WhenGet_ShouldReturnImageName()
        {
            Assert.AreEqual(DEFAULT_IMAGE_NAME, _image.Name);
        }

        [Test, Ignore("There is a bug when assigning my 'fake' pixbuf, but it actually tries to set it")]
        public void Path_WhenSet_ShouldSetImagePixbuf()
        {
            _imageMock.Object.Pixbuf = null;
      
            _image.Name = "no";
        
            Assert.IsNotNull(_imageMock.Object.Pixbuf);
        }

        #endregion
    }
}

