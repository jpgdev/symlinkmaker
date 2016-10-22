using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using SymlinkMaker.Core;

namespace SymlinkMaker.CLI.Tests
{

    [TestFixture]
    public class CLICommandParserTests
    {
        private CLICommandParser _commandParser;

        #region Constants

        private const string SOURCE_PATH_ARG_NAME = "sourcePath";
        private const string TARGET_PATH_ARG_NAME = "targetPath";

        private const string SOURCE_PATH = "/source/path/location";
        private const string TARGET_PATH = "/target/path/location";

        private static readonly string[] CONFIRM_FLAGS = { "-c", "--c", "--confirm", "-confirm" };
        private static readonly string[] NO_CONFIRM_FLAGS = { "-n", "--n", "--no-confirm", "-no-confirm" };

        private static readonly CommandType[] COMMANDS_0_ARG = { CommandType.ShowHelp };
        private static readonly CommandType[] COMMANDS_1_ARG = { CommandType.Delete };
        private static readonly CommandType[] COMMANDS_2_ARGS =
            { 
                CommandType.Copy, 
                CommandType.Copy, 
                CommandType.CreateSymLink, 
                CommandType.All
            };

        #endregion

        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _commandParser = new CLICommandParser();
        }

        #endregion

        #region ParseArgs for a Command which requires 0 args

        [Test]

        public void ParseArgs_WithValid0ArgsCommand_ShouldReturnIt(
            [ValueSource(nameof(COMMANDS_0_ARG))]CommandType commandType)
        {
            foreach (var commandName in GetCLICommandNamesFromType(commandType))
            {
                string[] args =
                    {
                        commandName 
                    };

                var commandInfo = _commandParser.ParseArgs(args);

                Assert.AreEqual(commandType, commandInfo.Type);
                // This is only for the Help command, maybe split this test?
                Assert.IsFalse(commandInfo.RequiresConfirm);
                Assert.AreEqual(0, commandInfo.Arguments.Count);
            }
        }

        [Test, Pairwise]
        public void ParseArgs_WithValid0ArgsCommandANDNoConfirmFlag_ShouldReturnIt(
            [ValueSource(nameof(COMMANDS_0_ARG))]CommandType commandType,
            [ValueSource(nameof(NO_CONFIRM_FLAGS))]string flag)
        {
            foreach (var commandName in GetCLICommandNamesFromType(commandType))
            {
                string[] args =
                    {
                        commandName,
                        flag
                    };

                var commandInfo = _commandParser.ParseArgs(args);

                Assert.AreEqual(commandType, commandInfo.Type);
                Assert.IsFalse(commandInfo.RequiresConfirm);
                Assert.AreEqual(0, commandInfo.Arguments.Count);
            }
        }

        [Test, Pairwise]
        public void ParseArgs_WithValid0ArgsCommandANDConfirmFlag_ShouldReturnIt(
            [ValueSource(nameof(COMMANDS_0_ARG))]CommandType commandType,
            [ValueSource(nameof(CONFIRM_FLAGS))]string flag)
        {
            // Note : ShowHelp sets the no-confirm manually
            // TODO : Add a property in the commands for this?
            if (commandType == CommandType.ShowHelp)
                return;

            foreach (var commandName in GetCLICommandNamesFromType(commandType))
            {
                string[] args =
                    {
                        commandName,
                        flag
                    };

                var commandInfo = _commandParser.ParseArgs(args);

                Assert.AreEqual(commandType, commandInfo.Type);
                Assert.IsTrue(commandInfo.RequiresConfirm);
                Assert.AreEqual(0, commandInfo.Arguments.Count);
            }
        }

        #endregion

        #region ParseArgs for a Command which requires 1 args

