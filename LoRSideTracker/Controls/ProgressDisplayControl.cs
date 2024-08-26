using System;
using System.Windows.Forms;

namespace LoRSideTracker
{
    /// <summary>
    /// ProgressDisplay interface to receive progress display updates
    /// </summary>
    public interface IProgressDisplay
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
    public partial class ProgressDisplayControl : UserControl, IProgressDisplay
    {
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
            Utilities.CallActionSafely(this, new Action(() =>
            {
                if (Visible)
                {
                    MyLabel.Text = message;
                    PercentageLabel.Text = string.Format("{0}%", (int)(0.5 + percentage));
                }
            }));
        }
    }
}
