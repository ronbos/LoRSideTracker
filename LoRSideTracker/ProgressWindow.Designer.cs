namespace LoRSideTracker
{
    partial class ProgressWindow
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
            this.MyProgressBar = new System.Windows.Forms.ProgressBar();
            this.MyLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // MyProgressBar
            // 
            this.MyProgressBar.Location = new System.Drawing.Point(50, 80);
            this.MyProgressBar.Name = "MyProgressBar";
            this.MyProgressBar.Size = new System.Drawing.Size(300, 23);
            this.MyProgressBar.Step = 1;
            this.MyProgressBar.TabIndex = 0;
            // 
            // MyLabel
            // 
            this.MyLabel.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MyLabel.Location = new System.Drawing.Point(15, 15);
            this.MyLabel.Name = "MyLabel";
            this.MyLabel.Size = new System.Drawing.Size(370, 50);
            this.MyLabel.TabIndex = 1;
            this.MyLabel.Text = "This is a sample text.";
            this.MyLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 120);
            this.panel1.TabIndex = 2;
            // 
            // ProgressWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 120);
            this.ControlBox = false;
            this.Controls.Add(this.MyLabel);
            this.Controls.Add(this.MyProgressBar);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "ProgressWindow";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar MyProgressBar;
        private System.Windows.Forms.Label MyLabel;
        private System.Windows.Forms.Panel panel1;
    }
}