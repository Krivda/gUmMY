using System;
using System.Collections.Generic;
using DexNetwork.Server;

namespace DexNetwork.DexInterpreter.Commands
{
    public class LoginCommand : CommandBase
    {
        private readonly Dictionary<string, string> _wellknownNames;
        public const string CmdName = "login";

        private const string REALM_LOCAL = "local";
        private const string REALM_DARKNET = "darknet";

        public string Password { get; private set; }
        public string Login { get; private set; }
        public string Realm { get; private set; }

        public LoginCommand(IDexPromise promise) : base(promise)
        {
            OptionalParamCount = 1;
            MandatoryParamCount = 2;
            CommandName = CmdName;
            CommandHelpString = "login <user> <password> [realm->local]";
            Status = CommadStatus.NotStarted;

            _wellknownNames = new Dictionary<string, string>();
            _wellknownNames.Add("calvin", "mypass");
        }

        public override CommandResult OnCommandInput(string input)
        {
            CommandResult result = EnsureStatus(CommadStatus.NotStarted);
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


            if (Realm.Equals(REALM_LOCAL))
            {
                _promise.XMPPClient = new XMPPLocal();
                string pwd;

                if (!_wellknownNames.TryGetValue(Login, out pwd))
                    pwd = Password;
                try
                {
                    _promise.XMPPClient.Login(Login, Realm, pwd);
                }
                catch (Exception e)
                {
                    return CreateError($"Couldn't connect to realm '{Realm}'. Got exception {e}.");
                }
            }
            else //if (Realm.Equals(REALM_DARKNET))
            {
                return CreateError($"Couldn't connect to realm '{Realm}'. It's not supported.");
            }

            result = CreateOutput(new TextOutput(Verbosity.Critical, $"Logged as user {Login} to realm {Realm}."), CommadStatus.Finished);
            result.Prompt = new Dictionary<string, string> {{"login", Login}};
            result.XMPPConnected = true;

            return result;
        }
                

        public override CommandResult OnXMPPInput(string message)
        {
            return CreateError($"XMPP is not supported for the command '{CommandName}'");
        }
    }
}