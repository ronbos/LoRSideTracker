﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LoRSideTracker
{
    /// <summary>
    /// Window for displaying full card art, with transparency
    /// </summary>
    public partial class CardArtView : PerPixelAlphaForm
    {
        private Card MyCard;

        /// <summary>
        /// Constuctor
        /// </summary>
        public CardArtView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set the card to display
        /// </summary>
        /// <param name="card">Card to display</param>
        public void SetCard(Card card)
        {
            Utilities.CallActionSafelyAndWait(this, new Action(() =>
            {
                MyCard = card;
                this.SelectBitmap(new Bitmap(MyCard.CardArt));
            }));
        }

        private void CardArtView_Paint(object sender, PaintEventArgs e)
        {
            Rectangle paintRect = this.ClientRectangle;
            if (MyCard != null)
            {
                e.Graphics.DrawImage(MyCard.CardArt, new Point(0, 0));
            }
            else
            {
                paintRect.Inflate(-5, -5);
                e.Graphics.DrawRectangle(new Pen(Color.Red), paintRect);
            }
        }
    }
}
