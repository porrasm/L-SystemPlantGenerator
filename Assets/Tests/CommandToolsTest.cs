using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {
    public class CommandToolsTest {
        [Test]
        public void CommandToolsTestSimplePasses() {
            string tree = "f+[angle = angle * 0.99]f";

            List<StringCommand> commands = CommandTools.GetCommands(tree);

            foreach (StringCommand c in commands) {
                Logger.Print(c);
            }

            Assert.AreEqual(4, commands.Count);

            Assert.AreEqual("f", commands[0].Command);
            Assert.AreEqual(StringCommand.CommandType.Command, commands[0].Type);

            Assert.AreEqual("+", commands[1].Command);
            Assert.AreEqual(StringCommand.CommandType.Command, commands[1].Type);

            Assert.AreEqual("angle = angle * 0.99", commands[2].Command);
            Assert.AreEqual(StringCommand.CommandType.CommandParameter, commands[2].Type);

            Assert.AreEqual("f", commands[3].Command);
            Assert.AreEqual(StringCommand.CommandType.Command, commands[3].Type);

            string[] parameters = commands[2].GetParameters();

            Assert.AreEqual(1, parameters.Length);
        }

    }
}
