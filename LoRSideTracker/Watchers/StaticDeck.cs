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

            WebString = new AutoUpdatingWebString(Constants.StaticDeckURL(), 1000, this);
        }

        /// <summary>
        /// Process newly updated web string to generate new deck contents
        /// </summary>
        /// <param name="newValue">new web string</param>
        /// <param name="timestamp">associated timestamp</param>
        public void OnWebStringUpdated(string newValue, double timestamp)
        {
            Cards = new List<CardWithCount>();
            string deckCode = "";
            if (Utilities.IsJsonStringValid(newValue))
            {
                // Load deck from JSON
                Dictionary<string, JsonElement> deck = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(newValue);
                if (deck != null)
                {
                    deckCode = deck["DeckCode"].ToString();
                    var deckList = deck["CardsInDeck"].ToObject<Dictionary<string, JsonElement>>();
                    if (deckList != null)
                    {
                        foreach (var j in deckList)
                        {
                            string cardCode = j.Key;
                            Card card = CardLibrary.GetCard(cardCode);
                            int count = Int32.Parse(j.Value.ToString());
                            Cards.Add(new CardWithCount(card, count));
                        }
                        Cards = Cards.OrderBy(card => card.Cost).ThenBy(card => card.Name).ToList();
                    }
                }
                else
                {
                    Log.WriteLine("{0} is producing invalid data", Constants.StaticDeckURL());
                }
            }
            if (Callback != null)
            {
                Callback.OnDeckUpdated(Cards, deckCode);
            }
        }
    }
}
