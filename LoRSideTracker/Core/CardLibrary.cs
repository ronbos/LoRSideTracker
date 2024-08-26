using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.Json;

namespace LoRSideTracker
{
    /// <summary>
    /// CardLibrary is used to download and load all available cards
    /// </summary>
    public static class CardLibrary
    {
        private static Dictionary<string, Card> Cards;
        private static Dictionary<string, Region> Regions;

        /// <summary>Desired image width for card sets</summary>
        public static readonly int TargetImageSize = 256;

        /// <summary>
        /// Gegin asynchronous download of a set zip file
        /// </summary>
        /// <param name="setNumber">Set number</param>
        /// <param name="onDownloadProgressChangedHandler">Interface to receive OnDownloadProgressChanged() updates</param>
        /// <param name="onDownloadFileCompletedHandler">Interface to receive OnDownloadFileCompleted() callback</param>
        public static void DownloadSet(int setNumber,
                DownloadProgressChangedEventHandler onDownloadProgressChangedHandler,
                AsyncCompletedEventHandler onDownloadFileCompletedHandler)
        {
            WebClient client = new WebClient();
            client.DownloadProgressChanged += onDownloadProgressChangedHandler;
            client.DownloadFileCompleted += onDownloadFileCompletedHandler;
            client.DownloadFileAsync(new Uri(Constants.GetSetURL(setNumber)), Constants.GetSetZipPath(setNumber));
        }

        /// <summary>
        /// Load all cards from disk
        /// </summary>
        /// <param name="pd">ProgressDisplay interface to use for progress updates</param>
        public static void LoadAllCards(ProgressDisplayControl pd)
        {
            Cards = new Dictionary<string, Card>();
            for (int i = 1; ; i++)
            {
                string setDir = Constants.GetSetPath(i);
                string setJsonFile = String.Format("{0}\\data\\{1}.json", setDir, Constants.GetSetName(i));
                if (!File.Exists(setJsonFile))
                {
                    // Done
                    break;
                }

                string coreJson = Utilities.ReadLocalFile(setJsonFile);
                Dictionary<string, JsonElement>[] set = JsonSerializer.Deserialize<Dictionary<string, JsonElement>[]>(coreJson);

                for (int j = 0; j < set.Length; j++)
                {
                    if (pd != null && 0 == (j & 31))
                    {
                        double percentage = 100.0 * (j + 1) / set.Length;
                        pd.Update(String.Format("Processing set {0}, card {1} of {2}.", i, j + 1, set.Length), percentage);
                    }
                    Dictionary<string, JsonElement> dict = set[j];
                    Cards.Add(dict["cardCode"].ToString(), new Card(setDir, dict));
                }
            }

            // Load all regions
            string coreDir = Constants.GetSetPath(0);
            string coreJsonFile = String.Format("{0}\\data\\globals-en_us.json", coreDir);
            string json = Utilities.ReadLocalFile(coreJsonFile);
            Dictionary<string, JsonElement> core = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            Dictionary<string, JsonElement>[] regions = core["regions"].ToObject<Dictionary<string, JsonElement>[]>();
            Regions = new Dictionary<string, Region>();
            foreach (var region in regions)
            {
                Region r = new Region(coreDir, region);
                Regions[r.NameRef] = r;
            }

        }

        /// <summary>
        /// Get the card corresponding to the card code
        /// </summary>
        /// <param name="cardCode">Card code</param>
        /// <returns>Corresponding Card object</returns>
        public static Card GetCard(string cardCode)
        {
            try
            {
                return Cards[cardCode];
            }
            catch
            {
                return Card.UnknownCard;
            }
        }

        /// <summary>
        /// Get the card matching the card name
        /// </summary>
        /// <param name="cardName">Card name</param>
        /// <returns>Corresponding Card object</returns>
        public static Card GetCardByName(string cardName)
        {
            foreach (var c in Cards)
            {
                if (c.Value.Name.Equals(cardName))
                {
                    return c.Value;
                }
            }
            return Card.UnknownCard;
        }

        /// <summary>
        /// Get the region corresponding to the region name
        /// </summary>
        /// <param name="regionNameRef">Region name</param>
        /// <returns>Corresponding Region object</returns>
        public static Region GetRegion(string regionNameRef)
        {
            try
            {
                return Regions[regionNameRef];
            }
            catch
            {
                return Region.UnknownRegion;
            }
        }
    }
}
