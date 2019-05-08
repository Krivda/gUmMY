using ConsoleStream;

namespace gUmMYConsole
{
    /*    class NetTextUIStream : ConsoleStreamBase
        {
            private readonly Network _net;
            private readonly NetTextMap _netModel;

            public NetTextUIStream(Network net)
            {
                _net = net;
                _netModel = new NetTextMap(net);
            }

            public void Redraw()
            {
                string netView =  _netModel.GetTextView("", 20);
                FireProcessOutputEvent(netView);
            }

            public void StartFeed()
            {
                IsProcessRunning = true;
            }

            public override void ExecuteCommand(string command)
            {
                FireProcessCommandExecute(command);

                string[] split = command.Split(' ');

                if (split[0].Equals("login"))
                {
                    //should have name arg and pwd arg
                    if (split.Length < 2)
                    {
                        FireProcessErrorEvent("ERROR, login syntax: login <acc> <pwd>");
                    }
                    else
                    {
                        FireProcessOutputEvent($"Logged in as {split[1]}");
                        FirePromptChangedEvent(split[1]);
                    }
                }
            }

        }*/
}