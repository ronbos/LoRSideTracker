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
            this.DecksListBox = new System.Windows.Forms.ListBox();
            this.DeckPanel = new System.Windows.Forms.Panel();
            this.HighlightedGameLogControl = new LoRSideTracker.GameLogControl();
            this.HighlightedDeckPanel = new System.Windows.Forms.Panel();
            this.HighlightedDeckStatsDisplay = new LoRSideTracker.DeckStatsDisplay();
            this.HighlightedDeckControl = new LoRSideTracker.DeckControl();
            this.DecksButton = new System.Windows.Forms.Button();
            this.ExpeditionsButton = new System.Windows.Forms.Button();
            this.ExpeditionsListBox = new System.Windows.Forms.ListBox();
            this.MyProgressDisplay = new LoRSideTracker.ProgressDisplayControl();
            this.MyMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.myDeckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawnCardsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playedCardsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.opponentPlayedCardsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.hideZeroCountCardsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.deckWindowSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallDeckSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mediumDeckSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.largeDeckSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deckOpacityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deckOpacity20ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deckOpacity40ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deckOpacity60ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deckOpacity80ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deckOpacity100ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.snapDecksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeckPanel.SuspendLayout();
            this.HighlightedDeckPanel.SuspendLayout();
            this.MyMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // DecksListBox
            // 
            this.DecksListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.DecksListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(36)))));
            this.DecksListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DecksListBox.CausesValidation = false;
            this.DecksListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.DecksListBox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DecksListBox.ForeColor = System.Drawing.Color.LightYellow;
            this.DecksListBox.IntegralHeight = false;
            this.DecksListBox.ItemHeight = 30;
            this.DecksListBox.Location = new System.Drawing.Point(9, 58);
            this.DecksListBox.Margin = new System.Windows.Forms.Padding(0);
            this.DecksListBox.MinimumSize = new System.Drawing.Size(198, 120);
            this.DecksListBox.Name = "DecksListBox";
            this.DecksListBox.Size = new System.Drawing.Size(198, 492);
            this.DecksListBox.TabIndex = 4;
            this.DecksListBox.TabStop = false;
            this.DecksListBox.Visible = false;
            this.DecksListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ListBox_DrawItem);
            this.DecksListBox.SelectedIndexChanged += new System.EventHandler(this.ListBox_SelectedIndexChanged);
            this.DecksListBox.SizeChanged += new System.EventHandler(this.ListBox_SizeChanged);
            this.DecksListBox.DoubleClick += new System.EventHandler(this.ListBox_DoubleClick);
            // 
            // DeckPanel
            // 
            this.DeckPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.DeckPanel.AutoScroll = true;
            this.DeckPanel.Controls.Add(this.HighlightedGameLogControl);
            this.DeckPanel.Location = new System.Drawing.Point(216, 29);
            this.DeckPanel.Name = "DeckPanel";
            this.DeckPanel.Size = new System.Drawing.Size(562, 523);
            this.DeckPanel.TabIndex = 5;
            this.DeckPanel.Visible = false;
            // 
            // HighlightedGameLogControl
            // 
            this.HighlightedGameLogControl.ForeColor = System.Drawing.Color.LightYellow;
            this.HighlightedGameLogControl.Location = new System.Drawing.Point(0, 0);
            this.HighlightedGameLogControl.Name = "HighlightedGameLogControl";
            this.HighlightedGameLogControl.Size = new System.Drawing.Size(541, 503);
            this.HighlightedGameLogControl.TabIndex = 2;
            this.HighlightedGameLogControl.TabStop = false;
            // 
            // HighlightedDeckPanel
            // 
            this.HighlightedDeckPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HighlightedDeckPanel.AutoScroll = true;
            this.HighlightedDeckPanel.Controls.Add(this.HighlightedDeckStatsDisplay);
            this.HighlightedDeckPanel.Controls.Add(this.HighlightedDeckControl);
            this.HighlightedDeckPanel.Location = new System.Drawing.Point(781, 32);
            this.HighlightedDeckPanel.Margin = new System.Windows.Forms.Padding(0);
            this.HighlightedDeckPanel.Name = "HighlightedDeckPanel";
            this.HighlightedDeckPanel.Size = new System.Drawing.Size(160, 523);
            this.HighlightedDeckPanel.TabIndex = 7;
            this.HighlightedDeckPanel.Visible = false;
            this.HighlightedDeckPanel.SizeChanged += new System.EventHandler(this.HighlightedDeckPanel_SizeChanged);
            // 
            // HighlightedDeckStatsDisplay
            // 
            this.HighlightedDeckStatsDisplay.BlockHeight = 4;
            this.HighlightedDeckStatsDisplay.BlockWidth = 7;
            this.HighlightedDeckStatsDisplay.Location = new System.Drawing.Point(0, 156);
            this.HighlightedDeckStatsDisplay.Name = "HighlightedDeckStatsDisplay";
            this.HighlightedDeckStatsDisplay.Size = new System.Drawing.Size(140, 65);
            this.HighlightedDeckStatsDisplay.SpellColor = System.Drawing.Color.MediumSeaGreen;
            this.HighlightedDeckStatsDisplay.TabIndex = 4;
            this.HighlightedDeckStatsDisplay.TabStop = false;
            this.HighlightedDeckStatsDisplay.TextColor = System.Drawing.Color.White;
            this.HighlightedDeckStatsDisplay.TheDeck = null;
            this.HighlightedDeckStatsDisplay.UnitColor = System.Drawing.Color.RoyalBlue;
            // 
            // HighlightedDeckControl
            // 
            this.HighlightedDeckControl.BackColor = System.Drawing.Color.Black;
            this.HighlightedDeckControl.BorderSize = 1;
            this.HighlightedDeckControl.IsMinimized = false;
            this.HighlightedDeckControl.Location = new System.Drawing.Point(0, 0);
            this.HighlightedDeckControl.Name = "HighlightedDeckControl";
            this.HighlightedDeckControl.Size = new System.Drawing.Size(140, 150);
            this.HighlightedDeckControl.TabIndex = 2;
            this.HighlightedDeckControl.TabStop = false;
            this.HighlightedDeckControl.Title = null;
            // 
            // DecksButton
            // 
            this.DecksButton.FlatAppearance.BorderColor = System.Drawing.Color.LightYellow;
            this.DecksButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.DecksButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.DecksButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DecksButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DecksButton.Location = new System.Drawing.Point(7, 31);
            this.DecksButton.Margin = new System.Windows.Forms.Padding(0);
            this.DecksButton.Name = "DecksButton";
            this.DecksButton.Size = new System.Drawing.Size(100, 23);
            this.DecksButton.TabIndex = 11;
            this.DecksButton.TabStop = false;
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
            this.ExpeditionsButton.Location = new System.Drawing.Point(106, 31);
            this.ExpeditionsButton.Margin = new System.Windows.Forms.Padding(0);
            this.ExpeditionsButton.Name = "ExpeditionsButton";
            this.ExpeditionsButton.Size = new System.Drawing.Size(100, 23);
            this.ExpeditionsButton.TabIndex = 12;
            this.ExpeditionsButton.TabStop = false;
            this.ExpeditionsButton.Text = "Expeditions";
            this.ExpeditionsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ExpeditionsButton.UseVisualStyleBackColor = false;
            this.ExpeditionsButton.Visible = false;
            this.ExpeditionsButton.Click += new System.EventHandler(this.ExpeditionsButton_Click);
            // 
            // ExpeditionsListBox
            // 
            this.ExpeditionsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ExpeditionsListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(36)))));
            this.ExpeditionsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ExpeditionsListBox.CausesValidation = false;
            this.ExpeditionsListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ExpeditionsListBox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExpeditionsListBox.ForeColor = System.Drawing.Color.LightYellow;
            this.ExpeditionsListBox.IntegralHeight = false;
            this.ExpeditionsListBox.ItemHeight = 30;
            this.ExpeditionsListBox.Location = new System.Drawing.Point(9, 58);
            this.ExpeditionsListBox.Margin = new System.Windows.Forms.Padding(0);
            this.ExpeditionsListBox.MinimumSize = new System.Drawing.Size(198, 120);
            this.ExpeditionsListBox.Name = "ExpeditionsListBox";
            this.ExpeditionsListBox.Size = new System.Drawing.Size(198, 492);
            this.ExpeditionsListBox.TabIndex = 13;
            this.ExpeditionsListBox.TabStop = false;
            this.ExpeditionsListBox.Visible = false;
            this.ExpeditionsListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ListBox_DrawItem);
            this.ExpeditionsListBox.SelectedIndexChanged += new System.EventHandler(this.ListBox_SelectedIndexChanged);
            this.ExpeditionsListBox.SizeChanged += new System.EventHandler(this.ListBox_SizeChanged);
            this.ExpeditionsListBox.DoubleClick += new System.EventHandler(this.ListBox_DoubleClick);
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
            // MyMenuStrip
            // 
            this.MyMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.windowToolStripMenuItem});
            this.MyMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MyMenuStrip.Name = "MyMenuStrip";
            this.MyMenuStrip.Size = new System.Drawing.Size(949, 24);
            this.MyMenuStrip.TabIndex = 14;
            this.MyMenuStrip.Text = "MyMenuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.logToolStripMenuItem,
            this.toolStripMenuItem8,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.DropDownOpened += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpened);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.logToolStripMenuItem.Text = "Log";
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(113, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.myDeckToolStripMenuItem,
            this.drawnCardsToolStripMenuItem,
            this.playedCardsToolStripMenuItem,
            this.opponentPlayedCardsToolStripMenuItem,
            this.toolStripMenuItem3,
            this.hideZeroCountCardsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.deckWindowSizeToolStripMenuItem,
            this.deckOpacityToolStripMenuItem,
            this.toolStripMenuItem1,
            this.snapDecksToolStripMenuItem});
            this.windowToolStripMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowToolStripMenuItem.Text = "Window";
            this.windowToolStripMenuItem.DropDownOpened += new System.EventHandler(this.windowToolStripMenuItem_DropDownOpened);
            // 
            // myDeckToolStripMenuItem
            // 
            this.myDeckToolStripMenuItem.Name = "myDeckToolStripMenuItem";
            this.myDeckToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.myDeckToolStripMenuItem.Text = "My Deck";
            this.myDeckToolStripMenuItem.Click += new System.EventHandler(this.myDeckToolStripMenuItem_Click);
            // 
            // drawnCardsToolStripMenuItem
            // 
            this.drawnCardsToolStripMenuItem.Name = "drawnCardsToolStripMenuItem";
            this.drawnCardsToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.drawnCardsToolStripMenuItem.Text = "Drawn Cards";
            this.drawnCardsToolStripMenuItem.Click += new System.EventHandler(this.drawnCardsToolStripMenuItem_Click);
            // 
            // playedCardsToolStripMenuItem
            // 
            this.playedCardsToolStripMenuItem.Name = "playedCardsToolStripMenuItem";
            this.playedCardsToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.playedCardsToolStripMenuItem.Text = "Played Cards";
            this.playedCardsToolStripMenuItem.Click += new System.EventHandler(this.playedCardsToolStripMenuItem_Click);
            // 
            // opponentPlayedCardsToolStripMenuItem
            // 
            this.opponentPlayedCardsToolStripMenuItem.Name = "opponentPlayedCardsToolStripMenuItem";
            this.opponentPlayedCardsToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.opponentPlayedCardsToolStripMenuItem.Text = "Opponent Played Cards";
            this.opponentPlayedCardsToolStripMenuItem.Click += new System.EventHandler(this.opponentPlayedCardsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(196, 6);
            // 
            // hideZeroCountCardsToolStripMenuItem
            // 
            this.hideZeroCountCardsToolStripMenuItem.Name = "hideZeroCountCardsToolStripMenuItem";
            this.hideZeroCountCardsToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.hideZeroCountCardsToolStripMenuItem.Text = "Hide Zero Count Cards";
            this.hideZeroCountCardsToolStripMenuItem.Click += new System.EventHandler(this.hideZeroCountCardsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(196, 6);
            // 
            // deckWindowSizeToolStripMenuItem
            // 
            this.deckWindowSizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.smallDeckSizeToolStripMenuItem,
            this.mediumDeckSizeToolStripMenuItem,
            this.largeDeckSizeToolStripMenuItem});
            this.deckWindowSizeToolStripMenuItem.Name = "deckWindowSizeToolStripMenuItem";
            this.deckWindowSizeToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.deckWindowSizeToolStripMenuItem.Text = "Deck Window Size";
            // 
            // smallDeckSizeToolStripMenuItem
            // 
            this.smallDeckSizeToolStripMenuItem.Name = "smallDeckSizeToolStripMenuItem";
            this.smallDeckSizeToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.smallDeckSizeToolStripMenuItem.Text = "Small";
            this.smallDeckSizeToolStripMenuItem.Click += new System.EventHandler(this.smallDeckSizeToolStripMenuItem_Click);
            // 
            // mediumDeckSizeToolStripMenuItem
            // 
            this.mediumDeckSizeToolStripMenuItem.Name = "mediumDeckSizeToolStripMenuItem";
            this.mediumDeckSizeToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.mediumDeckSizeToolStripMenuItem.Text = "Medium";
            this.mediumDeckSizeToolStripMenuItem.Click += new System.EventHandler(this.mediumDeckSizeToolStripMenuItem_Click);
            // 
            // largeDeckSizeToolStripMenuItem
            // 
            this.largeDeckSizeToolStripMenuItem.Name = "largeDeckSizeToolStripMenuItem";
            this.largeDeckSizeToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.largeDeckSizeToolStripMenuItem.Text = "Large";
            this.largeDeckSizeToolStripMenuItem.Click += new System.EventHandler(this.largeDeckSizeToolStripMenuItem_Click);
            // 
            // deckOpacityToolStripMenuItem
            // 
            this.deckOpacityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deckOpacity20ToolStripMenuItem,
            this.deckOpacity40ToolStripMenuItem,
            this.deckOpacity60ToolStripMenuItem,
            this.deckOpacity80ToolStripMenuItem,
            this.deckOpacity100ToolStripMenuItem});
            this.deckOpacityToolStripMenuItem.Name = "deckOpacityToolStripMenuItem";
            this.deckOpacityToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.deckOpacityToolStripMenuItem.Text = "Deck Opacity";
            // 
            // deckOpacity20ToolStripMenuItem
            // 
            this.deckOpacity20ToolStripMenuItem.Name = "deckOpacity20ToolStripMenuItem";
            this.deckOpacity20ToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.deckOpacity20ToolStripMenuItem.Text = "20%";
            this.deckOpacity20ToolStripMenuItem.Click += new System.EventHandler(this.deckOpacity20ToolStripMenuItem_Click);
            // 
            // deckOpacity40ToolStripMenuItem
            // 
            this.deckOpacity40ToolStripMenuItem.Name = "deckOpacity40ToolStripMenuItem";
            this.deckOpacity40ToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.deckOpacity40ToolStripMenuItem.Text = "40%";
            this.deckOpacity40ToolStripMenuItem.Click += new System.EventHandler(this.deckOpacity40ToolStripMenuItem_Click);
            // 
            // deckOpacity60ToolStripMenuItem
            // 
            this.deckOpacity60ToolStripMenuItem.Name = "deckOpacity60ToolStripMenuItem";
            this.deckOpacity60ToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.deckOpacity60ToolStripMenuItem.Text = "60%";
            this.deckOpacity60ToolStripMenuItem.Click += new System.EventHandler(this.deckOpacity60ToolStripMenuItem_Click);
            // 
            // deckOpacity80ToolStripMenuItem
            // 
            this.deckOpacity80ToolStripMenuItem.Name = "deckOpacity80ToolStripMenuItem";
            this.deckOpacity80ToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.deckOpacity80ToolStripMenuItem.Text = "80%";
            this.deckOpacity80ToolStripMenuItem.Click += new System.EventHandler(this.deckOpacity80ToolStripMenuItem_Click);
            // 
            // deckOpacity100ToolStripMenuItem
            // 
            this.deckOpacity100ToolStripMenuItem.Name = "deckOpacity100ToolStripMenuItem";
            this.deckOpacity100ToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.deckOpacity100ToolStripMenuItem.Text = "100%";
            this.deckOpacity100ToolStripMenuItem.Click += new System.EventHandler(this.deckOpacity100ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(196, 6);
            // 
            // snapDecksToolStripMenuItem
            // 
            this.snapDecksToolStripMenuItem.Name = "snapDecksToolStripMenuItem";
            this.snapDecksToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.snapDecksToolStripMenuItem.Text = "Snap Decks";
            this.snapDecksToolStripMenuItem.Click += new System.EventHandler(this.snapDecksToolStripMenuItem_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(949, 561);
            this.Controls.Add(this.ExpeditionsListBox);
            this.Controls.Add(this.ExpeditionsButton);
            this.Controls.Add(this.DecksButton);
            this.Controls.Add(this.MyProgressDisplay);
            this.Controls.Add(this.HighlightedDeckPanel);
            this.Controls.Add(this.DecksListBox);
            this.Controls.Add(this.DeckPanel);
            this.Controls.Add(this.MyMenuStrip);
            this.ForeColor = System.Drawing.Color.LightYellow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MyMenuStrip;
            this.MinimumSize = new System.Drawing.Size(965, 400);
            this.Name = "MainWindow";
            this.Text = "LoR Side Tracker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.DeckPanel.ResumeLayout(false);
            this.HighlightedDeckPanel.ResumeLayout(false);
            this.MyMenuStrip.ResumeLayout(false);
            this.MyMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
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
        private System.Windows.Forms.MenuStrip MyMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem myDeckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawnCardsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playedCardsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem opponentPlayedCardsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem deckOpacityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deckOpacity20ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deckOpacity40ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deckOpacity60ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deckOpacity80ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deckOpacity100ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem snapDecksToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem hideZeroCountCardsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deckWindowSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smallDeckSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mediumDeckSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem largeDeckSizeToolStripMenuItem;
    }
}

