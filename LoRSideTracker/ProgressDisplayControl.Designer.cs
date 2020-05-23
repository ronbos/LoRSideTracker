namespace LoRSideTracker
{
    partial class ProgressDisplayControl
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
            this.MyLabel = new System.Windows.Forms.Label();
            this.MyProgressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // MyLabel
            // 
            this.MyLabel.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MyLabel.Location = new System.Drawing.Point(15, 15);
            this.MyLabel.Name = "MyLabel";
            this.MyLabel.Size = new System.Drawing.Size(370, 50);
            this.MyLabel.TabIndex = 4;
            this.MyLabel.Text = "This is a sample text.";
            this.MyLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // MyProgressBar
            // 
            this.MyProgressBar.Location = new System.Drawing.Point(50, 80);
            this.MyProgressBar.Name = "MyProgressBar";
            this.MyProgressBar.Size = new System.Drawing.Size(300, 23);
            this.MyProgressBar.Step = 1;
            this.MyProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.MyProgressBar.TabIndex = 3;
            // 
            // ProgressDisplayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.MyLabel);
            this.Controls.Add(this.MyProgressBar);
            this.Name = "ProgressDisplayControl";
            this.Size = new System.Drawing.Size(400, 120);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label MyLabel;
        private System.Windows.Forms.ProgressBar MyProgressBar;
    }
}
