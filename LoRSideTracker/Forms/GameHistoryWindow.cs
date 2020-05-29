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
            if (MyGameHistoryControl.InvokeRequired)
            {
                MyGameHistoryControl.Invoke(new Action(() => { MyGameHistoryControl.SetBounds(0, 0, MyGameHistoryControl.BestWidth, MyGameHistoryControl.BestHeight); }));
            }
            else
            {
                MyGameHistoryControl.SetBounds(0, 0, MyGameHistoryControl.BestWidth, MyGameHistoryControl.BestHeight);

            }
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

        /// <summary>
        /// Code from here: https://nickstips.wordpress.com/2010/03/03/c-panel-resets-scroll-position-after-focus-is-lost-and-regained/
        /// To prevent scrollbar from snapping back to position zero
        /// </summary>
        /// <param name="activeControl"></param>
        /// <returns></returns>
        protected override System.Drawing.Point ScrollToControl(System.Windows.Forms.Control activeControl)
        {
            // Returning the current location prevents the panel from
            // scrolling to the active control when the panel loses and regains focus
            return this.DisplayRectangle.Location;
        }
    }
}
