namespace LoRSideTracker.Controls
{
    partial class DecksListControl
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
            this.AdventuresListBox = new System.Windows.Forms.ListBox();
            this.AdventuresButton = new System.Windows.Forms.Button();
            this.DecksButton = new System.Windows.Forms.Button();
            this.DecksListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // AdventuresListBox
            // 
            this.AdventuresListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AdventuresListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(36)))));
            this.AdventuresListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AdventuresListBox.CausesValidation = false;
            this.AdventuresListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.AdventuresListBox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AdventuresListBox.ForeColor = System.Drawing.Color.LightYellow;
            this.AdventuresListBox.IntegralHeight = false;
            this.AdventuresListBox.ItemHeight = 30;
            this.AdventuresListBox.Location = new System.Drawing.Point(0, 27);
            this.AdventuresListBox.Margin = new System.Windows.Forms.Padding(0);
            this.AdventuresListBox.MinimumSize = new System.Drawing.Size(198, 120);
            this.AdventuresListBox.Name = "AdventuresListBox";
            this.AdventuresListBox.Size = new System.Drawing.Size(199, 492);
            this.AdventuresListBox.TabIndex = 17;
            this.AdventuresListBox.TabStop = false;
            this.AdventuresListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ListBox_DrawItem);
            this.AdventuresListBox.SelectedIndexChanged += new System.EventHandler(this.ListBox_SelectedIndexChanged);
            this.AdventuresListBox.DoubleClick += new System.EventHandler(this.ListBox_DoubleClick);
            // 
            // AdventuresButton
            // 
            this.AdventuresButton.BackColor = System.Drawing.SystemColors.Control;
            this.AdventuresButton.FlatAppearance.BorderColor = System.Drawing.Color.LightYellow;
            this.AdventuresButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.AdventuresButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.AdventuresButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AdventuresButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AdventuresButton.Location = new System.Drawing.Point(99, 0);
            this.AdventuresButton.Margin = new System.Windows.Forms.Padding(0);
            this.AdventuresButton.Name = "AdventuresButton";
            this.AdventuresButton.Size = new System.Drawing.Size(100, 23);
            this.AdventuresButton.TabIndex = 16;
            this.AdventuresButton.TabStop = false;
            this.AdventuresButton.Text = "Adventures";
            this.AdventuresButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.AdventuresButton.UseVisualStyleBackColor = false;
            this.AdventuresButton.Click += new System.EventHandler(this.AdventuresButton_Click);
            // 
            // DecksButton
            // 
            this.DecksButton.FlatAppearance.BorderColor = System.Drawing.Color.LightYellow;
            this.DecksButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.DecksButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.DecksButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DecksButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DecksButton.Location = new System.Drawing.Point(0, 0);
            this.DecksButton.Margin = new System.Windows.Forms.Padding(0);
            this.DecksButton.Name = "DecksButton";
            this.DecksButton.Size = new System.Drawing.Size(100, 23);
            this.DecksButton.TabIndex = 15;
            this.DecksButton.TabStop = false;
            this.DecksButton.Text = "Constructed";
            this.DecksButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.DecksButton.UseVisualStyleBackColor = true;
            this.DecksButton.Click += new System.EventHandler(this.DecksButton_Click);
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
            this.DecksListBox.Location = new System.Drawing.Point(2, 27);
            this.DecksListBox.Margin = new System.Windows.Forms.Padding(0);
            this.DecksListBox.MinimumSize = new System.Drawing.Size(198, 120);
            this.DecksListBox.Name = "DecksListBox";
            this.DecksListBox.Size = new System.Drawing.Size(199, 492);
            this.DecksListBox.TabIndex = 14;
            this.DecksListBox.TabStop = false;
            this.DecksListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ListBox_DrawItem);
            this.DecksListBox.SelectedIndexChanged += new System.EventHandler(this.ListBox_SelectedIndexChanged);
            this.DecksListBox.DoubleClick += new System.EventHandler(this.ListBox_DoubleClick);
            // 
            // DecksListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.AdventuresListBox);
            this.Controls.Add(this.AdventuresButton);
            this.Controls.Add(this.DecksButton);
            this.Controls.Add(this.DecksListBox);
            this.Name = "DecksListControl";
            this.Size = new System.Drawing.Size(220, 540);
            this.Load += new System.EventHandler(this.DecksListControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox AdventuresListBox;
        private System.Windows.Forms.Button AdventuresButton;
        private System.Windows.Forms.Button DecksButton;
        private System.Windows.Forms.ListBox DecksListBox;
    }
}
