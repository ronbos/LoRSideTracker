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
        static LogWindow ActiveLogWindow = null;

        public static bool ShowDebugLog { get; set; } = false;

        public static bool IgnoreDebugVerboseLog { get; set; } = false;

        public static string CurrentLogText
        {
            get { return ActiveLogWindow.DebugText; }
        }

        public static string CurrentLogRtf
        {
            get
            {
                return ActiveLogWindow.DebugTextRtf;
            }
        }

        /// <summary>
        /// Set the log window to use for logging
        /// </summary>
        /// <param name="obj">Log window to use</param>
        public static void SetLogWindow(LogWindow obj)
        {
            ActiveLogWindow = obj;
        }

        /// <summary>
        /// Clear the log
        /// </summary>
        public static void Clear()
        {
            if (ActiveLogWindow != null)
            {
                Utilities.CallActionSafelyAndWait(ActiveLogWindow, new Action(() => 
                {
                    ActiveLogWindow.Clear();
                }));
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

            if (ActiveLogWindow != null)
            {
                ActiveLogWindow.WriteLine(type, format, arg);
            }
        }
    }
}
