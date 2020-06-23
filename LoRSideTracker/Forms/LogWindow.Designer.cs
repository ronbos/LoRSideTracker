namespace LoRSideTracker
{
    partial class LogWindow
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
            this.DebugLogsCheckBox = new System.Windows.Forms.CheckBox();
            this.LogTextBox = new System.Windows.Forms.RichTextBox();
            this.DebugLogTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // DebugLogsCheckBox
            // 
            this.DebugLogsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DebugLogsCheckBox.AutoSize = true;
            this.DebugLogsCheckBox.Location = new System.Drawing.Point(12, 338);
            this.DebugLogsCheckBox.Name = "DebugLogsCheckBox";
            this.DebugLogsCheckBox.Size = new System.Drawing.Size(114, 17);
            this.DebugLogsCheckBox.TabIndex = 3;
            this.DebugLogsCheckBox.Text = "Show Debug Logs";
            this.DebugLogsCheckBox.UseVisualStyleBackColor = true;
            this.DebugLogsCheckBox.CheckedChanged += new System.EventHandler(this.DebugLogsCheckBox_CheckedChanged);
            // 
            // LogTextBox
            // 
            this.LogTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogTextBox.Location = new System.Drawing.Point(12, 13);
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.Size = new System.Drawing.Size(491, 319);
            this.LogTextBox.TabIndex = 4;
            this.LogTextBox.Text = "";
            // 
            // DebugLogTextBox
            // 
            this.DebugLogTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DebugLogTextBox.Location = new System.Drawing.Point(12, 13);
            this.DebugLogTextBox.Name = "DebugLogTextBox";
            this.DebugLogTextBox.Size = new System.Drawing.Size(491, 319);
            this.DebugLogTextBox.TabIndex = 5;
            this.DebugLogTextBox.Text = "";
            // 
            // LogWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 367);
            this.Controls.Add(this.DebugLogTextBox);
            this.Controls.Add(this.LogTextBox);
            this.Controls.Add(this.DebugLogsCheckBox);
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "LogWindow";
            this.Text = "Log";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogWindow_FormClosing);
            this.Load += new System.EventHandler(this.LogWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox DebugLogsCheckBox;
        private System.Windows.Forms.RichTextBox LogTextBox;
        private System.Windows.Forms.RichTextBox DebugLogTextBox;
    }
}