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
    /// <summary>
    /// Overlay watch window
    /// </summary>
    public partial class GameBoardWatchWindow : Form
    {
        CardList<CardInPlay> PlayerElements = new CardList<CardInPlay>();
        CardList<CardInPlay> OpponentElements = new CardList<CardInPlay>();
        int ScreenWidth = 1024;
        int ScreenHeight = 768;

        bool FullArtView = true;

        /// <summary>
        /// Constructor
        /// </summary>
        public GameBoardWatchWindow()
        {
            InitializeComponent();
            this.ResizeRedraw = true;
        }

        /// <summary>
        /// Draw new set of elements
        /// </summary>
        /// <param name="playerElements"></param>
        /// <param name="opponentElements"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        public void Update(CardList<CardInPlay> playerElements, CardList<CardInPlay> opponentElements, int screenWidth, int screenHeight)
        {
            CardList<CardInPlay> playerElementsCopy= playerElements.Clone();
            CardList<CardInPlay> opponentElementsCopy = opponentElements.Clone();
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    PlayerElements = playerElementsCopy;
                    OpponentElements = opponentElementsCopy;
                    ScreenWidth = screenWidth;
                    ScreenHeight = screenHeight;
                    Invalidate();
                }));
            }
            else
            {
                PlayerElements = playerElementsCopy;
                OpponentElements = opponentElementsCopy;
                ScreenWidth = screenWidth;
                ScreenHeight = screenHeight;
                Invalidate();
            }
        }

        private void OverlayWatchWindow_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.Clear(SystemColors.Control);
            e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            if (ScreenWidth <= 0)
            {
                return;
            }

            float normalizedScreenHeight = (float)GameBoard.ComputeNormalizedScreenHeight(ScreenWidth, ScreenHeight);

            // Determine scale to use
            float scaleX = (float)ClientRectangle.Width / (float)ScreenWidth;
            float scaleY = (float)ClientRectangle.Height / normalizedScreenHeight;
            float scale = Math.Min(scaleX, scaleY);

            // Draw screen rectangle
            Rectangle screenRect = new Rectangle(0, 0,
                (int)(0.5f + ScreenWidth * scale),
                (int)(0.5f + normalizedScreenHeight * scale));
            screenRect.Offset(ClientRectangle.Width / 2 - screenRect.Width / 2, ClientRectangle.Height / 2 - screenRect.Height / 2);
            e.Graphics.DrawRectangle(new Pen(Color.Black, 2), screenRect);

            // Draw battlefield rectangles
            Pen battlegroundPen = new Pen(Color.Gray, 2);
            Rectangle battleground = new Rectangle(0, 0, screenRect.Height * 10 / 9, screenRect.Height / 2);
            battleground.Offset(screenRect.X + screenRect.Width / 2 - battleground.Width / 2, screenRect.Y + screenRect.Height / 4);
            e.Graphics.FillRectangle(new SolidBrush(Color.DarkSeaGreen), battleground);
            e.Graphics.DrawRectangle(battlegroundPen, battleground);
            Rectangle localField = new Rectangle(0, 0, screenRect.Height * 8 / 10, screenRect.Height / 6);
            Rectangle opponentField = localField;
            localField.Offset(screenRect.X + screenRect.Width / 2 - localField.Width / 2, battleground.Bottom);
            opponentField.Offset(screenRect.X + screenRect.Width / 2 - localField.Width / 2, battleground.Top - opponentField.Bottom);
            e.Graphics.FillRectangle(new SolidBrush(Color.Silver), localField);
            e.Graphics.FillRectangle(new SolidBrush(Color.Silver), opponentField);
            e.Graphics.DrawRectangle(battlegroundPen, localField);
            e.Graphics.DrawRectangle(battlegroundPen, opponentField);

            // Draw all player card rectangles
            foreach (var el in PlayerElements)
            {
                DrawElement(el, e.Graphics, Color.Blue, screenRect, scale);
            }

            // Draw all opponent card rectangles
            foreach (var el in OpponentElements)
            {
                DrawElement(el, e.Graphics, Color.Red, screenRect, scale);
            }
        }

        void DrawElement(CardInPlay card, Graphics g, Color borderColor, Rectangle screenRect, double scale)
        {
            Rectangle r = new Rectangle(
                (int)(0.5f + card.BoundingBox.X * scale),
                (int)(0.5f + card.BoundingBox.Y * scale),
                (int)(0.5f + card.BoundingBox.Width * scale),
                (int)(0.5f + card.BoundingBox.Height * scale));
            r.Offset(screenRect.X, screenRect.Y);
            if (FullArtView)
            {
                card.TheCard.LoadCardArt();
                if (card.CurrentZone == PlayZone.Zoom || card.CurrentZone == PlayZone.Stage || card.CurrentZone == PlayZone.Hand || card.CurrentZone == PlayZone.Field)
                {
                    g.DrawImage(card.TheCard.CardArt, r);
                }
                else if (card.CurrentZone == PlayZone.Cast || card.CurrentZone == PlayZone.Battle || card.CurrentZone == PlayZone.Windup || card.CurrentZone == PlayZone.Attack)
                {
                    card.TheCard.DrawCardBanner(g, r);
                }
            }
            else
            {
                r.Offset(screenRect.X, screenRect.Y);
                g.DrawRectangle(new Pen(borderColor, 2), r);
                string text = string.Format("{0}\r\n{1}\r\n{2}\r\n{3}", CardLibrary.GetCard(card.CardCode).Name, card.CardCode,
                    card.NormalizedCenter.Y, card.NormalizedBoundingBox.Height);
                TextRenderer.DrawText(g, text, this.Font, r, borderColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private void OverlayWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        /// <summary>
        /// Reduce flicker
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //empty implementation
        }
    }
}
