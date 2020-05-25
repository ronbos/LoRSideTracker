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
            this.DeckOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.PlayerDeckCheckBox = new System.Windows.Forms.CheckBox();
            this.PlayerDrawnCheckBox = new System.Windows.Forms.CheckBox();
            this.PlayerPlayedCheckBox = new System.Windows.Forms.CheckBox();
            this.OpponentPlayedCheckBox = new System.Windows.Forms.CheckBox();
            this.DeckStatsCheckBox = new System.Windows.Forms.CheckBox();
            this.DeckOptionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // LogTextBox
            // 
            this.LogTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogTextBox.Location = new System.Drawing.Point(12, 12);
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ReadOnly = true;
            this.LogTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.LogTextBox.Size = new System.Drawing.Size(546, 197);
            this.LogTextBox.TabIndex = 0;
            this.LogTextBox.Text = "";
            // 
            // DebugLogsCheckBox
            // 
            this.DebugLogsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DebugLogsCheckBox.AutoSize = true;
            this.DebugLogsCheckBox.Location = new System.Drawing.Point(571, 12);
            this.DebugLogsCheckBox.Name = "DebugLogsCheckBox";
            this.DebugLogsCheckBox.Size = new System.Drawing.Size(114, 17);
            this.DebugLogsCheckBox.TabIndex = 2;
            this.DebugLogsCheckBox.Text = "Show Debug Logs";
            this.DebugLogsCheckBox.UseVisualStyleBackColor = true;
            this.DebugLogsCheckBox.CheckedChanged += new System.EventHandler(this.DebugLogsCheckBox_CheckedChanged);
            // 
            // SnapWindowsButton
            // 
            this.SnapWindowsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SnapWindowsButton.Location = new System.Drawing.Point(617, 186);
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
            this.MyProgressDisplay.Location = new System.Drawing.Point(110, 35);
            this.MyProgressDisplay.Name = "MyProgressDisplay";
            this.MyProgressDisplay.Size = new System.Drawing.Size(360, 120);
            this.MyProgressDisplay.TabIndex = 1;
            this.MyProgressDisplay.Visible = false;
            // 
            // DeckOptionsGroupBox
            // 
            this.DeckOptionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DeckOptionsGroupBox.Controls.Add(this.DeckStatsCheckBox);
            this.DeckOptionsGroupBox.Controls.Add(this.OpponentPlayedCheckBox);
            this.DeckOptionsGroupBox.Controls.Add(this.PlayerPlayedCheckBox);
            this.DeckOptionsGroupBox.Controls.Add(this.PlayerDrawnCheckBox);
            this.DeckOptionsGroupBox.Controls.Add(this.PlayerDeckCheckBox);
            this.DeckOptionsGroupBox.Location = new System.Drawing.Point(564, 41);
            this.DeckOptionsGroupBox.Name = "DeckOptionsGroupBox";
            this.DeckOptionsGroupBox.Size = new System.Drawing.Size(151, 138);
            this.DeckOptionsGroupBox.TabIndex = 4;
            this.DeckOptionsGroupBox.TabStop = false;
            this.DeckOptionsGroupBox.Text = "Show Decks";
            // 
            // PlayerDeckCheckBox
            // 
            this.PlayerDeckCheckBox.AutoSize = true;
            this.PlayerDeckCheckBox.Checked = true;
            this.PlayerDeckCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PlayerDeckCheckBox.Location = new System.Drawing.Point(7, 20);
            this.PlayerDeckCheckBox.Name = "PlayerDeckCheckBox";
            this.PlayerDeckCheckBox.Size = new System.Drawing.Size(114, 17);
            this.PlayerDeckCheckBox.TabIndex = 0;
            this.PlayerDeckCheckBox.Text = "Your Current Deck";
            this.PlayerDeckCheckBox.UseVisualStyleBackColor = true;
            this.PlayerDeckCheckBox.CheckedChanged += new System.EventHandler(this.PlayerDeckCheckBox_CheckedChanged);
            // 
            // PlayerDrawnCheckBox
            // 
            this.PlayerDrawnCheckBox.AutoSize = true;
            this.PlayerDrawnCheckBox.Location = new System.Drawing.Point(7, 44);
            this.PlayerDrawnCheckBox.Name = "PlayerDrawnCheckBox";
            this.PlayerDrawnCheckBox.Size = new System.Drawing.Size(87, 17);
            this.PlayerDrawnCheckBox.TabIndex = 1;
            this.PlayerDrawnCheckBox.Text = "Drawn Cards";
            this.PlayerDrawnCheckBox.UseVisualStyleBackColor = true;
            this.PlayerDrawnCheckBox.CheckedChanged += new System.EventHandler(this.PlayerDrawnCheckBox_CheckedChanged);
            // 
            // PlayerPlayedCheckBox
            // 
            this.PlayerPlayedCheckBox.AutoSize = true;
            this.PlayerPlayedCheckBox.Location = new System.Drawing.Point(7, 68);
            this.PlayerPlayedCheckBox.Name = "PlayerPlayedCheckBox";
            this.PlayerPlayedCheckBox.Size = new System.Drawing.Size(88, 17);
            this.PlayerPlayedCheckBox.TabIndex = 2;
            this.PlayerPlayedCheckBox.Text = "Played Cards";
            this.PlayerPlayedCheckBox.UseVisualStyleBackColor = true;
            this.PlayerPlayedCheckBox.CheckedChanged += new System.EventHandler(this.PlayerPlayedCheckBox_CheckedChanged);
            // 
            // OpponentPlayedCheckBox
            // 
            this.OpponentPlayedCheckBox.AutoSize = true;
            this.OpponentPlayedCheckBox.Location = new System.Drawing.Point(7, 92);
            this.OpponentPlayedCheckBox.Name = "OpponentPlayedCheckBox";
            this.OpponentPlayedCheckBox.Size = new System.Drawing.Size(138, 17);
            this.OpponentPlayedCheckBox.TabIndex = 3;
            this.OpponentPlayedCheckBox.Text = "Opponent Played Cards";
            this.OpponentPlayedCheckBox.UseVisualStyleBackColor = true;
            this.OpponentPlayedCheckBox.CheckedChanged += new System.EventHandler(this.OpponentPlayedCheckBox_CheckedChanged);
            // 
            // DeckStatsCheckBox
            // 
            this.DeckStatsCheckBox.AutoSize = true;
            this.DeckStatsCheckBox.Location = new System.Drawing.Point(7, 115);
            this.DeckStatsCheckBox.Name = "DeckStatsCheckBox";
            this.DeckStatsCheckBox.Size = new System.Drawing.Size(79, 17);
            this.DeckStatsCheckBox.TabIndex = 3;
            this.DeckStatsCheckBox.Text = "Deck Stats";
            this.DeckStatsCheckBox.UseVisualStyleBackColor = true;
            this.DeckStatsCheckBox.CheckedChanged += new System.EventHandler(this.DecksStatsCheckBox_CheckedChanged);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(728, 221);
            this.Controls.Add(this.MyProgressDisplay);
            this.Controls.Add(this.DeckOptionsGroupBox);
            this.Controls.Add(this.SnapWindowsButton);
            this.Controls.Add(this.DebugLogsCheckBox);
            this.Controls.Add(this.LogTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(420, 260);
            this.Name = "MainWindow";
            this.Text = "LoR Side Tracker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.DeckOptionsGroupBox.ResumeLayout(false);
            this.DeckOptionsGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox LogTextBox;
        private ProgressDisplayControl MyProgressDisplay;
        private System.Windows.Forms.CheckBox DebugLogsCheckBox;
        private System.Windows.Forms.Button SnapWindowsButton;
        private System.Windows.Forms.GroupBox DeckOptionsGroupBox;
        private System.Windows.Forms.CheckBox OpponentPlayedCheckBox;
        private System.Windows.Forms.CheckBox PlayerPlayedCheckBox;
        private System.Windows.Forms.CheckBox PlayerDrawnCheckBox;
        private System.Windows.Forms.CheckBox PlayerDeckCheckBox;
        private System.Windows.Forms.CheckBox DeckStatsCheckBox;
    }
}

