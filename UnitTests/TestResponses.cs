using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DexNetwork.DexInterpreter.Response;
using NUnit.Framework;

namespace UnitTests
{
    partial class TestResponses
    {
        [TestCase(TestName = "Status command response")]
        public void TestStatus()
        {
            string status = @"calvin276 status:
Current target: BlackMirror11
Current administrating system: none
Proxy level: 6
Current proxy address: kenguru2157@sydney";

            var statusResponse = StatusInstruction.Parse(status);

            Assert.NotNull(status, "Parsed not successfully");

            Assert.AreEqual("calvin276", statusResponse.Login, "Login");
            Assert.AreEqual("BlackMirror11", statusResponse.Target, "Target");
            Assert.AreEqual("none", statusResponse.AdminSystem, "AdminSystem");
            Assert.AreEqual(6, statusResponse.Proxy, "Proxy");
            Assert.AreEqual("kenguru2157@sydney", statusResponse.VisibleAs, "VisibleAs");

        }

        [TestCase(TestName = "info command response")]
        public void TestInfo()
        {
            string hack = @"--------------------
#180 programm info:
Effect: disable
Allowed node types:
 -Firewall
 -Antivirus
 -VPN
 -Brandmauer
 -Router
 -Traffic monitor
 -Cyptographic system
Duration: 600sec.
END ----------------";

            var infoResponse = InfoInstruction.Parse(hack);

            Assert.NotNull(hack, "Parsed not successfully");
            Assert.IsTrue(string.IsNullOrEmpty(infoResponse.Error), "Should be a success");

            Assert.AreEqual(180, infoResponse.Code, "Code");
            Assert.AreEqual(600, infoResponse.Duration, "Duration");
            Assert.AreEqual("disable", infoResponse.Effect, "Effect");
            Assert.AreEqual("", infoResponse.InevitableEffect, "InevitableEffect");
            Assert.AreEqual("[Firewall,Antivirus,VPN,Brandmauer,Router,Traffic monitor,Cyptographic system]", infoResponse.SupportedNodes, "SupportedNodes");
            Assert.AreEqual("exploit", infoResponse.Software.SoftwareType, "SoftwareType");



            hack = @"--------------------
#180 programm info:
Effect: disable
Inevitable effect: logname
Allowed node types:
 -Firewall
 -Antivirus
 -VPN
 -Brandmauer
 -Router
 -Traffic monitor
 -Cyptographic system
Duration: 600sec.
END ----------------";

            infoResponse = InfoInstruction.Parse(hack);

            Assert.NotNull(hack, "Parsed not successfully");
            Assert.IsTrue(string.IsNullOrEmpty(infoResponse.Error), "Should be a success");


            Assert.AreEqual("logname", infoResponse.InevitableEffect, "InevitableEffect");

            string soft = @"--------------------
#2294523 programm info:
Effect: trace
Inevitable effect: logname
Allowed node types:
 -Firewall
 -Antivirus
 -VPN
 -Brandmauer
 -Router
 -Traffic monitor
 -Cyptographic system
END ----------------";

             infoResponse = InfoInstruction.Parse(soft);

            Assert.NotNull(soft, "Parsed not successfully");
            Assert.IsTrue(string.IsNullOrEmpty(infoResponse.Error), "Should be a success");

            Assert.AreEqual(2294523, infoResponse.Code, "Code");
            Assert.AreEqual(0, infoResponse.Duration, "Duration");
            Assert.AreEqual("trace", infoResponse.Effect, "Effect");
            Assert.AreEqual("logname", infoResponse.InevitableEffect, "InevitableEffect");
            Assert.AreEqual("[Firewall,Antivirus,VPN,Brandmauer,Router,Traffic monitor,Cyptographic system]", infoResponse.SupportedNodes, "SupportedNodes");
            Assert.AreEqual("protection", infoResponse.Software.SoftwareType, "SoftwareType");


            soft = @"--------------------
#2294523 programm info:
Allowed node types:
 -Firewall
END ----------------";

            infoResponse = InfoInstruction.Parse(soft);

            Assert.NotNull(soft, "Parsed not successfully");
            Assert.IsTrue(string.IsNullOrEmpty(infoResponse.Error), "Should be a success");

            Assert.AreEqual(2294523, infoResponse.Code, "2294523");
            Assert.AreEqual(0, infoResponse.Duration, "Duration");
            Assert.AreEqual("", infoResponse.Effect, "");
            Assert.AreEqual("", infoResponse.InevitableEffect, "");
            Assert.AreEqual("[Firewall]", infoResponse.SupportedNodes, "SupportedNodes");
            Assert.AreEqual("protection", infoResponse.Software.SoftwareType, "SoftwareType");

        }
    }
}
