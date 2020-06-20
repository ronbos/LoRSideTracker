using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace LoRSideTracker
{
    /// <summary>
    /// Interface to receive notification that web string has changed
    /// </summary>
    public interface AutoUpdatingWebStringCallback
    {
        /// <summary>
        /// Called when AutoUpdatingWebString has detected that
        /// the queried page string has changed
        /// </summary>
        /// <param name="newValue">New web string value</param>
        /// <param name="timestamp">associated timestamp</param>
        void OnWebStringUpdated(string newValue, double timestamp);
    }

    /// <summary>
    /// AutoUpdatingWebString queries URL for a string content and reports
    /// when the string has changed through AutoUpdatingWebStringCallback interface
    /// </summary>
    public class AutoUpdatingWebString : AutoUpdatingObject
    {
        private string URL;
        private string Value = string.Empty;
        private DateTime LastUpdateTimestamp;
        private DateTime StartTimestamp;
        private TimeSpan ForceUpdateInterval;
        private bool ShouldSuppressMouseDownEvents = false;

        AutoUpdatingWebStringCallback CallbackObject;

        private List<string> FullLog;

        private List<string> ContentFromFile;
        private Thread PlaybackThread;
        private bool ShouldStopPlayback = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">URL to query</param>
        /// <param name="intervalMs">Interval in ms between queries</param>
        /// <param name="callbackObject">Object to receive OnWebStringUpdated() notifications</param>
        /// <param name="forceUpdateInterval">If > 0, mnumber of milliseconds when next update is forced even with the same string</param>
        /// <param name="shouldSuppressMouseDownUpdates">If true, updates when mouse id down are ignored</param>
        public AutoUpdatingWebString(string url, 
            int intervalMs = 1000, 
            AutoUpdatingWebStringCallback callbackObject = null,
            int forceUpdateInterval = 0,
            bool shouldSuppressMouseDownUpdates = false)
        {
            URL = url;
            CallbackObject = callbackObject;
            ForceUpdateInterval = TimeSpan.FromMilliseconds(forceUpdateInterval);
            StartTimestamp = DateTime.Now;
            LastUpdateTimestamp = StartTimestamp - ForceUpdateInterval;
            ShouldSuppressMouseDownEvents = shouldSuppressMouseDownUpdates;
            Start(intervalMs);
        }

        /// <summary>
        /// Constructor for playback from file
        /// </summary>
        /// <param name="gameLog"></param>
        /// <param name="intervalMs"></param>
        /// <param name="callbackObject"></param>
        /// <param name="forceUpdateInterval"></param>
        public AutoUpdatingWebString(
            List<string> gameLog,
            int intervalMs,
            AutoUpdatingWebStringCallback callbackObject,
            int forceUpdateInterval)
        {
            CallbackObject = callbackObject;
            ContentFromFile = gameLog;
            PlaybackThread = new Thread(RunUpdateFromFile);
            ForceUpdateInterval = TimeSpan.FromMilliseconds(forceUpdateInterval);
            ShouldStopPlayback = false;
            PlaybackThread.Start();
        }

        /// <summary>
        /// Begins an event log
        /// </summary>
        public void StartLog()
        {
            // Only start log if we are not in playback mode
            if (PlaybackThread == null)
            {
                FullLog = new List<string>();
            }
        }

        /// <summary>
        /// Stop log and write out the log to file
        /// </summary>
        public List<string> StopLog()
        {
            List<string> result = FullLog;
            FullLog = null;
            return result;
        }

        private void RunUpdateFromFile()
        {
            Thread.Sleep(5000);
            double time = 0;
            for (int i = 0; i + 1 < ContentFromFile.Count && !ShouldStopPlayback; i += 2)
            {
                // Read the timestamp
                double timestamp = double.Parse(ContentFromFile[i]);
                while (i > 1 && time > 0 && time + 100 < timestamp)
                {
                    time += 100;
                    CallbackObject.OnWebStringUpdated(ContentFromFile[i - 1], time);
                }
                CallbackObject.OnWebStringUpdated(ContentFromFile[i + 1], timestamp);
                time = timestamp;
            }
            ContentFromFile = null;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~AutoUpdatingWebString()
        {
            if (PlaybackThread != null)
            {
                ShouldStopPlayback = true;
                PlaybackThread.Join();
                PlaybackThread = null;
            }
            else
            {
                Stop();
                StopLog();
            }
        }

        /// <summary>
        /// Implementation of AutoUpdatingObject.AutoUpdate(),
        /// Runs a new web query and checks if the string has changed
        /// </summary>
        public override void AutoUpdate()
        {
            string newValue = Utilities.GetStringFromURL(URL);
            DateTime now = DateTime.Now;
            bool valueChanged = (Value != newValue);
            if (valueChanged && ShouldSuppressMouseDownEvents && Control.MouseButtons.HasFlag(MouseButtons.Left))
            {
                valueChanged = false;
            }
            if (valueChanged || (ForceUpdateInterval.TotalMilliseconds > 0 && 
                now > LastUpdateTimestamp + ForceUpdateInterval))
            {
                double timestamp = (now - StartTimestamp).TotalMilliseconds;
                if (valueChanged)
                {
                    Value = newValue;
                }

                // Log this value if log is set up
                bool logged = false;
                if (FullLog != null && valueChanged)
                {
                    FullLog.Add(string.Format("{0}", timestamp));
                    FullLog.Add(Value);
                    logged = true;
                }

                if (CallbackObject != null)
                {
                    CallbackObject.OnWebStringUpdated(Value, timestamp);

                    // In case log was just enabled during callback, also log this value
                    if (!logged && FullLog != null && valueChanged)
                    {
                        FullLog.Add(string.Format("{0}", timestamp));
                        FullLog.Add(Value);
                    }
                }

                LastUpdateTimestamp = now;
            }
        }
    }
}
