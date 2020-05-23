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
    /// <summary>
    /// Overlay update callback interface
    /// </summary>
    public interface OverlayUpdateCallback
    {
        /// <summary>
        /// Callback for when player drawn set has been changed
        /// </summary>
        /// <param name="cards">Cards in the set</param>
        void OnPlayerDrawnSetUpdated(List<CardWithCount> cards);

        /// <summary>
        /// Callback for when opponent played set has been changed
        /// </summary>
        /// <param name="cards">Cards in the set</param>
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
        public List<string> Stage { get; private set; }

        private bool IsInitialDraw = true;

        public PlayerOverlay()
        {
            Hand = new List<string>();
            Field = new List<string>();
            Stage = new List<string>();
        }

        public void Reset()
        {
            Hand.Clear();
            Field.Clear();
            Stage.Clear();
            IsInitialDraw = true;
        }

        public void Update(LogType logType, List<OverlayElement> elements, int screenWidth, int screenHeight, List<string> cardsDrawn, List<string> cardsPlayed)
        {
            List<string> newHand = new List<string>();
            List<string> newStage = new List<string>();
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
                        newStage.Add(element.CardCode);
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
                Update(logType, newHand, newField, newStage, cardsDrawn, cardsPlayed);
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

        private void Update(LogType logType, List<string> newHand, List<string> newField, List<string> newStage, List<string> cardsDrawn, List<string> cardsPlayed)
        {
            List<string> movedToStage = GetDifference(newStage, Stage);
            List<string> movedFromStage = GetDifference(Stage, newStage);
            List<string> movedToHand = GetDifference(newHand, Hand);
            List<string> movedFromHand = GetDifference(Hand, newHand);
            List<string> movedToField = GetDifference(newField, Field);
            List<string> movedFromField = GetDifference(Field, newField);

            List<string> movedFromStageToHand = ExtractIntersection(movedFromStage, movedToHand);
            List<string> movedFromHandToStage = ExtractIntersection(movedFromHand, movedToStage);
            List<string> movedFromStageToField = ExtractIntersection(movedFromStage, movedToField);
            List<string> movedFromFieldToStage = ExtractIntersection(movedFromField, movedToStage);
            List<string> movedFromHandToField = ExtractIntersection(movedFromHand, movedToField);
            List<string> movedFromFieldToHand = ExtractIntersection(movedFromField, movedToHand);

            if (Hand.Count > 0 && IsInitialDraw)
            {
                IsInitialDraw = false;
            }

            foreach (var c in movedFromStageToHand) { Log.WriteLine(logType, "[SH] Drawn: {0}", CardLibrary.GetCard(c).Name); }

            // Reporting disabled to make output less verbose
            foreach (var c in movedFromHandToStage) { Log.WriteLine(LogType.Debug, "[HS] Playing: {0}", CardLibrary.GetCard(c).Name); }

            foreach (var c in movedFromStageToField)
            {
                Card card = CardLibrary.GetCard(c);
                Log.WriteLine(logType, "[SF] {0}: {1}", card.Type.Equals("Unit") ? "Summoned" : "Cast", card.Name);
            }
            // Reporting disabled due to bugs in rectangles
            foreach (var c in movedFromFieldToStage) { Log.WriteLine(LogType.Debug, "[FS] UNEXPECTED: {0}", CardLibrary.GetCard(c).Name); }

            foreach (var c in movedFromHandToField) { Log.WriteLine(logType, "[HF] Cast: {0}", CardLibrary.GetCard(c).Name); }

            foreach (var c in movedFromFieldToHand) { Log.WriteLine(logType, "[FH] Recalled: {0}", CardLibrary.GetCard(c).Name); }

            // Reporting disabled due to bugs in rectangles
            foreach (var c in movedFromHand) { Log.WriteLine(LogType.Debug, "[HX] Discarded: {0}", CardLibrary.GetCard(c).Name); }
            if (IsInitialDraw)
            {
                foreach (var c in movedToHand) { Log.WriteLine(logType, "[XH] Initial Hand: {0}", CardLibrary.GetCard(c).Name); }
            }
            else
            {
                // Reporting disabled due to bugs in rectangles
                foreach (var c in movedToHand) { Log.WriteLine(LogType.Debug, "[XH] Added to Hand: {0}", CardLibrary.GetCard(c).Name); }
            }

            foreach (var c in movedFromField) { Log.WriteLine(logType, "[FX] Removed from Battlefield: {0}", CardLibrary.GetCard(c).Name); }
            foreach (var c in movedToField)
            {
                Card card = CardLibrary.GetCard(c);
                Log.WriteLine(logType, "[XF] {0}: {1}", card.Type.Equals("Unit") ? "Summoned" : "Cast", card.Name);
            }

            if (IsInitialDraw && logType != LogType.Opponent)
            {
                foreach (var c in movedFromStage) { Log.WriteLine(LogType.Debug, "[SX] Mulliganed: {0}", CardLibrary.GetCard(c).Name); }
            }
            if (logType != LogType.Player)
            {
                foreach (var c in movedToStage) { Log.WriteLine(logType, "[XS] Drawing: {0}", CardLibrary.GetCard(c).Name); }
            }
            else
            {
                // Reporting disabled to make output less verbose
                foreach (var c in movedToStage) { Log.WriteLine(LogType.Debug, "[XS] Playing: {0}", CardLibrary.GetCard(c).Name); }
            }

            if (cardsDrawn != null)
            {
                cardsDrawn.AddRange(movedFromStageToHand);
                if (IsInitialDraw)
                {
                    cardsDrawn.AddRange(movedToHand);
                }
            }
            if (cardsPlayed != null)
            {
                cardsPlayed.AddRange(movedFromStageToField);
                cardsPlayed.AddRange(movedFromHandToField);
                cardsPlayed.AddRange(movedToField);
            }

            Hand = GetDifference(Hand, movedFromHandToStage);
            Hand = GetDifference(Hand, movedFromHandToField);
            Hand.AddRange(movedFromStageToHand);
            Hand.AddRange(movedFromFieldToHand);
            if (IsInitialDraw)
            {
                Hand.AddRange(movedToHand);
            }
            Stage = newStage;
            Field = newField;

            if (Hand.Count > 0 && IsInitialDraw)
            {
                IsInitialDraw = false;
                Log.WriteLine("==============");
            }
        }
    }

    class Overlay : AutoUpdatingWebStringCallback
    {
        public string GameState { get; private set; }
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }

        public string PlayerName { get; private set; } = "";
        public List<OverlayElement> PlayerElements { get; private set; }

        public List<CardWithCount> PlayerDrawnCards { get; private set; }
        public List<CardWithCount> OpponentPlayedCards { get; private set; }

        public string OpponentName { get; private set; } = "";
        public List<OverlayElement> OpponentElements { get; private set; }

        private PlayerOverlay PlayerTracker;
        private PlayerOverlay OpponentTracker;

        private bool GameWasAnnounced = false;

        private bool NotRespondingHasBeenReported = false;

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

            WebString = new AutoUpdatingWebString(Constants.OverlayStateURL(), 30, this);
        }


        /// <summary>
        /// Process newly updated web string to generate new overlay state
        /// </summary>
        /// <param name="newValue">new web string</param>
        public void OnWebStringUpdated(string newValue)
        {
            if (Utilities.IsJsonStringValid(newValue))
            {
                NotRespondingHasBeenReported = false;
                Reload(JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(newValue));
            }
            else
            {
                if (!NotRespondingHasBeenReported)
                {
                    Log.WriteLine("{0} is producing invalid data", Constants.OverlayStateURL());
                    NotRespondingHasBeenReported = true;
                }
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
                Log.WriteLine("Game state changed from {0} to {1}", oldGameState, GameState);
                PlayerDrawnCards.Clear();
                OpponentPlayedCards.Clear();
                PlayerTracker.Reset();
                OpponentTracker.Reset();
                GameWasAnnounced = false;
                Callback.OnPlayerDrawnSetUpdated(PlayerDrawnCards);
                Callback.OnOpponentPlayedSetUpdated(OpponentPlayedCards);
                if (GameState.Equals("InProgress"))
                {
                    Log.Clear();
                }
                if (oldGameState.Equals("InProgress"))
                {
                    string json = Utilities.GetStringFromURL(Constants.GameResultURL());
                    Dictionary<string, JsonElement> gameResult = JsonSerializer.Deserialize< Dictionary<string, JsonElement> >(json);
                    Log.WriteLine("Game no. {0} Result: {1}", gameResult["GameID"].ToObject<int>(), 
                        gameResult["LocalPlayerWon"].ToObject<bool>() ? "Win" : "Loss");
                }
            }

            if (PlayerName != null && PlayerName.Length > 0 && OpponentName != null && OpponentName.Length > 0)
            {
                if (!GameWasAnnounced && PlayerName.Length > 0)
                {
                    Log.WriteLine("New Game: {0} vs {1}", PlayerName, OpponentName);
                    GameWasAnnounced = true;
                }
                List<string> cardsDrawn = new List<string>();
                List<string> cardsPlayed = new List<string>();
                PlayerTracker.Update(LogType.Player, PlayerElements, ScreenWidth, ScreenHeight, cardsDrawn, null);
                OpponentTracker.Update(LogType.Opponent, OpponentElements, ScreenWidth, ScreenHeight, null, cardsPlayed);
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
        }
    }
}
