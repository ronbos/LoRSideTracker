namespace LoRSideTracker
{
    partial class DeckWindow
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
            this.MyDeckStatsDisplay = new LoRSideTracker.DeckStatsDisplay();
            this.MyDeckControl = new LoRSideTracker.DeckControl();
            this.SuspendLayout();
            // 
            // MyDeckStatsDisplay
            // 
            //this.MyDeckStatsDisplay.BlockHeight = 3;
            //this.MyDeckStatsDisplay.BlockWidth = 7;
            this.MyDeckStatsDisplay.Location = new System.Drawing.Point(0, 156);
            this.MyDeckStatsDisplay.Name = "MyDeckStatsDisplay";
            this.MyDeckStatsDisplay.Size = new System.Drawing.Size(150, 150);
            this.MyDeckStatsDisplay.TabIndex = 1;
            this.MyDeckStatsDisplay.TextColor = System.Drawing.Color.White;
            this.MyDeckStatsDisplay.TheDeck = null;
            // 
            // MyDeckControl
            // 
            this.MyDeckControl.BorderSize = 1;
            this.MyDeckControl.IsMinimized = false;
            this.MyDeckControl.Location = new System.Drawing.Point(0, 0);
            this.MyDeckControl.Name = "MyDeckControl";
            this.MyDeckControl.Size = new System.Drawing.Size(150, 150);
            this.MyDeckControl.TabIndex = 0;
            this.MyDeckControl.Title = null;
            this.MyDeckControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MyDeckControl_MouseDown);
            this.MyDeckControl.MouseLeave += new System.EventHandler(this.MyDeckControl_MouseLeave);
            // 
            // DeckWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(150, 266);
            this.Controls.Add(this.MyDeckStatsDisplay);
            this.Controls.Add(this.MyDeckControl);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DeckWindow";
            this.ShowInTaskbar = false;
            this.Text = "Deck Window";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DeckWindow_Load);
            this.MouseLeave += new System.EventHandler(this.DeckWindow_MouseLeave);
            this.ResumeLayout(false);

        }

        #endregion

        private DeckControl MyDeckControl;
        private DeckStatsDisplay MyDeckStatsDisplay;
    }
}