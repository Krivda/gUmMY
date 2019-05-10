using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ConsoleStream;
using me.krivda.utils;


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
    public delegate void ConsoleEventHandler(object sender, ConsoleEventArgs args);


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

        private const string TAG_VALUE_DELIMITER = ": ";
        private string TAG_COLOR= "color";


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

        }


        private void AttachToStream(ConsoleStreamBase attachedStream)
        {
            processInterface = attachedStream;

            //  Handle process events.
            processInterface.OnProcessOutput += new ProcessEventHandler(ProcessInterface_OnProcessOutput);
            processInterface.OnProcessError += new ProcessEventHandler(ProcessInterface_OnProcessError);
            processInterface.OnCommandExecute += new ProcessEventHandler(ProcessInterface_OnProcessCommand);
            processInterface.OnProcessExit += new ProcessEventHandler(ProcessInterface_OnProcessExit);
            processInterface.OnPromptChanged += new ProcessEventHandler(ProcessInterface_OnPromptChange);

            processInterface.StartProcess("", "");

        }

        /// <summary>
        /// Handles the OnProcessError event of the processInterface control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ConsoleStreamEventArgs"/> instance containing the event data.</param>
        void ProcessInterface_OnProcessError(object sender, ConsoleStreamEventArgs args)
        {
            string output = args.Content;

            if (!output.EndsWith("\n"))
            {
                output += "\n";
            }

            //  Write the output, in red
            WriteOutput(output, Color.Red);

            //  Fire the output event.
            FireConsoleOutputEvent(output);
        }

        /// <summary>
        /// Handles the OnProcessOutput event of the processInterface control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ConsoleStreamEventArgs"/> instance containing the event data.</param>
        void ProcessInterface_OnProcessOutput(object sender, ConsoleStreamEventArgs args)
        {
            string output = args.Content;

            if (!output.EndsWith("\n"))
            {
                output += "\n";
            }

            Color defaultColor = Color.White;

            if (args.Content.Contains("["))
            {
                output = WriteFormattedOutput(output, defaultColor);
            }
            else
            {
                //  Write the output, in white
                WriteOutput(output, defaultColor);
            }

            //  Fire the output event.
            FireConsoleOutputEvent(output);
        }

        /// <summary>
        /// Handles the OnCommandExecute event of the processInterface control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ConsoleStreamEventArgs"/> instance containing the event data.</param>
        void ProcessInterface_OnProcessCommand(object sender, ConsoleStreamEventArgs args)
        {
            //nothing to do here
        }

        /// <summary>
        /// Handles the OnProcessExit event of the processInterface control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ConsoleStreamEventArgs"/> instance containing the event data.</param>
        void ProcessInterface_OnProcessExit(object sender, ConsoleStreamEventArgs args)
        {
            //  Are we showing diagnostics?
            if (ShowDiagnostics)
            {
                WriteOutput(Environment.NewLine + processInterface.ProcessFileName + " exited.", Color.FromArgb(255, 0, 255, 0));
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
        private void ProcessInterface_OnPromptChange(object sender, ConsoleStreamEventArgs args)
        {

            if (this.InvokeRequired)
            {
                Invoke((Action) (() =>
                {
                    Prompt = args.Content;
                }));
            }
            else
            {
                Prompt = args.Content;
            }

        }

        /// <summary>
        /// Initializes the key mappings.
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
            bool resetInputHistoryIndex = true;
            
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
//ExecuteCommand("\x3", Color.White, false);
                }

                //  If we handled a mapping, we're done here.
                if (mappings.Any())
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
            else
            {
                //we are in input zone

                //handle UpArrow to surf last inputs
                if (e.KeyCode == Keys.Up)
                {
                    if (_lastInputIndex < recentInput.Count)
                    {
                        string input = recentInput[recentInput.Count - 1 - _lastInputIndex];

                        richTextBoxConsole.SelectionStart = inputStart;
                        richTextBoxConsole.SelectionLength = richTextBoxConsole.TextLength - inputStart;
                        richTextBoxConsole.SelectedText = input;

                        if (_lastInputIndex +1 != recentInput.Count)
                            _lastInputIndex++;
                    }

                    resetInputHistoryIndex = false;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Down)
                {
                    if (_lastInputIndex  < recentInput.Count)
                    {
                        string input = recentInput[recentInput.Count - 1 - _lastInputIndex];

                        richTextBoxConsole.SelectionStart = inputStart;
                        richTextBoxConsole.SelectionLength = richTextBoxConsole.TextLength - inputStart;
                        richTextBoxConsole.SelectedText = input;

                        if (_lastInputIndex -1 != -1)
                           _lastInputIndex--;

                    }
                    resetInputHistoryIndex = false;
                    e.Handled = true;
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
                InputCommand(input);

                //Enter is already handled, don't give to the control
                e.SuppressKeyPress = true;
            }

            if (resetInputHistoryIndex)
                _lastInputIndex = 0;

        }

        /// <summary>
        /// Writes the output to the console control.
        /// </summary>
        /// <param name="output">The output string (formatted) to output to control.</param>
        /// <param name="defaultColor">The default color.</param>
        /// <returns>output content, stripped of formatting </returns>
        private string WriteFormattedOutput(string output, Color defaultColor)
        {
            string clearString="";

            List<Token> tokens = output.TokenizeAll("[", "]");

            if (! tokens.Any())
            {
                WriteOutput(output, defaultColor);
            }
            else
            {
                int currentPos = 0;

                Token lastToken = null;
                foreach (var token in tokens)
                {
                    lastToken = token;

                    //output before token
                    string beforeToken = output.Slice(currentPos, token.Start);
                    clearString += beforeToken;
                    WriteOutput(beforeToken, defaultColor);
                    currentPos = token.Start;

                    //output token
                    string tokenOutput = token.Content;
                    Color tokenColor = defaultColor;

                    int tagNameEnd = token.Content.IndexOf(TAG_VALUE_DELIMITER, StringComparison.Ordinal);
                    if (tagNameEnd != -1)
                    {
                        string tagValue = token.Content.Substring(0, tagNameEnd);
                        string tagContent = token.Content.Substring(tagNameEnd + TAG_VALUE_DELIMITER.Length);
                        
                        if (tagValue.Contains(TAG_COLOR))
                        {
                            tokenOutput = tagContent;
                            tokenColor = DecodeColor(tagValue, defaultColor);
                        }
                        //if tag's unknown, we output it as is
                    }

                    clearString += tokenOutput;
                    WriteOutput(tokenOutput, tokenColor);

                    currentPos = token.End;
                }

                //output after last token
                if (lastToken != null)
                {
                    string afterToken = output.Substring(lastToken.End);
                    clearString += afterToken;
                    WriteOutput(afterToken, defaultColor);
                }
            }

            return clearString;
        }

        private Color DecodeColor(string tagValue, Color defaultColor)
        {
            Color result = defaultColor;

            string[] split = tagValue.Split(',');

            if (split.Length > 1)
            {
                try
                {
                    result = Color.FromArgb(int.Parse(split[1]));
                }
                catch (Exception ex)
                {
                    ; //swallow
                }
            }

            return result;
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
            promptStart = 0;
            outputStart = 0;

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
        public void InputCommand(string input)
        {
            Color color = Color.LightGray;
            recentInput.Add(input);

            Invoke((Action)(() =>
            {

                //clear input
                inputStart = promptStart + GetPrompt().Length;
                richTextBoxConsole.SelectionStart = inputStart;
                richTextBoxConsole.SelectionLength = richTextBoxConsole.Text.Length;
                richTextBoxConsole.SelectedText = "";
                richTextBoxConsole.SelectionLength = 0;

               if (Echo)
               {
                   string echoText = $"{GetPrompt()}{input}\n\n";
                   WriteOutput(echoText, color);
               }

            }));

            //  Fire the event.
            FireConsoleInputEvent(input);

            //  Write the input.
            processInterface.ExecuteCommand(input);
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
            processInterface.StartProcess(fileName, arguments);

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
            processInterface.StopProcess();
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
            /*string leadCrlf = "\n";
            if (promptStart == 0)
                leadCrlf = "";*/

            //{leadCrlf}
            return $"{Prompt}{PromptSuffix}";
        }

        /// <summary>
        /// NOT THREAD SAFE!
        /// Writes console stream output to control
        /// </summary>
        /// <param name="content"></param>
        /// <param name="color"></param>
        /// <param name="newLine">indicates whether to put a newline after ouput </param>
        private void WriteOutputUnsafe(string content, Color color, bool newLine = false)
        {
            if (content.Length==0)
                return;
            
            //save caret
            int caretPos = richTextBoxConsole.SelectionStart;
            string savedInput = richTextBoxConsole.Text.Substring(Math.Min(richTextBoxConsole.Text.Length, inputStart));
            int inputOffset = 0;

            if (caretPos > inputStart)
            {
                //in input line 
                inputOffset = caretPos - inputStart;
            }

            richTextBoxConsole.SelectionStart = outputStart;
            richTextBoxConsole.SelectionColor = color;
            richTextBoxConsole.SelectedText += content;
            outputStart = richTextBoxConsole.SelectionStart + richTextBoxConsole.SelectionLength;

            //move prompt to next line
            if (!content.EndsWith("\n"))
            {
                //content already has newline, no need to move prompt further
                richTextBoxConsole.SelectedText += "\n";
                promptStart = outputStart + 1; //1 for \n
            }
            else
            {
                promptStart = outputStart;
            }

            //clear everything after prompt start
            richTextBoxConsole.SelectionLength = richTextBoxConsole.Text.Length;
            richTextBoxConsole.SelectedText = "";

            //recalculate prompt
            richTextBoxConsole.SelectionStart = promptStart;
            richTextBoxConsole.SelectionLength = 0;
            string prompt = GetPrompt();
            richTextBoxConsole.SelectionColor = Color.White;
            richTextBoxConsole.SelectionLength = richTextBoxConsole.TextLength;
            richTextBoxConsole.SelectedText = prompt + savedInput;
            richTextBoxConsole.SelectionLength = 0;

            //restore input point
            inputStart = promptStart + prompt.Length;
            richTextBoxConsole.SelectionStart = inputStart + inputOffset;
            richTextBoxConsole.SelectionLength = 0;
        }

        private void UpdatePrompt()
        {
           
            //save caret
            int caretPos = richTextBoxConsole.SelectionStart;
            int offset = 0;

            //check where to restore caret after manipulating with prompt
            if (caretPos > inputStart)
            {
                //we are in input, we'll need to adjust it after change
                offset = caretPos - inputStart;
            }
            
            richTextBoxConsole.SelectionStart = promptStart;
            richTextBoxConsole.SelectionLength = inputStart-promptStart;

            richTextBoxConsole.SelectedText = GetPrompt();
            inputStart = richTextBoxConsole.SelectionStart;

            //restore caret
            richTextBoxConsole.SelectionStart = inputStart;
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
        private ConsoleStreamBase processInterface;
        
        /// <summary>
        /// Current position that input starts at.
        /// </summary>
        int inputStart = 0;

        /// <summary>
        /// Current position that output starts at.
        /// </summary>
        int outputStart = 0;

        /// <summary>
        /// Text position where readonly part of the console ends and prompt is started
        /// </summary>
        private int promptStart = 0;

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

        private int _lastInputIndex;


        /// <summary>
        /// Occurs when console output is produced.
        /// </summary>
        public event ConsoleEventHandler OnConsoleOutput;

        /// <summary>
        /// Occurs when console input is produced.
        /// </summary>
        public event ConsoleEventHandler OnConsoleInput;

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
                    //when enabling input - reset console input and tromp to curren  text end
                    if (value)
                    {
                        richTextBoxConsole.SelectionStart = richTextBoxConsole.TextLength;
                        promptStart = richTextBoxConsole.SelectionStart;
                        inputStart = promptStart;
                        //will update inputStart
                        UpdatePrompt();
                        richTextBoxConsole.SelectionStart = inputStart;
                    }
                    else
                    {
                        richTextBoxConsole.SelectionStart = promptStart;
                        richTextBoxConsole.SelectionLength = richTextBoxConsole.TextLength - promptStart;

                        //bug: this don't work as expected - "" is not assigned
                        //richTextBoxConsole.SelectedText = "";

                        //workaround: if text is only prompt/input - reset the text
                        if (richTextBoxConsole.SelectionStart == 0)
                            richTextBoxConsole.Text = "";
                        else
                        { 
                            //workaround: if text is NOT only prompt/input increase selection by 1 left and replace it with that symbol
                            richTextBoxConsole.SelectionStart = richTextBoxConsole.SelectionStart -1;
                            richTextBoxConsole.SelectionLength=1;
                            string newSelText = richTextBoxConsole.SelectedText.Substring(0, 1);
                            richTextBoxConsole.SelectionLength = richTextBoxConsole.TextLength - richTextBoxConsole.SelectionStart;
                            richTextBoxConsole.SelectedText = newSelText;
                        }

                        promptStart = richTextBoxConsole.TextLength; 
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
            get { return processInterface.IsProcessRunning; }
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
                return processInterface;
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
            get { return promptStart; }
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
        /// output start position
        /// </summary>
        [Browsable(false)]
        public int OutputStart
        {
            get { return outputStart; }
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