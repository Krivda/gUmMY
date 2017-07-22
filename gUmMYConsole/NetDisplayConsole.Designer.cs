namespace gUmMYConsole
{
    partial class NetDisplayConsole
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
            ConsoleControl.DummyStream dummyStream1 = new ConsoleControl.DummyStream();
            this.consNetShow = new ConsoleControl.ConsoleControl();
            this.SuspendLayout();
            // 
            // consNetShow
            // 
            dummyStream1.IsProcessRunning = false;
            dummyStream1.ProcessFileName = null;
            this.consNetShow.ConsoleStream = dummyStream1;
            this.consNetShow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.consNetShow.Echo = true;
            this.consNetShow.IsInputEnabled = false;
            this.consNetShow.Location = new System.Drawing.Point(0, 0);
            this.consNetShow.Name = "consNetShow";
            this.consNetShow.Prompt = ">";
            this.consNetShow.PromptSuffix = ">";
            this.consNetShow.SendKeyboardCommandsToProcess = false;
            this.consNetShow.ShowDiagnostics = true;
            this.consNetShow.Size = new System.Drawing.Size(853, 444);
            this.consNetShow.TabIndex = 0;
            // 
            // NetDisplayConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 444);
            this.Controls.Add(this.consNetShow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "NetDisplayConsole";
            this.Text = "NetDisplayConsole";
            this.ResumeLayout(false);

        }

        #endregion

        private ConsoleControl.ConsoleControl consNetShow;
    }
}