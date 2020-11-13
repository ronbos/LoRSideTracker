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
        public static readonly Dictionary<string, string> AIDeckNames = new Dictionary<string, string>()
        {
            { "decks_badstuns_name", "Flash of Steel (AI)" },
            { "decks_badbarriers_name", "Sword and Shield (AI)" },
            { "decks_badvengeance_name", "Retribution (AI)" },
            { "decks_badspellbound_name", "Ghastly Experimentation (AI)" },
            { "decks_badfrostbite_name", "Avarosan Permafrost (AI)" },
            { "decks_easybraum_name", "Stay Warm (AI)" },
            { "decks_easyteemo_name", "Scout's Honor (AI)" },
            { "decks_easythresh_name", "Tortured Souls (AI)" },
            { "decks_mediumelise_name", "Spider Swarm (AI)" },
            { "decks_mediumdraven_name", "The Main Event (AI)" },
            { "decks_mediumzed_name", "Shadow and Blades (AI)" },
            { "front_five_game_one", "Poro Trouble (AI)" },
            { "decks_hardkatarina_name", "Frontline Assault (AI)" },
            { "decks_hardtryndamere_name", "Ancient Wisdom (AI)" },
            { "deckname_kinkou_keepers", "Stealthy Strikes (AI)" },
            { "deckname_trifarian_incursion", "Noxian Strength (AI)" }
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
        /// Adda game to history, and optionally save to file
        /// </summary>
        /// <param name="gr">Game record to add</param>
        /// <param name="saveGame">If true, game should be saved</param>
        /// <param name="gameLog">Game event log to save</param>
        public static void AddGameRecord(GameRecord gr, bool saveGame = false, List<string> gameLog = null)
        {
            if (saveGame)
            {
                // Save game record to file
                gr.Timestamp = DateTime.Now;
                gr.Log = Log.CurrentLogRtf;
                string fileName = string.Format(@"{0}_{1}_{2}_{3}_{4}_{5}",
                    gr.Timestamp.Year,
                    gr.Timestamp.Month,
                    gr.Timestamp.Day,
                    gr.Timestamp.Hour,
                    gr.Timestamp.Minute,
                    gr.Timestamp.Second);
                gr.SaveToFile(Constants.GetLocalGamesPath() + "\\" + fileName + ".txt");
                if (gameLog != null && gameLog.Count > 0)
                {
                    File.WriteAllBytes(Constants.GetLocalGamesPath() + "\\" + fileName + ".playback", Utilities.ZipFromStringList(gameLog));
                }
            }
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
            List<string> keys = new List<string>();
            try 
            { 
                string previousName = DeckNames[deckSignature];

                // Collect all signatures mapping to this name
                foreach (var item in DeckNames)
                {
                    if (item.Value == previousName)
                    {
                        keys.Add(item.Key);
                    }
                }
            }
            catch
            {
                keys.Add(deckSignature);
            }

            foreach (var k in keys)
            {
                DeckNames[k] = name;
            }

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
                if (!Games[i].IsExpedition() && keys.Contains(Games[i].GetDeckSignature()))
                {
                    Games[i].MyDeckName = name;
                }
            }
        }
    }
}
