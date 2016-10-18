using Moq;
using NUnit.Framework;
using SymlinkMaker.GUI.GTKSharp;

namespace SymlinkMaker.GUI.GtkSharp.Tests
{
    [TestFixture]
    public class GtkIconNameConverterTests
    {
        private GtkIconNameConverter _converter;

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _converter = new GtkIconNameConverter();
        }

        [Test]
        [TestCase("yes", "gtk-yes")]
        [TestCase("no", "gtk-no")]
        public void FromGtk_ToGtkConversion_ShouldWork(
            string name, 
            string gtkName
        )
        {
            // NOTE: This is done just to validate that all the cases required are available
            Assert.AreEqual(_converter.GetGtkNameFromImageName(name), gtkName);
            Assert.AreEqual(_converter.GetImageNameFromGtkName(gtkName), name);
        }
    }
}
