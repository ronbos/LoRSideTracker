using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoRSideTracker
{
    /// <summary>
    /// Log Window
    /// </summary>
    public partial class LogWindow : Form
    {
        private RichTextBox DebugLogTextBox;
        private RichTextBox LogTextBox;
        /// <summary>
        /// Current unformated text (with debug)
        /// </summary>
        public string DebugText
        {
            get
            {
                string result = string.Empty;
                if (DebugLogTextBox != null)
                {
                    Utilities.CallActionSafelyAndWait(DebugLogTextBox, new Action(() => { result = DebugLogTextBox.Text; }));
                }
                return result;
            }
        }

        /// <summary>
        /// Current RTF-formated text (with debug)
        /// </summary>
        public string DebugTextRtf
        {
            get
            {
                string result = string.Empty;
                if (DebugLogTextBox != null)
                {
                    Utilities.CallActionSafelyAndWait(DebugLogTextBox, new Action(() => { result = DebugLogTextBox.Rtf; }));
                }
                return result;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public LogWindow()
        {
            InitializeComponent();
        }

        private RichTextBox CreateRichTextBox()
        {
            RichTextBox rtb = new RichTextBox();
            rtb.Parent = this;
            rtb.CreateControl();
            rtb.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            rtb.SetBounds(
                DebugLogsCheckBox.Left,
                DebugLogsCheckBox.Left,
                ClientRectangle.Width - 2 * DebugLogsCheckBox.Left,
                DebugLogsCheckBox.Top - 2 * DebugLogsCheckBox.Left);
            return rtb;
        }

        private void LogWindow_Load(object sender, EventArgs e)
        {
            if (LogTextBox == null)
            {
                LogTextBox = CreateRichTextBox();
                DebugLogTextBox = CreateRichTextBox();
                DebugLogsCheckBox.Checked = Properties.Settings.Default.MainWindowShowDebugLog;
                if (Properties.Settings.Default.MainWindowShowDebugLog)
                {
                    DebugLogTextBox.Show();
                    LogTextBox.Hide();
                }
                else
                {
                    DebugLogTextBox.Hide();
                    LogTextBox.Show();
                }
            }
        }

        /// <summary>
        /// Populate the contents of logs with a copy of RTF-formatted text
        /// </summary>
        /// <param name="rtfText">RTF text to use</param>
        public void SetRtf(string rtfText)
        {
            if (LogTextBox == null)
            {
                LogWindow_Load(this, null);
            }
            DebugLogTextBox.Rtf = rtfText;

            // Remove all "\cf2" blocks
            int nextCf = rtfText.IndexOf(@"\cf");
            int nextCf2 = rtfText.IndexOf(@"\cf2");
            while (nextCf2 >= 0 && nextCf >= 0)
            {
                if (nextCf <= nextCf2)
                {
                    nextCf = rtfText.IndexOf(@"\cf", nextCf + 1);
                }
                else
                {
                    // Ready to erase
                    rtfText = rtfText.Remove(nextCf2, nextCf - nextCf2);
                    nextCf = nextCf2;
                    nextCf2 = rtfText.IndexOf(@"\cf2", nextCf2);
                }
            }

            if (nextCf2 >= 0)
            {
                rtfText = rtfText.Remove(nextCf2);
            }
            rtfText = System.Text.RegularExpressions.Regex.Replace(rtfText, @"\\cf2.*\\cf[^2]", @"XXX");
            LogTextBox.Rtf = rtfText;
        }

        private void DebugLogsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MainWindowShowDebugLog = DebugLogsCheckBox.Checked;
            if (DebugLogsCheckBox.Checked)
            {
                DebugLogTextBox.Show();
                LogTextBox.Hide();
            }
            else
            {
                DebugLogTextBox.Hide();
                LogTextBox.Show();
            }
        }

        /// <summary>
        /// Clear log window contents
        /// </summary>
        public void Clear()
        {
            if (LogTextBox != null)
            {
                Utilities.CallActionSafelyAndWait(LogTextBox, new Action(() =>
                {
                    LogTextBox.Clear();
                    LogTextBox.ScrollToCaret();
                }));
            }
            if (DebugLogTextBox != null)
            {
                Utilities.CallActionSafelyAndWait(DebugLogTextBox, new Action(() =>
                {
                    DebugLogTextBox.Clear();
                    DebugLogTextBox.ScrollToCaret();
                }));
            }
        }

        /// <summary>
        /// Write single line to the log
        /// </summary>
        /// <param name="type"></param>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        public void WriteLine(LogType type, string format, params object[] arg)
        {
            Utilities.CallActionSafelyAndWait(this, new Action(() => { WriteLineSafe(type, format, arg); }));
        }

        private void WriteLineSafe(LogType type, string format, params object[] arg)
        {
            string text = String.Format(format, arg);
            string frontText, backText;
            int index = text.IndexOf(':');
            if (index >= 0)
            {
                frontText = text.Substring(0, index + 1);
                backText = text.Substring(index + 1) + "\r\n";
            }
            else
            {
                frontText = string.Empty;
                backText = text + "\r\n";
            }
            Color textColor = Color.Black;
            switch (type)
            {
                case LogType.Player:
                    textColor = Color.Blue;
                    break;
                case LogType.Opponent:
                    textColor = Color.Red;
                    break;
                case LogType.Debug:
                case LogType.DebugVerbose:
                    textColor = Color.Gray;
                    break;
                default:
                    break;
            }

            if (DebugLogTextBox != null)
            {
                Utilities.CallActionSafelyAndWait(DebugLogTextBox, new Action(() =>
                {
                    if (!string.IsNullOrEmpty(frontText)) DebugLogTextBox.AppendText(frontText, textColor, true);
                    DebugLogTextBox.AppendText(backText, textColor, false);
                    DebugLogTextBox.ScrollToCaret();
                }));
            }

            if (LogTextBox != null && type != LogType.Debug)
            {
                Utilities.CallActionSafelyAndWait(LogTextBox, new Action(() =>
                {
                    if (!string.IsNullOrEmpty(frontText)) LogTextBox.AppendText(frontText, textColor, true);
                    LogTextBox.AppendText(backText, textColor, false);
                    LogTextBox.ScrollToCaret();
                }));
            }
        }

        private void LogWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}
