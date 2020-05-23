using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace LoRSideTracker
{
    public interface DeckUpdateCallback
    {
        void OnDeckUpdated(List<CardWithCount> Cards);
    }

    public class Deck : AutoUpdatingWebStringCallback
    {
        public List<CardWithCount> Cards { get; private set; }

        private AutoUpdatingWebString WebString;

        private DeckUpdateCallback Callback;

        public Deck(DeckUpdateCallback callback = null)
        {
            Callback = callback;

            Cards = new List<CardWithCount>();

            WebString = new AutoUpdatingWebString(Constants.StaticDeckURL(), 1000, this);
        }

        public void OnWebStringUpdated(string newValue)
        {
            if (Utilities.IsJsonStringValid(newValue))
            {
                LoadDeckFromJson(JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(newValue));
            }
            else
            {
                Cards = new List<CardWithCount>();
            }
        }

        public void LoadDeckFromJson(Dictionary<string, JsonElement> deck)
        {
            Cards = new List<CardWithCount>();
            if (deck == null)
            {
                return;
            }

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
            if (Callback != null)
            {
                Callback.OnDeckUpdated(Cards);
            }
        }
    }
}
