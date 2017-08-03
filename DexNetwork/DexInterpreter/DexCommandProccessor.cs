using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ConsoleStream;
using DexNetwork.DexInterpreter.Commands;
using DexNetwork.Multithreading;
using DexNetwork.Server;
using DexNetwork.Structure;
using DexNetwork.UI;
using NLog;

namespace DexNetwork.DexInterpreter
{
    public interface IDexPromise
    {
        Network Network { get; set; }
        SoftwareLib SoftwareLib { get; set; }
        LoggedUser LoggedUser { get; set; }
        Form BaseForm { get; }
        IXMPPClient XmppClient { get; set; }
        WPFHostForm HostForm { get; set; }
    }

    public class DexCommandProccessor : ConsoleStreamBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Form _baseForm;
        public Network Network { get; private set; }
        public SoftwareLib SoftwareLib { get; private set; }
        public LoggedUser User { get; private set; } = new LoggedUser();
        public IXMPPClient XmppClient { get; private set; }
        public CommandBase ActiveCommand { get; private set; }
        WPFHostForm HostForm { get; set; }

        private readonly DexPromise _dexPromise;

        public ConcurrentQueue<String> XmppQueue = new ConcurrentQueue<string>();

        class DexPromise : IDexPromise
        {
            private readonly DexCommandProccessor _proccessor;

            public Network Network
            {
                get => _proccessor.Network;
                set => _proccessor.Network = value;
            }

            public SoftwareLib SoftwareLib
            {
                get => _proccessor.SoftwareLib;
                set => _proccessor.SoftwareLib = value;
            }

            public LoggedUser LoggedUser
            {
                get => _proccessor.User;
                set => _proccessor.User = value;
            }
            public IXMPPClient XmppClient
            {
                get => _proccessor.XmppClient;
                set => _proccessor.XmppClient = value;
            }

            public WPFHostForm HostForm
            {
                get => _proccessor.HostForm;
                set => _proccessor.HostForm = value;
            }


            public Form BaseForm => _proccessor._baseForm;

            public DexPromise(DexCommandProccessor proccessor)
            {
                if (proccessor != null) _proccessor = proccessor;
            }
        }
        
        public DexCommandProccessor(Form baseForm)
        {
            _baseForm = baseForm;
            _dexPromise = new DexPromise(this);

        }

        public override void StartProcess(string fileName, string arguments)
        {
            string softwareLibPath = @"Software/lib.xml";

            try
            {
                SoftwareLib = Serializer.Deserialize<SoftwareLib>(softwareLibPath);
                SoftwareLib.Init(softwareLibPath);
            }
            catch (Exception e)
            {
                FireProcessErrorEvent(e.ToString());
            }
        }

        public override void StopProcess()
        {
            base.StopProcess();
        }

        public override void ExecuteCommad(string input)
        {
            Monitor.Enter(this);
            try
            {
                if (ActiveCommand == null)
                {
                    ActiveCommand = GetCommand(input);
                    if (ActiveCommand == null)
                    {
                        FireProcessErrorEvent($"Command {input.Split(' ')[0]} is not found!");
                        return;
                    }
                }

                CommandResult result = ActiveCommand.OnCommandInput(input);
                HandleResult(result);
            }
            catch (Exception e)
            {
                string errorMsg = e.ToString();
                if (e.Message.StartsWith("XMPP Connection is not set up"))
                    errorMsg = e.Message;

                FireProcessErrorEvent(errorMsg);
                ActiveCommand = null;
            }
            finally
            {
                Monitor.Exit(this);

            }
        }

