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
            this.MyDeckControl = new LoRSideTracker.DeckControl();
            this.SuspendLayout();
            // 
            // MyDeckControl
            // 
            this.MyDeckControl.Location = new System.Drawing.Point(-1, 3);
            this.MyDeckControl.Name = "MyDeckControl";
            this.MyDeckControl.Size = new System.Drawing.Size(150, 150);
            this.MyDeckControl.TabIndex = 0;
            this.MyDeckControl.Title = null;
            this.MyDeckControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MyDeckControl_MouseDown);
            // 
            // DeckWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 592);
            this.Controls.Add(this.MyDeckControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DeckWindow";
            this.Text = "DeckWindow";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.DeckWindow_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private DeckControl MyDeckControl;
    }
}