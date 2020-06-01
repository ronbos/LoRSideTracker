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
            this.HighlightedGameLogControl = new LoRSideTracker.GameLogControl();
            this.HighlightedDeckPanel = new System.Windows.Forms.Panel();
            this.HighlightedDeckStatsDisplay = new LoRSideTracker.DeckStatsDisplay();
            this.HighlightedDeckControl = new LoRSideTracker.DeckControl();
            this.MyProgressDisplay = new LoRSideTracker.ProgressDisplayControl();
            this.DecksButton = new System.Windows.Forms.Button();
            this.ExpeditionsButton = new System.Windows.Forms.Button();
            this.ExpeditionsListBox = new System.Windows.Forms.ListBox();
            this.DeckPanel.SuspendLayout();
            this.HighlightedDeckPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // SnapWindowsButton
            // 
            this.SnapWindowsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SnapWindowsButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.SnapWindowsButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.SnapWindowsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.SnapWindowsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SnapWindowsButton.Location = new System.Drawing.Point(981, 10);
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
            this.OptionsButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.OptionsButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.OptionsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.OptionsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OptionsButton.Location = new System.Drawing.Point(981, 39);
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
            this.LogButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(36)))));
            this.LogButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.LogButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.LogButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.LogButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LogButton.Location = new System.Drawing.Point(981, 68);
            this.LogButton.Name = "LogButton";
            this.LogButton.Size = new System.Drawing.Size(85, 23);
            this.LogButton.TabIndex = 3;
            this.LogButton.Text = "Log";
            this.LogButton.UseVisualStyleBackColor = false;
            this.LogButton.Visible = false;
            this.LogButton.Click += new System.EventHandler(this.LogButton_Click);
            // 
            // DecksListBox
            // 
            this.DecksListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DecksListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(36)))));
            this.DecksListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DecksListBox.CausesValidation = false;
            this.DecksListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.DecksListBox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DecksListBox.ForeColor = System.Drawing.Color.LightYellow;
            this.DecksListBox.IntegralHeight = false;
            this.DecksListBox.ItemHeight = 30;
            this.DecksListBox.Location = new System.Drawing.Point(9, 38);
            this.DecksListBox.Margin = new System.Windows.Forms.Padding(0);
            this.DecksListBox.MinimumSize = new System.Drawing.Size(198, 120);
            this.DecksListBox.Name = "DecksListBox";
            this.DecksListBox.Size = new System.Drawing.Size(198, 512);
            this.DecksListBox.TabIndex = 4;
            this.DecksListBox.Visible = false;
            this.DecksListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.DecksListBox_DrawItem);
            this.DecksListBox.SelectedIndexChanged += new System.EventHandler(this.DecksListBox_SelectedIndexChanged);
            this.DecksListBox.DoubleClick += new System.EventHandler(this.DecksListBox_DoubleClick);
            // 
            // DeckPanel
            // 
            this.DeckPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DeckPanel.AutoScroll = true;
            this.DeckPanel.Controls.Add(this.HighlightedGameLogControl);
            this.DeckPanel.Location = new System.Drawing.Point(216, 9);
            this.DeckPanel.Name = "DeckPanel";
            this.DeckPanel.Size = new System.Drawing.Size(554, 543);
            this.DeckPanel.TabIndex = 5;
            this.DeckPanel.Visible = false;
            // 
            // HighlightedGameLogControl
            // 
            this.HighlightedGameLogControl.ForeColor = System.Drawing.Color.LightYellow;
            this.HighlightedGameLogControl.Location = new System.Drawing.Point(0, 0);
            this.HighlightedGameLogControl.Name = "HighlightedGameLogControl";
            this.HighlightedGameLogControl.Size = new System.Drawing.Size(546, 249);
            this.HighlightedGameLogControl.TabIndex = 2;
            // 
            // HighlightedDeckPanel
            // 
            this.HighlightedDeckPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HighlightedDeckPanel.AutoScroll = true;
            this.HighlightedDeckPanel.Controls.Add(this.HighlightedDeckStatsDisplay);
            this.HighlightedDeckPanel.Controls.Add(this.HighlightedDeckControl);
            this.HighlightedDeckPanel.Location = new System.Drawing.Point(773, 9);
            this.HighlightedDeckPanel.Margin = new System.Windows.Forms.Padding(0);
            this.HighlightedDeckPanel.Name = "HighlightedDeckPanel";
            this.HighlightedDeckPanel.Size = new System.Drawing.Size(200, 543);
            this.HighlightedDeckPanel.TabIndex = 7;
            this.HighlightedDeckPanel.Visible = false;
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
            // DecksButton
            // 
            this.DecksButton.FlatAppearance.BorderColor = System.Drawing.Color.LightYellow;
            this.DecksButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.DecksButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.DecksButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DecksButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DecksButton.Location = new System.Drawing.Point(7, 11);
            this.DecksButton.Margin = new System.Windows.Forms.Padding(0);
            this.DecksButton.Name = "DecksButton";
            this.DecksButton.Size = new System.Drawing.Size(100, 23);
            this.DecksButton.TabIndex = 11;
            this.DecksButton.Text = "Decks";
            this.DecksButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.DecksButton.UseVisualStyleBackColor = true;
            this.DecksButton.Visible = false;
            this.DecksButton.Click += new System.EventHandler(this.DecksButton_Click);
            // 
            // ExpeditionsButton
            // 
            this.ExpeditionsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(36)))));
            this.ExpeditionsButton.FlatAppearance.BorderColor = System.Drawing.Color.LightYellow;
            this.ExpeditionsButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ExpeditionsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.ExpeditionsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExpeditionsButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExpeditionsButton.Location = new System.Drawing.Point(106, 11);
            this.ExpeditionsButton.Margin = new System.Windows.Forms.Padding(0);
            this.ExpeditionsButton.Name = "ExpeditionsButton";
            this.ExpeditionsButton.Size = new System.Drawing.Size(100, 23);
            this.ExpeditionsButton.TabIndex = 12;
            this.ExpeditionsButton.Text = "Expeditions";
            this.ExpeditionsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ExpeditionsButton.UseVisualStyleBackColor = false;
            this.ExpeditionsButton.Visible = false;
            this.ExpeditionsButton.Click += new System.EventHandler(this.ExpeditionsButton_Click);
            // 
            // ExpeditionsListBox
            // 
            this.ExpeditionsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ExpeditionsListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(36)))));
            this.ExpeditionsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ExpeditionsListBox.CausesValidation = false;
            this.ExpeditionsListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ExpeditionsListBox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExpeditionsListBox.ForeColor = System.Drawing.Color.LightYellow;
            this.ExpeditionsListBox.IntegralHeight = false;
            this.ExpeditionsListBox.ItemHeight = 30;
            this.ExpeditionsListBox.Location = new System.Drawing.Point(9, 38);
            this.ExpeditionsListBox.Margin = new System.Windows.Forms.Padding(0);
            this.ExpeditionsListBox.MinimumSize = new System.Drawing.Size(198, 120);
            this.ExpeditionsListBox.Name = "ExpeditionsListBox";
            this.ExpeditionsListBox.Size = new System.Drawing.Size(198, 512);
            this.ExpeditionsListBox.TabIndex = 13;
            this.ExpeditionsListBox.Visible = false;
            this.ExpeditionsListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ExpeditionsListBox_DrawItem);
            this.ExpeditionsListBox.SelectedIndexChanged += new System.EventHandler(this.ExpeditionsListBox_SelectedIndexChanged);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(1078, 561);
            this.Controls.Add(this.ExpeditionsListBox);
            this.Controls.Add(this.ExpeditionsButton);
            this.Controls.Add(this.DecksButton);
            this.Controls.Add(this.MyProgressDisplay);
            this.Controls.Add(this.HighlightedDeckPanel);
            this.Controls.Add(this.DecksListBox);
            this.Controls.Add(this.LogButton);
            this.Controls.Add(this.OptionsButton);
            this.Controls.Add(this.SnapWindowsButton);
            this.Controls.Add(this.DeckPanel);
            this.ForeColor = System.Drawing.Color.LightYellow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1094, 400);
            this.Name = "MainWindow";
            this.Text = "LoR Side Tracker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.SizeChanged += new System.EventHandler(this.MainWindow_SizeChanged);
            this.DeckPanel.ResumeLayout(false);
            this.HighlightedDeckPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button SnapWindowsButton;
        private System.Windows.Forms.Button OptionsButton;
        private System.Windows.Forms.Button LogButton;
        private System.Windows.Forms.ListBox DecksListBox;
        private System.Windows.Forms.Panel DeckPanel;
        private GameLogControl HighlightedGameLogControl;
        private System.Windows.Forms.Panel HighlightedDeckPanel;
        private DeckStatsDisplay HighlightedDeckStatsDisplay;
        private DeckControl HighlightedDeckControl;
        private ProgressDisplayControl MyProgressDisplay;
        private System.Windows.Forms.Button DecksButton;
        private System.Windows.Forms.Button ExpeditionsButton;
        private System.Windows.Forms.ListBox ExpeditionsListBox;
    }
}

