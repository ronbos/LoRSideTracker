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
        private List<CardWithCount> TossedCards = new List<CardWithCount>();
        private List<CardWithCount> RemainingCards = new List<CardWithCount>();

        /// <summary>Are cards with zero count shown?</summary>
        public bool HideZeroCountCards { get; set; } = false;

        /// <summary>Are deck stats shown?</summary>
        public bool ShouldShowDeckStats { get; set; } = true;
        /// <summary>Window title</summary>
        public string Title
        {
            get { return MyDeckControl != null ? MyDeckControl.Title : string.Empty; }
            set { 
                MyDeckControl.Title = value; 
                MyDeckControl.Invalidate(new Rectangle(0, 0, MyDeckControl.Width, MyDeckControl.CustomDeckScale.TitleHeight));
            }
        }

        /// <summary>Custom deck scale</summary>
        public DeckControl.DeckScale CustomDeckScale
        {
            get { return MyDeckControl.CustomDeckScale; }
            set
            {
                if (MyDeckControl.CustomDeckScale.TitleHeight != value.TitleHeight)
                {
                    MyDeckControl.CustomDeckScale = value;
                    FixMySize();
                }
            }
        }
        /// <summary>Should window hide when mouse leaves?</summary>
        public bool ShouldHideOnMouseLeave { get; set; } = false;


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
            AllCards = Utilities.Clone(allCards);
            UpdateDeck();
        }

        /// <summary>
        /// Set set of cards that have been drawn from the deck
        /// </summary>
        /// <param name="drawnCards">Set of drawn cards</param>
        public void SetDrawnCards(List<CardWithCount> drawnCards)
        {
            DrawnCards = Utilities.Clone(drawnCards);
            UpdateDeck();
        }

        /// <summary>
        /// Set set of cards that have been tossed from the deck
        /// </summary>
        /// <param name="tossedCards">Set of drawn cards</param>
        public void SetTossedCards(List<CardWithCount> tossedCards)
        {
            TossedCards = Utilities.Clone(tossedCards);
            UpdateDeck();
        }

        /// <summary>
        /// Reevaluate current deck -- useful when flipping zero count visibility
        /// </summary>
        public void RefreshDeck()
        {
            if (this.InvokeRequired)
            {
                Invoke(new Action(() => { RefreshDeckSafe(); }));
            }
            else
            {
                RefreshDeckSafe();
            }
        }

        private void UpdateDeck()
        {
            List<CardWithCount> remainingCards = Utilities.Clone(AllCards);
            foreach (var card in DrawnCards)
            {
                int index = remainingCards.FindIndex(item => item.Code.Equals(card.Code));
                if (index >= 0)
                {
                    remainingCards[index].Count -= card.Count;
                }
            }
            foreach (var card in TossedCards)
            {
                int index = remainingCards.FindIndex(item => item.Code.Equals(card.Code));
                if (index >= 0)
                {
                    remainingCards[index].Count -= card.Count;
                }
            }

            RemainingCards = Utilities.Clone(remainingCards);

            RefreshDeck();
        }

        /// <summary>
        /// Safely update full deck and drawn cards contents, and redraw the set of displayed cards
        /// </summary>
        private void RefreshDeckSafe()
        {
            if (AllCards.Count == 0)
            {
                MyDeckControl.IsMinimized = false;
            }

            MyDeckControl.ClearDeck();
            int nextCard = 0;
            for (int i = 0; i < RemainingCards.Count; i++)
            {
                if (!HideZeroCountCards || RemainingCards[i].Count > 0)
                {
                    MyDeckControl.SetCard(nextCard, RemainingCards[i]);
                    nextCard++;
                }
            }
            MyDeckStatsDisplay.TheDeck = RemainingCards;
            MyDeckStatsDisplay.Invalidate();
            FixMySize();
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
            MyDeckControl.Invalidate();
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
                    if (e.Y < MyDeckControl.CustomDeckScale.TitleHeight)
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

        private void DeckWindow_MouseLeave(object sender, EventArgs e)
        {
            if (ShouldHideOnMouseLeave)
            {
                Hide();
            }
        }

        private void MyDeckControl_MouseLeave(object sender, EventArgs e)
        {
            //MyDeckControl.HighlightCard(-1);
            if (ShouldHideOnMouseLeave)
            {
                Hide();
            }
        }
    }
}