        [Test]
        public void ParseArgs_WithValid1ArgsCommand_ShouldReturnIt(
            [ValueSource(nameof(COMMANDS_1_ARG))]CommandType commandType)
        {
            foreach (var commandName in GetCLICommandNamesFromType(commandType))
            {
                string[] args =
                    {
                        commandName,
                        SOURCE_PATH
                    };

                var commandInfo = _commandParser.ParseArgs(args);

                Assert.AreEqual(commandType, commandInfo.Type);
                Assert.AreEqual(1, commandInfo.Arguments.Count);
                Assert.IsTrue(commandInfo.Arguments.ContainsKey(SOURCE_PATH_ARG_NAME));
                Assert.AreEqual(
                    SOURCE_PATH,
                    commandInfo.Arguments[SOURCE_PATH_ARG_NAME]);
            }
        }

        [Test, Pairwise]
        public void ParseArgs_WithValid1ArgsCommandANDNoConfirmFlag_ShouldReturnIt(
            [ValueSource(nameof(COMMANDS_1_ARG))]CommandType commandType,
            [ValueSource(nameof(NO_CONFIRM_FLAGS))]string flag)
        {

            foreach (var commandName in GetCLICommandNamesFromType(commandType))
            {
                string[] args =
                    {
                        commandName,
                        SOURCE_PATH,
                        flag
                    };

                var commandInfo = _commandParser.ParseArgs(args);

                Assert.AreEqual(commandType, commandInfo.Type);
                Assert.IsFalse(commandInfo.RequiresConfirm);

                Assert.AreEqual(1, commandInfo.Arguments.Count);
                Assert.IsTrue(commandInfo.Arguments.ContainsKey(SOURCE_PATH_ARG_NAME));
                Assert.AreEqual(
                    SOURCE_PATH,
                    commandInfo.Arguments[SOURCE_PATH_ARG_NAME]);
            }
        }

        [Test, Pairwise]
        public void ParseArgs_WithValid1ArgsCommandANDConfirmFlag_ShouldReturnIt(
            [ValueSource(nameof(COMMANDS_1_ARG))]CommandType commandType,
            [ValueSource(nameof(CONFIRM_FLAGS))]string flag)
        {

            foreach (var commandName in GetCLICommandNamesFromType(commandType))
            {
                string[] args =
                    {
                        commandName,
                        SOURCE_PATH,
                        flag
                    };

                var commandInfo = _commandParser.ParseArgs(args);

                Assert.AreEqual(commandType, commandInfo.Type);
                Assert.IsTrue(commandInfo.RequiresConfirm);

                Assert.AreEqual(1, commandInfo.Arguments.Count);
                Assert.IsTrue(commandInfo.Arguments.ContainsKey(SOURCE_PATH_ARG_NAME));
                Assert.AreEqual(
                    SOURCE_PATH,
                    commandInfo.Arguments[SOURCE_PATH_ARG_NAME]);
            }
        }

        #endregion

        #region ParseArgs for a Command which requires 2 args

        [Test]
        public void ParseArgs_WithValid2ArgsCommand_ShouldReturnIt(
            [ValueSource(nameof(COMMANDS_2_ARGS))]CommandType commandType)
        {
            foreach (var commandName in GetCLICommandNamesFromType(commandType))
            {
                string[] args =
                    {
                        commandName,
                        SOURCE_PATH,
                        TARGET_PATH
                    };

                var commandInfo = _commandParser.ParseArgs(args);

                Assert.AreEqual(commandType, commandInfo.Type);
                Assert.AreEqual(2, commandInfo.Arguments.Count);

                Assert.IsTrue(commandInfo.Arguments.ContainsKey(SOURCE_PATH_ARG_NAME));
                Assert.AreEqual(
                    SOURCE_PATH,
                    commandInfo.Arguments[SOURCE_PATH_ARG_NAME]);

                Assert.IsTrue(commandInfo.Arguments.ContainsKey(TARGET_PATH_ARG_NAME));
                Assert.AreEqual(
                    TARGET_PATH,
                    commandInfo.Arguments[TARGET_PATH_ARG_NAME]);
            }
        }



