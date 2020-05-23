using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml.XPath;
using System.ComponentModel;

namespace LoRSideTracker
{
    public interface OverlayUpdateCallback
    {
        void OnPlayerDrawnSetUpdated(List<CardWithCount> cards);

        void OnOpponentPlayedSetUpdated(List<CardWithCount> cards);
    }

    class OverlayElement
    {
        public Card TheCard { get; }
        public string CardCode { get; }
        public Rectangle BoundingBox { get; }
        public OverlayElement(Dictionary<string, JsonElement> dict, int ScreenHeight)
        {
            CardCode = dict["CardCode"].GetString();
            TheCard = CardLibrary.GetCard(CardCode);
            BoundingBox = new Rectangle(
                dict["TopLeftX"].GetInt32(),
                ScreenHeight - dict["TopLeftY"].GetInt32(),
                dict["Width"].GetInt32(),
                dict["Height"].GetInt32());
        }
    }

    class PlayerOverlay
    {
        public List<string> Hand { get; private set; }
        public List<string> Field { get; private set; }
        public List<string> Intro { get; private set; }

        private bool IsInitialDraw = true;

        public PlayerOverlay()
        {
            Hand = new List<string>();
            Field = new List<string>();
            Intro = new List<string>();
        }

        public void Reset()
        {
            if (Hand.Count > 0 || Field.Count > 0 || Intro.Count > 0)
            {
                Update(new List<string>(), new List<string>(), new List<string>(), null, null);
                IsInitialDraw = true;
            }
        }

        public void Update(List<OverlayElement> elements, int screenWidth, int screenHeight, List<string> cardsDrawn, List<string> cardsPlayed)
        {
            List<string> newHand = new List<string>();
            List<string> newIntro = new List<string>();
            List<string> newField = new List<string>();
            bool isDragging = false;

            foreach (OverlayElement element in elements)
            {
                // Ignore skills/abilities
                if (element.TheCard.Type.Equals("Ability"))
                {
                    continue;
                }

                float left = (float)(element.BoundingBox.X - screenWidth / 2) / (float)screenHeight;
                float right = (float)(element.BoundingBox.Right - screenWidth / 2) / (float)screenHeight;
                float top = (float)(element.BoundingBox.Y) / (float)screenHeight;
                float bottom = (float)(element.BoundingBox.Bottom) / (float)screenHeight;
                RectangleF normalizedRect = new RectangleF(left, top, right - left, bottom - top);
                PointF normalizedCenter = new PointF((normalizedRect.X + normalizedRect.Right) / 2, (normalizedRect.Y + normalizedRect.Bottom) / 2);

                if (normalizedCenter.Y > 1.0f)
                {
                    newHand.Add(element.CardCode);
                }
                else
                {
                    if (normalizedRect.Height < 0.15f)
                    {
                        newField.Add(element.CardCode);
                    }
                    else if (normalizedRect.Height > 0.30f)
                    {
                        newIntro.Add(element.CardCode);
                    }
                    else
                    {
                        isDragging = true;
                        break;
                    }
                }
            }
            if (!isDragging)
            {
                Update(newHand, newField, newIntro, cardsDrawn, cardsPlayed);
            }
        }

        private List<string> ExtractIntersection(List<string> l1, List<string> l2)
        {
            List<string> result = new List<string>();
            foreach (var x in l1)
            {
                if (l2.FindIndex(y => y.Equals(x)) >= 0)
                {
                    result.Add(x);
                }
            }
            // Remove intersection from both sets
            List<string> filteredResult = new List<string>();
            foreach (var x in result)
            {
                int index1 = l1.FindIndex(y => y.Equals(x));
                int index2 = l2.FindIndex(y => y.Equals(x));
                if (index1 >= 0 && index2 >= 0)
                {
                    l1.RemoveAt(index1);
                    l2.RemoveAt(index2);
                    filteredResult.Add(x);
                }
            }

            return filteredResult;
        }

        private List<string> GetDifference(List<string> l1, List<string> l2)
        {
            List<string> l1copy = Utilities.Clone(l1);
            List<string> l2copy = Utilities.Clone(l2);
            List<string> intersection = ExtractIntersection(l1copy, l2copy);

            List<string> result = Utilities.Clone(l1);
            ExtractIntersection(result, intersection);
            return result;
        }

