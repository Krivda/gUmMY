using System.Collections.Generic;
using me.krivda.utils;
using NUnit.Framework;
using SRMatrixNetwork.Formatter;

namespace UnitTests.NewTests
{
    class TestMatrixFormatting
    {
        [TestCase(TestName = "test formatting")]
        public void TestFormatting()
        {
            string test = "] [icon: someicon ] some text [token2]] other text [IC: someIC] [] [persona: gr8b][";

            string res = MatrixFormatter.ApplyMatrixFormatting(test);

            Assert.AreEqual("] [color,-256: someicon ] some text [token2]] other text [color,-65536: someIC] [] [color,-16711681: gr8b][", res);

        }
    }
}