using DexNetwork;
using NUnit.Framework;

namespace UnitTests
{
    public class TestLibSerialize
    {
        [TestCase(TestName = "Test serialization")]
        public void TestSerialization()
        {
            

            var softlib = Serializer.DeserializeSoft(TestContext.CurrentContext.TestDirectory + @"\\Software\lib.xml");

            string s = Serializer.SerializeSoft(softlib);
        }
    }
}