using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoRSideTracker
{
    /// <summary>
    /// Auto updating object executes AutoUpdate() function on a timer.
    /// Timer starts running after a Start() call and continues until
    /// Stop() is called or object is destroyed.
    /// </summary>
    public abstract class AutoUpdatingObject
    {
        private Task UpdaterTask;
        private bool IsUpdaterTaskActive = false;

        /// <summary>
        /// Interval between AutoUpdate calls, in milliseconds
        /// </summary>
        public int IntervalMs { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AutoUpdatingObject()
        {
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~AutoUpdatingObject()
        {
            Stop();
        }

        /// <summary>
        /// Start the timer to trigger AutoUpdate() callbacks
        /// </summary>
        /// <param name="intervalMs">Timer interval in milliseconds</param>
        public void Start(int intervalMs)
        {
            IntervalMs = intervalMs;
            if (!IsUpdaterTaskActive)
            {
                IsUpdaterTaskActive = true;
                UpdaterTask = Task.Run(() => UpdateWorker());
            }
        }

        /// <summary>
        /// Stop the time that driver AutoUpdate() calls
        /// </summary>
        public void Stop()
        {
            if (UpdaterTask != null)
            {
                IsUpdaterTaskActive = false;
                UpdaterTask.Wait();
                UpdaterTask = null;
            }
        }

        /// <summary>
        /// Implement this function to receive callbacks at intervals
        /// Specified in Start()
        /// </summary>
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
}
