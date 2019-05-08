using System.Linq;

namespace SRMatrixNetwork.Data
{
    internal class KnownDecker
    {
        public string Name { get; }
        public string Jid { get; }
        public string Pwd { get; }

        public KnownDecker(string name, string pwd, string jid)
        {
            Jid = jid;
            Name = name;
            Pwd = pwd;
        }

        
    }
}
