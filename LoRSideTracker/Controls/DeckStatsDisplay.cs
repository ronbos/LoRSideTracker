using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LoRSideTracker
{
    /// <summary>
    /// Display deck stats
    /// </summary>
    public partial class DeckStatsDisplay : UserControl
    {
        /// <summary>Deck to illustrate</summary>
        public List<CardWithCount> TheDeck { get; set; }

        private readonly Font CostsFont = new Font("Calibri", 8, FontStyle.Bold);

        /// <summary>Text color in histogram</summary>
        public Color TextColor { get; set; } = Color.White;

        /// <summary>Histogram block height</summary>
        public int BlockHeight { get; set; } = 3;
        /// <summary>Histogram block width</summary>
        public int BlockWidth { get; set; } = 7;

        private readonly int ColumnMargin = 2;

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

            if (numCols < 1 || TheDeck == null || TheDeck.Count == 0)
            {
                return;
            }

            int maxCost = 0;
            int[] unitCounts = new int[numCols];
            int[] spellCounts = new int[numCols];
            int[] landmarkCounts = new int[numCols];
            foreach (var card in TheDeck)
            {
                int index = Math.Min(numCols - 1, card.Cost);
                maxCost = Math.Max(maxCost, index);

                if (card.Type == "Unit")
                {
                    unitCounts[index] += card.Count;
                }
                else if (card.Type == "Spell")
                {
                    spellCounts[index] += card.Count;
                }
                else
                {
                    landmarkCounts[index] += card.Count;
                }
            }

            int maxCount = 0;
            numCols = 1;
            for (int i = 0; i < unitCounts.Length; i++)
            {
                maxCount = Math.Max(maxCount, unitCounts[i] + spellCounts[i] + landmarkCounts[i]);
                if (unitCounts[i] + spellCounts[i] + landmarkCounts[i] > 0) numCols = i + 1;
            }

            if (maxCount == 0)
            {
                return;
            }
            numCols = Math.Max(numCols, 5);

            // Draw the histogram
            for (int i = 0; i < numCols; i++)
            {
                int left = boundsRect.X + boundsRect.Width * i / numCols;
                int nextLeft = boundsRect.X + boundsRect.Width * (i + 1) / numCols;
                int right = (i + 1 == numCols) ? nextLeft : nextLeft - ColumnMargin;

                DrawColumn(e.Graphics, new Rectangle(left, boundsRect.Y, right - left, boundsRect.Height),
                    unitCounts[i], landmarkCounts[i], spellCounts[i]);

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
        private void DrawColumn(Graphics g, Rectangle boundsRect, int unitCount, int landmarkCount, int spellCount)
        {
            int blockHeight = BlockHeight + 1;
            if (boundsRect.Width > 3 * BlockWidth)
            {
                boundsRect.X += (boundsRect.Width - 3 * BlockWidth) / 2;
                boundsRect.Width = 3 * BlockWidth;
            }
            for (int i = 0; i < unitCount + spellCount + landmarkCount; i++)
            {
                int bottom = boundsRect.Bottom - blockHeight * i;
                int nextBottom = boundsRect.Bottom - blockHeight * (i + 1);
                int top = nextBottom + 1;
                g.FillRectangle(new SolidBrush((i < unitCount) ? Constants.UnitAccentColor : (i < unitCount + landmarkCount) ? Constants.LandmarkAccentColor : Constants.SpellAccentColor),
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
            // How many columns can we fit? We cap it at 10 (to show costs 0 ... 9+)
            int numCols = (int)((width + ColumnMargin) / (BlockWidth + ColumnMargin));
            numCols = Math.Min(numCols, 10);

            // Can we fit anything?
            if (numCols < 1 || TheDeck == null || TheDeck.Count == 0)
            {
                return 0;
            }

            // Count how many cards for each cost column
            int[] counts = new int[numCols];
            foreach (var card in TheDeck)
            {
                int index = Math.Min(numCols - 1, card.Cost);
                counts[index] += card.Count;
            }

            // Height is calculated from tallest column that needs to be drawn
            int maxCount = counts.Max();
            maxCount = Math.Max(maxCount, 4);
            return maxCount * (BlockHeight + 1) + 2 * ColumnMargin - 1;
        }
    }

}
