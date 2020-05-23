using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoRSideTracker
{

    public abstract class AutoUpdatingObject
    {
        private Task UpdaterTask;
        private bool IsUpdaterTaskActive = false;

        public int IntervalMs { get; private set; }

        public AutoUpdatingObject()
        {
        }

        ~AutoUpdatingObject()
        {
            Stop();
        }

        public void Start(int intervalMs)
        {
            IntervalMs = intervalMs;
            if (!IsUpdaterTaskActive)
            {
                IsUpdaterTaskActive = true;
                UpdaterTask = Task.Run(() => UpdateWorker());
            }
        }

        public void Stop()
        {
            if (UpdaterTask != null)
            {
                IsUpdaterTaskActive = false;
                UpdaterTask.Wait();
                UpdaterTask = null;
            }
        }

        public abstract void AutoUpdate();

        private void UpdateWorker()
        {
            while (IsUpdaterTaskActive)
            {
                DateTime begin = DateTime.UtcNow;
                AutoUpdate();
                double elapsed = (DateTime.UtcNow - begin).TotalMilliseconds;

                Thread.Sleep(IntervalMs);
            }
        }
    }

    public interface AutoUpdatingWebStringCallback
    {
        void OnWebStringUpdated(string newValue);
    }

    public class AutoUpdatingWebString : AutoUpdatingObject
    {
        private string URL;
        private string Value = "";

        AutoUpdatingWebStringCallback CallbackObject;

        public AutoUpdatingWebString(string url, int intervalMs = 1000, AutoUpdatingWebStringCallback callbackObject = null)
        {
            URL = url;
            CallbackObject = callbackObject;
            Start(intervalMs);
        }

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
