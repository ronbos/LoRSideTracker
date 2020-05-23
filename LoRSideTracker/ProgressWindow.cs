using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace LoRSideTracker
{
    public interface ProgressDisplay
    {
        void Update(string message, double percentage);
    }

    public partial class ProgressWindow : Form, ProgressDisplay
    {
        private delegate void UpdateDelegate(string message, double percentage);

        public ProgressWindow()
        {
            InitializeComponent();
        }

        public void Update(string message, double percentage)
        {
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
            MyProgressBar.Value = (int)(0.5 + percentage);
            MyProgressBar.Invalidate();
        }
    }
}
