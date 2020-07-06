using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoRSideTracker.Controls
{
    /// <summary>
    /// Deck list control
    /// </summary>
    public partial class DecksListControl : UserControl
    {
        private int ExpeditionsCount = 0;

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
            ListBox listBox = gr.IsExpedition() ? ExpeditionsListBox : DecksListBox;

            // Does the deck already exist in the list? If it does, remove it
            bool deckFound = false;
            string deckSig = gr.GetDeckSignature();
            string deckName = gr.MyDeckName;
            int index = 0;
            while (index >= 0)
            {
                index = listBox.Items.Cast<GameRecord>().ToList().FindIndex(x => deckSig == x.GetDeckSignature());
                if (index == -1 && !gr.IsExpedition() && gr.MyDeckName != GameRecord.DefaultConstructedDeckName)
                {
                    index = listBox.Items.Cast<GameRecord>().ToList().FindIndex(x => deckName == x.MyDeckName);
                }
                if (index != -1)
                {
                    // Keep the deck name
                    deckName = ((GameRecord)listBox.Items[index]).ToString();
                    if (gr.IsExpedition())
                    {
                        // Remove record
                        int baseNameEnd = deckName.LastIndexOf(" (");
                        if (baseNameEnd >= 0)
                        {
                            deckName = deckName.Substring(0, baseNameEnd);
                        }
                    }

                    // Remove old item
                    listBox.Items.RemoveAt(index);
                    deckFound = true;
                }
            }

            if (!deckFound)
            {
                if (gr.IsExpedition())
                {
                    // This is guaranteed to be a new expedition, increase expeditions count
                    ExpeditionsCount++;

                    // Default expedition name if it is not customized
                    deckName = string.Format("Expedition #{0}", ExpeditionsCount);
                }
                else
                {
                    deckName = gr.MyDeckName;
                }

                // Map the name if it has been customized (if not, default name is kept)
                //try { deckName = GameHistory.DeckNames[gr.GetDeckSignature()]; } catch { }
            }

            // Add the new item
            if (!string.IsNullOrEmpty(gr.ExpeditionSignature))
            {
                try { deckName = GameHistory.DeckNames[gr.ExpeditionSignature]; } catch { }
                deckName += GetExpeditionRecordString(gr);
            }
            gr.DisplayString = deckName;
            listBox.Items.Insert(0, gr);
            if (update)
            {
                SwitchDeckView(gr.IsExpedition());
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
        private void ExpeditionsButton_Click(object sender, EventArgs e)
        {
            SwitchDeckView(true);
        }

        /// <summary>
        /// Switch deck view to decks or expeditions
        /// </summary>
        /// <param name="showExpeditions">If true, switch to Expeditions</param>
        public void SwitchDeckView(bool showExpeditions)
        {
            ListBox fromListBox, toListBox;
            Button fromButton, toButton;
            if (showExpeditions)
            {
                fromListBox = DecksListBox;
                fromButton = DecksButton;
                toListBox = ExpeditionsListBox;
                toButton = ExpeditionsButton;
            }
            else
            {
                fromListBox = ExpeditionsListBox;
                fromButton = ExpeditionsButton;
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
            if (index >= 0)
            {
                GameRecord gr = (GameRecord)((GameRecord)listBox.Items[index]);

                string deckName = gr.ToString();
                if (deckName[deckName.Length - 5] == '(' && deckName[deckName.Length - 3] == '-' && deckName[deckName.Length - 1] == ')')
                {
                    deckName = deckName.Substring(0, deckName.Length - 6);
                }
                string result = Microsoft.VisualBasic.Interaction.InputBox("Name:", "Change Deck Name", deckName);
                if (!string.IsNullOrEmpty(result))
                {
                    gr.DisplayString = result + GetExpeditionRecordString(gr);
                    if (!gr.IsExpedition())
                    {
                        gr.MyDeckName = result;
                    }
                    listBox.Items[index] = gr;
                    listBox.Refresh();

                    GameHistory.SetDeckName(gr.GetDeckSignature(), result);

                    if (!gr.IsExpedition())
                    {
                        int firstIndex = listBox.Items.Cast<GameRecord>().ToList().FindIndex(x => result == x.MyDeckName);
                        int nextIndex = firstIndex;
                        while (nextIndex >= 0 && nextIndex < listBox.Items.Count - 1)
                        {
                            nextIndex = listBox.Items.Cast<GameRecord>().ToList().FindIndex(nextIndex + 1, x => result == x.MyDeckName);
                            if (nextIndex > firstIndex)
                            {
                                listBox.Items.RemoveAt(nextIndex);
                            }
                        }

                        listBox.SelectedIndex = firstIndex;
                    }
                    ListBox_SelectedIndexChanged(sender, null);
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
                foreach (var c in gr.MyDeck)
                {
                    regions.TryGetValue(c.TheCard.Region, out int currentCount);
                    regions[c.TheCard.Region] = currentCount + 1;
                }

                // Sort the regions from lowest to highest
                var regionsInReverseOrder = regions.OrderBy(i => i.Value).ToList();

                // Draw all regions from right to left
                int right = rect.Right;
                for (int i = 0; i < regionsInReverseOrder.Count; i++)
                {
                    Image img = CardLibrary.GetRegion(regionsInReverseOrder[i].Key).Banner;
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
            if (gr.IsExpedition() && deckName[deckName.Length - 3] == '-')
            {
                // Remove record
                deckName = deckName.Substring(0, deckName.Length - 6);
            }
            return deckName;
        }

        private string GetExpeditionRecordString(GameRecord gr)
        {
            string deckName = gr.MyDeckName;
            if (gr.IsExpedition())
            {
                if (deckName[deckName.Length - 2] == '-')
                {
                    // Remove record
                    return " (" + deckName.Substring(deckName.Length - 3, 3) + ")";
                }
                else if (deckName[deckName.Length - 3] == '-' && deckName[deckName.Length - 1] == '*')
                {
                    return " (" + deckName.Substring(deckName.Length - 4, 3) + ")";
                }
            }
            return "";

        }

        private void DecksListControl_Load(object sender, EventArgs e)
        {
            // Set up constructed deck list to be visible
            SwitchDeckView(false);
        }
    }
}