        private void Update(List<string> newHand, List<string> newField, List<string> newIntro, List<string> cardsDrawn, List<string> cardsPlayed)
        {
            List<string> movedToIntro = GetDifference(newIntro, Intro);
            List<string> movedFromIntro = GetDifference(Intro, newIntro);
            List<string> movedToHand = GetDifference(newHand, Hand);
            List<string> movedFromHand = GetDifference(Hand, newHand);
            List<string> movedToField = GetDifference(newField, Field);
            List<string> movedFromField = GetDifference(Field, newField);

            List<string> movedFromIntroToHand = ExtractIntersection(movedFromIntro, movedToHand);
            List<string> movedFromHandToIntro = ExtractIntersection(movedFromHand, movedToIntro);
            List<string> movedFromIntroToField = ExtractIntersection(movedFromIntro, movedToField);
            List<string> movedFromFieldToIntro = ExtractIntersection(movedFromField, movedToIntro);
            List<string> movedFromHandToField = ExtractIntersection(movedFromHand, movedToField);
            List<string> movedFromFieldToHand = ExtractIntersection(movedFromField, movedToHand);

            if (Hand.Count > 0)
            {
                IsInitialDraw = false;
            }
            Intro = newIntro;
            Hand = newHand;
            Field = newField;

            foreach (var c in movedFromIntroToHand) { Console.WriteLine("Drawn: {0}", CardLibrary.GetCard(c).Name); }
            foreach (var c in movedFromHandToIntro) { Console.WriteLine("Being Played: {0}", CardLibrary.GetCard(c).Name); }

            foreach (var c in movedFromIntroToField) { Console.WriteLine("Played: {0}", CardLibrary.GetCard(c).Name); }
            foreach (var c in movedFromFieldToIntro) { Console.WriteLine("UNEXPECTED: {0}", CardLibrary.GetCard(c).Name); }

            foreach (var c in movedFromHandToField) { Console.WriteLine("Played Quickly: {0}", CardLibrary.GetCard(c).Name); }
            foreach (var c in movedFromFieldToHand) { Console.WriteLine("Recalled: {0}", CardLibrary.GetCard(c).Name); }

            foreach (var c in movedFromHand) { Console.WriteLine("Discarded: {0}", CardLibrary.GetCard(c).Name); }
            foreach (var c in movedToHand) { Console.WriteLine((IsInitialDraw) ? "Initial Hand: {0}" : "Added to Hand: {0}", CardLibrary.GetCard(c).Name); }

            foreach (var c in movedFromField) { Console.WriteLine("Removed from Battlefield: {0}", CardLibrary.GetCard(c).Name); }
            foreach (var c in movedToField) { Console.WriteLine("Summoned: {0}", CardLibrary.GetCard(c).Name); }

            foreach (var c in movedFromIntro) { Console.WriteLine("Mulliganed: {0}", CardLibrary.GetCard(c).Name); }
            foreach (var c in movedToIntro) { Console.WriteLine("Being Drawn: {0}", CardLibrary.GetCard(c).Name); }

            if (cardsDrawn != null)
            {
                cardsDrawn.AddRange(movedFromIntroToHand);
                if (IsInitialDraw)
                {
                    cardsDrawn.AddRange(movedToHand);
                }
            }
            if (cardsPlayed != null)
            {
                cardsPlayed.AddRange(movedFromIntroToField);
                cardsPlayed.AddRange(movedFromHandToField);
                cardsPlayed.AddRange(movedToField);
            }

            if (movedToHand.Count > 0)
            {
                IsInitialDraw = false;
            }

            if (movedFromHandToIntro.Count > 0 || movedFromIntroToHand.Count > 0
                || movedFromIntroToField.Count > 0 || movedFromFieldToIntro.Count > 0
                || movedFromHandToField.Count > 0 || movedFromFieldToHand.Count > 0
                || movedFromHand.Count > 0 || movedToHand.Count > 0
                || movedFromField.Count > 0 || movedToField.Count > 0
                || movedFromIntro.Count > 0 || movedToIntro.Count > 0)
            {
                Console.WriteLine("==============");
            }
        }
    }

    class Overlay : AutoUpdatingWebStringCallback
    {
        public string GameState { get; private set; }
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }

        public string PlayerName { get; private set; }
        public List<OverlayElement> PlayerElements { get; private set; }

        public List<CardWithCount> PlayerDrawnCards { get; private set; }
        public List<CardWithCount> OpponentPlayedCards { get; private set; }

        public string OpponentName { get; private set; }
        public List<OverlayElement> OpponentElements { get; private set; }

        private PlayerOverlay PlayerTracker;
        private PlayerOverlay OpponentTracker;

        OverlayUpdateCallback Callback;

        private AutoUpdatingWebString WebString;

