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
            this.ExpeditionsListBox = new System.Windows.Forms.ListBox();
            this.ExpeditionsButton = new System.Windows.Forms.Button();
            this.DecksButton = new System.Windows.Forms.Button();
            this.DecksListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // ExpeditionsListBox
            // 
            this.ExpeditionsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ExpeditionsListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(36)))));
            this.ExpeditionsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ExpeditionsListBox.CausesValidation = false;
            this.ExpeditionsListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ExpeditionsListBox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExpeditionsListBox.ForeColor = System.Drawing.Color.LightYellow;
            this.ExpeditionsListBox.IntegralHeight = false;
            this.ExpeditionsListBox.ItemHeight = 30;
            this.ExpeditionsListBox.Location = new System.Drawing.Point(0, 27);
            this.ExpeditionsListBox.Margin = new System.Windows.Forms.Padding(0);
            this.ExpeditionsListBox.MinimumSize = new System.Drawing.Size(198, 120);
            this.ExpeditionsListBox.Name = "ExpeditionsListBox";
            this.ExpeditionsListBox.Size = new System.Drawing.Size(199, 492);
            this.ExpeditionsListBox.TabIndex = 17;
            this.ExpeditionsListBox.TabStop = false;
            this.ExpeditionsListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ListBox_DrawItem);
            this.ExpeditionsListBox.SelectedIndexChanged += new System.EventHandler(this.ListBox_SelectedIndexChanged);
            this.ExpeditionsListBox.DoubleClick += new System.EventHandler(this.ListBox_DoubleClick);
            // 
            // ExpeditionsButton
            // 
            this.ExpeditionsButton.BackColor = System.Drawing.SystemColors.Control;
            this.ExpeditionsButton.FlatAppearance.BorderColor = System.Drawing.Color.LightYellow;
            this.ExpeditionsButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ExpeditionsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.ExpeditionsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExpeditionsButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExpeditionsButton.Location = new System.Drawing.Point(99, 0);
            this.ExpeditionsButton.Margin = new System.Windows.Forms.Padding(0);
            this.ExpeditionsButton.Name = "ExpeditionsButton";
            this.ExpeditionsButton.Size = new System.Drawing.Size(100, 23);
            this.ExpeditionsButton.TabIndex = 16;
            this.ExpeditionsButton.TabStop = false;
            this.ExpeditionsButton.Text = "Expeditions";
            this.ExpeditionsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ExpeditionsButton.UseVisualStyleBackColor = false;
            this.ExpeditionsButton.Click += new System.EventHandler(this.ExpeditionsButton_Click);
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
            this.DecksButton.Text = "Decks";
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
            this.Controls.Add(this.ExpeditionsListBox);
            this.Controls.Add(this.ExpeditionsButton);
            this.Controls.Add(this.DecksButton);
            this.Controls.Add(this.DecksListBox);
            this.Name = "DecksListControl";
            this.Size = new System.Drawing.Size(220, 540);
            this.Load += new System.EventHandler(this.DecksListControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox ExpeditionsListBox;
        private System.Windows.Forms.Button ExpeditionsButton;
        private System.Windows.Forms.Button DecksButton;
        private System.Windows.Forms.ListBox DecksListBox;
    }
}
