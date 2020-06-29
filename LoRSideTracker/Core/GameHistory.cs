using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LoRSideTracker
{
    /// <summary>
    /// Persistent history of all the games played
    /// </summary>
    public static class GameHistory
    {
        private static readonly Dictionary<string, string> AIDeckNames = new Dictionary<string, string>()
        {
            { "decks_badstuns_name", "Flash of Steel (AI)" },
            { "decks_easybraum_name", "Stay Warm (AI)" },
            { "decks_easyteemo_name", "Scout's Honor (AI)" },
            { "deckname_kinkou_keepers", "Stealthy Strikes (AI)" },
            { "decks_mediumelise_name", "Spider Swarm (AI)" },
            { "decks_mediumdraven_name", "The Main Event (AI)" },
            { "decks_mediumzed_name", "Shadow and Blades (AI)" },
            { "front_five_game_one", "Poro Trouble (AI)" }
        };

        /// <summary></summary>
        public static List<GameRecord> Games { get; private set; }

        /// <summary></summary>
        public static Dictionary<string, string> DeckNames { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public static void LoadAllGames(IProgressDisplay pdc)
        {
            // Load all known deck names
            DeckNames = new Dictionary<string, string>();
            if (File.Exists(Constants.GetLocalDeckNamesFilePath()))
            {
                try
                {
                    var json = Utilities.ReadLocalFile(Constants.GetLocalDeckNamesFilePath());
                    Dictionary<string, JsonElement> deckNames = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                    DeckNames = deckNames["DeckNames"].ToObject<Dictionary<string, string>>();
                }
                catch
                {
                    if (File.Exists(Constants.GetLocalDeckNamesFilePath() + ".backup"))
                    {
                        try
                        {
                            var json = Utilities.ReadLocalFile(Constants.GetLocalDeckNamesFilePath() + ".backup");
                            Dictionary<string, JsonElement> deckNames = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                            DeckNames = deckNames["DeckNames"].ToObject<Dictionary<string, string>>();
                        }
                        catch { }
                    }
                }
            }

            // Load all games
            Games = new List<GameRecord>();
            if (Directory.Exists(Constants.GetLocalGamesPath()))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Constants.GetLocalGamesPath());
                FileInfo[] files = dirInfo.GetFiles("*.txt");
                var filesInOrder = files.OrderBy(x => x.CreationTime);
                for (int i = 0; i < filesInOrder.Count(); i++)
                {
                    filesInOrder.Count();
                    try
                    {
                        AddGameRecord(GameRecord.LoadFromFile(filesInOrder.ElementAt(i).FullName));
                    }
                    catch
                    {
                        // Skip bad records
                    }
                    pdc.Update("Loading game records...", 100.0 * (i + 1) / filesInOrder.Count());
                    //Thread.Yield();
                }
            }
        }

        /// <summary>
        /// Adda game to history
        /// </summary>
        /// <param name="gr">Game record to add</param>
        public static void AddGameRecord(GameRecord gr)
        {
            try {
                gr.OpponentName = AIDeckNames[gr.OpponentName];
                gr.OpponentIsAI = true;
            } catch { }
            Games.Insert(0, gr);
        }

        /// <summary>
        /// Set deck name
        /// </summary>
        /// <param name="deckSignature"></param>
        /// <param name="name"></param>
        public static void SetDeckName(string deckSignature, string name)
        {
            DeckNames[deckSignature] = name;
            var json = JsonSerializer.Serialize(new
            {
                DeckNames = DeckNames
            });

            if (File.Exists(Constants.GetLocalDeckNamesFilePath()))
            {
                if (File.Exists(Constants.GetLocalDeckNamesFilePath() + ".backup"))
                {
                    File.Delete(Constants.GetLocalDeckNamesFilePath() + ".backup");
                }
                File.Move(Constants.GetLocalDeckNamesFilePath(), Constants.GetLocalDeckNamesFilePath() + ".backup");
            }
            File.WriteAllText(Constants.GetLocalDeckNamesFilePath(), json);

            // Update individual game names, but only for constructed
            for (int i = 0; i < Games.Count; i++)
            {
                if (deckSignature == Games[i].GetDeckSignature() && !Games[i].IsExpedition())
                {
                    Games[i].MyDeckName = name;
                }
            }
        }
    }
}
