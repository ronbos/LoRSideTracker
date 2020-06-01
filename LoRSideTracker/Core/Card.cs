using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoRSideTracker
{
    /// <summary>
    /// Card class contains card info and art
    /// </summary>
    public class Card
    {
        /// <summary>Card Name</summary>
        public string Name { get; private set; }
        /// <summary>Card Code</summary>
        public string Code { get; private set; }
        /// <summary>Card Type</summary>
        public string Type { get; private set; }
        /// <summary>Card Super Type</summary>
        public string SuperType { get; private set; }
        /// <summary>Card Cost</summary>
        public int Cost { get; private set; }
        /// <summary>Card Region</summary>
        public string Region { get; private set; }
        /// <summary>Card Rarity</summary>
        public string Rarity { get; private set; }
        /// <summary>Card Flavor Text</summary>
        public string FlavorText { get; private set; }
        /// <summary>true if Card is collectible</summary>
        public bool IsCollectible { get; private set; }

        /// <summary>Default Card Attack Value</summary>
        public int Attack { get; private set; }
        /// <summary>Default Card Health Value</summary>
        public int Health { get; private set; }

        /// <summary>Spell Speed</summary>
        public string SpellSpeed { get; private set; }

        /// <summary>Associated Card Codes</summary>
        public string[] AssociatedCardCodes { get; private set; }

        /// <summary>Card Image</summary>
        public Image CardArt { get; private set; }
        /// <summary>Card Banner Image</summary>
        public Image CardBanner { get; private set; }

        /// <summary>Unknown Card Stub</summary>
        static public Card UnknownCard = new Card();

        /// <summary>
        /// Constructor for default unknown card
        /// </summary>
        private Card()
        {
            Name = "Unknown Card";
            Code = "UNKNOWN";
            Type = "Unit";
            SuperType = "";
            Cost = 0;
            Region = "";
            Rarity = "";
            FlavorText = "";
            IsCollectible = false;

            // Unit info
            Attack = 0;
            Health = 0;

            // Load images
            Bitmap bmp = new Bitmap(128, 128);
            using (Graphics g = Graphics.FromImage(bmp)) { g.Clear(Color.DarkGray); }
            CardArt = (Image) bmp;
            CardBanner = (Image)bmp;

            // Spell info
            SpellSpeed = "";

            // Spell info
            AssociatedCardCodes = new string[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="setPath">Set path</param>
        /// <param name="dict">Associated JSON</param>
        public Card(string setPath, Dictionary<string, JsonElement> dict)
        {
            Name = dict["name"].ToString();
            Code = dict["cardCode"].ToString();
            Type = dict["type"].ToString();
            SuperType = dict["supertype"].ToString();
            Cost = dict["cost"].ToObject<int>();
            Region = dict["regionRef"].ToString();
            Rarity = dict["rarity"].ToString();
            FlavorText = dict["flavorText"].ToString();
            IsCollectible = dict["collectible"].ToObject<bool>();

            // Unit info
            Attack = dict["attack"].ToObject<int>();
            Health = dict["health"].ToObject<int>();

            // Load images
            string cardArtPath = String.Format("{0}\\img\\cards\\{1}.png", setPath, Code);
            string cardBannerPath = String.Format("{0}\\img\\cards\\{1}-full.png", setPath, Code);
            CardArt = Image.FromFile(cardArtPath);
            CardBanner = Image.FromFile(cardBannerPath);

            // Spell info
            SpellSpeed = dict["spellSpeed"].ToString();

            // Spell info
            AssociatedCardCodes = dict["associatedCardRefs"].ToObject<string[]>();
        }

        /// <summary>
        /// Draw the card
        /// </summary>
        /// <param name="g"></param>
        /// <param name="dstRect"></param>
        public void DrawCardArt(Graphics g, Rectangle dstRect)
        {
            Image img;
            Rectangle srcRect;

            img = this.CardBanner;
            if (this.Type == "Spell")
            {
                double diagonal = Math.Sqrt(dstRect.Width * dstRect.Width + dstRect.Height * dstRect.Height);
                double scale = img.Width / diagonal;
                int newWidth = (int)(dstRect.Width * scale);
                int newHeight = (int)(dstRect.Height * scale);
                srcRect = new Rectangle((img.Width - newWidth) / 2, (img.Height - newHeight) / 2, newWidth, newHeight);
            }
            else
            {
                srcRect = new Rectangle(0, 0, img.Width, img.Height);
                // Crop vertically to preserve aspect ratio
                if (dstRect.Width * srcRect.Height > srcRect.Width * dstRect.Height)
                {
                    int newHeight = srcRect.Width * dstRect.Height / dstRect.Width;
                    int newCenter = img.Height * 4 / 10;
                    int newY = Math.Max(newCenter - newHeight / 2, 0);
                    srcRect.Y = newY;
                    srcRect.Height = newHeight;
                }
                else
                {
                    int newWidth = srcRect.Height * dstRect.Width / dstRect.Height;
                    srcRect.X += (srcRect.Width - newWidth) / 2;
                    srcRect.Width = newWidth;
                }
            }

            g.DrawImage(img, dstRect, srcRect, GraphicsUnit.Pixel);

            Color regionColor = Constants.GetRegionAccentColor(this.Region);
            //dstRect.Width /= 2;
            LinearGradientBrush linGrBrush = new LinearGradientBrush(
               new Point(dstRect.X, 10),
               new Point(dstRect.X + dstRect.Width / 2, 10),
               Color.FromArgb(160, regionColor),
               Color.FromArgb(0, regionColor));
            g.FillRectangle(linGrBrush, dstRect.X, dstRect.Y, dstRect.Width / 2, dstRect.Height);
        }
    }

    /// <summary>
    /// CardWithCount is used for tracking decks and other sets that may contain multiple of one card
    /// </summary>
    public class CardWithCount : ICloneable
    {
        /// <summary>Card Object</summary>
        public Card TheCard { get; private set; }
        /// <summary>Card Name</summary>
        public string Name { get { return TheCard.Name; } }
        /// <summary>Card Code</summary>
        public string Code { get { return TheCard.Code; } }
        /// <summary>Card Type</summary>
        public string Type { get { return TheCard.Type; } }
        /// <summary>Card Cost</summary>
        public int Cost { get { return TheCard.Cost; } }

        /// <summary>Number of this card in the set</summary>
        public int Count { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="card">Card object</param>
        /// <param name="count">Initial count</param>
        public CardWithCount(Card card, int count)
        {
            TheCard = card;
            Count = count;
        }

        /// <summary>
        /// Clone the object, but do not clone the card itself
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            // We don't want to clone the Card, just the count, 
            // since the card contains image art and can be large
            return new CardWithCount(TheCard, Count);
        }
    }
}
