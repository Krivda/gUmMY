using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows.Forms;
using ConsoleStream;
using NLog;
using SRMatrixNetwork.Commands;
using SRMatrixNetwork.Multithreading;
using SRMatrixNetwork.Server;

namespace SRMatrixNetwork
{
    public interface IDexPromise
    {
        //Network Network { get; set; }
        //SoftwareLib SoftwareLib { get; set; }
        LoggedUser LoggedUser { get; set; }
        Form BaseForm { get; }
        IXmppClient XmppClient { get; set; }
        //WPFHostForm HostForm { get; set; }
        CommandResolver CommandResolver { get; set; }
    }

    public class MatrixProcessor : ConsoleStreamBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Form _baseForm;
        //public Network Network { get; private set; }
        //public SoftwareLib SoftwareLib { get; private set; }
        public LoggedUser User { get; private set; } = new LoggedUser();
        public IXmppClient XmppClient { get; private set; }
        public CommandBase ActiveCommand { get; private set; }
        //WPFHostForm HostForm { get; set; }
        CommandResolver CommandResolver { get; set; }
        private readonly DexPromise _dexPromise;

        public ConcurrentQueue<string> XmppQueue = new ConcurrentQueue<string>();

        class DexPromise : IDexPromise
        {
            private readonly MatrixProcessor _processor;

            public LoggedUser LoggedUser
            {
                get => _processor.User;
                set => _processor.User = value;
            }
            public IXmppClient XmppClient
            {
                get => _processor.XmppClient;
                set => _processor.XmppClient = value;
            }

            public CommandResolver CommandResolver
            {
                get => _processor.CommandResolver;
                set => _processor.CommandResolver = value;
            }


            public Form BaseForm => _processor._baseForm;

            public DexPromise(MatrixProcessor processor)
            {
                if (processor != null) _processor = processor;
            }
        }

        public MatrixProcessor(Form baseForm)
        {
            _baseForm = baseForm;
            _dexPromise = new DexPromise(this);

        }

        public override void StartProcess(string fileName, string arguments)
        {
            //string softwareLibPath = @"Software/lib.xml";

            try
            {
                /*
                SoftwareLib = Serializer.Deserialize<SoftwareLib>(softwareLibPath);
                SoftwareLib.Init(softwareLibPath);
                */
                CommandResolver = new CommandResolver();
            }
            catch (Exception e)
            {
                FireProcessErrorEvent(e.ToString());
            }
        }

        public override void ExecuteCommand(string input)
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
            return CommandResolver.ResolveCommand(input, _dexPromise);
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

            if (result.UpdatedNetStatus != null)
            {
                User.Login = result.UpdatedNetStatus.Login;

                string realm = "";
                if (result.Prompt != null && result.Prompt.ContainsKey("realm"))
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


            if (result.State == CommandState.RequestResume)
            {
                var newResult = ActiveCommand.Proceed();
                HandleResult(newResult);
            }
            else if (result.State == CommandState.Finished)
            {
                ActiveCommand = null;
            }
        }

        private void OnXmppResponse(IXmppClient sender, XmppEventArgs args)
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
                //todo wtf?
            }
            finally
            {
                Monitor.Exit(this);
            }

        }
    }
}
