using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LoRSideTracker
{
    public partial class GameHistoryControl : UserControl
    {
        List<GameRecord> Games;

        private Font TitleFont = new Font("Calibri", 9, FontStyle.Bold);
        private Font ListFont = new Font("Calibri", 9, FontStyle.Regular);

        private int DateTimeCellWidth = 150;
        private int MyDeckCellWidth = 250;
        private int OpponentCellWidth = 250;
        private int ResultCellWidth = 120;
        private int CellMargin = 2;
        private int CellHeight = 20;

        public int BestWidth
        {
            get { return 5 * CellMargin + DateTimeCellWidth + MyDeckCellWidth + OpponentCellWidth + ResultCellWidth; }
        }
        public int BestHeight
        {
            get { return (CellHeight + CellMargin) * (Games == null ? 1 : (1 + Games.Count)); }
        }

        public GameHistoryControl()
        {
            InitializeComponent();
        }

        public void AddGameRecord(GameRecord gr)
        {
            Games.Add((GameRecord)gr.Clone());
            Invalidate();
        }

        private void GameHistoryControl_Load(object sender, EventArgs e)
        {
            // Load all games
            Games = new List<GameRecord>();
            if (Directory.Exists(Constants.GetLocalGamesPath()))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Constants.GetLocalGamesPath());
                FileInfo[] files = dirInfo.GetFiles();
                foreach (FileInfo fi in files)
                {
                    try
                    {
                        GameRecord record = GameRecord.LoadFromFile(fi.FullName);
                        Games.Add(record);
                    }
                    catch
                    {
                        // Skip bad records
                    }
                }
            }

        }

        private void GameHistoryControl_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.FillRectangle()
            int margin = 2;
            int height = 20;
            TextFormatFlags dateTimeFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
            TextFormatFlags myDeckFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
            TextFormatFlags opponentFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
            TextFormatFlags resultFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
            Rectangle dateTimeRect = new Rectangle(margin, margin, 150, height);
            Rectangle myDeckRect = new Rectangle(dateTimeRect.Right + margin, dateTimeRect.Top, 250, height);
            Rectangle opponentRect = new Rectangle(myDeckRect.Right + margin, myDeckRect.Top, 250, height);
            Rectangle resultRect = new Rectangle(opponentRect.Right + margin, opponentRect.Top, 120, height);

            TextRenderer.DrawText(e.Graphics, "Date/Time", TitleFont, dateTimeRect, Color.Black, dateTimeFormat);
            TextRenderer.DrawText(e.Graphics, "My Deck", TitleFont, myDeckRect, Color.Black, myDeckFormat);
            TextRenderer.DrawText(e.Graphics, "Opponent", TitleFont, opponentRect, Color.Black, opponentFormat);
            TextRenderer.DrawText(e.Graphics, "Result", TitleFont, resultRect, Color.Black, resultFormat);
            e.Graphics.DrawLine(new Pen(Color.Black, 1), new Point(dateTimeRect.Left, dateTimeRect.Bottom), new Point(resultRect.Right, resultRect.Bottom));

            foreach (var game in Games)
            {
                dateTimeRect.Offset(0, height + margin);
                myDeckRect.Offset(0, height + margin);
                opponentRect.Offset(0, height + margin);
                resultRect.Offset(0, height + margin);
                TextRenderer.DrawText(e.Graphics, game.Timestamp.ToLocalTime().ToString(), ListFont, dateTimeRect, Color.Black, dateTimeFormat);
                TextRenderer.DrawText(e.Graphics, game.MyDeckName, ListFont, myDeckRect, Color.Black, myDeckFormat);
                TextRenderer.DrawText(e.Graphics, game.OpponentName, ListFont, opponentRect, Color.Black, opponentFormat);
                TextRenderer.DrawText(e.Graphics, game.Result, ListFont, resultRect, Color.Black, resultFormat);
            }
        }
    }
}
