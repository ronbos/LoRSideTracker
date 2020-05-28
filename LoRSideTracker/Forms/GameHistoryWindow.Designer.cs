namespace LoRSideTracker
{
    partial class GameHistoryWindow
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
            this.MyGameHistoryControl = new LoRSideTracker.GameHistoryControl();
            this.SuspendLayout();
            // 
            // MyGameHistoryControl
            // 
            this.MyGameHistoryControl.Location = new System.Drawing.Point(0, 0);
            this.MyGameHistoryControl.Name = "MyGameHistoryControl";
            this.MyGameHistoryControl.Size = new System.Drawing.Size(780, 331);
            this.MyGameHistoryControl.TabIndex = 0;
            // 
            // GameHistoryWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MyGameHistoryControl);
            this.Name = "GameHistoryWindow";
            this.Text = "Game History";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameHistoryWindow_FormClosing);
            this.Load += new System.EventHandler(this.GameHistory_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.GameHistory_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private GameHistoryControl MyGameHistoryControl;
    }
}