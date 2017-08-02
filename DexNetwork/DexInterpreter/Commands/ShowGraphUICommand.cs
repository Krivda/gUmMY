using System;
using DexNetwork.UI;

namespace DexNetwork.DexInterpreter.Commands
{
    class ShowGraphUICommand : CommandBase
    {
        private long _softwareCode;
        public const string CmdName = "draw";

        public ShowGraphUICommand(IDexPromise promise) : base(promise)
        {
            CommandHelpString = "info #<software code>";
            MandatoryParamCount = 0;
        }

        public override CommandResult OnCommandInput(string input)
        {
            if (Promise.BaseForm != null)
            {
                if (Promise.HostForm == null)
                {
                    Promise.HostForm = new WPFHostForm();
                    Promise.HostForm.SetPromise(Promise);
                    Promise.HostForm.Show(Promise.BaseForm);
                }
                else
                {
                    if (!Promise.HostForm.Visible)
                        Promise.HostForm.Show(Promise.BaseForm);

                    Promise.HostForm.SetPromise(Promise);
                }
            }

            return CreateOutput(new TextOutput(Verbosity.Critical, "Form is shown."), CommadState.Finished);
        }
    }
}