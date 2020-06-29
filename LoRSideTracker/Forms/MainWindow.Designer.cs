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
            this.HighlightedDeckPanel = new System.Windows.Forms.Panel();
            this.TheMenuBar = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsMenu = new System.Windows.Forms.ToolStripMenuItem();
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
            this.windowMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.hideGamesVsAIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DecksListCtrl = new LoRSideTracker.Controls.DecksListControl();
            this.HighlightedGameLogControl = new LoRSideTracker.GameLogControl();
            this.MyProgressDisplay = new LoRSideTracker.ProgressDisplayControl();
            this.HighlightedDeckStatsDisplay = new LoRSideTracker.DeckStatsDisplay();
            this.HighlightedDeckControl = new LoRSideTracker.DeckControl();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.HighlightedDeckPanel.SuspendLayout();
            this.TheMenuBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // HighlightedDeckPanel
            // 
            this.HighlightedDeckPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HighlightedDeckPanel.AutoScroll = true;
            this.HighlightedDeckPanel.Controls.Add(this.HighlightedDeckStatsDisplay);
            this.HighlightedDeckPanel.Controls.Add(this.HighlightedDeckControl);
            this.HighlightedDeckPanel.Location = new System.Drawing.Point(789, 32);
            this.HighlightedDeckPanel.Margin = new System.Windows.Forms.Padding(0);
            this.HighlightedDeckPanel.Name = "HighlightedDeckPanel";
            this.HighlightedDeckPanel.Size = new System.Drawing.Size(165, 523);
            this.HighlightedDeckPanel.TabIndex = 7;
            this.HighlightedDeckPanel.Visible = false;
            this.HighlightedDeckPanel.SizeChanged += new System.EventHandler(this.HighlightedDeckPanel_SizeChanged);
            // 
            // TheMenuBar
            // 
            this.TheMenuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.optionsMenu,
            this.windowMenu});
            this.TheMenuBar.Location = new System.Drawing.Point(0, 0);
            this.TheMenuBar.Name = "TheMenuBar";
            this.TheMenuBar.Size = new System.Drawing.Size(962, 24);
            this.TheMenuBar.TabIndex = 14;
            this.TheMenuBar.Text = "TheMenuBar";
            this.TheMenuBar.Visible = false;
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem8,
            this.exitToolStripMenuItem});
            this.fileMenu.ForeColor = System.Drawing.SystemColors.WindowText;
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(37, 20);
            this.fileMenu.Text = "File";
            this.fileMenu.DropDownOpened += new System.EventHandler(this.fileMenu_DropDownOpened);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // optionsMenu
            // 
            this.optionsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.optionsMenu.ForeColor = System.Drawing.SystemColors.WindowText;
            this.optionsMenu.Name = "optionsMenu";
            this.optionsMenu.Size = new System.Drawing.Size(61, 20);
            this.optionsMenu.Text = "Options";
            this.optionsMenu.DropDownOpened += new System.EventHandler(this.optionsMenu_DropDownOpened);
            // 
            // myDeckToolStripMenuItem
            // 
            this.myDeckToolStripMenuItem.Name = "myDeckToolStripMenuItem";
            this.myDeckToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.myDeckToolStripMenuItem.Text = "My Deck";
            this.myDeckToolStripMenuItem.Click += new System.EventHandler(this.myDeckToolStripMenuItem_Click);
            // 
            // drawnCardsToolStripMenuItem
            // 
            this.drawnCardsToolStripMenuItem.Name = "drawnCardsToolStripMenuItem";
            this.drawnCardsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.drawnCardsToolStripMenuItem.Text = "My Drawn Cards";
            this.drawnCardsToolStripMenuItem.Click += new System.EventHandler(this.drawnCardsToolStripMenuItem_Click);
            // 
            // playedCardsToolStripMenuItem
            // 
            this.playedCardsToolStripMenuItem.Name = "playedCardsToolStripMenuItem";
            this.playedCardsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.playedCardsToolStripMenuItem.Text = "My Graveyard";
            this.playedCardsToolStripMenuItem.Click += new System.EventHandler(this.playedCardsToolStripMenuItem_Click);
            // 
            // opponentPlayedCardsToolStripMenuItem
            // 
            this.opponentPlayedCardsToolStripMenuItem.Name = "opponentPlayedCardsToolStripMenuItem";
            this.opponentPlayedCardsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.opponentPlayedCardsToolStripMenuItem.Text = "Opponent Graveyard";
            this.opponentPlayedCardsToolStripMenuItem.Click += new System.EventHandler(this.opponentPlayedCardsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(192, 6);
            // 
            // hideZeroCountCardsToolStripMenuItem
            // 
            this.hideZeroCountCardsToolStripMenuItem.Name = "hideZeroCountCardsToolStripMenuItem";
            this.hideZeroCountCardsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.hideZeroCountCardsToolStripMenuItem.Text = "Hide Zero Count Cards";
            this.hideZeroCountCardsToolStripMenuItem.Click += new System.EventHandler(this.hideZeroCountCardsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(192, 6);
            // 
            // deckWindowSizeToolStripMenuItem
            // 
            this.deckWindowSizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.smallDeckSizeToolStripMenuItem,
            this.mediumDeckSizeToolStripMenuItem,
            this.largeDeckSizeToolStripMenuItem});
            this.deckWindowSizeToolStripMenuItem.Name = "deckWindowSizeToolStripMenuItem";
            this.deckWindowSizeToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
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
            this.deckOpacityToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.deckOpacityToolStripMenuItem.Text = "Deck Opacity";
            // 
            // deckOpacity20ToolStripMenuItem
            // 
            this.deckOpacity20ToolStripMenuItem.Name = "deckOpacity20ToolStripMenuItem";
            this.deckOpacity20ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deckOpacity20ToolStripMenuItem.Text = "20%";
            this.deckOpacity20ToolStripMenuItem.Click += new System.EventHandler(this.deckOpacity20ToolStripMenuItem_Click);
            // 
            // deckOpacity40ToolStripMenuItem
            // 
            this.deckOpacity40ToolStripMenuItem.Name = "deckOpacity40ToolStripMenuItem";
            this.deckOpacity40ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deckOpacity40ToolStripMenuItem.Text = "40%";
            this.deckOpacity40ToolStripMenuItem.Click += new System.EventHandler(this.deckOpacity40ToolStripMenuItem_Click);
            // 
            // deckOpacity60ToolStripMenuItem
            // 
            this.deckOpacity60ToolStripMenuItem.Name = "deckOpacity60ToolStripMenuItem";
            this.deckOpacity60ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deckOpacity60ToolStripMenuItem.Text = "60%";
            this.deckOpacity60ToolStripMenuItem.Click += new System.EventHandler(this.deckOpacity60ToolStripMenuItem_Click);
            // 
            // deckOpacity80ToolStripMenuItem
            // 
            this.deckOpacity80ToolStripMenuItem.Name = "deckOpacity80ToolStripMenuItem";
            this.deckOpacity80ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deckOpacity80ToolStripMenuItem.Text = "80%";
            this.deckOpacity80ToolStripMenuItem.Click += new System.EventHandler(this.deckOpacity80ToolStripMenuItem_Click);
            // 
            // deckOpacity100ToolStripMenuItem
            // 
            this.deckOpacity100ToolStripMenuItem.Name = "deckOpacity100ToolStripMenuItem";
            this.deckOpacity100ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deckOpacity100ToolStripMenuItem.Text = "100%";
            this.deckOpacity100ToolStripMenuItem.Click += new System.EventHandler(this.deckOpacity100ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(192, 6);
            // 
            // snapDecksToolStripMenuItem
            // 
            this.snapDecksToolStripMenuItem.Name = "snapDecksToolStripMenuItem";
            this.snapDecksToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.snapDecksToolStripMenuItem.Text = "Snap Decks";
            this.snapDecksToolStripMenuItem.Click += new System.EventHandler(this.snapDecksToolStripMenuItem_Click);
            // 
            // windowMenu
            // 
            this.windowMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideGamesVsAIToolStripMenuItem,
            this.toolStripMenuItem4,
            this.optionsToolStripMenuItem,
            this.logToolStripMenuItem});
            this.windowMenu.ForeColor = System.Drawing.SystemColors.WindowText;
            this.windowMenu.Name = "windowMenu";
            this.windowMenu.Size = new System.Drawing.Size(63, 20);
            this.windowMenu.Text = "Window";
            this.windowMenu.DropDownOpened += new System.EventHandler(this.windowMenu_DropDownOpened);
            // 
            // hideGamesVsAIToolStripMenuItem
            // 
            this.hideGamesVsAIToolStripMenuItem.Name = "hideGamesVsAIToolStripMenuItem";
            this.hideGamesVsAIToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.hideGamesVsAIToolStripMenuItem.Text = "Hide Games vs. AI";
            this.hideGamesVsAIToolStripMenuItem.Click += new System.EventHandler(this.hideGamesVsAIToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(177, 6);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.logToolStripMenuItem.Text = "Log";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.logToolStripMenuItem_Click);
            // 
            // DecksListCtrl
            // 
            this.DecksListCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.DecksListCtrl.AutoScroll = true;
            this.DecksListCtrl.Location = new System.Drawing.Point(7, 31);
            this.DecksListCtrl.Name = "DecksListCtrl";
            this.DecksListCtrl.Size = new System.Drawing.Size(220, 524);
            this.DecksListCtrl.TabIndex = 16;
            this.DecksListCtrl.Visible = false;
            this.DecksListCtrl.SelectionChanged += new System.EventHandler(this.DecksListCtrl_SelectionChanged);
            // 
            // HighlightedGameLogControl
            // 
            this.HighlightedGameLogControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.HighlightedGameLogControl.AutoScroll = true;
            this.HighlightedGameLogControl.Location = new System.Drawing.Point(229, 29);
            this.HighlightedGameLogControl.Name = "HighlightedGameLogControl";
            this.HighlightedGameLogControl.Size = new System.Drawing.Size(558, 523);
            this.HighlightedGameLogControl.TabIndex = 15;
            this.HighlightedGameLogControl.TabStop = false;
            this.HighlightedGameLogControl.Visible = false;
            // 
            // MyProgressDisplay
            // 
            this.MyProgressDisplay.Location = new System.Drawing.Point(925, 381);
            this.MyProgressDisplay.MaximumSize = new System.Drawing.Size(360, 140);
            this.MyProgressDisplay.MinimumSize = new System.Drawing.Size(360, 140);
            this.MyProgressDisplay.Name = "MyProgressDisplay";
            this.MyProgressDisplay.Size = new System.Drawing.Size(360, 140);
            this.MyProgressDisplay.TabIndex = 9;
            this.MyProgressDisplay.TabStop = false;
            // 
            // HighlightedDeckStatsDisplay
            // 
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
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(962, 561);
            this.Controls.Add(this.TheMenuBar);
            this.Controls.Add(this.DecksListCtrl);
            this.Controls.Add(this.HighlightedGameLogControl);
            this.Controls.Add(this.MyProgressDisplay);
            this.Controls.Add(this.HighlightedDeckPanel);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.LightYellow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.TheMenuBar;
            this.MinimumSize = new System.Drawing.Size(978, 400);
            this.Name = "MainWindow";
            this.Text = "LoR Side Tracker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.HighlightedDeckPanel.ResumeLayout(false);
            this.TheMenuBar.ResumeLayout(false);
            this.TheMenuBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel HighlightedDeckPanel;
        private DeckStatsDisplay HighlightedDeckStatsDisplay;
        private DeckControl HighlightedDeckControl;
        private ProgressDisplayControl MyProgressDisplay;
        private System.Windows.Forms.MenuStrip TheMenuBar;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem optionsMenu;
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
        private GameLogControl HighlightedGameLogControl;
        private Controls.DecksListControl DecksListCtrl;
        private System.Windows.Forms.ToolStripMenuItem windowMenu;
        private System.Windows.Forms.ToolStripMenuItem hideGamesVsAIToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
    }
}

