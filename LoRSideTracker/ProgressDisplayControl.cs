using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace LoRSideTracker
{
    /// <summary>
    /// ProgressDisplay interface to receive progress display updates
    /// </summary>
    public interface ProgressDisplay
    {
        /// <summary>
        /// Update progress bar percentage and display message
        /// </summary>
        /// <param name="message">Display message</param>
        /// <param name="percentage">Progress bar percentage</param>
        void Update(string message, double percentage);
    }

    /// <summary>
    /// Control to display progress with message
    /// </summary>
    public partial class ProgressDisplayControl : UserControl, ProgressDisplay
    {
        private delegate void UpdateDelegate(string message, double percentage);

        /// <summary>
        /// Constructor
        /// </summary>
        public ProgressDisplayControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Update progress bar percentage and display message
        /// </summary>
        /// <param name="message">Display message</param>
        /// <param name="percentage">Progress bar percentage</param>
        public void Update(string message, double percentage)
        {
            // Make sure update is done on the UI thread
            if (this.InvokeRequired)
            {
                var d = new UpdateDelegate(SafeUpdate);
                this.Invoke(d, new object[] { message, percentage });
            }
            else
            {
                SafeUpdate(message, percentage);
            }
        }

        private void SafeUpdate(string message, double percentage)
        {
            MyLabel.Text = message;
            MyProgressBar.Increment((int)(0.5 + percentage) - MyProgressBar.Value);
            //MyProgressBar.Value = 99;// (int)(0.5 + percentage);
            //MyProgressBar.Invalidate();
            Application.DoEvents();
        }
    }
}
