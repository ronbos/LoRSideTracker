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
        /// Setting playback deck path replaces real-time LoR watcher with playback from a file
        /// </summary>
        public const string PlayBackDeckPath = null;
        //public const string PlayBackDeckPath = @"2021_3_16_16_43_51.playback";

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
        /// <returns>URL for expedition state. This is deprecated as expeditions are mo longer a part of the game</returns>
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
        /// <param name="setNumber">Set number (0 for core set)</param>
        /// <returns>Localized set name, or core set name if zero</returns>
        public static string GetSetName(int setNumber)
        {
            // Riot stop naming the sets nicely with set 6 :(
            // Latest set URLs can be seen at https://developer.riotgames.com/docs/lor#data-dragon_set-bundles
            string[] KnownSets =
            {
                "core",
                "set1",
                "set2",
                "set3",
                "set4",
                "set5",
                "set6",
                "set6cde",
                "set7",
                "set7b",
                "set8",
                "set9",
                // Confirmed up to here. Guesses below:
                "set10",
                "set11",
                "set12",
                "set13"
                ///...
            };

            return (setNumber < KnownSets.Length) ? String.Format("{0}-{1}", KnownSets[setNumber], Language)
                : "";
        }

        /// <summary>
        /// Produce local set path
        /// </summary>
        /// <returns>Local set path</returns>
        public static string GetSetPath(int setNumber)
        {
            return String.Format("{0}\\{1}\\{2}", GetLocalSetsPath(), GetSetName(setNumber), Language);
        }

        /// <summary>
        /// Produce local set zip path
        /// </summary>
        /// <returns>Local set zip path</returns>
        public static string GetSetZipPath(int setNumber)
        {
            return String.Format("{0}\\{1}.zip", GetLocalSetsPath(), GetSetName(setNumber));
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
        /// Produce local deck names file path
        /// </summary>
        /// <returns>Deck names file path</returns>
        public static string GetLocalDeckNamesFilePath()
        {
            return Application.LocalUserAppDataPath + @"\DeckNames.json";
        }

        /// <summary>
        /// Accent color for LoR region
        /// </summary>
        /// <param name="region">region string</param>
        /// <returns>Accent color</returns>
        public static Color GetRegionAccentColor(string region)
        {
            if (region == "Bilgewater") { return Color.SaddleBrown; }
            if (region == "Demacia") { return Color.LightYellow; }
            if (region == "Freljord") { return Color.DodgerBlue; }
            if (region == "Ionia") { return Color.HotPink; }
            if (region == "Noxus") { return Color.Crimson; }
            if (region == "PiltoverZaun") { return Color.Orange; }
            if (region == "ShadowIsles") { return Color.Teal; }
            if (region == "Targon") { return Color.Purple; }
            if (region == "Shurima") { return Color.Gold; }
            if (region == "BandleCity") { return Color.YellowGreen; }
            return Color.Gray;
        }

        /// <summary>Unit color in histogram</summary>
        public static Color UnitAccentColor = Color.RoyalBlue;
        /// <summary>Spell color in histogram</summary>
        public static Color SpellAccentColor = Color.MediumSeaGreen;
        /// <summary>Landmark color in histogram</summary>
        public static Color LandmarkAccentColor = Color.Goldenrod;
    }
}
