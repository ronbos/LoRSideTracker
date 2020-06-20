using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace LoRSideTracker
{
    /// <summary>
    /// Extensions to JsonElement and JsonDocument
    /// </summary>
    public static partial class JsonExtensions
    {
        /// <summary>
        /// Convert JsonElement contents to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">Element to convert</param>
        /// <returns>Converted element</returns>
        public static T ToObject<T>(this JsonElement element)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Convert JsonDocument contents to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document">Document to convert</param>
        /// <returns>Converted Document</returns>
        public static T ToObject<T>(this JsonDocument document)
        {
            var json = document.RootElement.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }
    }

    /// <summary>
    /// Extension for RichTextBox
    /// </summary>
    public static class RichTextBoxExtensions
    {
        /// <summary>
        /// Allow RichTextBox to append text with specified color and bold flag
        /// </summary>
        /// <param name="box">RichTextBox object</param>
        /// <param name="text">Text to append</param>
        /// <param name="color">Color to use</param>
        /// <param name="isBold">Make text bold if true</param>
        public static void AppendText(this RichTextBox box, string text, Color color, bool isBold = false)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            if (isBold)
            {
                box.SelectionFont = new Font(box.Font, FontStyle.Bold);
            }
            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
            box.SelectionFont = box.Font;
        }
    }

    /// <summary>
    /// Duplicate key enumerator, allows duplicates in SortedList
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
    {
        /// <summary>
        /// Compare function, forces non-equality
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            if (result == 0)
                return 1;   // Handle equality as beeing greater
            else
                return result;
        }
    }

    /// <summary>
    /// Set of utility functions
    /// </summary>
    static class Utilities
    {
        /// <summary>
        /// Clone a list of ICloneable objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToClone">List to clone</param>
        /// <returns>Cloed list</returns>
        public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        /// <summary>
        /// Chek if a URL is valid
        /// </summary>
        /// <param name="url">URL to check</param>
        /// <returns>File size, or -1 if invalid</returns>
        public static long CheckURLExists(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";
                request.Timeout = 1000;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK) ? response.ContentLength : -1;
            }
            catch
            {
                //Any exception will returns -1.
                return -1;
            }
        }

        /// <summary>
        /// Download web page as a string
        /// </summary>
        /// <param name="url">Page URL</param>
        /// <returns>Web page string contents, or empty string in case of error</returns>
        public static string GetStringFromURL(string url)
        {
            try
            {
                string html = string.Empty;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }

                return html;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Download image from URL
        /// </summary>
        /// <param name="url">Image URL</param>
        /// <returns>Downloaded Image</returns>
        public static Image GetImageFromURL(string url)
        {
            WebClient wc = new WebClient();
            byte[] bytes = wc.DownloadData(url);
            MemoryStream ms = new MemoryStream(bytes);
            Image img = Image.FromStream(ms);
            return img;
        }

        /// <summary>
        /// Sanity check if a JSON string is valid
        /// Checks for balanced parentheses to see if tail of the string was truncated
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <returns>true if JSON string appears valid</returns>
        public static bool IsJsonStringValid(string json)
        {
            if (json.Count(f => f == '{') == 0 || json.Count(f => f == '{') != json.Count(f => f == '}')) return false;
            if (json.Count(f => f == '[') != json.Count(f => f == ']')) return false;
            return true;
        }

        /// <summary>
        /// Read local file to string
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>Contents of the file</returns>
        public static string ReadLocalFile(string path)
        {
            StreamReader r = new StreamReader(path);
            string str = r.ReadToEnd();
            r.Close();
            return str;
        }

        private static void RecursiveDelete(DirectoryInfo baseDir)
        {
            if (!baseDir.Exists)
            return;

            foreach (var dir in baseDir.EnumerateDirectories())
            {
                RecursiveDelete(dir);
            }
            baseDir.Delete(true);
        }

        public static void DeleteEntireDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                RecursiveDelete(new DirectoryInfo(path));
            }
        }

        /// <summary>
        /// Resize image helper function -- does not preserve aspect ratio
        /// </summary>
        /// <param name="image">image to resize</param>
        /// <param name="width">Target width</param>
        /// <param name="height">Target height</param>
        /// <returns>Resized image</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Crop image helper function
        /// </summary>
        /// <param name="img">Image to crop</param>
        /// <param name="cropArea">Crop rectangle</param>
        /// <returns>Cropped image</returns>
        public static Image CropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        /// <summary>
        /// Load expedition deck from a string code list
        /// </summary>
        /// <param name="cardCodes">List of card codes</param>
        /// <returns>Deck contents</returns>
        public static List<CardWithCount> LoadDeckFromStringCodeList(string[] cardCodes)
        {
            List<CardWithCount> cards = new List<CardWithCount>();
            if (cardCodes == null)
            {
                return cards;
            }
            foreach (var cardCode in cardCodes)
            {
                int index = cards.FindIndex(item => item.Code.Equals(cardCode));
                if (index >= 0)
                {
                    cards[index].Count++;
                }
                else
                {
                    Card card = CardLibrary.GetCard(cardCode);
                    cards.Add(new CardWithCount(card, 1));
                }
            }
            cards = cards.OrderBy(card => card.Cost).ThenBy(card => card.Name).ToList();

            return cards;
        }

        public static void CallActionSafelyAndWait(Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                bool done = false;
                control.Invoke(new Action(() => { action.Invoke(); done = true; }));
                while (!done)
                {
                    Thread.Yield();
                }
            }
            else
            {
                action.Invoke();
            }
        }

        private const int SW_SHOWNOACTIVATE = 4;
        private const int HWND_TOPMOST = -1;
        private const uint SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(
             int hWnd,             // Window handle
             int hWndInsertAfter,  // Placement-order handle
             int X,                // Horizontal position
             int Y,                // Vertical position
             int cx,               // Width
             int cy,               // Height
             uint uFlags);         // Window positioning flags

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void ShowInactiveTopmost(Form frm)
        {
            ShowWindow(frm.Handle, SW_SHOWNOACTIVATE);
            SetWindowPos(frm.Handle.ToInt32(), HWND_TOPMOST,
            frm.Left, frm.Top, frm.Width, frm.Height,
            SWP_NOACTIVATE);
        }

        public static byte[] ZipFromStringList(List<string> strList)
        {
            var outputStream = new MemoryStream();
            var gzipStream = new GZipStream(outputStream, CompressionMode.Compress);
            using (var writer = new StreamWriter(gzipStream, Encoding.UTF8))
            {
                foreach (var str in strList)
                {
                    writer.WriteLine(str);
                }
            }

            return outputStream.ToArray();
        }

        public static List<string> UnzipToStringList(byte[] bytes)
        {
            var inputStream = new MemoryStream(bytes);
            var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            List<string> result = new List<string>();
            using (var reader = new StreamReader(gzipStream, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    result.Add(line);
                }
            }
            return result;
        }
    }
}