        public Overlay(OverlayUpdateCallback callback)
        {
            Callback = callback;
            PlayerElements = new List<OverlayElement>();
            OpponentElements = new List<OverlayElement>();
            PlayerDrawnCards = new List<CardWithCount>();
            OpponentPlayedCards = new List<CardWithCount>();
            PlayerTracker = new PlayerOverlay();
            OpponentTracker = new PlayerOverlay();
            GameState = "Unknown";

            WebString = new AutoUpdatingWebString(Constants.OverlayStateURL(), 100, this);
        }


        public void OnWebStringUpdated(string newValue)
        {
            if (Utilities.IsJsonStringValid(newValue))
            {
                Reload(JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(newValue));
            }
            else
            {
                Reload(null);
            }
        }

        private void Reload(Dictionary<string, JsonElement> currentOverlay)
        {
            PlayerElements.Clear();
            OpponentElements.Clear();
            string oldGameState = GameState;

            if (currentOverlay != null)
            {
                PlayerName = currentOverlay["PlayerName"].GetString();
                OpponentName = currentOverlay["OpponentName"].GetString();
                GameState = currentOverlay["GameState"].GetString();
                if (GameState == null) GameState = "Unknown";
                PlayerElements = new List<OverlayElement>();
                OpponentElements = new List<OverlayElement>();

                var screen = currentOverlay["Screen"].ToObject<Dictionary<string, JsonElement>>();
                ScreenWidth = screen["ScreenWidth"].GetInt32();
                ScreenHeight = screen["ScreenHeight"].GetInt32();
                var rectangles = currentOverlay["Rectangles"].ToObject<Dictionary<string, JsonElement>[]>();
                foreach (var dict in rectangles)
                {
                    string cardCode = dict["CardCode"].GetString();
                    if (!cardCode.Equals("face"))
                    {
                        if (dict["LocalPlayer"].GetBoolean())
                        {
                            PlayerElements.Add(new OverlayElement(dict, ScreenHeight));
                        }
                        else
                        {
                            OpponentElements.Add(new OverlayElement(dict, ScreenHeight));
                        }
                    }
                    int TopLeftX = dict["TopLeftX"].GetInt32();
                    int TopLeftY = dict["TopLeftY"].GetInt32();
                    int Width = dict["Width"].GetInt32();
                    int Height = dict["Height"].GetInt32();
                }
            }
            else
            {
                GameState = "Unknown";
            }

            if (!oldGameState.Equals(GameState))
            {
                PlayerDrawnCards.Clear();
                OpponentPlayedCards.Clear();
                PlayerTracker.Reset();
                OpponentTracker.Reset();
                Callback.OnPlayerDrawnSetUpdated(PlayerDrawnCards);
                Callback.OnOpponentPlayedSetUpdated(OpponentPlayedCards);
            }

            if (GameState.Equals("InProgress"))
            {
                List<string> cardsDrawn = new List<string>();
                List<string> cardsPlayed = new List<string>();
                PlayerTracker.Update(PlayerElements, ScreenWidth, ScreenHeight, cardsDrawn, null);
                OpponentTracker.Update(OpponentElements, ScreenWidth, ScreenHeight, null, cardsPlayed);
                if (cardsDrawn.Count > 0)
                {
                    foreach (string cardCode in cardsDrawn)
                    {
                        int index = PlayerDrawnCards.FindIndex(item => item.Code.Equals(cardCode));
                        if (index >= 0)
                        {
                            PlayerDrawnCards[index].Count++;
                        }
                        else
                        {
                            PlayerDrawnCards.Add(new CardWithCount(CardLibrary.GetCard(cardCode), 1));
                        }
                        PlayerDrawnCards = PlayerDrawnCards.OrderBy(card => card.Cost).ThenBy(card => card.Name).ToList();
                    }

                    Callback.OnPlayerDrawnSetUpdated(Utilities.Clone(PlayerDrawnCards));
                }
                if (cardsPlayed.Count > 0)
                {
                    foreach (string cardCode in cardsPlayed)
                    {
                        int index = OpponentPlayedCards.FindIndex(item => item.Code.Equals(cardCode));
                        if (index >= 0)
                        {
                            OpponentPlayedCards[index].Count++;
                        }
                        else
                        {
                            OpponentPlayedCards.Add(new CardWithCount(CardLibrary.GetCard(cardCode), 1));
                        }
                        OpponentPlayedCards = OpponentPlayedCards.OrderBy(card => card.Cost).ThenBy(card => card.Name).ToList();
                    }

                    Callback.OnOpponentPlayedSetUpdated(Utilities.Clone(OpponentPlayedCards));
                }
            }
            else
            {
                //Callback.OnPlayerDrawnSetUpdated(PlayerDrawnCards);
                //Callback.OnOpponentDrawnSetUpdated(OpponentPlayedCards);
            }
        }
    }
}
