using DexNetwork;
using DexNetwork.Structure;
using NUnit.Framework;

namespace UnitTests
{
    public class TestLibSerialize
    {
        [TestCase(TestName = "Test serialization")]
        public void TestSerialization()
        {
            

            var softlib = Serializer.Deserialize<Software>(TestContext.CurrentContext.TestDirectory + @"\\Software\lib.xml");

            string s = Serializer.Serialize(softlib);
        }
    }
}