using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoRSideTracker
{
    public class Card
    {
        // General info
        public string Name { get; private set; }
        public string Code { get; private set; }
        public string Type { get; private set; }
        public int Cost { get; private set; }
        public string Region { get; private set; }
        public string Rarity { get; private set; }
        public string FlavorText { get; private set; }
        public bool IsCollectible { get; private set; }

        // Unit info
        public int Attack { get; private set; }
        public int Health { get; private set; }

        // Spell info
        public string SpellSpeed { get; private set; }

        // Display Info
        public Image CardArt { get; private set; }
        public Image CardBanner { get; private set; }


        public Card(string setPath, Dictionary<string, JsonElement> dict)
        {
            const string IsCollectibleString = "True";
            Name = dict["name"].ToString();
            Code = dict["cardCode"].ToString();
            Type = dict["type"].ToString();
            Cost = dict["cost"].ToObject<int>();
            Region = dict["region"].ToString();
            Rarity = dict["rarity"].ToString();
            FlavorText = dict["flavorText"].ToString();
            IsCollectible = IsCollectibleString.Equals(dict["collectible"].ToString());

            // Unit info
            Attack = dict["attack"].ToObject<int>();
            Health = dict["health"].ToObject<int>();

            // Load images
            string cardArtPath = String.Format("{0}\\img\\cards\\{1}.png", setPath, Code);
            string cardBannerPath = String.Format("{0}\\img\\cards\\{1}-full.png", setPath, Code);
            CardArt = Image.FromFile(cardArtPath);
            CardBanner = Image.FromFile(cardBannerPath);

            // Spell info
            SpellSpeed = dict["spellSpeed"].ToString();
        }
    }

    public class CardWithCount : ICloneable
    {
        public Card TheCard { get; private set; }
        public string Name { get { return TheCard.Name; } }
        public string Code { get { return TheCard.Code; } }
        public string Type { get { return TheCard.Type; } }
        public int Cost { get { return TheCard.Cost; } }

        // Deck metadata
        public int Count { get; set; }

        public CardWithCount(Card card, int count)
        {
            TheCard = card;
            Count = count;
        }

        public object Clone()
        {
            return new CardWithCount(TheCard, Count);
        }
    }
}
