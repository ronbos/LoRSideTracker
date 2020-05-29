﻿using System;
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
        class GameHistoryColumn
        {
            public string Title;
            public int Width;
            public string PropertyName;
            public Color TextColor;
            public TextFormatFlags TextFormat;
            public bool AcceptMouseOverEvents;

            public GameHistoryColumn(string title, int width, string propertyName, Color textColor, TextFormatFlags textFormat, bool acceptMouseOverEvents = false)
            {
                Title = title;
                Width = width;
                PropertyName = propertyName;
                TextColor = textColor;
                TextFormat = textFormat;
                AcceptMouseOverEvents = acceptMouseOverEvents;
            }
        }

        List<GameRecord> Games;
        private List<Rectangle[]> GameTextRectangles;

        private Font TitleFont = new Font("Calibri", 9, FontStyle.Bold);
        private Font ListFont = new Font("Calibri", 9, FontStyle.Regular);

        private GameHistoryColumn[] Columns = new GameHistoryColumn[] {
            new GameHistoryColumn("Date and Time", 150, "Timestamp", Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter),
            new GameHistoryColumn("My Deck", 200, "MyDeckName", Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.Left, true),
            new GameHistoryColumn("Opponent", 200, "OpponentName", Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.Left, true),
            new GameHistoryColumn("Result", 120, "Result", Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter),
        };

        private int CellMargin = 2;
        private int CellHeight = 20;

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
            get { return (CellHeight + CellMargin) * (Games == null ? 1 : (1 + Games.Count)); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public GameHistoryControl()
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

            InitializeComponent();

            // Load all games
            Games = new List<GameRecord>();
            if (Directory.Exists(Constants.GetLocalGamesPath()))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Constants.GetLocalGamesPath());
                FileInfo[] files = dirInfo.GetFiles();

                foreach (FileInfo fi in files.OrderBy(x => x.CreationTime))
                {
                    try
                    {
                        AddGameRecord(GameRecord.LoadFromFile(fi.FullName), false);
                    }
                    catch
                    {
                        // Skip bad records
                    }
                }
            }

        }

        /// <summary>
        /// Adda game to history
        /// </summary>
        /// <param name="gr">Game record to add</param>
        /// <param name="shouldInvalidate">Should window be redrawn</param>
        public void AddGameRecord(GameRecord gr, bool shouldInvalidate = true)
        {
            Games.Add((GameRecord)gr.Clone());
            Rectangle[] titleTextRects = new Rectangle[Columns.Length];
            int left = CellMargin;
            for (int i = 0; i < Columns.Length; i++)
            {
                GameHistoryColumn col = Columns[i];
                string text = gr.ReadPropertyAsString(col.PropertyName);
                titleTextRects[i] = new Rectangle(new Point(left, CellMargin + (CellHeight + CellMargin) * Games.Count), TextRenderer.MeasureText(text, ListFont, CellRectangles[i].Size));
                left += col.Width + CellMargin;
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
            if (shouldInvalidate) Invalidate();
        }

        /// <summary>
        /// Load all existing game records
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameHistoryControl_Load(object sender, EventArgs e)
        {
            PopupDeckWindow = new DeckWindow();
            PopupDeckWindow.ShouldHideOnMouseLeave = true;
            PopupDeckWindow.StartPosition = FormStartPosition.Manual;

        }

        private void GameHistoryControl_Paint(object sender, PaintEventArgs e)
        {
            int top = CellMargin;
            Rectangle currentRect = new Rectangle(0, top, 0, top + CellHeight);
            foreach (var col in Columns)
            {
                currentRect.X = currentRect.Right + CellMargin;
                currentRect.Width = col.Width;
                TextRenderer.DrawText(e.Graphics, col.Title, TitleFont, currentRect, Color.Black, col.TextFormat);
            }
            e.Graphics.DrawLine(new Pen(Color.Black, 1), new Point(CellMargin, currentRect.Bottom), new Point(currentRect.Right, currentRect.Bottom));

            currentRect = new Rectangle(0, top, 0, top + CellHeight);
            for (int i = 0; i < Games.Count; i++)
            {
                GameRecord game = Games[i];
                currentRect.X = 0;
                currentRect.Width = 0;
                currentRect.Y += CellHeight + CellMargin;
                for (int j = 0; j < Columns.Length; j++)
                {
                    var col = Columns[j];
                    currentRect.X = currentRect.Right + CellMargin;
                    currentRect.Width = col.Width;
                    string text = game.ReadPropertyAsString(col.PropertyName);

                    TextRenderer.DrawText(e.Graphics, text, 
                        ListFont, 
                        currentRect,
                        (i == HighlightedCell.Y && j == HighlightedCell.X) ? Color.Blue : col.TextColor, 
                        col.TextFormat);
                }
            }
        }

        private Rectangle GetCellRectangle(int row, int column)
        {
            Rectangle rect = new Rectangle(CellRectangles[row].Location, CellRectangles[row].Size);
            rect.Offset(0, (column + 1) * (CellHeight + CellMargin));
            return rect;
        }

        private void GameHistoryControl_MouseMove(object sender, MouseEventArgs e)
        {
            // Find the column
            int column = -1, row = -1;
            for (int i = 0; i < CellRectangles.Length; i++)
            {
                if (e.X >= CellRectangles[i].Left && e.X <= CellRectangles[i].Right)
                {
                    column = i;
                    break;
                }
            }
            if (column >= 0 && !Columns[column].AcceptMouseOverEvents)
            {
                column = -1;
            }

            if (column != -1)
            {
                // Find the row
                row = (e.Y - CellMargin) / (CellHeight + CellMargin);
                if (row > Games.Count || e.Y - (CellMargin + row * (CellHeight + CellMargin)) >= CellHeight)
                {
                    // In the margin
                    row = -1;
                }
                else
                {
                    row--;
                }
                if (row == -1) column = -1;

                if (row != -1 && !GameTextRectangles[row + 1][column].Contains(e.X, e.Y))
                {
                    row = -1;
                    column = -1;
                }
            }

            HighlightCell(row, column);
        }

        private void HighlightCell(int row, int column)
        {
            if (HighlightedCell.X != column || HighlightedCell.Y != row)
            {
                if (PopupDeckWindow.Visible && !PopupDeckWindow.DesktopBounds.Contains(PointToScreen(MousePosition)))
                {
                    PopupDeckWindow.Hide();
                }
                if (HighlightedCell.X != -1)
                {
                    Invalidate(GetCellRectangle(HighlightedCell.X, HighlightedCell.Y));
                }
                HighlightedCell.X = column;
                HighlightedCell.Y = row;
                if (HighlightedCell.X != -1)
                {
                    Rectangle cellRectangle = GameTextRectangles[HighlightedCell.Y + 1][HighlightedCell.X];// GetCellRectangle(HighlightedCell.X, HighlightedCell.Y);
                    Invalidate(cellRectangle);

                    if (Columns[column].Title == "My Deck" || Columns[column].Title == "Opponent")
                    {
                        if (Columns[column].Title == "Opponent")
                        {
                            PopupDeckWindow.SetFullDeck(Games[row].OpponentDeck);
                        }
                        else
                        {
                            PopupDeckWindow.SetFullDeck(Games[row].MyDeck);
                        }
                        Point pos = PointToScreen(new Point(cellRectangle.Right, cellRectangle.Top));
                        PopupDeckWindow.SetBounds(pos.X, pos.Y, 0, 0);
                        PopupDeckWindow.Show();
                        PopupDeckWindow.UpdateSize();
                    }
                }
            }
        }

        private void GameHistoryControl_MouseLeave(object sender, EventArgs e)
        {
            //MouseEventArgs me = (MouseEventArgs) e;
            if (!PopupDeckWindow.Visible || !PopupDeckWindow.DesktopBounds.Contains(MousePosition))
            {
                PopupDeckWindow.Hide();
            }
        }
    }
}
