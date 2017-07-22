using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xmpp
{
    public class XMPPClient
    {
        private string outputPath;
        private string inputPath;

        private string sessionId;
        private string sessionDir;

        public XMPPClient(string sessionDir)
        {
            if (!Directory.Exists(sessionDir))
                Directory.CreateDirectory(sessionDir);

            this.sessionDir = sessionDir;
        }


        public void Connect(string host, string user, string pwd, string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = GenerateId(user);


            }
        }

        private string GenerateId(string user)
        {
            string result = string.Format("{0}-{1}", user, DateTime.Now.ToShortDateString());

            foreach (var VARIABLE in COLLECTION)
            {
                
            }
        }

        public void OpenChat(string user)
        {
            
        }

        public void Send(string message)
        {
            
        }

        public List<String> Receive(string message)
        {
            return null;
        }


    }
}
