using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ConsoleStream;


/**
 * This control is heavily based on marvelous https://github.com/dwmkerr/consolecontrol 
 * **/
namespace ConsoleControl
{
    /// <summary>
    /// The console event handler is used for console events.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="ConsoleEventArgs"/> instance containing the event data.</param>
    public delegate void ConsoleEventHanlder(object sender, ConsoleEventArgs args);


    /// <summary>
    /// A dummy ConsoleStream to avoid NPEs before proper stream is attached
    /// </summary>
    public class DummyStream : ConsoleStreamBase {}



    /// <summary>
    /// The Console Control allows you to embed a basic console in your application.
    /// </summary>
    [ToolboxBitmap(typeof(resfinder), "ConsoleControl.ConsoleControl.bmp")]
    [SuppressMessage("ReSharper", "RedundantDelegateCreation")]
    [SuppressMessage("ReSharper", "ArrangeAccessorOwnerBody")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public partial class ConsoleControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleControl"/> class.
        /// </summary>
        public ConsoleControl()
        {
            //  Initialise the component.
            InitializeComponent();

            AttachToStream(new DummyStream());

            //  Show diagnostics disabled by default.
            ShowDiagnostics = true;

            //  Input disabled by default.
            IsInputEnabled = false;

            //  Disable special commands by default.
            SendKeyboardCommandsToProcess = false;

            //  Initialise the keymappings.
            InitialiseKeyMappings();

            //  Wait for key down messages on the rich text box.
            richTextBoxConsole.KeyDown += new KeyEventHandler(RichTextBoxConsole_KeyDown);

            richTextBoxConsole.TextChanged += RichTextBoxConsoleOnTextChanged;
        }

        private void RichTextBoxConsoleOnTextChanged(object sender, EventArgs eventArgs)
        {
            
        }

        private void AttachToStream(ConsoleStreamBase attachedStream)
        {
            processInterace = attachedStream;

            //  Handle process events.
            processInterace.OnProcessOutput += new ProcessEventHanlder(ProcessInterace_OnProcessOutput);
            processInterace.OnProcessError += new ProcessEventHanlder(ProcessInterace_OnProcessError);
            processInterace.OnCommandExecute += new ProcessEventHanlder(ProcessInterace_OnProcessCommand);
            processInterace.OnProcessExit += new ProcessEventHanlder(ProcessInterace_OnProcessExit);
            processInterace.OnPromptChanged += new ProcessEventHanlder(ProcessInterace_OnPromptChange);

            processInterace.StartProcess("", "");

        }

        /// <summary>
        /// Handles the OnProcessError event of the processInterace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ConsoleStreamEventArgs"/> instance containing the event data.</param>
        void ProcessInterace_OnProcessError(object sender, ConsoleStreamEventArgs args)
        {
            //  Write the output, in red
            WriteOutput(args.Content, Color.Red);

            //  Fire the output event.
            FireConsoleOutputEvent(args.Content);
        }

        /// <summary>
        /// Handles the OnProcessOutput event of the processInterace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ConsoleStreamEventArgs"/> instance containing the event data.</param>
        void ProcessInterace_OnProcessOutput(object sender, ConsoleStreamEventArgs args)
        {
            //  Write the output, in white
            WriteOutput(args.Content, Color.White);

            //  Fire the output event.
            FireConsoleOutputEvent(args.Content);
        }

        /// <summary>
        /// Handles the OnCommandExecute event of the processInterace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ConsoleStreamEventArgs"/> instance containing the event data.</param>
        void ProcessInterace_OnProcessCommand(object sender, ConsoleStreamEventArgs args)
        {
            if (Echo)
            {
                string propmt = GetPrompt();
                //handle 1-st line (prompt doesn't begin with crlf)
                if (PromptStart != 0)
                {
                    propmt = propmt.Substring(1);
                }
                //  Write the output, in LightGray
                WriteOutput(propmt + args.Content, Color.LightGray);
            }
        }

        /// <summary>
        /// Handles the OnProcessExit event of the processInterace control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ConsoleStreamEventArgs"/> instance containing the event data.</param>
        void ProcessInterace_OnProcessExit(object sender, ConsoleStreamEventArgs args)
        {
            //  Are we showing diagnostics?
            if (ShowDiagnostics)
            {
                WriteOutput(System.Environment.NewLine + processInterace.ProcessFileName + " exited.", Color.FromArgb(255, 0, 255, 0));
            }

            //  Read only again.
            Invoke((Action)(() =>
            {
                richTextBoxConsole.ReadOnly = false;
            }));
        }

        /// <summary>
        /// Handles the OnPromptChanged event of the consoleStream interface.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ConsoleStreamEventArgs"/> instance containing the event data.</param>
        private void ProcessInterace_OnPromptChange(object sender, ConsoleStreamEventArgs args)
        {
            Prompt = args.Content;
            UpdatePrompt();
        }

        /// <summary>
        /// Initialises the key mappings.
        /// </summary>
        private void InitialiseKeyMappings()
        {
            //  Map 'tab'.
            keyMappings.Add(new KeyMapping(false, false, false, Keys.Tab, "{TAB}", "\t"));

            //  Map 'Ctrl-C'.
            keyMappings.Add(new KeyMapping(true, false, false, Keys.C, "^(c)", "\x03\r\n"));
        }

        /// <summary>
        /// Handles the KeyDown event of the richTextBoxConsole control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        void RichTextBoxConsole_KeyDown(object sender, KeyEventArgs e)
        {
            //  Are we sending keyboard commands to the process?
            if (SendKeyboardCommandsToProcess && IsProcessRunning)
            {
                //  Get key mappings for this key event?
                var mappings = from k in keyMappings
                               where 
                               (k.KeyCode == e.KeyCode &&
                               k.IsAltPressed == e.Alt &&
                               k.IsControlPressed == e.Control &&
                               k.IsShiftPressed == e.Shift)
                               select k;

                //  Go through each mapping, send the message.
                foreach (var mapping in mappings)
                {
                    //SendKeysEx.SendKeys(CurrentProcessHwnd, mapping.SendKeysMapping);
                    //inputWriter.WriteLine(mapping.StreamMapping);
//ExecuteCommad("\x3", Color.White, false);
                }

                //  If we handled a mapping, we're done here.
                if (mappings.Count() > 0)
                {
                    e.SuppressKeyPress = true;
                    return;
                }
            }

            //  If we're at the input point and it's backspace, bail.
            if ((richTextBoxConsole.SelectionStart <= inputStart) && e.KeyCode == Keys.Back) e.SuppressKeyPress = true;

            //  Are we in the read-only zone?
            if (richTextBoxConsole.SelectionStart < inputStart)
            {
                //  Allow arrows and Ctrl-C.
                if (!(e.KeyCode == Keys.Left ||
                    e.KeyCode == Keys.Right ||
                    e.KeyCode == Keys.Up ||
                    e.KeyCode == Keys.Down ||
                    e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown || e.KeyCode == Keys.Home || e.KeyCode == Keys.End ||
                    (e.KeyCode == Keys.C && e.Control)))
                {
                    e.SuppressKeyPress = true;
                }
            }


            if(e.KeyCode == Keys.Return && e.Shift)
            {
                if (richTextBoxConsole.SelectionStart > inputStart)
                {
                    richTextBoxConsole.Text += "\n";
                    richTextBoxConsole.SelectionStart = richTextBoxConsole.Text.Length;
                }

                e.Handled = true;
            }
            else
            //  Is it the return key?
            if (e.KeyCode == Keys.Return && !e.Shift)
            {
                int inputLen = richTextBoxConsole.TextLength- inputStart;
                inputLen = inputLen < 0 ? 0 : inputLen;

                //  Get the input.
                string input = richTextBoxConsole.Text.Substring(inputStart, inputLen);

                //  Write the input (without echoing).
                WriteInput(input, Color.White, false);

                //Enter is already handled, don't give to the cotrol
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// Writes the output to the console control.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="color">The color.</param>
        public void WriteOutput(string output, Color color)
        {
            //todo: some crappy echo-prevention
            /*if (string.IsNullOrEmpty(recentInput) == false && 
                (output == recentInput || output.Replace("\r\n", "") == recentInput))
                return;*/

            if (this.InvokeRequired)
            {
                Invoke((Action)(() =>
                {
                    //  Write the output.
                    WriteOutputUnsafe(output, color);
                }));
            }
            else
            {
                //  Write the output.
                WriteOutputUnsafe(output, color);
            }
        }

        public void ClearOutput()
        {
            richTextBoxConsole.Clear();
            inputStart = 0;
            propmtStart = 0;

            //force redraw prompt
            if (IsInputEnabled)
            {
                IsInputEnabled = false;
                IsInputEnabled = true;
            }
        }

        /// <summary>
        /// Writes the input to the console control.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="color">The color.</param>
        /// <param name="echo">if set to <c>true</c> echo the input.</param>
        public void WriteInput(string input, Color color, bool echo)
        {
            Invoke((Action)(() =>
            {
                recentInput.Add(input);

                //  Write the input.
                processInterace.ExecuteCommad(input);

                //remove the comand from current input
                richTextBoxConsole.SelectionStart = inputStart;
                richTextBoxConsole.SelectionLength = richTextBoxConsole.TextLength - inputStart;
                richTextBoxConsole.SelectedText = "";

                //  Fire the event.
                FireConsoleInputEvent(input);
            }));
        }

        /// <summary>
        /// Runs a process.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="arguments">The arguments.</param>
        public void StartProcess(string fileName, string arguments)
        {
            //  Are we showing diagnostics?
            if (ShowDiagnostics)
            {
                WriteOutput("Preparing to run " + fileName, Color.FromArgb(255, 0, 255, 0));
                if (!string.IsNullOrEmpty(arguments))
                    WriteOutput(" with arguments " + arguments + "." + Environment.NewLine, Color.FromArgb(255, 0, 255, 0));
                else
                    WriteOutput("." + Environment.NewLine, Color.FromArgb(255, 0, 255, 0));
            }

            //  Start the process.
            processInterace.StartProcess(fileName, arguments);

            //  If we enable input, make the control not read only.
            if (IsInputEnabled)
                richTextBoxConsole.ReadOnly = false;
        }

        /// <summary>
        /// Stops the process.
        /// </summary>
        public void StopProcess()
        {
            //  Stop the interface.
            processInterace.StopProcess();
        }
        
        /// <summary>
        /// Fires the console output event.
        /// </summary>
        /// <param name="content">The content.</param>
        private void FireConsoleOutputEvent(string content)
        {
            //  Get the event.
            OnConsoleOutput?.Invoke(this, new ConsoleEventArgs(content));
        }

        /// <summary>
        /// Fires the console input event.
        /// </summary>
        /// <param name="content">The content.</param>
        private void FireConsoleInputEvent(string content)
        {
            //  Get the event.
            OnConsoleInput?.Invoke(this, new ConsoleEventArgs(content));
        }

        /// <summary>
        /// Gets the current prompt with suffix
        /// </summary>
        /// <returns></returns>
        private string GetPrompt()
        {
            string leadCrlf = "\n";
            if (propmtStart == 0)
                leadCrlf = "";
            
            return $"{leadCrlf}{Prompt}{PromptSuffix}";
        }

        /// <summary>
        /// NOT THREAD SAFE!
        /// Writes console stream output to control
        /// </summary>
        /// <param name="content"></param>
        /// <param name="color"></param>
        private void WriteOutputUnsafe(string content, Color color)
        {
            if (content.Length==0)
                return;
            
            //save caret
            int caretPos = richTextBoxConsole.SelectionStart;

            //manage crlf's
            string contentPrefix = "\n";
            string contentSuffix = "";
            int inputOffset = 0;
            //if is the first line
            if (propmtStart == 0)
            {
                contentPrefix = "";
                contentSuffix = "\n";
                inputOffset = 1;
            }
            richTextBoxConsole.SelectionStart = propmtStart;
            richTextBoxConsole.SelectionColor = color;
            richTextBoxConsole.SelectedText += $"{contentPrefix}{content}{contentSuffix}";
            propmtStart = richTextBoxConsole.SelectionStart- inputOffset;
            inputStart = propmtStart + GetPrompt().Length;

            //restore caret
            richTextBoxConsole.SelectionStart = caretPos;
        }

        private void UpdatePrompt()
        {
           
            //save caret
            int caretPos = richTextBoxConsole.SelectionStart;
            int offset = 0;

            //check where to resore carret after manupulating with prompt
            if (caretPos > inputStart)
            {
                //we are in input, we'll need to adjust it after change
                offset = caretPos - inputStart;
            }
            
            richTextBoxConsole.SelectionStart = propmtStart;
            richTextBoxConsole.SelectionLength = inputStart-propmtStart;

            richTextBoxConsole.SelectedText = GetPrompt();
            inputStart = richTextBoxConsole.SelectionStart;

            //restore caret
            richTextBoxConsole.SelectionStart = caretPos+offset;
        }

        public void Test(int start, int len)
        {
            richTextBoxConsole.SelectionStart = start;
            richTextBoxConsole.SelectionLength = len;

            richTextBoxConsole.SelectedText = "";
        }


        /// <summary>
        /// The internal process interface used to interface with the process.
        /// </summary>
        private ConsoleStreamBase processInterace;
        
        /// <summary>
        /// Current position that input starts at.
        /// </summary>
        int inputStart = 0;
        
        /// <summary>
        /// Text position where readonly part of the console ends and prompt is started
        /// </summary>
        private int propmtStart = 0;

        /// <summary>
        /// The is input enabled flag.
        /// </summary>
        private bool isInputEnabled = false;

        /// <summary>
        /// The last input string (used so that we can make sure we don't echo input twice).
        /// </summary>
        private List<string> recentInput = new List<string>();

        /// <summary>
        /// The key mappings.
        /// </summary>
        private List<KeyMapping> keyMappings = new List<KeyMapping>();

        /// <summary>
        /// Backing field for Prompt
        /// </summary>
        private string promptPrefix=">";


        /// <summary>
        /// Occurs when console output is produced.
        /// </summary>
        public event ConsoleEventHanlder OnConsoleOutput;

        /// <summary>
        /// Occurs when console input is produced.
        /// </summary>
        public event ConsoleEventHanlder OnConsoleInput;

        /// <summary>
        /// Gets or sets a value indicating whether to show diagnostics.
        /// </summary>
        /// <value>
        ///   <c>true</c> if show diagnostics; otherwise, <c>false</c>.
        /// </value>
        [Category("Console Control"), Description("Show diagnostic information, such as exceptions.")]
        public bool ShowDiagnostics
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets commands echoing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if echo is enabled, otherwise - <c>false</c>.
        /// </value>
        public bool Echo { get; set; }


        /// <summary>
        ///  Gets or sets console prompt suffix part. This is Typycally '>'
        /// </summary>
        /// <value>
        ///   Value of the prompt suffix
        /// </value>
        [Category("Console Control"), Description("Сonsole prompt suffix part. This is Typycally '>'")]
        public String PromptSuffix
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is input enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is input enabled; otherwise, <c>false</c>.
        /// </value>
        [Category("Console Control"), Description("If true, the user can key in input.")]
        public bool IsInputEnabled
        {
            get { return isInputEnabled; }
            set
            {
                richTextBoxConsole.ReadOnly = !value;
                if (value!= isInputEnabled)
                {
                    //when enabling input - reset console input and promt to curren  text end
                    if (value)
                    {
                        richTextBoxConsole.SelectionStart = richTextBoxConsole.TextLength;
                        propmtStart = richTextBoxConsole.SelectionStart;
                        inputStart = propmtStart;
                        //will update inputStart
                        UpdatePrompt();
                        richTextBoxConsole.SelectionStart = inputStart;
                    }
                    else
                    {
                        richTextBoxConsole.SelectionStart = propmtStart;
                        richTextBoxConsole.SelectionLength = richTextBoxConsole.TextLength - propmtStart;

                        //bug: this don't work as expected - "" is not assigned
                        //richTextBoxConsole.SelectedText = "";

                        //workaround: if text is only propmpt/input - reset the text
                        if (richTextBoxConsole.SelectionStart == 0)
                            richTextBoxConsole.Text = "";
                        else
                        { 
                            //workaround: if text is NOT only propmpt/input increase selection by 1 left and replace it with that symbol
                            richTextBoxConsole.SelectionStart = richTextBoxConsole.SelectionStart -1;
                            richTextBoxConsole.SelectionLength=1;
                            string newSelText = richTextBoxConsole.SelectedText.Substring(0, 1);
                            richTextBoxConsole.SelectionLength = richTextBoxConsole.TextLength - richTextBoxConsole.SelectionStart;
                            richTextBoxConsole.SelectedText = newSelText;
                        }

                        propmtStart = richTextBoxConsole.TextLength; 
                        inputStart = richTextBoxConsole.TextLength; 
                    }
                }
                isInputEnabled = value;

            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [send keyboard commands to process].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [send keyboard commands to process]; otherwise, <c>false</c>.
        /// </value>
        [Category("Console Control"), Description("If true, special keyboard commands like Ctrl-C and tab are sent to the process.")]
        public bool SendKeyboardCommandsToProcess
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is process running.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is process running; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public bool IsProcessRunning
        {
            get { return processInterace.IsProcessRunning; }
        }

        /// <summary>
        /// Gets the internal rich text box.
        /// </summary>
        [Browsable(false)]
        public RichTextBox InternalRichTextBox
        {
            get { return richTextBoxConsole; }
        }

        /// <summary>
        ///  console prompt prefix part. I.E. current path, or path + branch in git
        /// </summary>
        /// <value>
        ///   Console stream may change the current prompt
        /// </value>
        [Category("Console Control"), Description("Variable Console prompt prefix part. I.E. current path, or path + branch in git")]
        public String Prompt
        {
            get { return promptPrefix; }
            set
            {
                promptPrefix = value;
                if (isInputEnabled)
                    UpdatePrompt();
            }
        }


        /// <summary>
        /// Gets the process interface.
        /// </summary>
        [Browsable(false)]
        public ConsoleStreamBase ConsoleStream
        {
            get
            {
                //  Return the base class font.
                return processInterace;
            }
            set
            {
                //attach to new ConsoleStream
                AttachToStream(value);
            }
        }

        /// <summary>
        /// Gets the key mappings.
        /// </summary>
        [Browsable(false)]
        public List<KeyMapping> KeyMappings
        {
            get { return keyMappings; }
        }

        /// <summary>
        /// Prompt start position
        /// </summary>
        [Browsable(false)]
        public int PromptStart
        {
            get { return propmtStart; }
        }

        /// <summary>
        /// Input start position
        /// </summary>
        [Browsable(false)]
        public int InputStart
        {
            get { return inputStart; }
        }



        /// <summary>
        /// Gets or sets the font of the text displayed by the control.
        /// </summary>
        /// <returns>The <see cref="T:System.Drawing.Font" /> to apply to the text displayed by the control. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultFont" /> property.</returns>
        ///   <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   </PermissionSet>
        public override Font Font
        {
            get
            {
                //  Return the base class font.
                return base.Font;
            }
            set
            {
                //  Set the base class font...
                base.Font = value;

                //  ...and the internal control font.
                richTextBoxConsole.Font = value;
            }
        }
    }

}