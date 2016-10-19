using NUnit.Framework;
using SymlinkMaker.GUI.GTKSharp;
using Gtk;
using System;

namespace SymlinkMaker.GUI.GtkSharp.Tests
{
    [TestFixture]
    public class GtkSharpWindowTests
    {
        [Test]
        public void Constructor_ShouldSubscribeToDestroyedEvent()
        {
            var called = false;
            var window = new GtkSharpWindow(WindowType.Toplevel);
            window.Closed += (btn, eArgs) => called = true;

            window.Destroy();

            Assert.IsTrue(called);
        }

        [Test]
        public void Dispose_ShouldUnsubscribeToDestroyedevent()
        {
            var called = false;
            var window = new GtkSharpWindow(WindowType.Toplevel);
            window.Closed += (btn, eArgs) => called = true;
            window.Dispose();

            window.Destroy();

            Assert.IsFalse(called);
        }

        [Test]
        public void Close_ShouldCallDestroy()
        {
            var called = false;
            var window = new GtkSharpWindow(WindowType.Toplevel);
            window.Destroyed += (btn, eArgs) => called = true;

            window.Close();

            Assert.IsTrue(called);
        }
    }
}

