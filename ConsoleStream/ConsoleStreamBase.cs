namespace ConsoleStream
{
    /// <summary>
    /// A ProcessEventHandler is a delegate for process input/output events.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="ConsoleStreamEventArgs"/> instance containing the event data.</param>
    public delegate void ProcessEventHanlder(object sender, ConsoleStreamEventArgs args);

    /// <summary>
    /// Inerface between console-streaming backend and ConsoleContol
    /// </summary>
    public abstract class ConsoleStreamBase  
    {
        public bool IsProcessRunning { get; set; }
        public object ProcessFileName { get; set; }


        #region Events
        /// <summary>
        /// Occurs when process output is produced.
        /// </summary>
        public event ProcessEventHanlder OnProcessOutput;

        /// <summary>
        /// Occurs when process error output is produced.
        /// </summary>
        public event ProcessEventHanlder OnProcessError;

        /// <summary>
        /// Occurs when process input is produced.
        /// </summary>
        public event ProcessEventHanlder OnCommandExecute;

        /// <summary>
        /// Occurs when prompt change is requested
        /// </summary>
        public event ProcessEventHanlder OnPromptChanged;


        //TODO: fixit, should be attachable to cotrol
        /// <summary>
        /// Occurs when the process ends.
        /// </summary>
        public event ProcessEventHanlder OnProcessExit;

        /// <summary>
        /// Fires the process output event.
        /// </summary>
        /// <param name="content">The content.</param>
        protected void FireProcessOutputEvent(string content)
        {
            //  Get the event and fire it.
            var theEvent = OnProcessOutput;
            if (theEvent != null)
                theEvent(this, new ConsoleStreamEventArgs(content));
        }

        /// <summary>
        /// Fires the process error output event.
        /// </summary>
        /// <param name="content">The content.</param>
        protected void FireProcessErrorEvent(string content)
        {
            //  Get the event and fire it.
            var theEvent = OnProcessError;
            if (theEvent != null)
                theEvent(this, new ConsoleStreamEventArgs(content));
        }

        /// <summary>
        /// Fires the process input event.
        /// </summary>
        /// <param name="content">The content.</param>
        protected void FireProcessCommadExecute(string content)
        {
            //  Get the event and fire it.
            var theEvent = OnCommandExecute;
            if (theEvent != null)
                theEvent(this, new ConsoleStreamEventArgs(content));
        }

        /// <summary>
        /// Fires the process exit event.
        /// </summary>
        /// <param name="code">The code.</param>
        protected void FireProcessExitEvent(int code)
        {
            //  Get the event and fire it.
            var theEvent = OnProcessExit;
            if (theEvent != null)
                theEvent(this, new ConsoleStreamEventArgs(code));
        }

        /// <summary>
        /// Fires the promt changed event
        /// </summary>
        /// <param name="content">new propmt value</param>
        protected void FirePromptChangedEvent(string content)
        {
            //  Get the event and fire it.
            var theEvent = OnPromptChanged;
            if (theEvent != null)
                theEvent(this, new ConsoleStreamEventArgs(content));
        }

        #endregion

        public virtual void StartProcess(string fileName, string arguments)
        {
            //nothing
        }

        //TODO: fixit, should be attachable to cotrol
        /// <summary>
        /// Occurs when the process ends.
        /// </summary>
        public virtual void StopProcess()
        {
            // nothing
        }


        /// <summary>
        /// Called by Console Control to pass input to console stream
        /// </summary>
        /// <param name="command">executed commad</param>
        public virtual void ExecuteCommad(string command)
        {
            FireProcessCommadExecute(command);
        }

    }
}