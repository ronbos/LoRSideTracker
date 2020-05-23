using System;
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
        private string Value = "";

        AutoUpdatingWebStringCallback CallbackObject;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">URL to query</param>
        /// <param name="intervalMs">Interval in ms between queries</param>
        /// <param name="callbackObject">Object to receive OnWebStringUpdated() notifications</param>
        public AutoUpdatingWebString(string url, int intervalMs = 1000, AutoUpdatingWebStringCallback callbackObject = null)
        {
            URL = url;
            CallbackObject = callbackObject;
            Start(intervalMs);
        }

        /// <summary>
        /// Implementation of AutoUpdatingObject.AutoUpdate(),
        /// Runs a new web query and checks if the string has changed
        /// </summary>
        public override void AutoUpdate()
        {
            string newValue;
            try
            {
                newValue = Utilities.GetStringFromURL(URL);
                if (newValue == null) newValue = "";
            }
            catch
            {
                newValue = "";
            }

            if (!Value.Equals(newValue))
            {
                Value = newValue;
                if (CallbackObject != null)
                {
                    CallbackObject.OnWebStringUpdated(newValue);
                }
            }
        }
    }
}
