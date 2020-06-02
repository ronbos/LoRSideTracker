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
        /// <summary>Drawing size of deck</summary>
        public class DeckScale
        {
            /// <summary>Title Heght</summary>
            public int TitleHeight { get; private set; }
            /// <summary>Title Font</summary>
            public Font TitleFont { get; private set; }
            /// <summary>Card Font</summary>
            public Font CardFont { get; private set; }
            /// <summary>Card Size</summary>
            public Size CardSize { get; private set; }

            /// <summary>Small scale deck</summary>
            public static readonly DeckScale Small = new DeckScale(19, "Calibri", 11, 10, 140, 19);
            /// <summary>Medium scale deck</summary>
            public static readonly DeckScale Medium = new DeckScale(27, "Calibri", 13, 11, 180, 25);
            /// <summary>Large scale deck</summary>
            public static readonly DeckScale Large = new DeckScale(35, "Calibri", 16, 14, 220, 31);

            private DeckScale(int titleHeight, string fontName, int titleFontSize, int cardFontSize, int cardWidth, int cardHeight)
            {
                TitleHeight = titleHeight;
                TitleFont = new Font(fontName, titleFontSize, FontStyle.Bold);
                CardFont = new Font(fontName, cardFontSize, FontStyle.Regular);
                CardSize = new Size(cardWidth, cardHeight);
            }
        }

        /// <summary>Drawing size of deck</summary>
        public DeckScale CustomDeckScale { get; set; } = DeckScale.Medium;

        /// <summary>Deck contents</summary>
        public List<CardWithCount> Cards;

        /// <summary>Border size</summary>
        public int BorderSize { get; set; } = 1;
        /// <summary>Size of spacing between cards</summary>
        public int SpacingSize { get; private set; } = 1;

        /// <summary>If true, only title is shown</summary>
        public bool IsMinimized { get; set; } = false;

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
            int topBorderSize = BorderSize;
            if (!string.IsNullOrEmpty(this.Title))
            {
                topBorderSize += CustomDeckScale.TitleHeight;
            }
            Size result = new Size(2 * BorderSize + CustomDeckScale.CardSize.Width, topBorderSize);
            if (!IsMinimized)
            {
                result.Height += BorderSize + CustomDeckScale.CardSize.Height * Cards.Count 
                    + SpacingSize * (Cards.Count - 1) + BorderSize;
            }
            return result;
        }

        private void DeckControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), Bounds);
            int topBorderSize = BorderSize;
            if (!string.IsNullOrEmpty(this.Title))
            {
                topBorderSize += CustomDeckScale.TitleHeight;
            }

            Rectangle cardRect = new Rectangle(BorderSize, topBorderSize, 
                this.Bounds.Width - 2 * BorderSize, CustomDeckScale.CardSize.Height);
            if (!string.IsNullOrEmpty(this.Title))
            {
                Rectangle titleRect = new Rectangle(1, 1, Bounds.Width - 2, topBorderSize - 2);
                TextRenderer.DrawText(e.Graphics, this.Title, CustomDeckScale.TitleFont, titleRect, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            }

            for (int i = 0; i < Cards.Count; i++)
            {
                DrawCard(e.Graphics, Cards[i], cardRect, i == HighlightedCard);
                cardRect.Y += CustomDeckScale.CardSize.Height + SpacingSize;
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
            g.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Black)), paintRect);

            // Create font
            TextRenderer.DrawText(g, card.Cost.ToString(), CustomDeckScale.TitleFont, costRect, Color.LightGray, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            TextRenderer.DrawText(g, card.Name, CustomDeckScale.CardFont, cardRect, 
                (card.TheCard.SuperType == "Champion") ? Color.Gold : Color.White, 
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            TextRenderer.DrawText(g, "x" + card.Count.ToString(), CustomDeckScale.TitleFont, countRect, Color.LightGray, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

            // Highlight
            Rectangle frameRect = new Rectangle(paintRect.X - 5, paintRect.Y, paintRect.Width + 10, paintRect.Height - 1);
            if (isHighlighted)
            {
                g.DrawRectangle(new Pen(Color.DarkGray, 1), frameRect);
            }
            // Make x0 tile darker
            //if (card.Count == 0)
            //{
            //    g.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Black)), paintRect);
            //}
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
            int topBorderSize = BorderSize;
            if (!string.IsNullOrEmpty(this.Title))
            {
                topBorderSize += CustomDeckScale.TitleHeight;
            }
            return new Rectangle(BorderSize, topBorderSize + index * (CustomDeckScale.CardSize.Height + SpacingSize), 
                this.ClientRectangle.Width - 2 * BorderSize, CustomDeckScale.CardSize.Height);
        }

        private void DeckControl_MouseMove(object sender, MouseEventArgs e)
        {
            int topBorderSize = BorderSize;
            if (!string.IsNullOrEmpty(this.Title))
            {
                topBorderSize += CustomDeckScale.TitleHeight;
            }
            int index = (e.Y - topBorderSize) / (CustomDeckScale.CardSize.Height + SpacingSize);
            if (e.Y >= topBorderSize && index >= 0 && index < Cards.Count)
            {
                HighlightCard(index);
            }
            else
            {
                HighlightCard(-1);
            }
        }
    }
}
