using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoRSideTracker
{
    public partial class CardArtView : CSWinFormLayeredWindow.PerPixelAlphaForm
    {
        private Card MyCard;
        public CardArtView()
        {
            InitializeComponent();
        }

        public void SetCard(string cardCode)
        {
            try
            {
                MyCard = CardLibrary.GetCard(cardCode);
                this.SelectBitmap(new Bitmap(MyCard.CardArt));
            }
            catch
            {
                MyCard = null;
            }
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

        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //empty implementation
        //}
    }
}
