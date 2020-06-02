using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
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

        /// <summary>
        /// Find the list of missing sets that can be downloaded
        /// </summary>
        /// <returns>List of missing set indices</returns>
        public static List<ValueTuple<int, long>> FindMissingSets()
        {
            if (!Directory.Exists(Constants.GetLocalSetsPath()))
            {
                Directory.CreateDirectory(Constants.GetLocalSetsPath());
            }
            var missingSets = new List<ValueTuple<int, long>>();
            for (int i = 0; i < 99; i++)
            {
                long remoteZipSize = Utilities.CheckURLExists(Constants.GetSetURL(i));
                if (remoteZipSize <= 0)
                {
                    break;
                }
                if (!Directory.Exists(Constants.GetSetPath(i)))
                {
                    if (remoteZipSize > 0)
                    {
                        missingSets.Add((i, remoteZipSize));
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    var json = Utilities.ReadLocalFile(Constants.GetSetVersionInfoPath(i));
                    Dictionary<string, JsonElement> info = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                    long refRemoteZipSize = info["remoteZipSize"].ToObject<long>();
                    if (remoteZipSize != refRemoteZipSize)
                    {
                        missingSets.Add((i, remoteZipSize));
                    }
                }
            }
            return missingSets;
        }

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
        /// Unzip downloaded set, Resine image to manageable size, and delete the zip file
        /// </summary>
        /// <param name="setNumber">Set number</param>
        /// <param name="remoteZipSize">Zip file size as given by remote URL query</param>
        public static void ProcessDownloadedSet(int setNumber, long remoteZipSize)
        {
            string setPath = Constants.GetSetPath(setNumber);
            string setZip = Constants.GetSetZipPath(setNumber);
            if (!Directory.Exists(setPath))
            {
                ZipFile.ExtractToDirectory(setZip, setPath);
            }

            // We don't have a way of checking version. Check by download size instead
            var json = JsonSerializer.Serialize(new
            {
                setNumber = setNumber,
                remoteZipSize = remoteZipSize
            });
            File.WriteAllText(Constants.GetSetVersionInfoPath(setNumber), json);

            // Resize all images to manageable size
            if (setNumber > 0)
            {
                string imagesDir = String.Format("{0}\\img\\cards", setPath);
                DirectoryInfo dirInfo = new DirectoryInfo(imagesDir);
                FileInfo[] files = dirInfo.GetFiles();
                const int targetWidth = 256;
                foreach (FileInfo fi in files)
                {
                    Image fullSizeImage = Image.FromFile(fi.FullName);
                    double ratio = (double)targetWidth / (double)fullSizeImage.Width;

                    if (ratio < 1.0)
                    {
                        int targetHeight = (int)(0.5 + fullSizeImage.Height * ratio);
                        Image smallImage = Utilities.ResizeImage(fullSizeImage, targetWidth, targetHeight);
                        fullSizeImage.Dispose();
                        File.Delete(fi.FullName);
                        smallImage.Save(fi.FullName, System.Drawing.Imaging.ImageFormat.Png);
                        smallImage.Dispose();
                    }
                    else
                    {
                        fullSizeImage.Dispose();
                    }
                }
            }
            File.Delete(setZip);
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
