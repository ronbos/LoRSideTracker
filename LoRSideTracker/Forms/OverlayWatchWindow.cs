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
    public partial class OverlayWatchWindow : Form
    {
        List<OverlayElement> PlayerElements = new List<OverlayElement>();
        List<OverlayElement> OpponentElements = new List<OverlayElement>();
        int ScreenWidth = 1024;
        int ScreenHeight = 768;

        /// <summary>
        /// Constructor
        /// </summary>
        public OverlayWatchWindow()
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
        public void Update(List<OverlayElement> playerElements, List<OverlayElement> opponentElements, int screenWidth, int screenHeight)
        {
            List<OverlayElement> playerElementsCopy= Utilities.Clone(playerElements);
            List<OverlayElement> opponentElementsCopy = Utilities.Clone(opponentElements);
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

            // Determine scale to use
            float scaleX = (float)ClientRectangle.Width / (float)ScreenWidth;
            float scaleY = (float)ClientRectangle.Height / (float)ScreenHeight;
            float scale = Math.Min(scaleX, scaleY);

            // Draw screen rectangle
            Rectangle screenRect = new Rectangle(0, 0,
                (int)(0.5f + ScreenWidth * scale),
                (int)(0.5f + ScreenHeight * scale));
            screenRect.Offset(ClientRectangle.Width / 2 - screenRect.Width / 2, ClientRectangle.Height / 2 - screenRect.Height / 2);
            e.Graphics.DrawRectangle(new Pen(Color.Black, 2), screenRect);

            // Draw all player card rectangles
            foreach (var el in PlayerElements)
            {
                Rectangle r = new Rectangle(
                    (int)(0.5f + el.BoundingBox.X * scale),
                    (int)(0.5f + el.BoundingBox.Y * scale),
                    (int)(0.5f + el.BoundingBox.Width * scale),
                    (int)(0.5f + el.BoundingBox.Height * scale));
                r.Offset(screenRect.X, screenRect.Y);
                e.Graphics.DrawRectangle(new Pen(Color.Blue, 2), r);
                string text = string.Format("{0}\r\n{1}\r\n{2}", CardLibrary.GetCard(el.CardCode).Name, el.CardCode, el.NormalizedBoundingBox.Height);
                TextRenderer.DrawText(e.Graphics, text, this.Font, r, Color.Blue, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            // Draw all opponent card rectangles
            foreach (var el in OpponentElements)
            {
                Rectangle r = new Rectangle(
                    (int)(0.5f + el.BoundingBox.X * scale),
                    (int)(0.5f + el.BoundingBox.Y * scale),
                    (int)(0.5f + el.BoundingBox.Width * scale),
                    (int)(0.5f + el.BoundingBox.Height * scale));
                r.Offset(screenRect.X, screenRect.Y);
                e.Graphics.DrawRectangle(new Pen(Color.Red, 2), r);
                string text = string.Format("{0}\r\n{1} {2}", CardLibrary.GetCard(el.CardCode).Name, el.CardCode, el.NormalizedBoundingBox.Height);
                TextRenderer.DrawText(e.Graphics, text, this.Font, r, Color.Red, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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
