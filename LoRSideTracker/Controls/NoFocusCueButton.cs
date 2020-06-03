using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoRSideTracker
{
    /// <summary>
    /// Button that does not show focus cues
    /// </summary>
    public class NoFocusCueButton : Button
    {
        /// <summary>
        /// Make sure focus cues does not show
        /// </summary>
        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }
    }
}
