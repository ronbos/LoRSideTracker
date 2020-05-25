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
    /// Top-most window to display deck contents
    /// </summary>
    public partial class DeckWindow : Form
    {
        private List<CardWithCount> AllCards = new List<CardWithCount>();
        private List<CardWithCount> DrawnCards = new List<CardWithCount>();
        private List<CardWithCount> RemainingCards = new List<CardWithCount>();

        private delegate void UpdateDeckSafeDelegate(List<CardWithCount> allCards, List<CardWithCount> drawnCards);

        /// <summary>If true, window is hidden when empty</summary>
        public bool ShouldHideWhenEmpty { get; set; } = false;

        /// <summary>Deck stats height</summary>
        public bool ShouldShowDeckStats { get; set; } = true;

        /// <summary>Window title</summary>
        public string Title
        {
            get { return MyDeckControl != null ? MyDeckControl.Title : ""; }
            set { MyDeckControl.Title = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DeckWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set the full deck
        /// </summary>
        /// <param name="allCards">Deck contents</param>
        public void SetFullDeck(List<CardWithCount> allCards)
        {
            if (this.InvokeRequired)
            {
                var d = new UpdateDeckSafeDelegate(UpdateDeckSafe);
                this.Invoke(d, new object[] { Utilities.Clone(allCards), Utilities.Clone(DrawnCards) });
            }
            else
            {
                UpdateDeckSafe(allCards, DrawnCards);
            }
        }

        /// <summary>
        /// Set set of cards that have been drawn from the deck
        /// </summary>
        /// <param name="drawnCards">Set of drawn cards</param>
        public void SetDrawnCards(List<CardWithCount> drawnCards)
        {
            if (this.InvokeRequired)
            {
                var d = new UpdateDeckSafeDelegate(UpdateDeckSafe);
                this.Invoke(d, new object[] { Utilities.Clone(AllCards), Utilities.Clone(drawnCards) });
            }
            else
            {
                UpdateDeckSafe(AllCards, Utilities.Clone(drawnCards));
            }
        }

        /// <summary>
        /// Safely update full deck and drawn cards contents, and redraw the set of displayed cards
        /// </summary>
        /// <param name="allCards"></param>
        /// <param name="drawnCards"></param>
        private void UpdateDeckSafe(List<CardWithCount> allCards, List<CardWithCount> drawnCards)
        {
            List<CardWithCount> remainingCards = Utilities.Clone(allCards);
            foreach (var card in drawnCards)
            {
                int index = remainingCards.FindIndex(item => item.Code.Equals(card.Code));
                if (index >= 0)
                {
                    remainingCards[index].Count -= card.Count;
                }
            }

            if (AllCards.Count == 0)
            {
                MyDeckControl.IsMinimized = false;
            }

            AllCards = Utilities.Clone(allCards);
            DrawnCards = Utilities.Clone(drawnCards);
            RemainingCards = Utilities.Clone(remainingCards);

            MyDeckControl.ClearDeck();
            for (int i = 0; i < RemainingCards.Count; i++)
            {
                MyDeckControl.SetCard(i, RemainingCards[i]);
            }
            MyDeckStatsDisplay.TheDeck = RemainingCards;
            MyDeckStatsDisplay.Invalidate();
            FixMySize();

            if (RemainingCards.Count == 0)
            {
                if (ShouldHideWhenEmpty) Hide();
                return;
            }
        }

        /// <summary>
        /// Update the size of the window and the controls
        /// </summary>
        public void UpdateSize()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => { FixMySize(); }));
            }
            else
            {
                FixMySize();
            }
        }

        /// <summary>
        /// Fix the size of the window to match the size of the deck control
        /// </summary>
        private void FixMySize()
        {
            Size bestSize = MyDeckControl.GetBestSize();
            int deckStatsHeight = MyDeckStatsDisplay.GetBestHeight(bestSize.Width);
            MyDeckControl.SetBounds(0, 0, bestSize.Width, bestSize.Height, BoundsSpecified.Size);
            if (ShouldShowDeckStats && deckStatsHeight > 0)
            {
                MyDeckStatsDisplay.SetBounds(0, bestSize.Height, bestSize.Width, deckStatsHeight, BoundsSpecified.All);
                MyDeckStatsDisplay.Visible = true;
            }
            else
            {
                MyDeckStatsDisplay.Visible = false;
                deckStatsHeight = 0;
            }
            SetBounds(0, 0, bestSize.Width, bestSize.Height + deckStatsHeight, BoundsSpecified.Size);
        }

        private void DeckWindow_Load(object sender, EventArgs e)
        {
            Size bestSize = MyDeckControl.GetBestSize();
            int deckStatsHeight = MyDeckStatsDisplay.GetBestHeight(bestSize.Width);
            MyDeckControl.SetBounds(0, 0, bestSize.Width, bestSize.Height, BoundsSpecified.All);
            if (deckStatsHeight > 0)
            {
                MyDeckStatsDisplay.SetBounds(0, bestSize.Height, bestSize.Width, deckStatsHeight, BoundsSpecified.All);
                MyDeckStatsDisplay.Visible = true;
            }
            else
            {
                MyDeckStatsDisplay.Visible = false;
            }
            SetBounds(0, 0, bestSize.Width, bestSize.Height + deckStatsHeight, BoundsSpecified.Size);
        }

        /// <summary>
        /// Imported Win32 function to send message
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        /// <summary>
        /// Imported Win32 function to release mouse capture
        /// </summary>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();


        /// <summary>
        /// Process mouse down events to allow dragging of window, or
        /// shrink when double-clicking the title
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        private void MyDeckControl_MouseDown(object sender, MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                if (e.Clicks == 2)
                {
                    if (e.Y < MyDeckControl.TopBorderSize)
                    {
                        MyDeckControl.IsMinimized = !MyDeckControl.IsMinimized;
                        FixMySize();
                    }
                }
                else
                {
                    const int WM_NCLBUTTONDOWN = 0xA1;
                    const int HT_CAPTION = 0x2;

                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
        }
    }
}
