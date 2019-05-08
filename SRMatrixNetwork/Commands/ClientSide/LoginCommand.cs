using System;
using System.Collections.Generic;

namespace SRMatrixNetwork.Commands.ClientSide
{
    public class LoginCommand : CommandBase
    {
           
        private readonly Dictionary<string, string> _wellknownNames;
        public const string CmdName = "login";

        private const string REALM_LOCAL = "local";
        private const string REALM_DARKNET = "darknet";
        private const string REALM_MATRIX = "matrix";

        public string Password { get; private set; }
        public string Login { get; private set; }
        public string Realm { get; private set; }

        public LoginCommand(IDexPromise promise) : base(promise)
        {
            OptionalParamCount = 1;
            MandatoryParamCount = 2;
            CommandName = CmdName;
            CommandHelpString = "login <user> <password> [realm->local]";
            State = CommandState.NotStarted;

            _wellknownNames = new Dictionary<string, string> {{"gr8b", "639924"}};

        }

        public override CommandResult OnCommandInput(string input)
        {
            CommandResult result = EnsureState(CommandState.NotStarted);
            if (result != null)
                return result;

            var validateArgsResult = ParseArguments(input);
            if (validateArgsResult != null)
                throw new Exception(validateArgsResult.Error.Text);

            Login = Parameters[0];
            Password = Parameters[1];

            if (String.IsNullOrEmpty(Parameters[2]))
                Realm = REALM_LOCAL;
            else
                Realm = Parameters[2];


            string pwd;

            if (!_wellknownNames.TryGetValue(Login, out pwd))
                pwd = Password;

            if (Realm.Equals(REALM_MATRIX))
            {
                Promise.XmppClient = new Matrix();
                try
                {
                    Promise.XmppClient.Login(Login, Realm, pwd);
                }
                catch (Exception e)
                {
                    return CreateError($"Couldn't connect to realm '{Realm}'. Got exception {e}.");
                }
            }
            else
            {
                result = CreateError($"Connection not available for  {Login} to realm {Realm}. Login format is {CommandHelpString}");
                return result;
            }

            result = CreateOutput(new TextOutput(Verbosity.Critical, $"Logged as user {Login} to realm {Realm}."), CommandState.Finished);
            
            result.Prompt = new Dictionary<string, string> { { "realm", Realm } };
            result.XmppConnected = true;


            Logger.Info("Session started");

            return result;
        }
                

        public override CommandResult OnXmppMessageReceived(string message)
        {
            return CreateError($"XMPP is not supported for the command '{CommandName}'");
        }
    }
}