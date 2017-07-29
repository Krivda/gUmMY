using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows.Forms;
using ConsoleStream;
using DexNetwork.DexInterpreter.Commands;
using DexNetwork.Multithreading;
using DexNetwork.Server;
using DexNetwork.Structure;

namespace DexNetwork.DexInterpreter
{
    public interface IDexPromise
    {
        Network Network { get; set; }
        LoggedUser LoggedUser { get; set; }
        IXMPPClient XMPPClient { get; set; }
        Form BaseForm { get; }
    }

    public class DexCommandProccessor : ConsoleStreamBase
    {
        private readonly Form _baseForm;
        public Network Network { get; private set; }
        public LoggedUser User { get; private set; }
        public IXMPPClient XMPPClient { get; private set; }
        public CommandBase ActiveCommand { get; private set; }

        private DexPromise _dexPromise;

        public ConcurrentQueue<String> XMPPQueue = new ConcurrentQueue<string>();

        class DexPromise : IDexPromise
        {
            private readonly DexCommandProccessor _proccessor;

            public Network Network
            {
                get => _proccessor.Network;
                set => _proccessor.Network = value;
            }

            public LoggedUser LoggedUser
            {
                get => _proccessor.User;
                set => _proccessor.User = value;
            }

            public IXMPPClient XMPPClient
            {
                get => _proccessor.XMPPClient;
                set => _proccessor.XMPPClient = value;
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
                FireProcessErrorEvent(e.ToString());
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

            if (DexStatusCommand.CmdName.Equals(split[0].ToLower()))
                return new DexStatusCommand(Verbosity.Critical, _dexPromise);

            if (LoginCommand.CmdName.Equals(split[0].ToLower()))
                return new LoginCommand(_dexPromise);


            return null;
        }

        private void HandleResult(CommandResult result)
        {
            //FireBlockInput(false)

            //firePromptChanged

            if (result.XMPPConnected)
            {
                if (XMPPClient != null)
                {
                    Action xmppTimerAction = () =>
                    {
                        string command;
                        if (XMPPQueue.TryDequeue(out command))
                        {
                            XMPPClient.SendMessage(command);
                        }
                    };


                    XMPPClient.OnMessageRecieved += OnXMPPResponse;
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

            if (result.XMPPCommand != null)
            {
                if (XMPPClient == null)
                    throw new Exception("XMPP Connection is not set up, use login command to establish connection");

                XMPPQueue.Enqueue(result.XMPPCommand);
            }
                

            if (result.Status == CommadStatus.RequestResume)
            {
                var newResult = ActiveCommand.Proceed();
                HandleResult(newResult);
            }
            else if (result.Status == CommadStatus.Finished)
            {
                ActiveCommand = null;
            }
        }

        private void OnXMPPResponse(IXMPPClient sender, XMPPEventArgs args)
        {
            Monitor.Enter(this);
            try
            {

                if (ActiveCommand == null)
                {
                    FireProcessErrorEvent($"Got XMPP message {args.Message} when no ActiveCommand exists!");
                    return;
                }

                HandleResult(ActiveCommand.OnXMPPInput(args.Message));
            }
            finally
            {
                Monitor.Exit(this);
            }

        }

    }
}