        private CommandBase GetCommand(string input)
        {
            string[] split = input.Split(' ');

            if (InitCommand.CmdName.Equals(split[0].ToLower()))
                return new InitCommand(_dexPromise);

            if (DexStatusInstructionCommand.CmdName.Equals(split[0].ToLower()))
                return new DexStatusInstructionCommand(Verbosity.Critical, _dexPromise);

            if (LoginCommand.CmdName.Equals(split[0].ToLower()))
                return new LoginCommand(_dexPromise);

            if (DexInfoInstructionCommand.CmdName.Equals(split[0].ToLower()))
                return new DexInfoInstructionCommand(Verbosity.Critical, _dexPromise);

            if (DexTargetInstructionCommand.CmdName.Equals(split[0].ToLower()))
                return new DexTargetInstructionCommand(Verbosity.Critical, _dexPromise);

            if (TargetCommand.CmdName.Equals(split[0].ToLower()))
                return new TargetCommand(_dexPromise);

            if (DexLookInstructionCommand.CmdName.Equals(split[0].ToLower()))
                return new DexLookInstructionCommand(Verbosity.Critical,  _dexPromise);

            if (HackCommand.CmdName.Equals(split[0].ToLower()))
                return new HackCommand(_dexPromise);

            if (ShowGraphUICommand.CmdName.Equals(split[0].ToLower()))
                return new ShowGraphUICommand(_dexPromise);

            if (split[0].ToLower().StartsWith("#"))
                return new DexHackInstructionCommand(Verbosity.Critical, _dexPromise);

            if (split[0].ToLower().StartsWith("!"))
                return new DexDirectInstructionCommand( Verbosity.Critical, _dexPromise);

            return null;
        }

        private void HandleResult(CommandResult result)
        {
            //FireBlockInput(false)

            if (result.XMPPConnected)
            {
                if (XmppClient != null)
                {
                    Action xmppTimerAction = () =>
                    {
                        string command;
                        if (XmppQueue.TryDequeue(out command))
                        {
                            XmppClient.SendMessage(command);
                        }
                    };


                    XmppClient.OnMessageReceived += OnXmppResponse;
                    PeriodicTaskFactory.Start(xmppTimerAction, 1000, 1, -1, -1, true);
                }
            }

            if (result.Output != null)
                foreach (var textOutput in result.Output)
                {
                    //todo: verbosity
                    FireProcessOutputEvent(textOutput.Text);
                }

            if (result.Error != null)
                FireProcessErrorEvent(result.Error.Text);

            if (result.UpdatedNetStatus!=null)
            {
                User.Login = result.UpdatedNetStatus.Login;

                string realm = "";
                if(result.Prompt != null  && result.Prompt.ContainsKey("realm"))
                    realm = $"@{result.Prompt["realm"]}";

                User.Login = result.UpdatedNetStatus.Login;
                User.AdminSystem = result.UpdatedNetStatus.AdminSystem;
                User.Proxy = result.UpdatedNetStatus.Proxy;
                User.Target = result.UpdatedNetStatus.Target;
                User.VisibleAs = result.UpdatedNetStatus.VisibleAs;


                string prompt = $"{User.Login}{realm}# prx:{User.Proxy} :{User.VisibleAs} $$ {User.Target}";

                FirePromptChangedEvent(prompt);
            }

            if (result.XMPPCommand != null)
            {
                if (XmppClient == null)
                    throw new Exception("XMPP Connection is not set up, use login command to establish connection");

                XmppQueue.Enqueue(result.XMPPCommand);
            }
                
            
            if (result.State == CommadState.RequestResume)
            {
                var newResult = ActiveCommand.Proceed();
                HandleResult(newResult);
            }
            else if (result.State == CommadState.Finished)
            {
                ActiveCommand = null;
            }
        }

        private void OnXmppResponse(IXMPPClient sender, XMPPEventArgs args)
        {
            Monitor.Enter(this);
            try
            {
                logger.Info($"darknet@cyberspace:>>\n{args.Message}");
                if (ActiveCommand == null)
                {
                    FireProcessErrorEvent($"Got XMPP message {args.Message} when no ActiveCommand exists!");
                    return;
                }

                HandleResult(ActiveCommand.OnXmppMessageReceived(args.Message));
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Monitor.Exit(this);
            }

        }

    }
}
