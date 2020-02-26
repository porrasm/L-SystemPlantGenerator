using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {
    public class LSystemGrammarTest {

        [Test]
        public void LSystemGrammarTestSimplePasses() {
            var grammar = TestGrammar1();

            string it0 = grammar.PerformIterations("f", 0);
            string it1 = grammar.PerformIterations("f", 1);
            string it2 = grammar.PerformIterations("f", 2);
            string it3 = grammar.PerformIterations("f", 3);

            Assert.AreEqual(it0, "0");

        }

        private LSystemGrammar TestGrammar1() {

            LSystemGrammar grammar = new LSystemGrammar();

            CommandDefinition d = new CommandDefinition();
            d.Type = CommandDefinition.CommandType.Variable;
            d.Command = "f";
            d.Rules.Add(new ProbabilityRule());
            d.Rules[0].Rule = "f+[angle = angle * 0.99]f";

            grammar.CharacterDefinitions.Add(d);

            

            return grammar;
        }
    }
}
