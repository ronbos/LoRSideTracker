using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoRSideTracker
{
    /// <summary>
    /// Log type to use with Utilities.Log() functions
    /// </summary>
    public enum LogType
    {
        /// <summary>Log player action</summary>
        Player,
        /// <summary>Log opponent action</summary>
        Opponent,
        /// <summary>Log other actions</summary>
        Other,
        /// <summary>Debug log only logs to console</summary>
        Debug,
        /// <summary>Debug log only logs to console in debug builds</summary>
        DebugVerbose
    }

    /// <summary>
    /// Set of logging utility functions
    /// </summary>
    static class Log
    {
        static RichTextBox LogTextBox = null;

        public static bool ShowDebugLog { get; set; } = false;

        public static bool IgnoreDebugVerboseLog { get; set; } = false;

        public static string CurrentLogText
        {
            get { return LogTextBox == null ? string.Empty : LogTextBox.Text; }
        }

        public static string CurrentLogRtf
        {
            get
            {
                string result = string.Empty;
                if (LogTextBox != null)
                {
                    Utilities.CallActionSafelyAndWait(LogTextBox, new Action(() => { result = LogTextBox.Rtf; }));
                }
                return result;
            }
        }

        /// <summary>
        /// Set the RichTextBox to use for logging
        /// </summary>
        /// <param name="obj">RichTextBox to use</param>
        public static void SetTextBox(RichTextBox obj)
        {
            LogTextBox = obj;
        }

        /// <summary>
        /// Clear the log RichTextBox, if specified
        /// </summary>
        public static void Clear()
        {
            if (LogTextBox != null)
            {
                if (LogTextBox.InvokeRequired)
                {
                    LogTextBox.Invoke(new Action(() => { LogTextBox.Clear(); LogTextBox.ScrollToCaret(); }));
                }
                else
                {
                    LogTextBox.Clear();
                    LogTextBox.ScrollToCaret();
                }
            }
        }

        /// <summary>
        /// Log a string to console and RichTextBox, if specified, using LogType.General
        /// </summary>
        /// <param name="format">Format string</param>
        /// <param name="arg">Format string arguments</param>
        public static void WriteLine(string format, params object[] arg)
        {
            WriteLine(LogType.Other, format, arg);
        }

        /// <summary>
        /// Log a string to console and RichTextBox, if specified
        /// </summary>
        /// <param name="type">LogType to use</param>
        /// <param name="format">Format string</param>
        /// <param name="arg">Format string arguments</param>
        public static void WriteLine(LogType type, string format, params object[] arg)
        {
            if (type == LogType.DebugVerbose
#if DEBUG
                && IgnoreDebugVerboseLog
#endif
                )
            {
                return;
            }

            string text = String.Format(format, arg);
            Console.WriteLine(text);

            if (LogTextBox != null && (type != LogType.Debug || ShowDebugLog))
            {
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

                if (LogTextBox.InvokeRequired)
                {
                    LogTextBox.Invoke(new Action(() => {
                        if (!string.IsNullOrEmpty(frontText)) LogTextBox.AppendText(frontText, textColor, true);
                        LogTextBox.AppendText(backText, textColor, false);
                        LogTextBox.ScrollToCaret();
                    }));
                }
                else
                {
                    if (!string.IsNullOrEmpty(frontText)) LogTextBox.AppendText(frontText, textColor, true);
                    LogTextBox.AppendText(backText, textColor, false);
                    LogTextBox.ScrollToCaret();
                }
            }
        }
    }
}
