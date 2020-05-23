using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoRSideTracker
{    public partial class DeckControl : UserControl
    {
        public List<CardWithCount> Cards;

        private int TopBorderSize = 30;
        private int BottomBorderSize = 1;
        private int SideBorderSize = 1;
        private int SpacingSize = 1;
        private Size CardSize = new Size(192, 28);

        private Font TitleFont = new Font("Calibri", 12, FontStyle.Bold);
        private Font CardFont = new Font("Calibri", 12, FontStyle.Bold);
        private Font StatsFont = new Font("Calibri", 12, FontStyle.Bold);

        public string Title { get; set; }

        private CardArtView CardPopup;
        private int HighlightedCard = -1;

        private delegate void SetCardSafeDelegate(int index, string cardCode, int count);

        public DeckControl()
        {
            InitializeComponent();

            Cards = new List<CardWithCount>();
            CardPopup = new CardArtView();
        }
        public void SetCard(int index, string cardCode, int count)
        {
            if (this.InvokeRequired)
            {
                var d = new SetCardSafeDelegate(SetCardSafe);
                this.Invoke(d, new object[] { index, cardCode, count });
            }
            else
            {
                SetCardSafe(index, cardCode, count);
            }
        }


        private void SetCardSafe(int index, string cardCode, int count)
        {
            if (index == Cards.Count)
            {
                Card newCard = CardLibrary.GetCard(cardCode);
                Cards.Add(new CardWithCount(newCard, count));
                Invalidate(GetCardRectangle(Cards.Count - 1));
            }
            else if (!Cards[index].Code.Equals(cardCode))
            {
                Card newCard = CardLibrary.GetCard(cardCode);
                Cards[index] = new CardWithCount(newCard, count);
                Invalidate(GetCardRectangle(index));
            }
            else if (Cards[index].Count != count)
            {
                Cards[index].Count = count;
                Invalidate(GetCardRectangle(index));
            }
        }

        public void ClearDeck()
        {
            if (Cards.Count > 0)
            {
                Cards.Clear();
                Invalidate();
            }
        }

        public Size GetBestSize()
        {
            return new Size(2 * SideBorderSize + CardSize.Width, TopBorderSize + BottomBorderSize + (CardSize.Height + SpacingSize) * Cards.Count - SpacingSize);
        }

        private void DeckControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), Bounds);
            Rectangle titleRect = new Rectangle(1, 1, Bounds.Width - 2, TopBorderSize - 2);
            TextRenderer.DrawText(e.Graphics, this.Title, TitleFont, titleRect, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

            Rectangle cardRect = new Rectangle(SideBorderSize, TopBorderSize, this.Bounds.Width - 2 * SideBorderSize, CardSize.Height);
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
            DrawCardArt(g, card.TheCard, cardRect);
            g.FillRectangle(new SolidBrush(Color.FromArgb(96, Color.Black)), cardRect);

            // Create font
            TextRenderer.DrawText(g, card.Cost.ToString(), StatsFont, costRect, Color.Yellow, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            TextRenderer.DrawText(g, card.Name, CardFont, cardRect, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            TextRenderer.DrawText(g, "x" + card.Count.ToString(), StatsFont, countRect, Color.GreenYellow, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

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

        private void DrawCardArt(Graphics g, Card card, Rectangle dstRect)
        {
            Image img;
            Rectangle srcRect;

            img = card.CardBanner;
            if (card.Type.Equals("Spell"))
            {
                double diagonal = Math.Sqrt(dstRect.Width * dstRect.Width + dstRect.Height * dstRect.Height);
                double scale = img.Width / diagonal;
                int newWidth = (int)(dstRect.Width * scale);
                int newHeight = (int)(dstRect.Height * scale);
                srcRect = new Rectangle((img.Width - newWidth) / 2, (img.Height - newHeight) / 2, newWidth, newHeight);
            }
            else
            {
                srcRect = new Rectangle(0, 0, img.Width, img.Height);
                // Crop vertically to preserve aspect ratio
                if (dstRect.Width * srcRect.Height > srcRect.Width * dstRect.Height)
                {
                    int newHeight = srcRect.Width * dstRect.Height / dstRect.Width;
                    int newCenter = img.Height * 4 / 10;
                    int newY = Math.Max(newCenter - newHeight / 2, 0);
                    srcRect.Y = newY;
                    srcRect.Height = newHeight;
                }
                else
                {
                    int newWidth = srcRect.Height * dstRect.Width / dstRect.Height;
                    srcRect.X += (srcRect.Width - newWidth) / 2;
                    srcRect.Width = newWidth;
                }
            }

            g.DrawImage(img, dstRect, srcRect, GraphicsUnit.Pixel);
        }

        private void DeckControl_MouseLeave(object sender, EventArgs e)
        {
            HighlightCard(-1);
        }

        private void HighlightCard(int index)
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
                CardPopup.SetCard(Cards[index].Code);
                Point topLeft = PointToScreen(new Point(0, 0));
                Rectangle cardRect = GetCardRectangle(index);
                CardPopup.SetBounds(topLeft.X + cardRect.X + cardRect.Width, topLeft.Y + cardRect.Y,
                    Cards[index].TheCard.CardArt.Width, Cards[index].TheCard.CardArt.Height);
                Invalidate(GetCardRectangle(HighlightedCard));
                if (!CardPopup.Visible)
                {
                    CardPopup.Show();
                }
            }
            else
            {
                CardPopup.Hide();
            }
        }

        private Rectangle GetCardRectangle(int index)
        {
            return new Rectangle(SideBorderSize, TopBorderSize + index * (CardSize.Height + SpacingSize), this.ClientRectangle.Width - 2 * SideBorderSize, CardSize.Height);
        }

        private void DeckControl_MouseMove(object sender, MouseEventArgs e)
        {
            int index = (e.Y - TopBorderSize) / (CardSize.Height + SpacingSize);
            if (e.Y >= TopBorderSize && index >= 0 && index < Cards.Count)
            {
                HighlightCard(index);
            }
            else
            {
                HighlightCard(-1);
            }
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //empty implementation
        }
    }
}
