namespace gUmMYConsole
{
    partial class ConsoleForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            ConsoleControl.DummyStream dummyStream1 = new ConsoleControl.DummyStream();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsoleForm));
            this.pnlConsole = new System.Windows.Forms.Panel();
            this.consConsole = new ConsoleControl.ConsoleControl();
            this.dbgStream = new System.Windows.Forms.Panel();
            this.cmdDumpNet = new System.Windows.Forms.Button();
            this.cmdClear = new System.Windows.Forms.Button();
            this.lbDebg = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.chkInputable = new System.Windows.Forms.CheckBox();
            this.txtStreamInput = new System.Windows.Forms.TextBox();
            this.cmdSendFromApp = new System.Windows.Forms.Button();
            this.txtStreamOutput = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cmdSoft = new System.Windows.Forms.Button();
            this.pnlConsole.SuspendLayout();
            this.dbgStream.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlConsole
            // 
            this.pnlConsole.Controls.Add(this.consConsole);
            this.pnlConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlConsole.Location = new System.Drawing.Point(0, 135);
            this.pnlConsole.Name = "pnlConsole";
            this.pnlConsole.Size = new System.Drawing.Size(1317, 559);
            this.pnlConsole.TabIndex = 1;
            // 
            // consConsole
            // 
            dummyStream1.IsProcessRunning = false;
            dummyStream1.ProcessFileName = null;
            this.consConsole.ConsoleStream = dummyStream1;
            this.consConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.consConsole.Echo = true;
            this.consConsole.IsInputEnabled = false;
            this.consConsole.Location = new System.Drawing.Point(0, 0);
            this.consConsole.Name = "consConsole";
            this.consConsole.Prompt = "";
            this.consConsole.PromptSuffix = ">";
            this.consConsole.SendKeyboardCommandsToProcess = false;
            this.consConsole.ShowDiagnostics = true;
            this.consConsole.Size = new System.Drawing.Size(1317, 559);
            this.consConsole.TabIndex = 0;
            // 
            // dbgStream
            // 
            this.dbgStream.Controls.Add(this.cmdSoft);
            this.dbgStream.Controls.Add(this.cmdDumpNet);
            this.dbgStream.Controls.Add(this.cmdClear);
            this.dbgStream.Controls.Add(this.lbDebg);
            this.dbgStream.Controls.Add(this.button1);
            this.dbgStream.Controls.Add(this.chkInputable);
            this.dbgStream.Controls.Add(this.txtStreamInput);
            this.dbgStream.Controls.Add(this.cmdSendFromApp);
            this.dbgStream.Controls.Add(this.txtStreamOutput);
            this.dbgStream.Dock = System.Windows.Forms.DockStyle.Top;
            this.dbgStream.Location = new System.Drawing.Point(0, 0);
            this.dbgStream.Name = "dbgStream";
            this.dbgStream.Size = new System.Drawing.Size(1317, 135);
            this.dbgStream.TabIndex = 5;
            // 
            // cmdDumpNet
            // 
            this.cmdDumpNet.Location = new System.Drawing.Point(997, 77);
            this.cmdDumpNet.Name = "cmdDumpNet";
            this.cmdDumpNet.Size = new System.Drawing.Size(59, 29);
            this.cmdDumpNet.TabIndex = 10;
            this.cmdDumpNet.Text = "dump net";
            this.cmdDumpNet.UseVisualStyleBackColor = true;
            this.cmdDumpNet.Click += new System.EventHandler(this.CmdDumpNet_Click);
            // 
            // cmdClear
            // 
            this.cmdClear.Location = new System.Drawing.Point(893, 14);
            this.cmdClear.Name = "cmdClear";
            this.cmdClear.Size = new System.Drawing.Size(75, 23);
            this.cmdClear.TabIndex = 9;
            this.cmdClear.Text = "Clear";
            this.cmdClear.UseVisualStyleBackColor = true;
            this.cmdClear.Click += new System.EventHandler(this.CmdClear_Click);
            // 
            // lbDebg
            // 
            this.lbDebg.AutoSize = true;
            this.lbDebg.Location = new System.Drawing.Point(890, 106);
            this.lbDebg.Name = "lbDebg";
            this.lbDebg.Size = new System.Drawing.Size(0, 13);
            this.lbDebg.TabIndex = 8;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(977, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // chkInputable
            // 
            this.chkInputable.AutoSize = true;
            this.chkInputable.Checked = true;
            this.chkInputable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkInputable.Location = new System.Drawing.Point(890, 76);
            this.chkInputable.Name = "chkInputable";
            this.chkInputable.Size = new System.Drawing.Size(91, 17);
            this.chkInputable.TabIndex = 6;
            this.chkInputable.Text = "Input enabled";
            this.chkInputable.UseVisualStyleBackColor = true;
            this.chkInputable.CheckedChanged += new System.EventHandler(this.ChkInputable_CheckedChanged);
            // 
            // txtStreamInput
            // 
            this.txtStreamInput.Location = new System.Drawing.Point(1075, 3);
            this.txtStreamInput.Multiline = true;
            this.txtStreamInput.Name = "txtStreamInput";
            this.txtStreamInput.Size = new System.Drawing.Size(239, 117);
            this.txtStreamInput.TabIndex = 5;
            // 
            // cmdSendFromApp
            // 
            this.cmdSendFromApp.Location = new System.Drawing.Point(889, 41);
            this.cmdSendFromApp.Name = "cmdSendFromApp";
            this.cmdSendFromApp.Size = new System.Drawing.Size(168, 28);
            this.cmdSendFromApp.TabIndex = 4;
            this.cmdSendFromApp.Text = "send";
            this.cmdSendFromApp.UseVisualStyleBackColor = true;
            this.cmdSendFromApp.Click += new System.EventHandler(this.CmdSendFromApp_Click);
            // 
            // txtStreamOutput
            // 
            this.txtStreamOutput.Location = new System.Drawing.Point(3, 3);
            this.txtStreamOutput.Multiline = true;
            this.txtStreamOutput.Name = "txtStreamOutput";
            this.txtStreamOutput.Size = new System.Drawing.Size(880, 117);
            this.txtStreamOutput.TabIndex = 3;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // cmdSoft
            // 
            this.cmdSoft.Location = new System.Drawing.Point(897, 96);
            this.cmdSoft.Name = "cmdSoft";
            this.cmdSoft.Size = new System.Drawing.Size(75, 23);
            this.cmdSoft.TabIndex = 11;
            this.cmdSoft.Text = "Soft";
            this.cmdSoft.UseVisualStyleBackColor = true;
            this.cmdSoft.Click += new System.EventHandler(this.cmdSoft_Click);
            // 
            // ConsoleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1317, 694);
            this.Controls.Add(this.pnlConsole);
            this.Controls.Add(this.dbgStream);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConsoleForm";
            this.Text = "gUmMy";
            this.Load += new System.EventHandler(this.ConsoleForm_Load);
            this.pnlConsole.ResumeLayout(false);
            this.dbgStream.ResumeLayout(false);
            this.dbgStream.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlConsole;
        private ConsoleControl.ConsoleControl consConsole;
        private System.Windows.Forms.Panel dbgStream;
        private System.Windows.Forms.TextBox txtStreamInput;
        private System.Windows.Forms.Button cmdSendFromApp;
        private System.Windows.Forms.TextBox txtStreamOutput;
        private System.Windows.Forms.CheckBox chkInputable;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lbDebg;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button cmdClear;
        private System.Windows.Forms.Button cmdDumpNet;
        private System.Windows.Forms.Button cmdSoft;
    }
}

