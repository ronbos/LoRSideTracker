﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoRSideTracker
{
    public static class CardLibrary
    {
        private static Dictionary<string, Card> m_cards;

        public static List<int> FindMissingSets()
        {
            if (!Directory.Exists(Constants.GetLocalSetsPath()))
            {
                Directory.CreateDirectory(Constants.GetLocalSetsPath());
            }
            List<int> missingSets = new List<int>();
            for (int i = 1; i < 99; i++)
            {
                if (!Directory.Exists(Constants.GetSetPath(i)))
                {
                    if (Utilities.CheckURLExists(Constants.GetSetURL(i)))
                    {
                        missingSets.Add(i);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return missingSets;
        }


        public static bool DownloadSet(int setNumber,
                DownloadProgressChangedEventHandler onDownloadProgressChangedHandler,
                AsyncCompletedEventHandler onDownloadFileCompletedHandler)
        {
            string setZip = Constants.GetSetZip(setNumber);
            if (!File.Exists(setZip))
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += onDownloadProgressChangedHandler;
                client.DownloadFileCompleted += onDownloadFileCompletedHandler;
                client.DownloadFileAsync(new Uri(Constants.GetSetURL(setNumber)), setZip);
                return true;
            }
            return false;
        }

        public static void ProcessDownloadedSet(int setNumber)
        {
            string setPath = Constants.GetSetPath(setNumber);
            string setZip = Constants.GetSetZip(setNumber);
            if (!Directory.Exists(setPath))
            {
                ZipFile.ExtractToDirectory(setZip, setPath);
            }

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
        public static void LoadAllCards(ProgressDisplay pd)
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

        public static Card GetCard(string cardCode)
        {
            try
            {
                return (Card)m_cards[cardCode];
            }
            catch (NullReferenceException)
            {
                return null;
            }

        }
    }
}
