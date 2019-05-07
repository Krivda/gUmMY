namespace SRMatrixNetwork.Commands
{
    public class TextOutput
    {
        public bool IsRtf { get; set; } = false;
        public Verbosity Vebosity { get; set; } = 0;
        public string Text { get; set; }

        public TextOutput(Verbosity vebosity, string text)
        {
            Vebosity = vebosity;
            Text = text;
        }
    }
}