namespace LoRSideTracker
{
    partial class OptionsWindow
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
            this.DeckOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TransparencyTrackBar = new System.Windows.Forms.TrackBar();
            this.DeckStatsCheckBox = new System.Windows.Forms.CheckBox();
            this.OpponentPlayedCheckBox = new System.Windows.Forms.CheckBox();
            this.PlayerPlayedCheckBox = new System.Windows.Forms.CheckBox();
            this.HideZeroCountCheckBox = new System.Windows.Forms.CheckBox();
            this.PlayerDrawnCheckBox = new System.Windows.Forms.CheckBox();
            this.PlayerDeckCheckBox = new System.Windows.Forms.CheckBox();
            this.CloseButton = new System.Windows.Forms.Button();
            this.DeckOptionsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TransparencyTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // DeckOptionsGroupBox
            // 
            this.DeckOptionsGroupBox.Controls.Add(this.label3);
            this.DeckOptionsGroupBox.Controls.Add(this.label2);
            this.DeckOptionsGroupBox.Controls.Add(this.label1);
            this.DeckOptionsGroupBox.Controls.Add(this.TransparencyTrackBar);
            this.DeckOptionsGroupBox.Controls.Add(this.DeckStatsCheckBox);
            this.DeckOptionsGroupBox.Controls.Add(this.OpponentPlayedCheckBox);
            this.DeckOptionsGroupBox.Controls.Add(this.PlayerPlayedCheckBox);
            this.DeckOptionsGroupBox.Controls.Add(this.HideZeroCountCheckBox);
            this.DeckOptionsGroupBox.Controls.Add(this.PlayerDrawnCheckBox);
            this.DeckOptionsGroupBox.Controls.Add(this.PlayerDeckCheckBox);
            this.DeckOptionsGroupBox.Location = new System.Drawing.Point(12, 12);
            this.DeckOptionsGroupBox.Name = "DeckOptionsGroupBox";
            this.DeckOptionsGroupBox.Size = new System.Drawing.Size(322, 216);
            this.DeckOptionsGroupBox.TabIndex = 5;
            this.DeckOptionsGroupBox.TabStop = false;
            this.DeckOptionsGroupBox.Text = "Show Decks";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(283, 181);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "100%";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 181);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "20%";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Deck Transparency Level:";
            // 
            // TransparencyTrackBar
            // 
            this.TransparencyTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TransparencyTrackBar.AutoSize = false;
            this.TransparencyTrackBar.LargeChange = 20;
            this.TransparencyTrackBar.Location = new System.Drawing.Point(34, 175);
            this.TransparencyTrackBar.Maximum = 100;
            this.TransparencyTrackBar.Minimum = 20;
            this.TransparencyTrackBar.Name = "TransparencyTrackBar";
            this.TransparencyTrackBar.Size = new System.Drawing.Size(243, 31);
            this.TransparencyTrackBar.SmallChange = 5;
            this.TransparencyTrackBar.TabIndex = 6;
            this.TransparencyTrackBar.TickFrequency = 10;
            this.TransparencyTrackBar.Value = 100;
            this.TransparencyTrackBar.ValueChanged += new System.EventHandler(this.TransparencyTrackBar_ValueChanged);
            // 
            // DeckStatsCheckBox
            // 
            this.DeckStatsCheckBox.AutoSize = true;
            this.DeckStatsCheckBox.Location = new System.Drawing.Point(7, 135);
            this.DeckStatsCheckBox.Name = "DeckStatsCheckBox";
            this.DeckStatsCheckBox.Size = new System.Drawing.Size(84, 17);
            this.DeckStatsCheckBox.TabIndex = 5;
            this.DeckStatsCheckBox.Text = "Mana Curve";
            this.DeckStatsCheckBox.UseVisualStyleBackColor = true;
            this.DeckStatsCheckBox.CheckedChanged += new System.EventHandler(this.DeckStatsCheckBox_CheckedChanged);
            // 
            // OpponentPlayedCheckBox
            // 
            this.OpponentPlayedCheckBox.AutoSize = true;
            this.OpponentPlayedCheckBox.Location = new System.Drawing.Point(7, 112);
            this.OpponentPlayedCheckBox.Name = "OpponentPlayedCheckBox";
            this.OpponentPlayedCheckBox.Size = new System.Drawing.Size(138, 17);
            this.OpponentPlayedCheckBox.TabIndex = 4;
            this.OpponentPlayedCheckBox.Text = "Opponent Played Cards";
            this.OpponentPlayedCheckBox.UseVisualStyleBackColor = true;
            this.OpponentPlayedCheckBox.CheckedChanged += new System.EventHandler(this.OpponentPlayedCheckBox_CheckedChanged);
            // 
            // PlayerPlayedCheckBox
            // 
            this.PlayerPlayedCheckBox.AutoSize = true;
            this.PlayerPlayedCheckBox.Location = new System.Drawing.Point(7, 89);
            this.PlayerPlayedCheckBox.Name = "PlayerPlayedCheckBox";
            this.PlayerPlayedCheckBox.Size = new System.Drawing.Size(88, 17);
            this.PlayerPlayedCheckBox.TabIndex = 3;
            this.PlayerPlayedCheckBox.Text = "Played Cards";
            this.PlayerPlayedCheckBox.UseVisualStyleBackColor = true;
            this.PlayerPlayedCheckBox.CheckedChanged += new System.EventHandler(this.PlayerPlayedCheckBox_CheckedChanged);
            // 
            // HideZeroCountCheckBox
            // 
            this.HideZeroCountCheckBox.AutoSize = true;
            this.HideZeroCountCheckBox.Location = new System.Drawing.Point(17, 43);
            this.HideZeroCountCheckBox.Name = "HideZeroCountCheckBox";
            this.HideZeroCountCheckBox.Size = new System.Drawing.Size(104, 17);
            this.HideZeroCountCheckBox.TabIndex = 1;
            this.HideZeroCountCheckBox.Text = "Hide Zero Count";
            this.HideZeroCountCheckBox.UseVisualStyleBackColor = true;
            this.HideZeroCountCheckBox.CheckedChanged += new System.EventHandler(this.HideZeroCountCheckBox_CheckedChanged);
            // 
            // PlayerDrawnCheckBox
            // 
            this.PlayerDrawnCheckBox.AutoSize = true;
            this.PlayerDrawnCheckBox.Location = new System.Drawing.Point(7, 66);
            this.PlayerDrawnCheckBox.Name = "PlayerDrawnCheckBox";
            this.PlayerDrawnCheckBox.Size = new System.Drawing.Size(87, 17);
            this.PlayerDrawnCheckBox.TabIndex = 2;
            this.PlayerDrawnCheckBox.Text = "Drawn Cards";
            this.PlayerDrawnCheckBox.UseVisualStyleBackColor = true;
            this.PlayerDrawnCheckBox.CheckedChanged += new System.EventHandler(this.PlayerDrawnCheckBox_CheckedChanged);
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
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(261, 234);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 7;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // OptionsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 264);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.DeckOptionsGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsWindow";
            this.Text = "Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OptionsWindow_FormClosing);
            this.Load += new System.EventHandler(this.OptionsWindow_Load);
            this.DeckOptionsGroupBox.ResumeLayout(false);
            this.DeckOptionsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TransparencyTrackBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox DeckOptionsGroupBox;
        private System.Windows.Forms.TrackBar TransparencyTrackBar;
        private System.Windows.Forms.CheckBox DeckStatsCheckBox;
        private System.Windows.Forms.CheckBox OpponentPlayedCheckBox;
        private System.Windows.Forms.CheckBox PlayerPlayedCheckBox;
        private System.Windows.Forms.CheckBox HideZeroCountCheckBox;
        private System.Windows.Forms.CheckBox PlayerDrawnCheckBox;
        private System.Windows.Forms.CheckBox PlayerDeckCheckBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button CloseButton;
    }
}