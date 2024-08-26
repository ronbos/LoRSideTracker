using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LoRSideTracker.Controls
{
    /// <summary>
    /// Deck list control
    /// </summary>
    public partial class DecksListControl : UserControl
    {
        // The event does not have any data, so EventHandler is adequate
        // as the event delegate.
        private EventHandler onSelectionChanged;

        /// <summary>
        /// Define the event member using the event keyword.  
        /// In this case, for efficiency, the event is defined
        /// using the event property construct.  
        /// </summary>
        public event EventHandler SelectionChanged
        {
            add
            {
                onSelectionChanged += value;
            }
            remove
            {
                onSelectionChanged -= value;
            }
        }

        /// <summary>
        /// Currently selected game record (null if none is selected)
        /// </summary>
        public GameRecord SelectedItem { get; private set; }


        /// <summary>
        /// Constructor
        /// </summary>
        public DecksListControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Add a deck to the top of the list of either decks or expeditions
        /// If game record matches existing entry, old entry is removed from list
        /// </summary>
        /// <param name="gr">Game record to add</param>
        /// <param name="update">Should UI be refreshed</param>
        public void AddToDeckList(GameRecord gr, bool update = false)
        {
            int numConstructedWins = 0;
            int numConstructedLosses = 0;
            int numConstructedWinsVsAI = 0;
            int numConstructedLossesVsAI = 0;

            ListBox listBox = gr.IsAdventure() ? AdventuresListBox : DecksListBox;

            // Does the deck already exist in the list? If it does, remove it
            bool deckFound = false;
            string deckSig = gr.GetDeckSignature();
            string deckName = gr.MyDeckName;
            int index = 0;
            //while (index >= 0)
            {
                index = listBox.Items.Cast<GameRecord>().ToList().FindIndex(x => deckSig == x.GetDeckSignature());
                if (index == -1 && !gr.IsAdventure() && gr.MyDeckName != GameRecord.DefaultConstructedDeckName)
                {
                    index = listBox.Items.Cast<GameRecord>().ToList().FindIndex(x => deckName == x.MyDeckName);
                }
                if (index != -1)
                {
                    // Keep the deck name
                    GameRecord grPrevious = (GameRecord)listBox.Items[index];

                    if (!gr.IsAdventure() || Utilities.SameAdventureDecks(grPrevious.MyDeck, gr.MyDeck))
                    {
                        numConstructedWins = grPrevious.NumWins;
                        numConstructedLosses = grPrevious.NumLosses;
                        numConstructedWinsVsAI = grPrevious.NumWinsVsAI;
                        numConstructedLossesVsAI = grPrevious.NumLossesVsAI;
                        gr.DeckOrdinal = grPrevious.DeckOrdinal;

                        deckName = grPrevious.ToString();
                        // Remove record
                        int baseNameEnd = deckName.LastIndexOf(" (");
                        if (baseNameEnd >= 0)
                        {
                            deckName = deckName.Substring(0, baseNameEnd);
                        }

                        // Remove old item
                        listBox.Items.RemoveAt(index);
                        deckFound = true;
                    }
                    else
                    {
                        gr.DeckOrdinal = grPrevious.DeckOrdinal + 1;
                    }
                }
            }

            if (!deckFound)
            {
                deckName = gr.MyDeckName;
            }

            // Update record
            gr.NumWins = numConstructedWins;
            gr.NumLosses = numConstructedLosses;
            gr.NumWinsVsAI = numConstructedWinsVsAI;
            gr.NumLossesVsAI = numConstructedLossesVsAI;
            if (gr.OpponentIsAI)
            {
                if (gr.Result == "Win") gr.NumWinsVsAI++;
                else gr.NumLossesVsAI++;
            }
            else
            {
                if (gr.Result == "Win") gr.NumWins++;
                else gr.NumLosses++;
            }

            deckName += GetWinLossRecordString(gr);

            gr.DisplayString = deckName;
            listBox.Items.Insert(0, gr);
            if (update)
            {
                SwitchDeckView(gr.IsAdventure());
            }
        }


        /// <summary>
        /// Remove a deck from the list
        /// </summary>
        /// <param name="gr">Game record to remove</param>
        public void RemoveFromDeckList(GameRecord gr)
        {
            ListBox listBox = gr.IsAdventure() ? AdventuresListBox : DecksListBox;

            // Does the deck already exist in the list? If it does, remove it
            string deckSig = gr.GetDeckSignature();
            string deckName = gr.MyDeckName;
            int index = listBox.Items.Cast<GameRecord>().ToList().FindIndex(x => deckSig == x.GetDeckSignature());
            if (index == -1 && !gr.IsAdventure() && deckName != GameRecord.DefaultConstructedDeckName)
            {
                index = listBox.Items.Cast<GameRecord>().ToList().FindIndex(x => deckName == x.MyDeckName);
            }
            if (index != -1)
            {
                listBox.Items.RemoveAt(index);
                if (listBox.Items.Count > 0)
                {
                    listBox.SelectedIndex = index + 1 < listBox.Items.Count ? index : index - 1;
                }
                listBox.Refresh();
                ListBox_SelectedIndexChanged(listBox, null);
            }
        }

        /// <summary>
        /// Switch to viewing decks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecksButton_Click(object sender, EventArgs e)
        {
            SwitchDeckView(false);
        }

        /// <summary>
        /// Switch to viewing expeditions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdventuresButton_Click(object sender, EventArgs e)
        {
            SwitchDeckView(true);
        }

        /// <summary>
        /// Switch deck view to decks or expeditions
        /// </summary>
        /// <param name="showAdventures">If true, switch to Expeditions</param>
        public void SwitchDeckView(bool showAdventures)
        {
            ListBox fromListBox, toListBox;
            Button fromButton, toButton;
            if (showAdventures)
            {
                fromListBox = DecksListBox;
                fromButton = DecksButton;
                toListBox = AdventuresListBox;
                toButton = AdventuresButton;
            }
            else
            {
                fromListBox = AdventuresListBox;
                fromButton = AdventuresButton;
                toListBox = DecksListBox;
                toButton = DecksButton;
            }
            if (fromListBox.Visible)
            {
                fromButton.BackColor = BackColor;
                fromButton.FlatAppearance.MouseOverBackColor = toButton.FlatAppearance.MouseOverBackColor;
                fromButton.FlatAppearance.MouseDownBackColor = toButton.FlatAppearance.MouseDownBackColor;
                toButton.BackColor = Color.FromArgb(BackColor.R * 2, BackColor.G * 2, BackColor.B * 2);
                toButton.FlatAppearance.MouseOverBackColor = toButton.BackColor;
                toButton.FlatAppearance.MouseDownBackColor = toButton.BackColor;
                fromListBox.Visible = false;
                toListBox.Visible = true;
                fromListBox.SelectedIndex = -1;
            }

            if (toListBox.Items.Count > 0)
            {
                toListBox.SelectedIndex = 0;
            }
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            SelectedItem = (GameRecord)listBox.SelectedItem;
            onSelectionChanged?.Invoke(this, null);
        }

        private void ListBox_DoubleClick(object sender, EventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            int index = listBox.SelectedIndex;
            if (listBox == AdventuresListBox)
            {
                // Nothing for now
            }
            else
            {
                // Rename the deck
                if (index >= 0)
                {
                    GameRecord gr = (GameRecord)listBox.Items[index];

                    string deckName = gr.ToString();
                    int scoreIndex = deckName.LastIndexOf(" (");
                    if (scoreIndex > 0)
                    {
                        deckName = deckName.Substring(0, scoreIndex);
                    }

                    string result = Microsoft.VisualBasic.Interaction.InputBox("Name:", "Change Deck Name", deckName);
                    if (!string.IsNullOrEmpty(result) && deckName != result)
                    {
                        if (!gr.IsAdventure())
                        {
                            gr.MyDeckName = result;
                        }

                        GameHistory.SetDeckName(gr.GetDeckSignature(), result);

                        if (!gr.IsAdventure())
                        {
                            index = listBox.Items.Cast<GameRecord>().ToList().FindIndex(x => x.MyDeckName.StartsWith(result));
                            int nextIndex = index;
                            while (nextIndex >= 0 && nextIndex < listBox.Items.Count - 1)
                            {
                                nextIndex = listBox.Items.Cast<GameRecord>().ToList().FindIndex(nextIndex + 1, x => result == x.MyDeckName);
                                if (nextIndex > index)
                                {
                                    ((GameRecord)listBox.Items[index]).NumWins += ((GameRecord)listBox.Items[nextIndex]).NumWins;
                                    ((GameRecord)listBox.Items[index]).NumLosses += ((GameRecord)listBox.Items[nextIndex]).NumLosses;
                                    ((GameRecord)listBox.Items[index]).NumWinsVsAI += ((GameRecord)listBox.Items[nextIndex]).NumWinsVsAI;
                                    ((GameRecord)listBox.Items[index]).NumLossesVsAI += ((GameRecord)listBox.Items[nextIndex]).NumLossesVsAI;
                                    listBox.Items.RemoveAt(nextIndex);
                                }
                            }
                        }

                        ((GameRecord)listBox.Items[index]).DisplayString = result + GetWinLossRecordString((GameRecord)listBox.Items[index]);
                        listBox.Items[index] = (GameRecord)listBox.Items[index];
                        listBox.SelectedIndex = index;
                        listBox.Refresh();
                        ListBox_SelectedIndexChanged(sender, null);
                    }
                }
            }

        }

        private void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            e.DrawBackground();
            Rectangle rect = e.Bounds;
            rect.Height--;
            if (e.Index >= 0)
            {
                Color borderColor = e.ForeColor;
                if (!e.State.HasFlag(DrawItemState.Selected))
                {
                    borderColor = Color.FromArgb(
                        (e.ForeColor.R + e.BackColor.R) / 2,
                        (e.ForeColor.G + e.BackColor.G) / 2,
                        (e.ForeColor.B + e.BackColor.B) / 2);
                }
                e.Graphics.FillRectangle(new SolidBrush(borderColor), rect);
                rect.Inflate(-1, -1);

                GameRecord gr = (GameRecord)listBox.Items[e.Index];
                // Find the card to use for art
                int drawIndex = -1;
                int championCount = 0;
                for (int i = gr.MyDeck.Count - 1; i >= 0; i--)
                {
                    if (gr.MyDeck[i].TheCard.SuperType == "Champion" && gr.MyDeck[i].Count > championCount)
                    {
                        drawIndex = i;
                        championCount = gr.MyDeck[i].Count;
                    }
                    else if (gr.MyDeck[i].TheCard.Type == "Unit" && drawIndex == -1)
                    {
                        drawIndex = i;
                    }
                }
                if (drawIndex >= 0)
                {
                    gr.MyDeck[drawIndex].TheCard.DrawCardBanner(e.Graphics, rect);
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(
                        e.State.HasFlag(DrawItemState.Selected) ? 96 : 160, Color.Black)), rect);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(e.BackColor), rect);
                }

                // Determine deck regions
                Dictionary<string, int> regions = new Dictionary<string, int>();
                Dictionary<string, int> uniqueRegions = new Dictionary<string, int>();
                if (gr.IsAdventure() && gr.MyDeckCode.StartsWith("adventure_"))
                {
                    string[] champions = gr.MyDeckCode.Split(new char[] { '_' });
                    int orderingCount = 10;
                    for (int i = 1; i < champions.Length; i++)
                    {
                        Card champCard = CardLibrary.GetCardByName(champions[i]);
                        foreach (var r in champCard.Regions)
                        {
                            regions.TryGetValue(r, out int currentCount);
                            if (currentCount == 0)
                            {
                                regions[r] = orderingCount--;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var c in gr.MyDeck)
                    {
                        if (c.TheCard.Regions.Length == 1)
                        {
                            regions.TryGetValue(c.TheCard.Regions[0], out int currentCount);
                            regions[c.TheCard.Regions[0]] = currentCount + 1;
                        }
                    }
                }

                // Sort the regions from lowest to highest
                var regionsInOrder = regions.OrderBy(i => i.Value).ToList();

                // Draw regions from right to left
                // Multiregion cards increase the population here
                int right = rect.Right;
                foreach (var r in regionsInOrder)
                {
                    Image img = CardLibrary.GetRegion(r.Key).Banner;
                    int width = img.Width * rect.Height / img.Height * 7 / 8;
                    int height = width * img.Height / img.Height;
                    Rectangle imgRect = new Rectangle(right - width, rect.Top, width, height);
                    e.Graphics.DrawImage(img, imgRect, new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                    right -= width * 7 / 8;
                }

                TextRenderer.DrawText(e.Graphics, gr.ToString(), e.Font, rect, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
        }

        private string GetBaseDeckName(GameRecord gr)
        {
            string deckName = gr.MyDeckName;
            if (gr.IsAdventure() && deckName[deckName.Length - 3] == '-')
            {
                // Remove record
                deckName = deckName.Substring(0, deckName.Length - 6);
            }
            return deckName;
        }

        private string GetWinLossRecordString(GameRecord gr)
        {
            if (gr.IsAdventure())
            {
                return string.Format(@" ({0}-{1})", gr.NumWinsVsAI, gr.NumLossesVsAI);
            }
            else if ((gr.NumWins + gr.NumLosses) > 0)
            {
                double winPercentage = (double)gr.NumWins / (gr.NumWins + gr.NumLosses);
                return string.Format(@" ({0:0.0}%)", winPercentage * 100);
            }
            return " (-)";
        }

        private void DecksListControl_Load(object sender, EventArgs e)
        {
            // Set up constructed deck list to be visible
            SwitchDeckView(false);
        }

        //public bool DeleteCurrentDeck(object sender, KeyEventArgs e)
        //{
        //}
    }
}
