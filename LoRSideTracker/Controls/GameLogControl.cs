using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoRSideTracker
{
    /// <summary>
    /// Game log control
    /// </summary>
    public partial class GameLogControl : UserControl
    {
        class GameHistoryColumn
        {
            public string Title;
            public int Width;
            public string PropertyName;
            public Color TextColor;
            public TextFormatFlags TextFormat;

            public GameHistoryColumn(string title, int width, string propertyName, Color textColor, TextFormatFlags textFormat)
            {
                Title = title;
                Width = width;
                PropertyName = propertyName;
                TextColor = textColor;
                TextFormat = textFormat;
            }
        }

        List<GameRecord> Games;
        private List<Rectangle[]> GameTextRectangles;

        private readonly Font TitleFont = new Font("Calibri", 9, FontStyle.Bold);
        private readonly Font ListFont = new Font("Calibri", 9, FontStyle.Regular);

        private readonly GameHistoryColumn[] Columns = new GameHistoryColumn[] {
            new GameHistoryColumn("Game End Time", 150, "Timestamp", Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.Left),
            new GameHistoryColumn("My Deck", 150, "MyDeckName", Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.Left),
            new GameHistoryColumn("Opponent", 150, "OpponentName", Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.Left),
            new GameHistoryColumn("Result", 80, "Result", Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter),
        };

        private readonly int CellMargin = 2;
        private readonly int CellHeight = 20;

        private Point HighlightedCell = new Point(-1, -1);

        private Rectangle[] CellRectangles;

        private DeckWindow PopupDeckWindow;

        /// <summary>
        /// Report best width for the control
        /// </summary>
        public int BestWidth
        {
            get { return CellMargin + Columns.Sum(x => x.Width + CellMargin); }
        }
        /// <summary>
        /// Report best height for the control
        /// </summary>
        public int BestHeight
        {
            get { return 2 * CellMargin + (CellHeight + CellMargin) * (Games == null ? 1 : (1 + Games.Count)); }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public GameLogControl()
        {
            GameTextRectangles = new List<Rectangle[]>();
            CellRectangles = new Rectangle[Columns.Length];
            Rectangle[] titleTextRects = new Rectangle[Columns.Length];
            int left = CellMargin;
            for (int i = 0; i < Columns.Length; i++)
            {
                GameHistoryColumn col = Columns[i];
                int right = left + col.Width;
                CellRectangles[i] = new Rectangle(left, CellMargin, right - left, CellHeight);
                left = right + CellMargin;

                titleTextRects[i] = new Rectangle(new Point(left, CellMargin), TextRenderer.MeasureText(col.Title, TitleFont, CellRectangles[i].Size));
                if (col.TextFormat.HasFlag(TextFormatFlags.VerticalCenter))
                {
                    titleTextRects[i].Offset(0, (CellHeight - titleTextRects[i].Height) / 2);
                }
                if (col.TextFormat.HasFlag(TextFormatFlags.HorizontalCenter))
                {
                    titleTextRects[i].Offset((CellRectangles[i].Width - titleTextRects[i].Width) / 2, 0);
                }
            }
            GameTextRectangles.Add(titleTextRects);
            Games = new List<GameRecord>();

            InitializeComponent();
        }

        private void GameLogControl_Load(object sender, EventArgs e)
        {
            PopupDeckWindow = new DeckWindow();
            PopupDeckWindow.ShouldHideOnMouseLeave = true;
            PopupDeckWindow.StartPosition = FormStartPosition.Manual;
            GameLogDisplay.Width = BestWidth;
            GameLogDisplay.Height = BestHeight;
        }

        /// <summary>
        /// Load games matching deck signature
        /// </summary>
        /// <param name="deckSignature"></param>
        public void LoadGames(string deckSignature)
        {
            Utilities.CallActionSafelyAndWait(this, new Action(() =>
            {
                // Load all games
                Games = GameHistory.Games.FindAll(x => deckSignature == x.GetDeckSignature()).ToList();
                GameTextRectangles.RemoveRange(1, GameTextRectangles.Count - 1);
                foreach (var gr in Games)
                {
                    AddGameRecord(gr, false);
                }
                Invalidate();
            }));
        }

        /// <summary>
        /// Clear the contents
        /// </summary>
        public void Clear()
        {
            Utilities.CallActionSafelyAndWait(this, new Action(() =>
            {
                // Load all games
                Games = new List<GameRecord>();
                GameTextRectangles.RemoveRange(1, GameTextRectangles.Count - 1);
                Invalidate();
            }));
        }


        /// <summary>
        /// Adda game to history
        /// </summary>
        /// <param name="gr">Game record to add</param>
        /// <param name="shouldInvalidate">Should window be redrawn</param>
        public void AddGameRecord(GameRecord gr, bool shouldInvalidate = true)
        {
            GameTextRectangles.Insert(1, new Rectangle[Columns.Length]);
            // Move all existing rectangles down
            for (int i = 2; i < GameTextRectangles.Count; i++)
            {
                for (int j = 0; j < GameTextRectangles[i].Length; j++)
                {
                    GameTextRectangles[i][j].Offset(0, CellHeight + CellMargin);
                }
            }

            int left = CellMargin;
            for (int i = 0; i < Columns.Length; i++)
            {
                GameHistoryColumn col = Columns[i];
                string text = gr.ReadPropertyAsString(col.PropertyName);
                GameTextRectangles[1][i] = new Rectangle(new Point(left, CellMargin + CellHeight + CellMargin), TextRenderer.MeasureText(text, ListFont, CellRectangles[i].Size));
                left += col.Width + CellMargin;
                if (col.TextFormat.HasFlag(TextFormatFlags.VerticalCenter))
                {
                    GameTextRectangles[1][i].Offset(0, (CellHeight - GameTextRectangles[1][i].Height) / 2);
                }
                if (col.TextFormat.HasFlag(TextFormatFlags.HorizontalCenter))
                {
                    GameTextRectangles[1][i].Offset((CellRectangles[i].Width - GameTextRectangles[1][i].Width) / 2, 0);
                }
            }
            GameLogDisplay.Width = BestWidth;
            GameLogDisplay.Height = BestHeight;

            if (shouldInvalidate) Invalidate();
        }

        /// <summary>
        /// Make sure child controls get invalidated
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
            GameLogDisplay.Invalidate(e.InvalidRect);
        }

        private void GameLogDisplay_Paint(object sender, PaintEventArgs e)
        {
            Color highlightBackColor = Color.FromArgb(
                (ForeColor.R + 3 * BackColor.R) / 4,
                (ForeColor.G + 3 * BackColor.G) / 4,
                (ForeColor.B + 3 * BackColor.B) / 4);

            int top = CellMargin;
            Rectangle currentRect = new Rectangle(0, top, 0, top + CellHeight);
            foreach (var col in Columns)
            {
                currentRect.X = currentRect.Right + CellMargin;
                currentRect.Width = col.Width;
                TextRenderer.DrawText(e.Graphics, col.Title, TitleFont, currentRect, ForeColor, col.TextFormat);
            }
            e.Graphics.DrawLine(new Pen(ForeColor, 1), new Point(CellMargin, currentRect.Bottom), new Point(currentRect.Right, currentRect.Bottom));

            currentRect = new Rectangle(0, top, 0, top + CellHeight);
            Color lineColor = Color.FromArgb(
                (ForeColor.R + 3 * BackColor.R) / 4,
                (ForeColor.G + 3 * BackColor.G) / 4,
                (ForeColor.B + 3 * BackColor.B) / 4);
            for (int i = 0; i < Games.Count; i++)
            {
                GameRecord game = Games[i];
                currentRect.X = 0;
                currentRect.Width = 0;
                currentRect.Y += CellHeight + CellMargin;
                if (i == HighlightedCell.Y)
                {
                    e.Graphics.FillRectangle(new SolidBrush(highlightBackColor), GetCellRectangle(-1, HighlightedCell.Y));
                }
                for (int j = 0; j < Columns.Length; j++)
                {
                    var col = Columns[j];
                    currentRect.X = currentRect.Right + CellMargin;
                    currentRect.Width = col.Width;
                    string text = game.ReadPropertyAsString(col.PropertyName);

                    TextRenderer.DrawText(e.Graphics, text,
                        ListFont,
                        currentRect,
                        ForeColor,
                        col.TextFormat);
                }
                e.Graphics.DrawLine(new Pen(lineColor, 1), new Point(CellMargin, currentRect.Bottom), new Point(currentRect.Right, currentRect.Bottom));
            }
        }

        private Rectangle GetCellRectangle(int x, int y)
        {
            Rectangle rect;
            if (x >= 0)
            {
                rect = new Rectangle(CellRectangles[x].Location, CellRectangles[x].Size);
            }
            else
            {
                rect = new Rectangle(CellMargin, 2 * CellMargin, BestWidth, CellHeight - 1);
            }
            rect.Offset(0, (y + 1) * (CellHeight + CellMargin));
            return rect;
        }
        private Point FindCell(int mouseX, int mouseY)
        {
            // Find the column
            int column = -1, row = -1;
            for (int i = 0; i < CellRectangles.Length; i++)
            {
                if (mouseX >= CellRectangles[i].Left && mouseX <= CellRectangles[i].Right)
                {
                    column = i;
                    break;
                }
            }

            // Find the row
            row = (mouseY - CellMargin) / (CellHeight + CellMargin);
            if (row > Games.Count || mouseY - (CellMargin + row * (CellHeight + CellMargin)) >= CellHeight)
            {
                // In the margin
                row = -1;
            }
            else
            {
                row--;
            }
            if (row == -1) column = -1;

            if (row != -1 && column != -1 && !GameTextRectangles[row + 1][column].Contains(mouseX, mouseY))
            {
                column = -1;
            }

            return new Point(column, row);
        }

        private void GameLogDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            Point cellIndex = FindCell(e.X, e.Y);
            HighlightCell(cellIndex);
        }

        private void HighlightCell(Point cellIndex)
        {
            if (HighlightedCell != cellIndex)
            {
                if (HighlightedCell.Y != cellIndex.Y)
                {
                    // Invalidate old row
                    if (HighlightedCell.Y >= 0)
                    {
                        Invalidate(GetCellRectangle(-1, HighlightedCell.Y));
                    }
                    // Invalidate new row
                    if (cellIndex.Y >= 0)
                    {
                        Invalidate(GetCellRectangle(-1, cellIndex.Y));
                    }
                }
                if (PopupDeckWindow.Visible && !PopupDeckWindow.DesktopBounds.Contains(PointToScreen(MousePosition)))
                {
                    PopupDeckWindow.Hide();
                }
                HighlightedCell = cellIndex;
                if (HighlightedCell.X != -1)
                {
                    Rectangle cellRectangle = GameTextRectangles[HighlightedCell.Y + 1][HighlightedCell.X];

                    if (Columns[HighlightedCell.X].Title == "My Deck" || Columns[HighlightedCell.X].Title == "Opponent")
                    {
                        List<CardWithCount> deck = (Columns[HighlightedCell.X].Title == "Opponent") ? Games[HighlightedCell.Y].OpponentDeck : Games[HighlightedCell.Y].MyDeck;

                        if (deck.Count > 0)
                        {
                            PopupDeckWindow.SetFullDeck(deck);
                            Point pos = PointToScreen(new Point(cellRectangle.Right, cellRectangle.Top));
                            PopupDeckWindow.SetBounds(pos.X, pos.Y, 0, 0);
                            Utilities.ShowInactiveTopmost(PopupDeckWindow);
                            PopupDeckWindow.UpdateSize();
                        }
                    }
                }
            }
        }

        private void GameLogDisplay_MouseLeave(object sender, EventArgs e)
        {
            if (!PopupDeckWindow.Visible || !PopupDeckWindow.DesktopBounds.Contains(MousePosition))
            {
                PopupDeckWindow.Hide();
            }
        }

        private void GameLogDisplay_Click(object sender, EventArgs _e)
        {
            MouseEventArgs e = (MouseEventArgs)_e;
            Point cellIndex = FindCell(e.X, e.Y);
            
            if (cellIndex.Y >= 0)
            {
                LogWindow logWindow = new LogWindow();
                Utilities.CallActionSafelyAndWait(logWindow, new Action(() =>
                {
                logWindow.CreateControl();
                }));
                logWindow.SetRtf(Games[cellIndex.Y].Log);
                logWindow.ShowDialog();
                HighlightCell(new Point(-1, -1));
            }

        }
    }
}
