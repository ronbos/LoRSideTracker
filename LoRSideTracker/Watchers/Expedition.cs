using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoRSideTracker
{
    /// <summary>
    /// Expedition draft pick
    /// </summary>
    public class ExpeditionPick
    {
        /// <summary>Is it a swap pick</summary>

        public bool IsSwap { get; private set; }
        /// <summary>Picks</summary>
        public string[] DraftPicks { get; private set; }
        /// <summary>What was swapped out if Swap</summary>
        public string[] SwappedOut { get; private set; }
        /// <summary>What was swapped in if Swap</summary>
        public string[] SwappedIn { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="json">JSON element containing pick info</param>
        public ExpeditionPick(Dictionary<string, JsonElement> json)
        {
            IsSwap = json["IsSwap"].ToObject<bool>();
            DraftPicks = json["Picks"].ToObject<string[]>();
            SwappedOut = json["SwappedOut"].ToObject<string[]>();
            SwappedIn = json["SwappedIn"].ToObject<string[]>();
        }

    }

    /// <summary>
    /// Interface to receive expedition updates
    /// </summary>
    public interface ExpeditionUpdateCallback
    {
        /// <summary>
        /// Receives updates that expedition deck changed
        /// </summary>
        /// <param name="Cards"></param>
        void OnExpeditionDeckUpdated(List<CardWithCount> Cards, string deckCode);
    }

    /// <summary>
    /// Expedition state
    /// </summary>
    public class Expedition : AutoUpdatingWebStringCallback
    {
        /// <summary>Is expedition active</summary>
        public bool IsActive { get; private set; }
        /// <summary>Current state</summary>
        public string State { get; private set; }
        /// <summary>Current record</summary>
        public string[] Record { get; private set; }
        /// <summary>List of draft picks</summary>
        public ExpeditionPick[] DraftPicks { get; private set; }
        /// <summary>Current deck</summary>
        public List<CardWithCount> Cards { get; private set; }
        /// <summary>Number of Games</summary>
        public int NumberOfGames { get; private set; }
        /// <summary>Number of Wins</summary>
        public int NumberOfWins { get; private set; }
        /// <summary>Number of Losses</summary>
        public int NumberOfLosses { get; private set; }

        private AutoUpdatingWebString WebString;

        private ExpeditionUpdateCallback Callback;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callback"></param>
        public Expedition(ExpeditionUpdateCallback callback = null)
        {
            Callback = callback;
            Clear();
            WebString = new AutoUpdatingWebString(Constants.ExpeditionStateURL(), 1000, this);
        }

        /// <summary>
        /// Clear expedition state
        /// </summary>
        public void Clear()
        {
            IsActive = false;
            State = "Unknown";
            Record = null;
            DraftPicks = null;
            Cards = new List<CardWithCount>();
            NumberOfGames = 0;
            NumberOfWins = 0;
            NumberOfLosses = 0;
        }

        /// <summary>
        /// Process newly updated web string to generate new expedition state
        /// </summary>
        /// <param name="newValue">new web string</param>
        public void OnWebStringUpdated(string newValue)
        {
            if (Utilities.IsJsonStringValid(newValue))
            {
                LoadFromJson(JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(newValue));
            }
            else
            {
                Clear();
            }
        }

        /// <summary>
        /// Load expedition state from JSON string
        /// </summary>
        /// <param name="json">JSON string</param>
        public void LoadFromJson(Dictionary<string, JsonElement> json)
        {
            if (json == null)
            {
                Clear();
            }
            else
            {
                IsActive = json["IsActive"].ToObject<bool>();
                State = json["State"].ToString();
                Record = json["Record"].ToObject<string[]>();
                Cards = Utilities.LoadDeckFromStringCodeList(json["Deck"].ToObject<string[]>());
                NumberOfGames = json["Games"].ToObject<int>();
                NumberOfWins = json["Wins"].ToObject<int>();
                NumberOfLosses = json["Losses"].ToObject<int>();

                var expeditionPicks = json["DraftPicks"].ToObject<Dictionary<string, JsonElement>[]>();
                if (expeditionPicks != null)
                {
                    DraftPicks = new ExpeditionPick[expeditionPicks.Length];
                    for (int i = 0; i < expeditionPicks.Length; i++)
                    {
                        DraftPicks[i] = new ExpeditionPick(expeditionPicks[i]);
                    }
                }
                else
                {
                    DraftPicks = null;
                }
            }

            if (Callback != null)
            {
                Callback.OnExpeditionDeckUpdated(Cards, "");
            }
        }

    }
}
