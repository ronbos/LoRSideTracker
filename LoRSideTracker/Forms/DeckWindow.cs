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
        private List<CardWithCount> CurrentCards = new List<CardWithCount>();

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

            MyDeckControl.BorderSize = 3;
        }

        /// <summary>
        /// Set the full deck
        /// </summary>
        /// <param name="allCards">Deck contents</param>
        public void SetFullDeck(List<CardWithCount> allCards)
        {
            if (!allCards.SequenceEqual(AllCards))
            {
                AllCards = Utilities.Clone(allCards);
                CurrentCards = Utilities.Clone(allCards);
                UpdateDeck();
            }
        }

        /// <summary>
        /// Set set of cards that have been drawn from the deck
        /// </summary>
        /// <param name="currentCards">Set of drawn cards</param>
        public void SetCurrentDeck(List<CardWithCount> currentCards)
        {
            if (!currentCards.SequenceEqual(CurrentCards))
            {
                CurrentCards = Utilities.Clone(currentCards);
                UpdateDeck();
            }
        }

        /// <summary>
        /// Reevaluate current deck -- useful when flipping zero count visibility
        /// </summary>
        public void UpdateDeck()
        {
            Utilities.CallActionSafelyAndWait(this, new Action(() => { UpdateDeckSafe(); }));
        }

        /// <summary>
        /// Safely update full deck and drawn cards contents, and redraw the set of displayed cards
        /// </summary>
        private void UpdateDeckSafe()
        {
            // Assume both AllCards and CurrentCards are sorted
            List<CardWithCount> remainingCards = CurrentCards;
            if (AllCards.Count > 0)
            {
                remainingCards = Utilities.Clone(AllCards);
                int j = 0;
                for (int i = 0; i < remainingCards.Count; i++)
                {
                    if (j < CurrentCards.Count && remainingCards[i].Code == CurrentCards[j].Code)
                    {
                        remainingCards[i].Count = CurrentCards[j].Count;
                        j++;
                    }
                    else
                    {
                        remainingCards[i].Count = 0;
                    }
                }

                if (HideZeroCountCards)
                {
                    remainingCards = remainingCards.FindAll(x => x.Count > 0).ToList();
                }
            }

            if (remainingCards.Count == 0)
            {
                MyDeckControl.IsMinimized = false;
            }

            MyDeckControl.SetDeck(remainingCards);
            MyDeckStatsDisplay.TheDeck = remainingCards;
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
        /// <param name="shouldInvalidate">If true, deck control is invalidated</param>
        /// </summary>
        private void FixMySize(bool shouldInvalidate = true)
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
            if (shouldInvalidate)
            {
                MyDeckControl.Invalidate();
            }
        }

        private void DeckWindow_Load(object sender, EventArgs e)
        {
            FixMySize(false);
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
