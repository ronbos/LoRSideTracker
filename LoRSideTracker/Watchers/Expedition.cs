﻿using System;
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
            try
            {
                DraftPicks = json["DraftPicks"].ToObject<string[]>();
            } catch
            {
                DraftPicks = json["Picks"].ToObject<string[]>();

            }
            SwappedOut = json["SwappedOut"].ToObject<string[]>();
            SwappedIn = json["SwappedIn"].ToObject<string[]>();
        }
    }

    /// <summary>
    /// Interface to receive expedition updates
    /// </summary>
    public interface IExpeditionUpdateCallback
    {
        /// <summary>
        /// Receives updates that expedition deck changed
        /// </summary>
        /// <param name="cards"></param>
        void OnExpeditionDeckUpdated(List<CardWithCount> cards);
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
        /// <summary>Is this game an elimination game?</summary>
        public bool IsEliminationGame { get; private set; }

        /// <summary>Expedition signature based on first 14 draft picks</summary>
        public string Signature
        {
            get
            {
                string result = "";
                for (int i = 0; DraftPicks != null && i < DraftPicks.Length && i < 14; i++)
                {
                    result += string.Join("", DraftPicks[i].DraftPicks);
                }
                return result;
            }
        }

        private readonly AutoUpdatingWebString WebString;
        private readonly IExpeditionUpdateCallback Callback;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callback"></param>
        public Expedition(IExpeditionUpdateCallback callback = null)
        {
            Callback = callback;
            Clear();
            if (callback != null)
            {
                WebString = new AutoUpdatingWebString(Constants.ExpeditionStateURL(), 1000, this);
            }
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
            IsEliminationGame = false;
        }

        /// <summary>
        /// Process newly updated web string to generate new expedition state
        /// </summary>
        /// <param name="newValue">new web string</param>
        /// <param name="timestamp">associated timestamp</param>
        public void OnWebStringUpdated(string newValue, double timestamp)
        {
            if (Utilities.IsJsonStringValid(newValue))
            {
                LoadFromJson(JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(newValue));
            }
            else
            {
                Clear();
            }
            if (Callback != null)
            {
                Callback.OnExpeditionDeckUpdated(Cards);
            }
        }

        /// <summary>
        /// Load expedition state from URL
        /// </summary>
        public bool Reload()
        {
            Clear();
            string webString = Utilities.GetStringFromURL(Constants.ExpeditionStateURL());
            if (Utilities.IsJsonStringValid(webString))
            {
                // Load deck from JSON
                LoadFromJson(JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(webString));
            }
            return Cards.Count > 0;
        }

        /// <summary>
        /// Load expedition state from JSON string
        /// </summary>
        /// <param name="json">JSON string</param>
        public void LoadFromJson(Dictionary<string, JsonElement> json)
        {
            if (json != null)
            {
                IsActive = json["IsActive"].ToObject<bool>();
                State = json["State"].ToString();
                Record = json["Record"].ToObject<string[]>();
                Cards = Utilities.DeckFromStringCodeList(json["Deck"].ToObject<string[]>());
                NumberOfGames = json["Games"].ToObject<int>();
                NumberOfWins = json["Wins"].ToObject<int>();
                NumberOfLosses = json["Losses"].ToObject<int>();

                IsEliminationGame = false;
                if (Record != null)
                {
                    IsEliminationGame = (Array.FindLastIndex(Record, item => item.Equals("win")) < Array.FindLastIndex(Record, item => item.Equals("loss")))
                    || (NumberOfWins == 6);
                }

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
        }
    }
}
