using System;
using System.Collections.Generic;
using SRMatrixNetwork.Data;

namespace SRMatrixNetwork.Commands.ClientSide
{
    public class LoginCommand : CommandBase
    {
           
        public const string CmdName = "login";
        public const string DEFAULT_HOST = "xmpp.co";

        public LoginCommand(IDexPromise promise) : base(promise)
        {
            OptionalParamCount = 1;
            MandatoryParamCount = 1;
            CommandName = CmdName;
            CommandHelpString = "login <user> [password]";
            State = CommandState.NotStarted;

        }

        public override CommandResult OnCommandInput(string input)
        {
            CommandResult result = EnsureState(CommandState.NotStarted);
            if (result != null)
                return result;

            var validateArgsResult = ParseArguments(input);
            if (validateArgsResult != null)
                throw new Exception(validateArgsResult.Error.Text);


            string name = Parameters[0];
            string jid = "";
            string pwd = "";

            Dictionary<string, KnownDecker> deckers = DeckersRepository.LoadDeckers();
            deckers.TryGetValue(name, out var knownDecker);
            if (knownDecker != null)
            {
                jid = knownDecker.Jid;
                pwd = knownDecker.Pwd;
            }

            //command param takes priority over stored data
            if (Parameters.Count > 1 && !string.IsNullOrEmpty(Parameters[1]))
            {
                pwd = Parameters[1];
            }

            if (string.IsNullOrEmpty(pwd))
            {
                return CreateError($"Couldn't find password for user {name}. Please use specify it in data file or via command parameter.");
            }

            if (string.IsNullOrEmpty(jid))
            {
                jid = name;
            }

            Promise.XmppClient = new Matrix();
            try
            {
                Promise.XmppClient.Login(jid, pwd, "matrix");
            }
            catch (Exception e)
            {
                return CreateError($"Couldn't connect to matrix. Got exception {e}.");
            }


            result = CreateOutput(new TextOutput(Verbosity.Critical, $"Connect to grid as persona {name}."), CommandState.Finished);
            
            result.XmppCommand = "status";
            result.XmppConnected = true;

            Logger.Info($"Session for {name} started");

            return result;
        }
                

        public override CommandResult OnXmppMessageReceived(string message)
        {
            return CreateError($"XMPP is not supported for the command '{CommandName}'");
        }
    }
}