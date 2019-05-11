using System;

namespace SRMatrixNetwork
{
    public class LoggedUser
    {
        public string Realm { get; set; }
        public string Login { get; set; }
        public string Target { get; set; }
        public string Host { get; set; }
        public int MatrixCondition { get; set; } = int.MinValue;
        public string VisibleAs { get; set; }
        public int MaxMatrixCondition { get; set; } = int.MinValue;
    }
}