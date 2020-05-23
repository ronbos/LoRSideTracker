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
    public partial class DeckWindow : Form
    {
        private List<CardWithCount> AllCards = new List<CardWithCount>();
        private List<CardWithCount> DrawnCards = new List<CardWithCount>();
        private List<CardWithCount> RemainingCards = new List<CardWithCount>();

        private delegate void UpdateDeckSafeDelegate(List<CardWithCount> allCards, List<CardWithCount> drawnCards);

        public string Title
        {
            get { return MyDeckControl != null ? MyDeckControl.Title : ""; }
            set { MyDeckControl.Title = value; }
        }

        public DeckWindow()
        {
            InitializeComponent();
        }

        //public Deck GetCurrentDeck()
        //{
        //    return List<CardWithCount>;
        //}

        public void SetFullDeck(List<CardWithCount> allCards)
        {
            if (this.InvokeRequired)
            {
                var d = new UpdateDeckSafeDelegate(UpdateDeckSafe);
                this.Invoke(d, new object[] { allCards, DrawnCards });
            }
            else
            {
                UpdateDeckSafe(allCards, DrawnCards);
            }
        }

        public void SetDrawnCards(List<CardWithCount> drawnCards)
        {
            if (this.InvokeRequired)
            {
                var d = new UpdateDeckSafeDelegate(UpdateDeckSafe);
                this.Invoke(d, new object[] { AllCards, drawnCards });
            }
            else
            {
                UpdateDeckSafe(AllCards, Utilities.Clone(drawnCards));
            }
        }

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

            AllCards = Utilities.Clone(allCards);
            DrawnCards = Utilities.Clone(drawnCards);
            RemainingCards = Utilities.Clone(remainingCards);

            if (RemainingCards.Count == 0)
            {
                Hide();
                return;
            }
            
            MyDeckControl.ClearDeck();
            for (int i = 0; i < RemainingCards.Count; i++)
            {
                MyDeckControl.SetCard(i, RemainingCards[i].Code, RemainingCards[i].Count);
            }
            Size bestSize = MyDeckControl.GetBestSize();
            Rectangle currentBounds = MyDeckControl.Bounds;
            MyDeckControl.SetBounds(MyDeckControl.Bounds.X, MyDeckControl.Bounds.Y, bestSize.Width, bestSize.Height);
            SetBounds(0, 0, bestSize.Width, bestSize.Height, BoundsSpecified.Size);
            Show();
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void MyDeckControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //empty implementation
        }

        private void DeckWindow_Shown(object sender, EventArgs e)
        {
            Size bestSize = MyDeckControl.GetBestSize();
            Rectangle currentBounds = MyDeckControl.Bounds;
            MyDeckControl.SetBounds(MyDeckControl.Bounds.X, MyDeckControl.Bounds.Y, bestSize.Width, bestSize.Height);
            SetBounds(0, 0, bestSize.Width, bestSize.Height, BoundsSpecified.Size);
        }
    }
}
