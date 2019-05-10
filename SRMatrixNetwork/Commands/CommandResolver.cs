using SRMatrixNetwork.Commands.ClientSide;
using SRMatrixNetwork.Commands.ServerSide;

namespace SRMatrixNetwork.Commands
{
    public class CommandResolver
    {
        public CommandBase ResolveCommand(string input, IDexPromise dexPromise)
        {

            string[] split = input.Split(' ');

            if (LoginCommand.CmdName.Equals(split[0].Trim().ToLower()))
                return new LoginCommand(dexPromise);
            if (TestCommand.CmdName.Equals(split[0].Trim().ToLower()))
                return new TestCommand(dexPromise);
            else
            {
                return new SRDirectInstructionCommand(Verbosity.Critical, dexPromise);
            }
            
            /*
            if (input.Contains("\n"))
                return new MacroCommand(dexPromise);

            if (InitCommand.CmdName.Equals(split[0].ToLower()))
                return new InitCommand(dexPromise);

            if (SRStatusInstructionCommand.CmdName.Equals(split[0].ToLower()))
                return new SRStatusInstructionCommand(Verbosity.Critical, dexPromise);



            if (DexInfoInstructionCommand.CmdName.Equals(split[0].ToLower()))
                return new DexInfoInstructionCommand(Verbosity.Critical, dexPromise);

            if (DexTargetInstructionCommand.CmdName.Equals(split[0].ToLower()))
                return new DexTargetInstructionCommand(Verbosity.Critical, dexPromise);

            if (TargetCommand.CmdName.Equals(split[0].ToLower()))
                return new TargetCommand(dexPromise);

            if (DexLookInstructionCommand.CmdName.Equals(split[0].ToLower()))
                return new DexLookInstructionCommand(Verbosity.Critical, dexPromise);

            if (HackCommand.CmdName.Equals(split[0].ToLower()))
                return new HackCommand(dexPromise);

            if (ShowGraphUICommand.CmdName.Equals(split[0].ToLower()))
                return new ShowGraphUICommand(dexPromise);

            if (split[0].ToLower().StartsWith("#"))
                return new DexHackInstructionCommand(Verbosity.Critical, dexPromise);*/


            return null;
        }
    }
}