        [Test, Pairwise]
        public void ParseArgs_WithValid2ArgsCommandANDConfirmFlag_ShouldReturnIt(
            [ValueSource(nameof(COMMANDS_2_ARGS))]CommandType commandType,
            [ValueSource(nameof(CONFIRM_FLAGS))]string flag)
        {
          
            foreach (var commandName in GetCLICommandNamesFromType(commandType))
            {
                string[] args =
                    {
                        commandName,
                        SOURCE_PATH,
                        TARGET_PATH,
                        flag
                    };
           
                var commandInfo = _commandParser.ParseArgs(args);

                Assert.AreEqual(commandType, commandInfo.Type);
                Assert.IsTrue(commandInfo.RequiresConfirm);

                Assert.AreEqual(2, commandInfo.Arguments.Count);
                Assert.IsTrue(commandInfo.Arguments.ContainsKey(SOURCE_PATH_ARG_NAME));
                Assert.AreEqual(
                    SOURCE_PATH,
                    commandInfo.Arguments[SOURCE_PATH_ARG_NAME]);

                Assert.IsTrue(commandInfo.Arguments.ContainsKey(TARGET_PATH_ARG_NAME));
                Assert.AreEqual(
                    TARGET_PATH,
                    commandInfo.Arguments[TARGET_PATH_ARG_NAME]);
            }
        }

        [Test, Pairwise]
        public void ParseArgs_WithValid2ArgsCommandANDNoConfirmFlag_ShouldReturnIt(
            [ValueSource(nameof(COMMANDS_2_ARGS))]CommandType commandType,
            [ValueSource(nameof(NO_CONFIRM_FLAGS))]string flag)
        {
          
            foreach (var commandName in GetCLICommandNamesFromType(commandType))
            {
                string[] args =
                    {
                        commandName,
                        SOURCE_PATH,
                        TARGET_PATH,
                        flag
                    };
           
                var commandInfo = _commandParser.ParseArgs(args);

                Assert.AreEqual(commandType, commandInfo.Type);
                Assert.IsFalse(commandInfo.RequiresConfirm);

                Assert.AreEqual(2, commandInfo.Arguments.Count);
                Assert.IsTrue(commandInfo.Arguments.ContainsKey(SOURCE_PATH_ARG_NAME));
                Assert.AreEqual(
                    SOURCE_PATH,
                    commandInfo.Arguments[SOURCE_PATH_ARG_NAME]);

                Assert.IsTrue(commandInfo.Arguments.ContainsKey(TARGET_PATH_ARG_NAME));
                Assert.AreEqual(
                    TARGET_PATH,
                    commandInfo.Arguments[TARGET_PATH_ARG_NAME]);
            }
        }

        #endregion

        #region ParseArgs invalid args

        [Test]
        public void ParseArgs_WithEmptyArgs_ShouldThrow()
        {
            string[] args = { };

            var ex = Assert.Throws(
                         Is.TypeOf<ArgumentException>(),
                         () => _commandParser.ParseArgs(args)
                     );

            Assert.IsTrue(Regex.IsMatch(
                    ex.Message, 
                    "This cannot work without a command"));
        }

        [Test]
        public void ParseArgs_WithNullArgs_ShouldThrow()
        {
            var ex = Assert.Throws(
                         Is.TypeOf<ArgumentNullException>(),
                         () => _commandParser.ParseArgs(null)
                     );
            // This regex should match this message (with the new line character)
            // Example message:
            /* 
                Value cannot be null.
                Parameter name: arguments
            */
            Assert.IsTrue(Regex.IsMatch(
                    ex.Message,
                    "Value cannot be null(?s).*arguments"));
        }

        #endregion

        #region Helper methods

        private static IEnumerable<string> GetCLICommandNamesFromType(CommandType type)
        {
            var possibleNames = new List<string>();

            switch (type)
            {
                case CommandType.CreateSymLink:
                    possibleNames.Add("link");
                    break;
                case CommandType.ShowHelp:
                    possibleNames.Add("help");
                    possibleNames.Add("--help");
                    break;
                case CommandType.Delete:
                case CommandType.Copy:
                case CommandType.All:
                case CommandType.Move:
                    possibleNames.Add(type.ToString().ToLower());
                    break;
                default:
                    throw new NotSupportedException(string.Format(
                            "{0} is not a supported type",
                            type.ToString())
                    );
            }

            return possibleNames;
        }

        #endregion
    }
}
