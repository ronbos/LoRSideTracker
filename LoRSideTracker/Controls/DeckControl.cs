using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace LoRSideTracker
{
    /// <summary>
    /// Deck display control
    /// </summary>
    public partial class DeckControl : UserControl
    {
        /// <summary>Deck contents</summary>
        public List<CardWithCount> Cards;

        /// <summary>Top border size (including title)</summary>
        public int TopBorderSize { get; private set; } = 30;
        /// <summary>Bottom border size</summary>
        public int BottomBorderSize { get; private set; } = 1;
        /// <summary>Side border size</summary>
        public int SideBorderSize { get; private set; } = 1;
        /// <summary>Size of spacing between cards</summary>
        public int SpacingSize { get; private set; } = 1;

        /// <summary>If true, only title is shown</summary>
        public bool IsMinimized { get; set; } = false;

        /// <summary>Default card size</summary>
        public Size CardSize { get; private set; } = new Size(180, 25);

        private Font TitleFont = new Font("Calibri", 12, FontStyle.Bold);
        private Font CardFont = new Font("Calibri", 10, FontStyle.Regular);
        private Font StatsFont = new Font("Calibri", 12, FontStyle.Bold);

        /// <summary>Window Title</summary>
        public string Title { get; set; }

        private CardArtView CardPopup;
        private int HighlightedCard = -1;

        private delegate void SetCardSafeDelegate(int index, CardWithCount cardCode);

        /// <summary>
        /// Constructor
        /// </summary>
        public DeckControl()
        {
            InitializeComponent();

            Cards = new List<CardWithCount>();
            CardPopup = new CardArtView();
        }

        /// <summary>
        /// Set card
        /// </summary>
        /// <param name="index">Card index to set</param>
        /// <param name="card">Card</param>
        public void SetCard(int index, CardWithCount card)
        {
            if (this.InvokeRequired)
            {
                var d = new SetCardSafeDelegate(SetCardSafe);
                this.Invoke(d, new object[] { index, card });
            }
            else
            {
                SetCardSafe(index, card);
            }
        }


        private void SetCardSafe(int index, CardWithCount card)
        {
            if (index == Cards.Count)
            {
                Cards.Add((CardWithCount)card.Clone());
                Invalidate(GetCardRectangle(Cards.Count - 1));
            }
            else if (Cards[index].Code != card.Code)
            {
                Cards[index] = (CardWithCount)card.Clone();
                Invalidate(GetCardRectangle(index));
            }
            else if (Cards[index].Count != card.Count)
            {
                Cards[index].Count = card.Count;
                Invalidate(GetCardRectangle(index));
            }
        }

        /// <summary>
        /// Clear deck contents
        /// </summary>
        public void ClearDeck()
        {
            if (Cards.Count > 0)
            {
                Cards.Clear();
                Invalidate();
            }
        }

        /// <summary>
        /// Trim the deck to given size
        /// </summary>
        /// <param name="setSize">Size to trim to</param>
        public void TrimDeck(int setSize)
        {
            if (Cards.Count > setSize)
            {
                Cards.RemoveRange(setSize, Cards.Count - setSize);
                Invalidate();
            }
        }

        /// <summary>
        /// Computes best size to display title and all cards (if not minimized)
        /// </summary>
        /// <returns></returns>
        public Size GetBestSize()
        {
            int topBorderSize = string.IsNullOrEmpty(this.Title) ? BottomBorderSize : TopBorderSize;
            Size result = new Size(2 * SideBorderSize + CardSize.Width, topBorderSize);
            if (!IsMinimized) result.Height += BottomBorderSize + CardSize.Height * Cards.Count + SpacingSize * (Cards.Count - 1) + BottomBorderSize;
            return result;
        }

        private void DeckControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), Bounds);
            int topBorderSize = string.IsNullOrEmpty(this.Title) ? BottomBorderSize : TopBorderSize;

            Rectangle cardRect = new Rectangle(SideBorderSize, topBorderSize, this.Bounds.Width - 2 * SideBorderSize, CardSize.Height);
            if (!string.IsNullOrEmpty(this.Title))
            {
                Rectangle titleRect = new Rectangle(1, 1, Bounds.Width - 2, topBorderSize - 2);
                TextRenderer.DrawText(e.Graphics, this.Title, TitleFont, titleRect, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            }

            for (int i = 0; i < Cards.Count; i++)
            {
                DrawCard(e.Graphics, Cards[i], cardRect, i == HighlightedCard);
                cardRect.Y += CardSize.Height + SpacingSize;
            }
        }
        private void DrawCard(Graphics g, CardWithCount card, Rectangle paintRect, bool isHighlighted)
        {
            // Draw the 2 pixel border
            g.FillRectangle(new SolidBrush(Color.Black), paintRect);

            if (card == null)
            {
                return;
            }

            Rectangle cardRect = paintRect;
            cardRect.Inflate(-paintRect.Height, 0);
            Rectangle costRect = paintRect;
            costRect.Width = costRect.Height;
            Rectangle countRect = costRect;
            countRect.X = cardRect.X + cardRect.Width;

            // Draw the card and a translucent layer to make the tile darker
            card.TheCard.DrawCardArt(g, paintRect);
            g.FillRectangle(new SolidBrush(Color.FromArgb(96, Color.Black)), paintRect);

            // Create font
            TextRenderer.DrawText(g, card.Cost.ToString(), StatsFont, costRect, Color.LightGray, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            TextRenderer.DrawText(g, card.Name, CardFont, cardRect, 
                (card.TheCard.SuperType == "Champion") ? Color.Gold : Color.White, 
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            TextRenderer.DrawText(g, "x" + card.Count.ToString(), StatsFont, countRect, Color.LightGray, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

            // Highlight
            Rectangle frameRect = new Rectangle(paintRect.X - 5, paintRect.Y, paintRect.Width + 10, paintRect.Height - 1);
            if (isHighlighted)
            {
                g.DrawRectangle(new Pen(Color.DarkGray, 1), frameRect);
            }
            // Make x0 tile darker
            if (card.Count == 0)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Black)), paintRect);
            }
        }

        private void DeckControl_MouseLeave(object sender, EventArgs e)
        {
            HighlightCard(-1);
            //if (ShouldHideOnMouseLeave()) Hide();
        }

        /// <summary>
        /// Highlight specific card and show full art
        /// </summary>
        /// <param name="index">Index or -1 to remove highlight</param>
        public void HighlightCard(int index)
        {
            if (index == HighlightedCard)
            {
                return;
            }

            int oldIndex = HighlightedCard;
            HighlightedCard = index;
            if (oldIndex >= 0)
            {
                Invalidate(GetCardRectangle(oldIndex));
            }
            if (HighlightedCard >= 0)
            {
                CardPopup.SetCard(Cards[index].TheCard);
                Point topLeft = PointToScreen(new Point(0, 0));
                Rectangle cardRect = GetCardRectangle(index);
                CardPopup.SetBounds(topLeft.X + cardRect.X + cardRect.Width, topLeft.Y + cardRect.Y,
                    Cards[index].TheCard.CardArt.Width, Cards[index].TheCard.CardArt.Height);
                Invalidate(GetCardRectangle(HighlightedCard));
                if (!CardPopup.Visible)
                {
                    Utilities.ShowInactiveTopmost(CardPopup);
                }
            }
            else
            {
                CardPopup.Hide();
            }
        }

        private Rectangle GetCardRectangle(int index)
        {
            int topBorderSize = string.IsNullOrEmpty(this.Title) ? BottomBorderSize : TopBorderSize;
            return new Rectangle(SideBorderSize, topBorderSize + index * (CardSize.Height + SpacingSize), this.ClientRectangle.Width - 2 * SideBorderSize, CardSize.Height);
        }

        private void DeckControl_MouseMove(object sender, MouseEventArgs e)
        {
            int topBorderSize = string.IsNullOrEmpty(this.Title) ? BottomBorderSize : TopBorderSize;
            int index = (e.Y - topBorderSize) / (CardSize.Height + SpacingSize);
            if (e.Y >= topBorderSize && index >= 0 && index < Cards.Count)
            {
                HighlightCard(index);
            }
            else
            {
                HighlightCard(-1);
            }
        }

        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
            //empty implementation
        //}
    }
}
