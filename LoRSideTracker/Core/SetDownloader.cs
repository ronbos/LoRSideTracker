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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoRSideTracker
{
    /// <summary>
    /// Interface to receive SetDownloader notifications
    /// </summary>
    interface ISetDownloaderCallback
    {
        void OnAllSetsDownloaded();

        void OnDownloadCanceled(bool errorOccured);
    }

    /// <summary>
    /// Set downloader class
    /// </summary>
    class SetDownloader
    {
        public static List<ValueTuple<int, long>> MissingSets { get; private set; }

        private IProgressDisplay MyProgressDisplay;
        private readonly ISetDownloaderCallback Callback;

        private int CurrentDownloadIndex = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callback"></param>
        public SetDownloader(ISetDownloaderCallback callback)
        {
            Callback = callback;
        }

        /// <summary>
        /// Find all missing sets and download them
        /// </summary>
        /// <param name="progressDisplay"></param>
        /// <returns></returns>
        public bool DownloadAllSets(IProgressDisplay progressDisplay)
        {
            MyProgressDisplay = progressDisplay;

            MissingSets = FindMissingSets();
            if (MissingSets.Count > 0)
            {
                long totalDownloadSize = MissingSets.Sum(x => x.Item2);
                var result = MessageBox.Show(
                    string.Format("Card sets have been updated. Download size is {0} MB. Download new sets?", totalDownloadSize / 1024 / 1024),
                    "Sets Out of Date",
                    MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes)
                {
                    return false;
                }

                // Delete existing outdated sets
                foreach (var set in MissingSets)
                {
                    if (Directory.Exists(Constants.GetSetPath(set.Item1)))
                    {
                        Directory.Delete(Constants.GetSetPath(set.Item1), true);
                    }
                }

                CurrentDownloadIndex = 0;
                CardLibrary.DownloadSet(MissingSets[CurrentDownloadIndex].Item1,
                    new DownloadProgressChangedEventHandler(OnDownloadProgressChanged),
                    new AsyncCompletedEventHandler(OnDownloadFileCompleted));
            }
            else
            {
                Callback.OnAllSetsDownloaded();
            }
            return true;
        }

        /// <summary>
        /// Find the list of missing sets that can be downloaded
        /// </summary>
        /// <returns>List of missing set indices</returns>
        private static List<ValueTuple<int, long>> FindMissingSets()
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
        /// Receives notification that set download progress changed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            string message = String.Format("Downloading card set {0} ({1}/{2})", MissingSets[CurrentDownloadIndex].Item1, CurrentDownloadIndex + 1, MissingSets.Count);
            MyProgressDisplay.Update(message, percentage);
        }

        /// <summary>
        /// Receives notification that set download was completed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e != null && e.Error != null && e.Error.HResult != 0)
            {
                string localFile = Constants.GetSetZipPath(MissingSets[CurrentDownloadIndex].Item1);
                if (File.Exists(localFile))
                {
                    File.Delete(localFile);
                }

                // Error occured, finish up
                Callback.OnDownloadCanceled(true);
            }
            else
            {
                // Success, finish up and queue up the next one

                MyProgressDisplay.Update("Download completed. Processing...", 100);
                ProcessDownloadedSet(MissingSets[CurrentDownloadIndex].Item1, MissingSets[CurrentDownloadIndex].Item2);
                CurrentDownloadIndex++;
                if (CurrentDownloadIndex < MissingSets.Count)
                {
                    CardLibrary.DownloadSet(MissingSets[CurrentDownloadIndex].Item1,
                        new DownloadProgressChangedEventHandler(OnDownloadProgressChanged),
                        new AsyncCompletedEventHandler(OnDownloadFileCompleted));
                }
                else
                {
                    // Finished
                    Callback.OnAllSetsDownloaded();
                }
            }
        }


        /// <summary>
        /// Unzip downloaded set, Resize image to manageable size, and delete the zip file
        /// </summary>
        /// <param name="setNumber">Set number</param>
        /// <param name="remoteZipSize">Zip file size as given by remote URL query</param>
        public static void ProcessDownloadedSet(int setNumber, long remoteZipSize)
        {
            string setPath = Constants.GetSetPath(setNumber);
            string setZip = Constants.GetSetZipPath(setNumber);
            if (!Directory.Exists(setPath))
            {
                // .. is needed because set path includes language subfolder
                ZipFile.ExtractToDirectory(setZip, setPath + @"\..");
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
                int targetWidth = CardLibrary.TargetImageSize;
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
    }
}
