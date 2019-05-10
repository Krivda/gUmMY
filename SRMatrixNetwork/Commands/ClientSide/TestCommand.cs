using System.Drawing;

namespace SRMatrixNetwork.Commands.ClientSide
{
    class TestCommand : CommandBase
    {
        public const string CmdName = "test";

        public TestCommand(IDexPromise promise) : base(promise)
        {
            OptionalParamCount = 0;
            MandatoryParamCount = 0;
            CommandName = CmdName;
            CommandHelpString = "login";
            State = CommandState.NotStarted;
        }

        public override CommandResult OnCommandInput(string input)
        {
            CommandResult result = EnsureState(CommandState.NotStarted);

            result = CreateOutput(new TextOutput(Verbosity.Critical, $"prefix text [color,{Color.Aqua.ToArgb()}: aqua text] other text [color,{Color.Lime.ToArgb()}: lime] of ending."), CommandState.Finished);

            return result;
        }


        public override CommandResult OnXmppMessageReceived(string message)
        {
            return CreateError($"XMPP is not supported for the command '{CommandName}'");
        }
    }
}
