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
            this.PercentageLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // MyLabel
            // 
            this.MyLabel.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MyLabel.Location = new System.Drawing.Point(15, 15);
            this.MyLabel.Name = "MyLabel";
            this.MyLabel.Size = new System.Drawing.Size(330, 50);
            this.MyLabel.TabIndex = 4;
            this.MyLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // PercentageLabel
            // 
            this.PercentageLabel.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PercentageLabel.Location = new System.Drawing.Point(50, 80);
            this.PercentageLabel.Name = "PercentageLabel";
            this.PercentageLabel.Size = new System.Drawing.Size(260, 23);
            this.PercentageLabel.TabIndex = 4;
            this.PercentageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ProgressDisplayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.PercentageLabel);
            this.Controls.Add(this.MyLabel);
            this.MaximumSize = new System.Drawing.Size(360, 120);
            this.MinimumSize = new System.Drawing.Size(360, 120);
            this.Name = "ProgressDisplayControl";
            this.Size = new System.Drawing.Size(360, 120);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label MyLabel;
        private System.Windows.Forms.Label PercentageLabel;
    }
}
