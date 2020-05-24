using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;

namespace LoRSideTracker
{
    /// <summary>
    /// Display deck stats
    /// </summary>
    public partial class DeckStatsDisplay : UserControl
    {
        /// <summary>Deck to illustrate</summary>
        public List<CardWithCount> TheDeck { get; set; }

        private Font CostsFont = new Font("Calibri", 8, FontStyle.Bold);

        /// <summary>Unit color in histogram</summary>
        public Color UnitColor { get; set; } = Color.Blue;
        /// <summary>Spell color in histogram</summary>
        public Color SpellColor { get; set; } = Color.DarkCyan;
        /// <summary>Text color in histogram</summary>
        public Color TextColor { get; set; } = Color.White;

        /// <summary>Histogram block height</summary>
        public int BlockHeight { get; set; } = 4;
        /// <summary>Histogram block width</summary>
        public int BlockWidth { get; set; } = 7;

        private int ColumnMargin = 2;

        /// <summary>
        /// Constructor
        /// </summary>
        public DeckStatsDisplay()
        {
            InitializeComponent();
        }

        private void DeckStatsDisplay_Paint(object sender, PaintEventArgs e)
        {
            Rectangle boundsRect = this.ClientRectangle;
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), boundsRect);

            boundsRect.Inflate(-ColumnMargin, -ColumnMargin);
            int numCols = (int)((boundsRect.Width + ColumnMargin) / (BlockWidth + ColumnMargin));
            numCols = Math.Min(numCols, 10);

            if (numCols < 1 || TheDeck == null)
            {
                return;
            }

            int maxCost = 0;
            int[] unitCounts = new int[numCols];
            int[] spellCounts = new int[numCols];
            foreach (var card in TheDeck)
            {
                int index = Math.Min(numCols - 1, card.Cost);
                maxCost = Math.Max(maxCost, index);

                if (card.Type.Equals("Unit"))
                {
                    unitCounts[index] += card.Count;
                }
                else
                {
                    spellCounts[index] += card.Count;
                }
            }

            int maxCount = 0;
            numCols = 1;
            for (int i = 0; i < unitCounts.Length; i++)
            {
                maxCount = Math.Max(maxCount, unitCounts[i] + spellCounts[i]);
                if (unitCounts[i] + spellCounts[i] > 0) numCols = i + 1;
            }

            if (maxCount == 0 )
            {
                return;
            }
            numCols = Math.Max(numCols, 5);

            // Draw the histogram
            for (int i = 0; i < numCols; i++)
            {
                int left = boundsRect.X + boundsRect.Width * i / numCols;
                int nextLeft = boundsRect.X + boundsRect.Width * (i + 1) / numCols;
                int right = (i + 1 == numCols) ? nextLeft :  nextLeft - ColumnMargin;
                int bottom = boundsRect.Bottom;

                /*
                if (unitCounts[i] > 0)
                {
                    int top = boundsRect.Top + boundsRect.Height * (maxCount - unitCounts[i]) / maxCount;
                    e.Graphics.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(left, top, right - left, bottom - top));
                    bottom = top;
                }
                if (spellCounts[i] > 0)
                {
                    int top = boundsRect.Top + boundsRect.Height * (maxCount - unitCounts[i] - spellCounts[i]) / maxCount;
                    e.Graphics.FillRectangle(new SolidBrush(Color.Red), new Rectangle(left, top, right - left, bottom - top));
                }*/
                DrawColumn(e.Graphics, new Rectangle(left, boundsRect.Y, right - left, boundsRect.Height),
                    unitCounts[i], spellCounts[i], maxCount);

                String text = i.ToString();
                if (i + 1 == numCols && numCols < maxCost)
                {
                    text += "+";
                }

                TextRenderer.DrawText(e.Graphics, text, CostsFont, 
                    new Rectangle(left, boundsRect.Top, right - left, boundsRect.Height),
                    Color.White, TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter);
            }
        }
        private void DrawColumn(Graphics g, Rectangle boundsRect, int unitCount, int spellCount, int maxCount)
        {
            int blockHeight = BlockHeight + 1;
            if (boundsRect.Width > 3 * BlockWidth)
            {
                boundsRect.X += (boundsRect.Width - 3 * BlockWidth) / 2;
                boundsRect.Width = 3 * BlockWidth;
            }
            for (int i = 0; i < unitCount + spellCount; i++)
            {
                int bottom = boundsRect.Bottom - blockHeight * i;
                int nextBottom = boundsRect.Bottom - blockHeight * (i + 1);
                int top = nextBottom + 1;
                g.FillRectangle(new SolidBrush((i < unitCount) ? UnitColor : SpellColor), 
                    boundsRect.X,
                    top,
                    boundsRect.Width,
                    bottom - top);
            }
        }

        /// <summary>
        /// Return optimal height for given width
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public int GetBestHeight(int width)
        {
            int numCols = (int)((width + ColumnMargin) / (BlockWidth + ColumnMargin));
            numCols = Math.Min(numCols, 10);

            if (numCols < 1 || TheDeck == null)
            {
                return 0;
            }
            int[] counts = new int[numCols];
            foreach (var card in TheDeck)
            {
                int index = Math.Min(numCols - 1, card.Cost);
                counts[index] += card.Count;
            }

            int maxCount = 0;
            for (int i = 0; i < counts.Length; i++)
            {
                maxCount = Math.Max(maxCount, counts[i]);
            }

            if (maxCount == 0)
            {
                return 0;
            }
            maxCount = Math.Max(maxCount, 4);
            return maxCount * (BlockHeight + 1) + 2 * ColumnMargin - 1;
        }
    }

}
