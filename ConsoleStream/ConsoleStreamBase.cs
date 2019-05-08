namespace ConsoleStream
{
    /// <summary>
    /// A ProcessEventHandler is a delegate for process input/output events.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="ConsoleStreamEventArgs"/> instance containing the event data.</param>
    public delegate void ProcessEventHandler(object sender, ConsoleStreamEventArgs args);

    /// <summary>
    /// Interface between console-streaming backend and ConsoleControl
    /// </summary>
    public abstract class ConsoleStreamBase  
    {
        public bool IsProcessRunning { get; set; }
        public object ProcessFileName { get; set; }


        #region Events
        /// <summary>
        /// Occurs when process output is produced.
        /// </summary>
        public event ProcessEventHandler OnProcessOutput;

        /// <summary>
        /// Occurs when process error output is produced.
        /// </summary>
        public event ProcessEventHandler OnProcessError;

        /// <summary>
        /// Occurs when process input is produced.
        /// </summary>
        public event ProcessEventHandler OnCommandExecute;

        /// <summary>
        /// Occurs when prompt change is requested
        /// </summary>
        public event ProcessEventHandler OnPromptChanged;


        //TODO: fixit, should be attachable to control
        /// <summary>
        /// Occurs when the process ends.
        /// </summary>
        public event ProcessEventHandler OnProcessExit;

        /// <summary>
        /// Fires the process output event.
        /// </summary>
        /// <param name="content">The content.</param>
        protected virtual void FireProcessOutputEvent(string content)
        {
            //  Get the event and fire it.
            OnProcessOutput?.Invoke(this, new ConsoleStreamEventArgs(content));
        }

        /// <summary>
        /// Fires the process error output event.
        /// </summary>
        /// <param name="content">The content.</param>
        protected virtual void FireProcessErrorEvent(string content)
        {
            //  Get the event and fire it.
            var theEvent = OnProcessError;
            theEvent?.Invoke(this, new ConsoleStreamEventArgs(content));
        }

        /// <summary>
        /// Fires the process input event.
        /// </summary>
        /// <param name="content">The content.</param>
        protected virtual void FireProcessCommandExecute(string content)
        {
            //  Get the event and fire it.
            var theEvent = OnCommandExecute;
            theEvent?.Invoke(this, new ConsoleStreamEventArgs(content));
        }

        /// <summary>
        /// Fires the process exit event.
        /// </summary>
        /// <param name="code">The code.</param>
        protected virtual void FireProcessExitEvent(int code)
        {
            //  Get the event and fire it.
            OnProcessExit?.Invoke(this, new ConsoleStreamEventArgs(code));
        }

        /// <summary>
        /// Fires the prompt changed event
        /// </summary>
        /// <param name="content">new prompt value</param>
        protected virtual void FirePromptChangedEvent(string content)
        {
            //  Get the event and fire it.
            OnPromptChanged?.Invoke(this, new ConsoleStreamEventArgs(content));
        }

        #endregion

        public virtual void StartProcess(string fileName, string arguments)
        {
            //nothing
        }

        //TODO: fixit, should be attachable to control
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
        /// <param name="command">executed command</param>
        public virtual void ExecuteCommand(string command)
        {
            FireProcessCommandExecute(command);
        }

    }
}