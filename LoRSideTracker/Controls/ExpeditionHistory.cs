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
    public partial class ExpeditionHistory : Form
    {
        // <summary>Expedition history with picks</summary>
        public ExpeditionPick[] DraftPicks { get; set; }

        private DeckControl[] Decks;

        private static int InitialDraftSize = 15;
        private static int Margin = 40;


        /// <summary>
        /// Constructor
        /// </summary>
        public ExpeditionHistory()
        {
            InitializeComponent();
        }

        private void ApplyDraftPick(ref List<CardWithCount> cards, ExpeditionPick pick)
        {
            if (pick.IsSwap)
            {
                foreach (var cardCode in pick.SwappedOut)
                {
                    int index = cards.FindIndex(item => item.Code.Equals(cardCode));
                    if (0 == --cards[index].Count)
                    {
                        cards.RemoveAt(index);
                    }
                }
                foreach (var cardCode in pick.SwappedIn)
                {
                    int index = cards.FindIndex(item => item.Code.Equals(pick.SwappedOut));
                    if (index >= 0)
                    {
                        cards[index].Count++;
                    }
                    else
                    {
                        cards.Add(new CardWithCount(CardLibrary.GetCard(cardCode), 1, true));
                    }
                }
            }
            else
            {
                foreach (var cardCode in pick.DraftPicks)
                {
                    int index = cards.FindIndex(item => item.Code.Equals(cardCode));
                    if (index >= 0)
                    {
                        cards[index].Count++;
                    }
                    else
                    {
                        cards.Add(new CardWithCount(CardLibrary.GetCard(cardCode), 1, true));
                    }
                }
            }

        }

        private void ExpeditionHistory_Load(object sender, EventArgs e)
        {
            int numDecks = DraftPicks.Length - InitialDraftSize;
            if (numDecks > 0)
            {
                Decks = new DeckControl[numDecks];
            }
            List<CardWithCount> cards = new List<CardWithCount>();
            for (int i = 0; i < InitialDraftSize - 1; i++)
            {
                ApplyDraftPick(ref cards, DraftPicks[i]);
            }
            for (int i = 0; i < numDecks; i++)
            {
                ApplyDraftPick(ref cards, DraftPicks[InitialDraftSize - 1 + i]);
                Decks[i] = new DeckControl();
                Decks[i].Cards = cards.Clone().OrderBy(card => card.Cost).ThenBy(card => card.Name).ToList();

                ScrollablePanel.Controls.Add(Decks[i]);
                Decks[i].SetBounds(100 * i, 10, 100, 100);
                Decks[i].Visible = true;
            }
        }

        private void ExpeditionHistory_Shown(object sender, EventArgs e)
        {
            int left = 8;
            int top = 8;
            for (int i = 0; i < Decks.Length; i++)
            {
                Size size = Decks[i].GetBestSize();
                Decks[i].SetBounds(left, top, size.Width, size.Height);
                left += size.Width + Margin;
            }
        }

        private void ScrollablePanel_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i + 1 < Decks.Length; i++)
            {
                List<Rectangle> removedCards = new List<Rectangle>();
                List<Rectangle> addedCards = new List<Rectangle>();
                var draftPick = DraftPicks[InitialDraftSize + i];
                if (draftPick.IsSwap)
                {
                    foreach (var code in draftPick.SwappedOut)
                    {
                        removedCards.Add(Decks[i].GetCardRectangle(code));
                    }
                    foreach (var code in draftPick.SwappedIn)
                    {
                        addedCards.Add(Decks[i + 1].GetCardRectangle(code));
                    }
                }
                else
                {
                    foreach (var code in draftPick.DraftPicks)
                    {
                        addedCards.Add(Decks[i + 1].GetCardRectangle(code));
                    }
                }

                // Draw - where removed
                Rectangle textRect = new Rectangle();
                textRect.X = Decks[i].Bounds.X + Decks[i].Bounds.Width;
                textRect.Width = Margin / 2;
                foreach (var r in removedCards)
                {
                    textRect.Y = Decks[i].Bounds.Y + r.Y;
                    textRect.Height = r.Height;
                    TextRenderer.DrawText(e.Graphics, "-", Decks[i].CustomDeckScale.TitleFont, textRect, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                }

                // Draw + where added
                textRect.X = Decks[i + 1].Bounds.X - Margin / 2;
                textRect.Width = Margin / 2;
                foreach (var r in addedCards)
                {
                    textRect.Y = Decks[i + 1].Bounds.Y + r.Y;
                    textRect.Height = r.Height;
                    TextRenderer.DrawText(e.Graphics, "+", Decks[i + 1].CustomDeckScale.TitleFont, textRect, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                }
            }
        }
    }
}
