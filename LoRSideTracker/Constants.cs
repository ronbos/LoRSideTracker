using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoRSideTracker
{
    public static class Constants
    {
        public const int Port = 21337;
        public const string Host = "http://127.0.0.1";
        public const string Language = "en_us";

        public static string StaticDeckURL()
        {
            return String.Format(@"{0}:{1}/static-decklist", Constants.Host, Constants.Port);
        }
        public static string ExpeditionStateURL()
        {
            return String.Format(@"{0}:{1}/expeditions-state", Constants.Host, Constants.Port);
        }
        public static string OverlayStateURL()
        {
            return String.Format(@"{0}:{1}/positional-rectangles", Constants.Host, Constants.Port);
        }

        public static string GetLocalSetsPath()
        {
            return Application.LocalUserAppDataPath + @"\Sets";
        }

        public static string GetSetName(int setNumber)
        {
            return String.Format("set{0}-{1}", setNumber, Language);
        }

        public static string GetSetPath(int setNumber)
        {
            return String.Format("{0}\\{1}", GetLocalSetsPath(), GetSetName(setNumber));
        }

        public static string GetSetZip(int setNumber)
        {
            return GetSetPath(setNumber) + @".zip";
        }

        public static string GetSetURL(int setNumber)
        {
            return String.Format("https://dd.b.pvp.net/latest/{0}.zip", GetSetName(setNumber));
        }
    }
}
