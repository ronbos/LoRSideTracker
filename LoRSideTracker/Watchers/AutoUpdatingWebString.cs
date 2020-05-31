﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        void OnWebStringUpdated(string newValue);
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
        private TimeSpan ForceUpdateInterval;

        AutoUpdatingWebStringCallback CallbackObject;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">URL to query</param>
        /// <param name="intervalMs">Interval in ms between queries</param>
        /// <param name="callbackObject">Object to receive OnWebStringUpdated() notifications</param>
        /// <param name="forceUpdateInterval">If > 0, mnumber of milliseconds when next update is forced even with the same string</param>
        public AutoUpdatingWebString(string url, 
            int intervalMs = 1000, 
            AutoUpdatingWebStringCallback callbackObject = null,
            int forceUpdateInterval = 0)
        {
            URL = url;
            CallbackObject = callbackObject;
            ForceUpdateInterval = TimeSpan.FromMilliseconds(forceUpdateInterval);
            LastUpdateTimestamp = DateTime.Now - ForceUpdateInterval;
            Start(intervalMs);
        }

        /// <summary>
        /// Implementation of AutoUpdatingObject.AutoUpdate(),
        /// Runs a new web query and checks if the string has changed
        /// </summary>
        public override void AutoUpdate()
        {
            string newValue = Utilities.GetStringFromURL(URL);
            DateTime now = DateTime.Now;

            if (Value != newValue || (ForceUpdateInterval.TotalMilliseconds > 0 && 
                now > LastUpdateTimestamp + ForceUpdateInterval))
            {
                Value = newValue;
                if (CallbackObject != null)
                {
                    CallbackObject.OnWebStringUpdated(newValue);
                }
                LastUpdateTimestamp = now;
            }
        }
    }
}
