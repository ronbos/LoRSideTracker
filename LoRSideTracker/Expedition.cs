using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoRSideTracker
{
    //    {"IsSwap":true,"DraftPicks":null,"SwappedOut":["01IO037"],"SwappedIn":["02BW007"]}]
    public class ExpeditionPick
    {
        public bool IsSwap { get; private set; }
        public string[] DraftPicks { get; private set; }
        public string[] SwappedOut { get; private set; }
        public string[] SwappedIn { get; private set; }

        public ExpeditionPick(Dictionary<string, JsonElement> json)
        {
            IsSwap = json["IsSwap"].ToObject<bool>();
            DraftPicks = json["Picks"].ToObject<string[]>();
            SwappedOut = json["SwappedOut"].ToObject<string[]>();
            SwappedIn = json["SwappedIn"].ToObject<string[]>();
        }

    }
    public interface ExpeditionUpdateCallback
    {
        void OnExpeditionDeckUpdated(List<CardWithCount> Cards);
    }

    public class Expedition : AutoUpdatingWebStringCallback
    {
        public bool IsActive { get; private set; }
        public string State { get; private set; }
        public string[] Record { get; private set; }
        public ExpeditionPick[] DraftPicks { get; private set; }
        public List<CardWithCount> Cards { get; private set; }
        public int NumberOfGames { get; private set; }
        public int NumberOfWins { get; private set; }
        public int NumberOfLosses { get; private set; }

        private AutoUpdatingWebString WebString;

        private ExpeditionUpdateCallback Callback;

        public Expedition(ExpeditionUpdateCallback callback = null)
        {
            Callback = callback;
            Clear();
            WebString = new AutoUpdatingWebString(Constants.ExpeditionStateURL(), 1000, this);
        }

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
                Cards = LoadDeckFromStringCodeList(json["Deck"].ToObject<string[]>());
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
                Callback.OnExpeditionDeckUpdated(Cards);
            }
        }

        public List<CardWithCount> LoadDeckFromStringCodeList(string[] cardCodes)
        {
            List<CardWithCount> cards = new List<CardWithCount>();
            if (cardCodes == null)
            {
                //RawData = "";
                return cards;
            }
            foreach (var cardCode in cardCodes)
            {
                int index = cards.FindIndex(item => item.Code.Equals(cardCode));
                if (index >= 0)
                {
                    cards[index].Count++;
                }
                else
                {
                    Card card = CardLibrary.GetCard(cardCode);
                    cards.Add(new CardWithCount(card, 1));
                }
            }
            cards = cards.OrderBy(card => card.Cost).ThenBy(card => card.Name).ToList();

            return cards;
        }

    }
}
