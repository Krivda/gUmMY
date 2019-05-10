using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows.Forms;
using ConsoleStream;
using NLog;
using SRMatrixNetwork.Commands;
using SRMatrixNetwork.Formatter;
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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
            try
            {
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

            if (result.XmppConnected)
            {
                //this happens only after Login command runs.
                if (XmppClient != null)
                {
                    // start xmpp listening and prepare to send

                    Action xmppTimerAction = () =>
                    {
                        if (XmppQueue.TryDequeue(out var command))
                        {
                            XmppClient.SendMessage(command);
                        }
                    };

                    //subscribe to incoming messages
                    XmppClient.OnMessageReceived += OnXmppResponse;

                    //process outgoing messages (once per 500 m sec will deque and send 1 message)
                    PeriodicTaskFactory.Start(xmppTimerAction, 500, 1, -1, -1, true);
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

            if (result.XmppCommand != null)
            {
                if (XmppClient == null)
                    throw new Exception("XMPP Connection is not set up, use login command to establish connection");

                XmppQueue.Enqueue(result.XmppCommand);
            }

            //todo: fixit
            /*if (result.Prompt != null)
            {
                FirePromptChangedEvent(result.Prompt);
            }*/

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
                Logger.Info($"darknet@cyberspace:>>\n{args.Message}");
                if (ActiveCommand == null)
                {
                    FireProcessOutputEvent(args.Message);
                }
                else
                {
                    HandleResult(ActiveCommand.OnXmppMessageReceived(args.Message));
                }

            }
            catch (Exception ex)
            {
                string msg = $"Unexpected exception while processing {args.Message}. \n{ex}!";
                Logger.Error(msg);
                FireProcessErrorEvent(msg);
            }
            finally
            {
                Monitor.Exit(this);
            }

        }


        private string ProcessMarker(string marker)
        {
            string result = marker.Trim().ToLower();

            

            return marker;
        }



        protected override void FireProcessOutputEvent(string message)
        {
            message = MatrixFormatter.ApplyMatrixFormatting(message);

            base.FireProcessOutputEvent(message);
        }

    }
}
