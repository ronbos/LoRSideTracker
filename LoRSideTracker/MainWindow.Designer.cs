namespace LoRSideTracker
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.LogTextBox = new System.Windows.Forms.RichTextBox();
            this.DebugLogsCheckBox = new System.Windows.Forms.CheckBox();
            this.SnapWindowsButton = new System.Windows.Forms.Button();
            this.MyProgressDisplay = new LoRSideTracker.ProgressDisplayControl();
            this.SuspendLayout();
            // 
            // LogTextBox
            // 
            this.LogTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogTextBox.Location = new System.Drawing.Point(12, 41);
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ReadOnly = true;
            this.LogTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.LogTextBox.Size = new System.Drawing.Size(582, 188);
            this.LogTextBox.TabIndex = 0;
            this.LogTextBox.Text = "";
            // 
            // DebugLogsCheckBox
            // 
            this.DebugLogsCheckBox.AutoSize = true;
            this.DebugLogsCheckBox.Location = new System.Drawing.Point(480, 12);
            this.DebugLogsCheckBox.Name = "DebugLogsCheckBox";
            this.DebugLogsCheckBox.Size = new System.Drawing.Size(114, 17);
            this.DebugLogsCheckBox.TabIndex = 2;
            this.DebugLogsCheckBox.Text = "Show Debug Logs";
            this.DebugLogsCheckBox.UseVisualStyleBackColor = true;
            this.DebugLogsCheckBox.CheckedChanged += new System.EventHandler(this.DebugLogsCheckBox_CheckedChanged);
            // 
            // SnapWindowsButton
            // 
            this.SnapWindowsButton.Location = new System.Drawing.Point(12, 12);
            this.SnapWindowsButton.Name = "SnapWindowsButton";
            this.SnapWindowsButton.Size = new System.Drawing.Size(99, 23);
            this.SnapWindowsButton.TabIndex = 3;
            this.SnapWindowsButton.Text = "Snap Windows";
            this.SnapWindowsButton.UseVisualStyleBackColor = true;
            this.SnapWindowsButton.Click += new System.EventHandler(this.SnapWindowsButton_Click);
            // 
            // MyProgressDisplay
            // 
            this.MyProgressDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MyProgressDisplay.Location = new System.Drawing.Point(110, 69);
            this.MyProgressDisplay.Name = "MyProgressDisplay";
            this.MyProgressDisplay.Size = new System.Drawing.Size(400, 120);
            this.MyProgressDisplay.TabIndex = 1;
            this.MyProgressDisplay.Visible = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(606, 241);
            this.Controls.Add(this.SnapWindowsButton);
            this.Controls.Add(this.DebugLogsCheckBox);
            this.Controls.Add(this.MyProgressDisplay);
            this.Controls.Add(this.LogTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "LoR Side Tracker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox LogTextBox;
        private ProgressDisplayControl MyProgressDisplay;
        private System.Windows.Forms.CheckBox DebugLogsCheckBox;
        private System.Windows.Forms.Button SnapWindowsButton;
    }
}

