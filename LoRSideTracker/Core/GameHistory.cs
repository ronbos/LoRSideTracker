using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoRSideTracker
{
    public static class GameHistory
    {
        public static List<GameRecord> Games { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public static void LoadAllGames()
        {
            // Load all games
            Games = new List<GameRecord>();
            if (Directory.Exists(Constants.GetLocalGamesPath()))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Constants.GetLocalGamesPath());
                FileInfo[] files = dirInfo.GetFiles();

                foreach (FileInfo fi in files.OrderBy(x => x.CreationTime))
                {
                    try
                    {
                        AddGameRecord(GameRecord.LoadFromFile(fi.FullName));
                    }
                    catch
                    {
                        // Skip bad records
                    }
                }
            }

        }

        /// <summary>
        /// Adda game to history
        /// </summary>
        /// <param name="gr">Game record to add</param>
        public static void AddGameRecord(GameRecord gr)
        {
            Games.Insert(0, (GameRecord)gr.Clone());
        }

        public static List<GameRecord> GetGames(string signature)
        {
            return Games.FindAll(x => signature == x.GetDeckSignature()).ToList();
        }
    }
}
