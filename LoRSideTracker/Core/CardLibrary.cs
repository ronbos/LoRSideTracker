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
        private static Dictionary<string, Card> m_cards;

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
            for (int i = 1; i < 99; i++)
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
            string imagesDir = String.Format("{0}\\{1}\\img\\cards", setPath, Constants.Language);
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
            File.Delete(setZip);
        }

        /// <summary>
        /// Load all cards from disk
        /// </summary>
        /// <param name="pd">ProgressDisplay interface to use for progress updates</param>
        public static void LoadAllCards(ProgressDisplayControl pd)
        {
            m_cards = new Dictionary<string, Card>();
            for (int i = 1; ; i++)
            {
                string setName = Constants.GetSetName(i);
                string setDir = String.Format("{0}\\{1}\\{2}\\", Constants.GetLocalSetsPath(), setName, Constants.Language);
                string setJsonFile = String.Format("{0}\\data\\{1}.json", setDir, setName);
                if (!File.Exists(setJsonFile))
                {
                    // Done
                    return;
                }

                string json = Utilities.ReadLocalFile(setJsonFile);
                Dictionary<string, JsonElement>[] set = JsonSerializer.Deserialize<Dictionary<string, JsonElement>[]>(json);

                for (int j = 0; j < set.Length; j++)
                {
                    if (pd != null && 0 == (j & 31))
                    {
                        double percentage = 100.0 * (j + 1) / set.Length;
                        pd.Update(String.Format("Processing set {0}, card {1} of {2}.", i, j + 1, set.Length), percentage);
                    }
                    Dictionary<string, JsonElement> dict = set[j];
                    m_cards.Add(dict["cardCode"].ToString(), new Card(setDir, dict));
                }
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
                return m_cards[cardCode];
            }
            catch
            {
                return Card.UnknownCard;
            }
        }
    }
}
