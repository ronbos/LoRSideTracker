﻿namespace LoRSideTracker
{
    partial class GameLogControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.GameLogDisplay = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // GameLogDisplay
            // 
            this.GameLogDisplay.Location = new System.Drawing.Point(0, 0);
            this.GameLogDisplay.Name = "GameLogDisplay";
            this.GameLogDisplay.Size = new System.Drawing.Size(574, 100);
            this.GameLogDisplay.TabIndex = 0;
            this.GameLogDisplay.Paint += new System.Windows.Forms.PaintEventHandler(this.GameLogDisplay_Paint);
            this.GameLogDisplay.DoubleClick += new System.EventHandler(this.GameLogDisplay_Click);
            this.GameLogDisplay.MouseLeave += new System.EventHandler(this.GameLogDisplay_MouseLeave);
            this.GameLogDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GameLogDisplay_MouseMove);
            // 
            // GameLogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.GameLogDisplay);
            this.Name = "GameLogControl";
            this.Size = new System.Drawing.Size(574, 249);
            this.Load += new System.EventHandler(this.GameLogControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel GameLogDisplay;
    }
}
