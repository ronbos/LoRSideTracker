using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoRSideTracker
{
    /// <summary>
    /// Set of constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Port to use with LoR app
        /// </summary>
        public const int Port = 21337;

        /// <summary>
        /// Base URL for querying LoR app
        /// </summary>
        public const string Host = "http://127.0.0.1";

        /// <summary>
        /// Language set to use
        /// </summary>
        public static string Language
        {
            get { return Properties.Settings.Default.Language; }
            set { Properties.Settings.Default.Language = value; Properties.Settings.Default.Save(); }
        }


        /// <summary>
        /// Produce URL for static deck
        /// </summary>
        /// <returns>URL for static deck</returns>
        public static string StaticDeckURL()
        {
            return String.Format(@"{0}:{1}/static-decklist", Constants.Host, Constants.Port);
        }

        /// <summary>
        /// Produce URL for expedition state
        /// </summary>
        /// <returns>URL for expedition state</returns>
        public static string ExpeditionStateURL()
        {
            return String.Format(@"{0}:{1}/expeditions-state", Constants.Host, Constants.Port);
        }

        /// <summary>
        /// Produce URL for overlay state
        /// </summary>
        /// <returns>URL for overlay state</returns>
        public static string OverlayStateURL()
        {
            return String.Format(@"{0}:{1}/positional-rectangles", Constants.Host, Constants.Port);
        }

        /// <summary>
        /// Produce URL for game result
        /// </summary>
        /// <returns>URL for game result</returns>
        public static string GameResultURL()
        {
            return String.Format(@"{0}:{1}/game-result", Constants.Host, Constants.Port);
        }

        /// <summary>
        /// Produce local sets path
        /// </summary>
        /// <returns>Local sets path</returns>
        public static string GetLocalSetsPath()
        {
            return Application.LocalUserAppDataPath + @"\Sets";
        }

        /// <summary>
        /// Produce localized set name
        /// </summary>
        /// <returns>Localized set name</returns>
        public static string GetSetName(int setNumber)
        {
            return String.Format("set{0}-{1}", setNumber, Language);
        }

        /// <summary>
        /// Produce local set path
        /// </summary>
        /// <returns>Local set path</returns>
        public static string GetSetPath(int setNumber)
        {
            return String.Format("{0}\\{1}", GetLocalSetsPath(), GetSetName(setNumber));
        }

        /// <summary>
        /// Produce local set zip path
        /// </summary>
        /// <returns>Local set zip path</returns>
        public static string GetSetZipPath(int setNumber)
        {
            return GetSetPath(setNumber) + @".zip";
        }

        /// <summary>
        /// Produce local set version info path
        /// </summary>
        /// <returns>Local set version info file path</returns>
        public static string GetSetVersionInfoPath(int setNumber)
        {
            return GetSetPath(setNumber) + @"\versionInfo.json";
        }

        /// <summary>
        /// Produce URL for specific set
        /// </summary>
        /// <returns>URL for specific set</returns>
        public static string GetSetURL(int setNumber)
        {
            return String.Format("https://dd.b.pvp.net/latest/{0}.zip", GetSetName(setNumber));
        }

        /// <summary>
        /// Produce local games path
        /// </summary>
        /// <returns>Local games path</returns>
        public static string GetLocalGamesPath()
        {
            return Application.LocalUserAppDataPath + @"\Games";
        }

        /// <summary>
        /// Accent color for LoR region
        /// </summary>
        /// <param name="region">region string</param>
        /// <returns>Accent color</returns>
        public static Color GetRegionAccentColor(string region)
        {
            if (region == "Bilgewater") { return Color.Chocolate; }
            if (region == "Demacia") { return Color.Goldenrod; }
            if (region == "Freljord") { return Color.DodgerBlue; }
            if (region == "Ionia") { return Color.HotPink; }
            if (region == "Noxus") { return Color.Crimson; }
            if (region == "PiltoverZaun") { return Color.Orange; }
            if (region == "ShadowIsles") { return Color.Teal; }
            return Color.Gray;
        }
    }
}
