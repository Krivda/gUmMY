using System.Collections.Generic;
using me.krivda.utils;
using NUnit.Framework;

namespace UnitTests.NewTests
{
    class TestStringExtensions
    {
        [TestCase(TestName = "test tokenize")]
        public void TestSingleToken()
        {
            string test = "]] [[token1 ]] scascas [[token2]] scascas [[ [[token3]] [[";


            List<Token> tokens = test.TokenizeAll("[[", "]]", 0);

            Assert.AreEqual(3, tokens.Count);
            Assert.AreEqual("token1 ", tokens[0].Content);
            Assert.AreEqual(3, tokens[0].Start);
            Assert.AreEqual(14, tokens[0].End);

            Assert.AreEqual("token2", tokens[1].Content);
            Assert.AreEqual(23, tokens[1].Start);
            Assert.AreEqual(33, tokens[1].End);

            Assert.AreEqual(" [[token3", tokens[2].Content);
            Assert.AreEqual(42, tokens[2].Start);
            Assert.AreEqual(55, tokens[2].End);
        }

        [TestCase(TestName = "test tokenize2")]
        public void Testtoken2()
        {
            string test = "] [icon: someicon ] some text [token2]] other text [IC: someIC] [] [icon,persona gr8b][";


            List<Token> tokens = test.TokenizeAll("[", "]", 0);

            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("icon: someicon ", tokens[0].Content);
            Assert.AreEqual(2, tokens[0].Start);
            Assert.AreEqual(19, tokens[0].End);

            Assert.AreEqual("token2", tokens[1].Content);
            Assert.AreEqual(30, tokens[1].Start);
            Assert.AreEqual(38, tokens[1].End);

            Assert.AreEqual("IC: someIC", tokens[2].Content);
            Assert.AreEqual(51, tokens[2].Start);
            Assert.AreEqual(63, tokens[2].End);
        }

        [TestCase(TestName = "test between")]
        public void TestReplaceAt()
        {
            string test = "a [sdfg] drqka";


            string result = test.ReplaceBetween(3, 7, "newcontent");

            Assert.AreEqual("a [newcontent] drqka", result);


        }

    }
}
