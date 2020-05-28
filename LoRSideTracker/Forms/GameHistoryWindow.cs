using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoRSideTracker
{
    public partial class GameHistoryWindow : Form
    {
        public GameHistoryWindow()
        {
            InitializeComponent();
        }

        public void AddGameRecord(GameRecord gr)
        {
            MyGameHistoryControl.AddGameRecord(gr);
            MyGameHistoryControl.SetBounds(0, 0, MyGameHistoryControl.BestWidth, MyGameHistoryControl.BestHeight);
        }

        private void GameHistory_Load(object sender, EventArgs e)
        {
            MyGameHistoryControl.SetBounds(0, 0, MyGameHistoryControl.BestWidth, MyGameHistoryControl.BestHeight);
        }

        private void GameHistory_Paint(object sender, PaintEventArgs e)
        {
        }

        private void GameHistoryWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}
