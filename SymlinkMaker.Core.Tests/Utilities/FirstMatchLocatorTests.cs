using System;
using NUnit.Framework;

namespace SymlinkMaker.Core.Tests
{
    [TestFixture]
    public class FirstMatchLocatorTests
    {
        [Test]
        public void Get_WithAValidCondition_ShouldReturnTheFirstMatch()
        {
            var messages = new []
            {
                "Smaller than 5",
                "Between 5 & 10",
                "Between 10 & 15",
                "Bigger than 15"
            };

            var depLoader = new FirstMatchLocator<int, string>();
            depLoader.Register(value => value < 5, messages[0]);
            depLoader.Register(value => value < 10, messages[1]);
            depLoader.Register(value => value < 15, messages[2]);
            depLoader.Register(value => value > 15, messages[3]);

            string message = depLoader.Get(9);

            Assert.AreEqual(message, messages[1]);
        }

        [Test]
        public void Get_WithAnInvalidCondition_ShouldThrowAnExeption()
        {
            var depLoader = new FirstMatchLocator<int, string>();
            depLoader.Register(value => value < 5, "A string");

            Assert.Throws(
                Is.TypeOf(typeof(InvalidOperationException))
                    .And.Property("Message")
                    .Contains("Sequence contains no matching element"),
                () => depLoader.Get(10)
            );
        }
    }
}

