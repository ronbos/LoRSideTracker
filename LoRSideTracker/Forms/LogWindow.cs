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

        private void LogWindow_Load(object sender, EventArgs e)
        {
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

            // Remove all the gray text lines
            // First determine which color is gray. e.g.:
            // {\colortbl ;\red0\green0\blue0;\red0\green0\blue255;\red255\green0\blue0;\red128\green128\blue128;}
            string colorTableMarker = @"\colortbl";
            string grayColorMarker = @"\red128\green128\blue128";
            int colorTableOffset = rtfText.IndexOf(colorTableMarker);
            int grayColorOffset = rtfText.IndexOf(grayColorMarker);
            int grayColorIndex = 0;
            for (int i = rtfText.IndexOf(";", colorTableOffset + 1); i < grayColorOffset; i = rtfText.IndexOf(";", i + 1))
            {
                grayColorIndex++;
            }
            string cfGray = string.Format("\\cf{0}", grayColorIndex);
            int nextCf = rtfText.IndexOf(@"\cf");
            int nextCfGray = rtfText.IndexOf(cfGray);
            while (nextCfGray >= 0 && nextCf >= 0)
            {
                if (nextCf <= nextCfGray)
                {
                    nextCf = rtfText.IndexOf(@"\cf", nextCf + 1);
                }
                else
                {
                    // Ready to erase
                    rtfText = rtfText.Remove(nextCfGray, nextCf - nextCfGray);
                    nextCf = nextCfGray;
                    nextCfGray = rtfText.IndexOf(cfGray, nextCfGray);
                }
            }

            if (nextCfGray >= 0)
            {
                rtfText = rtfText.Remove(nextCfGray);
            }
            rtfText = System.Text.RegularExpressions.Regex.Replace(rtfText, string.Format("\\\\cf{0}.*\\\\cf[^{0}]", grayColorIndex), @"XXX");
            rtfText = System.Text.RegularExpressions.Regex.Replace(rtfText, @"\[..\] ", @"");
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
            string zoneText = "";
            string frontText, backText;
            if (text[0] == '[' && text[3] == ']' && text[4] == ' ')
            {
                zoneText = text.Substring(0, 5);
                text = text.Substring(5);
            }
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
                    if (!string.IsNullOrEmpty(frontText)) DebugLogTextBox.AppendText(zoneText + frontText, textColor, true);
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
