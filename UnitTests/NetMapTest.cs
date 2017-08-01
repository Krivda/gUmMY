using System;
using System.Collections.Generic;
using System.Reflection;
using DexNetwork.Structure;
using gUmMYConsole;
using NUnit.Framework;

namespace UnitTests
{
    public class NetMapTest
    {
        [TestCase(new object[] { "single node", 1,  "-" })]
        [TestCase(new object[] { "two  nodes", 2, "┌┤└" })]
        [TestCase(new object[] { "three  nodes", 3, "┌│┼│└" })]
        [TestCase(new object[] { "four  nodes", 4, "┌│├┤├│└" })]
        public void TestPrifix(string name, int nodeCount, string expected)
        {
            var Obj = new NetTextMap(null);

            var listPrefixes = Obj.InvokePrivate<List<String>>("createBlock", new object[]{nodeCount});

            Assert.AreEqual(expected.Length, listPrefixes.Count, "Number of lines is not as expected");

            int i = 0;
            foreach (var prefix in listPrefixes)
            {
                Assert.AreEqual(expected.Substring(i,1), prefix, $"{name} test failed for line {i}.");
                i++;
            }
        }

        [TestCase(TestName = "Simple network")]
        public void TestLayout()
        {

            string expected =
                "                     ┌Router1"     + "\r\n" +
                "          ┌VPN       ┤"            + "\r\n" +
                "          │          └Monitor3"    + "\r\n" +
                "firewall  ┤" + "\r\n" +
                "          │          ┌VPN2"        + "\r\n" +
                "          └Monitor1  ┤"            + "\r\n" +
                "                     └Monitor4"    + "\r\n" ;


            var net = NetGenerator.GetNet("simple");

            NetTextMap visual = new NetTextMap(net);
            string netText = visual.GetTextView("", 10);
            
            Console.WriteLine(netText);
            Assert.AreEqual(expected, netText, "simple network");
        }

        [TestCase(TestName = "Complicated tree network")]
        public void TestLayoutComplicated()
        {

            string expected =
                //          ********** 
                "                     ┌Router1"                   + "\r\n" +
                "          ┌VPN1      ┤"                          + "\r\n" +
                "          │          │          ┌cryptore1"      + "\r\n" +
                "          │          │          │"               + "\r\n" +
                "          │          └Monitor3  ┼data core1"     + "\r\n" +
                "          │                     │"               + "\r\n" +
                "          │                     └data core2"     + "\r\n" +
                "firewall  ┤"                                     + "\r\n" +
                "          │                     ┌VPN3"           + "\r\n" +
                "          │                     │"               + "\r\n" + 
                "          │          ┌VPN2      ┼Traffic2"       + "\r\n" +
                "          │          │          │"               + "\r\n" +
                "          │          │          └cryptocore3"    + "\r\n" +
                "          └Monitor1  ┤"                          + "\r\n" +
                "                     └Monitor4  ─Antivirus1"     + "\r\n";

            
            var net = NetGenerator.GetNet("complicated");

            NetTextMap visual = new NetTextMap(net);
            string netText = visual.GetTextView("", 10);

            Console.WriteLine(netText);
            Assert.AreEqual(expected, netText, "complicated network");
            
        }

        [TestCase(TestName = "Looped tree network")]
        public void TestLayoutLooped()
        {

            string expected =
                //          ********** 
                "                     ┌Router1" + "\r\n" +
                "          ┌VPN1      ┤" + "\r\n" +
                "          │          │          ┌cryptore1" + "\r\n" +
                "          │          │          │" + "\r\n" +
                "          │          └Monitor3  ┼data core1" + "\r\n" +
                "          │                     │" + "\r\n" +
                "          │                     └data core2" + "\r\n" +
                "firewall  ┤" + "\r\n" +
                "          │                     ┌VPN3" + "\r\n" +
                "          │                     │" + "\r\n" +
                "          │          ┌VPN2      ┼Traffic2" + "\r\n" +
                "          │          │          │" + "\r\n" +
                "          │          │          └cryptocore3" + "\r\n" +
                "          └Monitor1  ┤" + "\r\n" +
                "                     └Monitor4  ─Antivirus1" + "\r\n";


            var net = NetGenerator.GetNet("linked");

            NetTextMap visual = new NetTextMap(net);
            string netText = visual.GetTextView("", 15);

            Console.WriteLine(netText);
            Assert.AreEqual(expected, netText, "looped network");

        }

    }

    static class AccessExtensions
    {
        public static T InvokePrivate<T>(this object o, string methodName, params object[] args) where T:class
        {
            var mi = o.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (mi != null)
            {
                return  mi.Invoke(o, args) as T;
            }
            return null;
        }
    }
}
