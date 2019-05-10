namespace SRMatrixNetwork.Commands
{
    public class TextOutput
    {
        public Verbosity Vebosity { get; set; } = 0;
        public string Text { get; set; }

        public TextOutput(Verbosity vebosity, string text)
        {
            Vebosity = vebosity;
            Text = text;
        }
    }
}