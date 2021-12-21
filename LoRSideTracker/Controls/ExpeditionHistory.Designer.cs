
namespace LoRSideTracker
{
    partial class ExpeditionHistory
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
            this.ScrollablePanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // ScrollablePanel
            // 
            this.ScrollablePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ScrollablePanel.AutoScroll = true;
            this.ScrollablePanel.AutoScrollMargin = new System.Drawing.Size(8, 8);
            this.ScrollablePanel.Location = new System.Drawing.Point(0, 0);
            this.ScrollablePanel.Margin = new System.Windows.Forms.Padding(8);
            this.ScrollablePanel.Name = "ScrollablePanel";
            this.ScrollablePanel.Size = new System.Drawing.Size(802, 644);
            this.ScrollablePanel.TabIndex = 0;
            this.ScrollablePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.ScrollablePanel_Paint);
            // 
            // ExpeditionHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(800, 643);
            this.Controls.Add(this.ScrollablePanel);
            this.ForeColor = System.Drawing.Color.LightYellow;
            this.Name = "ExpeditionHistory";
            this.ShowIcon = false;
            this.Text = "Expedition History";
            this.Load += new System.EventHandler(this.ExpeditionHistory_Load);
            this.Shown += new System.EventHandler(this.ExpeditionHistory_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel ScrollablePanel;
    }
}