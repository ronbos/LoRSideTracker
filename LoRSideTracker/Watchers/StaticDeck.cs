using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace LoRSideTracker
{
    /// <summary>
    /// Interface to receive static deck updates
    /// </summary>
    public interface StaticDeckUpdateCallback
    {
        /// <summary>
        /// Callback called when static deck has changed
        /// </summary>
        /// <param name="cards">Updated set</param>
        /// <param name="deckCode">Deck code</param>
        void OnDeckUpdated(List<CardWithCount> cards, string deckCode);
    }

    /// <summary>
    /// Static deck parsing and maintenance
    /// </summary>
    public class StaticDeck : AutoUpdatingWebStringCallback
    {
        /// <summary>Current deck contents</summary>
        public List<CardWithCount> Cards { get; private set; }
        /// <summary>Current deck code</summary>
        public string DeckCode { get; private set; }
        /// <summary>Current deck name</summary>
        public string DeckName { get; private set; }

        private AutoUpdatingWebString WebString;
        private StaticDeckUpdateCallback Callback;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callback">Interface to receive updates</param>
        public StaticDeck(StaticDeckUpdateCallback callback = null)
        {
            Callback = callback;

            Cards = new List<CardWithCount>();
            DeckCode = "";
            DeckName = "";

            if (callback != null)
            {
                WebString = new AutoUpdatingWebString(Constants.StaticDeckURL(), 1000, this);
            }
        }

        /// <summary>
        /// Process newly updated web string to generate new deck contents
        /// </summary>
        /// <param name="newValue">new web string</param>
        /// <param name="timestamp">associated timestamp</param>
        public void OnWebStringUpdated(string newValue, double timestamp)
        {
            Cards.Clear();
            DeckCode = "";
            DeckName = "";
            if (Utilities.IsJsonStringValid(newValue))
            {
                // Load deck from JSON
                Dictionary<string, JsonElement> deck = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(newValue);
                if (deck != null)
                {
                    LoadFromJSON(deck);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Reload()
        {
            Cards.Clear();
            DeckCode = "";
            string webString = Utilities.GetStringFromURL(Constants.StaticDeckURL());
            if (Utilities.IsJsonStringValid(webString))
            {
                // Load deck from JSON
                Dictionary<string, JsonElement> deck = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(webString);
                if (deck != null)
                {
                    LoadFromJSON(deck);
                }
            }
            return Cards.Count > 0;
        }

        private void LoadFromJSON(Dictionary<string, JsonElement> deck)
        {
            Cards.Clear();
            DeckCode = deck["DeckCode"].ToString();
            try { DeckName = GameHistory.DeckNames[DeckCode]; } catch { DeckName = GameRecord.DefaultConstructedDeckName; }
            var deckList = deck["CardsInDeck"].ToObject<Dictionary<string, int>>();
            if (deckList != null)
            {
                foreach (var j in deckList)
                {
                    string cardCode = j.Key;
                    Card card = CardLibrary.GetCard(cardCode);
                    int count = j.Value;
                    Cards.Add(new CardWithCount(card, count, true));
                }

                // Sort the deck
                Cards = Cards.OrderBy(card => card.Cost).ThenBy(card => card.Name).ToList();
            }
        }
    }
}
