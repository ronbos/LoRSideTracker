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
            this.SnapWindowsButton = new System.Windows.Forms.Button();
            this.OptionsButton = new System.Windows.Forms.Button();
            this.LogButton = new System.Windows.Forms.Button();
            this.DecksListBox = new System.Windows.Forms.ListBox();
            this.DeckPanel = new System.Windows.Forms.Panel();
            this.DecksLabel = new System.Windows.Forms.Label();
            this.HighlightedDeckPanel = new System.Windows.Forms.Panel();
            this.MyProgressDisplay = new LoRSideTracker.ProgressDisplayControl();
            this.HighlightedDeckStatsDisplay = new LoRSideTracker.DeckStatsDisplay();
            this.HighlightedDeckControl = new LoRSideTracker.DeckControl();
            this.HighlightedGameLogControl = new LoRSideTracker.GameLogControl();
            this.DeckPanel.SuspendLayout();
            this.HighlightedDeckPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // SnapWindowsButton
            // 
            this.SnapWindowsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SnapWindowsButton.Location = new System.Drawing.Point(911, 10);
            this.SnapWindowsButton.Name = "SnapWindowsButton";
            this.SnapWindowsButton.Size = new System.Drawing.Size(85, 23);
            this.SnapWindowsButton.TabIndex = 3;
            this.SnapWindowsButton.Text = "Snap Decks";
            this.SnapWindowsButton.UseVisualStyleBackColor = true;
            this.SnapWindowsButton.Visible = false;
            this.SnapWindowsButton.Click += new System.EventHandler(this.SnapWindowsButton_Click);
            // 
            // OptionsButton
            // 
            this.OptionsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OptionsButton.Location = new System.Drawing.Point(911, 39);
            this.OptionsButton.Name = "OptionsButton";
            this.OptionsButton.Size = new System.Drawing.Size(85, 23);
            this.OptionsButton.TabIndex = 3;
            this.OptionsButton.Text = "Options";
            this.OptionsButton.UseVisualStyleBackColor = true;
            this.OptionsButton.Visible = false;
            this.OptionsButton.Click += new System.EventHandler(this.OptionsButton_Click);
            // 
            // LogButton
            // 
            this.LogButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LogButton.Location = new System.Drawing.Point(911, 68);
            this.LogButton.Name = "LogButton";
            this.LogButton.Size = new System.Drawing.Size(85, 23);
            this.LogButton.TabIndex = 3;
            this.LogButton.Text = "Log";
            this.LogButton.UseVisualStyleBackColor = true;
            this.LogButton.Visible = false;
            this.LogButton.Click += new System.EventHandler(this.LogButton_Click);
            // 
            // DecksListBox
            // 
            this.DecksListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DecksListBox.CausesValidation = false;
            this.DecksListBox.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DecksListBox.IntegralHeight = false;
            this.DecksListBox.ItemHeight = 14;
            this.DecksListBox.Location = new System.Drawing.Point(9, 33);
            this.DecksListBox.Margin = new System.Windows.Forms.Padding(0);
            this.DecksListBox.MinimumSize = new System.Drawing.Size(120, 120);
            this.DecksListBox.Name = "DecksListBox";
            this.DecksListBox.Size = new System.Drawing.Size(131, 518);
            this.DecksListBox.TabIndex = 4;
            this.DecksListBox.Visible = false;
            this.DecksListBox.SelectedIndexChanged += new System.EventHandler(this.DecksListBox_SelectedIndexChanged);
            this.DecksListBox.DoubleClick += new System.EventHandler(this.DecksListBox_DoubleClick);
            // 
            // DeckPanel
            // 
            this.DeckPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DeckPanel.AutoScroll = true;
            this.DeckPanel.Controls.Add(this.HighlightedGameLogControl);
            this.DeckPanel.Location = new System.Drawing.Point(146, 9);
            this.DeckPanel.Name = "DeckPanel";
            this.DeckPanel.Size = new System.Drawing.Size(554, 543);
            this.DeckPanel.TabIndex = 5;
            this.DeckPanel.Visible = false;
            // 
            // DecksLabel
            // 
            this.DecksLabel.AutoSize = true;
            this.DecksLabel.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DecksLabel.Location = new System.Drawing.Point(10, 15);
            this.DecksLabel.Name = "DecksLabel";
            this.DecksLabel.Size = new System.Drawing.Size(37, 14);
            this.DecksLabel.TabIndex = 6;
            this.DecksLabel.Text = "Decks";
            this.DecksLabel.Visible = false;
            // 
            // HighlightedDeckPanel
            // 
            this.HighlightedDeckPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HighlightedDeckPanel.AutoScroll = true;
            this.HighlightedDeckPanel.Controls.Add(this.HighlightedDeckStatsDisplay);
            this.HighlightedDeckPanel.Controls.Add(this.HighlightedDeckControl);
            this.HighlightedDeckPanel.Location = new System.Drawing.Point(703, 9);
            this.HighlightedDeckPanel.Margin = new System.Windows.Forms.Padding(0);
            this.HighlightedDeckPanel.Name = "HighlightedDeckPanel";
            this.HighlightedDeckPanel.Size = new System.Drawing.Size(200, 543);
            this.HighlightedDeckPanel.TabIndex = 7;
            this.HighlightedDeckPanel.Visible = false;
            // 
            // MyProgressDisplay
            // 
            this.MyProgressDisplay.Location = new System.Drawing.Point(925, 405);
            this.MyProgressDisplay.MaximumSize = new System.Drawing.Size(360, 120);
            this.MyProgressDisplay.MinimumSize = new System.Drawing.Size(360, 120);
            this.MyProgressDisplay.Name = "MyProgressDisplay";
            this.MyProgressDisplay.Size = new System.Drawing.Size(360, 120);
            this.MyProgressDisplay.TabIndex = 9;
            this.MyProgressDisplay.TabStop = false;
            // 
            // HighlightedDeckStatsDisplay
            // 
            this.HighlightedDeckStatsDisplay.BlockHeight = 4;
            this.HighlightedDeckStatsDisplay.BlockWidth = 7;
            this.HighlightedDeckStatsDisplay.Location = new System.Drawing.Point(0, 156);
            this.HighlightedDeckStatsDisplay.Name = "HighlightedDeckStatsDisplay";
            this.HighlightedDeckStatsDisplay.Size = new System.Drawing.Size(177, 65);
            this.HighlightedDeckStatsDisplay.SpellColor = System.Drawing.Color.MediumSeaGreen;
            this.HighlightedDeckStatsDisplay.TabIndex = 4;
            this.HighlightedDeckStatsDisplay.TextColor = System.Drawing.Color.White;
            this.HighlightedDeckStatsDisplay.TheDeck = null;
            this.HighlightedDeckStatsDisplay.UnitColor = System.Drawing.Color.RoyalBlue;
            // 
            // HighlightedDeckControl
            // 
            this.HighlightedDeckControl.BackColor = System.Drawing.Color.Black;
            this.HighlightedDeckControl.IsMinimized = false;
            this.HighlightedDeckControl.Location = new System.Drawing.Point(0, 0);
            this.HighlightedDeckControl.Name = "HighlightedDeckControl";
            this.HighlightedDeckControl.Size = new System.Drawing.Size(180, 150);
            this.HighlightedDeckControl.TabIndex = 2;
            this.HighlightedDeckControl.Title = null;
            // 
            // HighlightedGameLogControl
            // 
            this.HighlightedGameLogControl.Location = new System.Drawing.Point(0, 0);
            this.HighlightedGameLogControl.Name = "HighlightedGameLogControl";
            this.HighlightedGameLogControl.Size = new System.Drawing.Size(546, 249);
            this.HighlightedGameLogControl.TabIndex = 2;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1008, 561);
            this.Controls.Add(this.MyProgressDisplay);
            this.Controls.Add(this.HighlightedDeckPanel);
            this.Controls.Add(this.DecksLabel);
            this.Controls.Add(this.DecksListBox);
            this.Controls.Add(this.LogButton);
            this.Controls.Add(this.OptionsButton);
            this.Controls.Add(this.SnapWindowsButton);
            this.Controls.Add(this.DeckPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1024, 400);
            this.Name = "MainWindow";
            this.Text = "LoR Side Tracker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.SizeChanged += new System.EventHandler(this.MainWindow_SizeChanged);
            this.DeckPanel.ResumeLayout(false);
            this.HighlightedDeckPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button SnapWindowsButton;
        private System.Windows.Forms.Button OptionsButton;
        private System.Windows.Forms.Button LogButton;
        private System.Windows.Forms.ListBox DecksListBox;
        private System.Windows.Forms.Panel DeckPanel;
        private GameLogControl HighlightedGameLogControl;
        private System.Windows.Forms.Label DecksLabel;
        private System.Windows.Forms.Panel HighlightedDeckPanel;
        private DeckStatsDisplay HighlightedDeckStatsDisplay;
        private DeckControl HighlightedDeckControl;
        private ProgressDisplayControl MyProgressDisplay;
    }
}

