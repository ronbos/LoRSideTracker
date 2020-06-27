using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoRSideTracker
{
    /// <summary>
    /// Card class contains card info and art
    /// </summary>
    public class Region
    {
        /// <summary>Region Name</summary>
        public string Name { get; private set; }
        /// <summary>Region abbreviation</summary>
        public string Abbreviation { get; private set; }

        /// <summary>Region Name Reference</summary>
        public string NameRef { get; private set; }

        /// <summary>Region banner Image</summary>
        public Image Banner { get; private set; }

        /// <summary>Unknown region placeholder</summary>
        static public Region UnknownRegion { get; private set; } = new Region();

        private Region()
        {
            Name = "Unknown Region";
            NameRef = "";
            Abbreviation = "XX";

            // Load image
            Bitmap bmp = new Bitmap(128, 128);
            using (Graphics g = Graphics.FromImage(bmp)) { g.Clear(Color.DarkGray); }
            Banner = (Image)bmp;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Region(string corePath, Dictionary<string, JsonElement> region)
        {
            Name = region["name"].ToString();
            Abbreviation = region["abbreviation"].ToString();
            NameRef = region["nameRef"].ToString();

            // Load images
            string bannerPath = String.Format("{0}\\img\\regions\\icon-{1}.png", corePath, NameRef);
            Banner = Image.FromFile(bannerPath);
        }
    }
}
