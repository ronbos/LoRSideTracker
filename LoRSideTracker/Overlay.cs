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
        /// Callback for when player played set has been changed
        /// </summary>
        /// <param name="cards">Cards in the set</param>
        void OnPlayerPlayedSetUpdated(List<CardWithCount> cards);

        /// <summary>
        /// Callback for when player tossed set has been changed
        /// </summary>
        /// <param name="cards">Cards in the set</param>
        void OnPlayerTossedSetUpdated(List<CardWithCount> cards);

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
        public List<string> Ether { get; private set; }
        public List<string> Tossing { get; private set; }

        private bool IsInitialDraw = true;

        private LogType MyLogType;
        public PlayerOverlay(LogType logType)
        {
            MyLogType = logType;
            Hand = new List<string>();
            Field = new List<string>();
            Stage = new List<string>();
            Ether = new List<string>();
            Tossing = new List<string>();
        }

        public void Reset()
        {
            Hand.Clear();
            Field.Clear();
            Stage.Clear();
            Ether.Clear();
            Tossing.Clear();
            IsInitialDraw = true;
        }

        public void Update(List<OverlayElement> elements, 
            int screenWidth, 
            int screenHeight, 
            List<string> cardsDrawn,
            List<string> cardsPlayed,
            List<string> cardsTossed)
        {
            List<string> newHand = new List<string>();
            List<string> newStage = new List<string>();
            List<string> newField = new List<string>();
            List<string> newTossing = new List<string>();
            bool isDragging = false;

            // We normalize everything based on screen height. However, if screen ratio becomes
            // too high, screen expands height-wise. To make sure we have same behavior as before,
            // We adjust the height accordingly.
            int screenHeightAdjustment = 0;
            if (screenWidth * 0.66 < screenHeight)
            {
                int newScreenHeight = (int)(0.5 + screenWidth * 0.66);
                screenHeightAdjustment = (screenHeight - newScreenHeight) / 2;
            }

            foreach (OverlayElement element in elements)
            {
                // Ignore skills/abilities
                if (element.TheCard.Type == "Ability")
                {
                    continue;
                }

                // Place elements into one of three sets based on size and position
                // We have an issue with cards that are being hovered over due to
                // a bug, and cards being dragged to be played are problematic as well
                float left = (float)(element.BoundingBox.X - screenWidth / 2) / (float)screenHeight;
                float right = (float)(element.BoundingBox.Right - screenWidth / 2) / (float)screenHeight;
                float top = (float)(element.BoundingBox.Y + screenHeightAdjustment) / (float)screenHeight;
                float bottom = (float)(element.BoundingBox.Bottom) / (float)screenHeight;
                RectangleF normalizedRect = new RectangleF(left, top, right - left, bottom - top);
                PointF normalizedCenter = new PointF((normalizedRect.X + normalizedRect.Right) / 2, (normalizedRect.Y + normalizedRect.Bottom) / 2);
                //Log.WriteLine(LogType.DebugVerbose, "[--] {0} ({1},{2},{3},{4})", element.TheCard.Name, left, top, right-left, bottom - top);

                if (normalizedRect.Height == 0)
                {
                    newTossing.Add(element.CardCode);
                }
                else if (normalizedCenter.Y > 1.0f)
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
                        //break;
                    }
                }
            }

            //if (elements.Count > 0)
            //{
            //    Log.WriteLine(LogType.DebugVerbose, "[--] ---------");
            //}

            // Update tossing set here since it does ot depend on others
            if (cardsTossed != null)
            {
                List<string> movedFromTossing = GetDifference(Tossing, newTossing);
                cardsTossed.AddRange(movedFromTossing);
                Tossing = newTossing;
                foreach (var c in movedFromTossing) { Log.WriteLine(MyLogType, "[FH] Tossed: {0}", CardLibrary.GetCard(c).Name); }
            }

            if (!isDragging)
            {
                Update(newHand, newField, newStage, cardsDrawn, cardsPlayed);
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

        private void Update(List<string> newHand, List<string> newField, List<string> newStage, List<string> cardsDrawn, List<string> cardsPlayed)
        {
            List<string> movedToStage = GetDifference(newStage, Stage);
            List<string> movedFromStage = GetDifference(Stage, newStage);
            List<string> movedToHand = GetDifference(newHand, Hand);
            List<string> movedFromHand = GetDifference(Hand, newHand);
            List<string> movedToField = GetDifference(newField, Field);
            List<string> movedFromField = GetDifference(Field, newField);
            List<string> movedFromEther = Ether.Clone();

            List<string> movedFromStageToHand = ExtractIntersection(movedFromStage, movedToHand);
            List<string> movedFromHandToStage = ExtractIntersection(movedFromHand, movedToStage);
            List<string> movedFromStageToField = ExtractIntersection(movedFromStage, movedToField);
            List<string> movedFromFieldToStage = ExtractIntersection(movedFromField, movedToStage);
            List<string> movedFromHandToField = ExtractIntersection(movedFromHand, movedToField);
            List<string> movedFromFieldToHand = ExtractIntersection(movedFromField, movedToHand);
            List<string> movedFromEtherToHand = ExtractIntersection(movedFromEther, movedToHand);
            List<string> movedFromHandToEther = movedFromHand;
            movedFromHand.Clear();

            // Burst spell don't make it to field, as they appear discarded
            List<string> movedFromHandBurst = movedFromHand.FindAll(c => CardLibrary.GetCard(c).SpellSpeed.Equals("Burst"));
            movedFromHand = GetDifference(movedFromHand, movedFromHandBurst);
            movedFromHandToField.AddRange(movedFromHandBurst);

            if (Hand.Count > 0 && IsInitialDraw)
            {
                IsInitialDraw = false;
            }

            foreach (var c in movedFromStageToHand) { Log.WriteLine(MyLogType, "[SH] Drawn: {0}", CardLibrary.GetCard(c).Name); }

            // Reporting disabled to make output less verbose
            foreach (var c in movedFromHandToStage) { Log.WriteLine(LogType.Debug, "[HS] Playing: {0}", CardLibrary.GetCard(c).Name); }

            foreach (var c in movedFromStageToField)
            {
                Card card = CardLibrary.GetCard(c);
                Log.WriteLine(MyLogType, "[SF] {0}: {1}", card.Type.Equals("Unit") ? "Summoned" : "Cast", card.Name);
            }
            // Reporting disabled due to bugs in rectangles
            foreach (var c in movedFromFieldToStage) { Log.WriteLine(LogType.Debug, "[FS] UNEXPECTED: {0}", CardLibrary.GetCard(c).Name); }

            foreach (var c in movedFromHandToField) { Log.WriteLine(MyLogType, "[HF] Cast: {0}", CardLibrary.GetCard(c).Name); }

            foreach (var c in movedFromFieldToHand) { Log.WriteLine(MyLogType, "[FH] Recalled: {0}", CardLibrary.GetCard(c).Name); }

            // Reporting disabled due to bugs in rectangles
            foreach (var c in movedFromHand) { Log.WriteLine(LogType.Debug, "[HX] Discarded: {0}", CardLibrary.GetCard(c).Name); }
            if (IsInitialDraw)
            {
                foreach (var c in movedToHand) { Log.WriteLine(MyLogType, "[XH] Initial Hand: {0}", CardLibrary.GetCard(c).Name); }
            }
            else
            {
                // Reporting disabled due to bugs in rectangles
                foreach (var c in movedToHand) { Log.WriteLine(LogType.Debug, "[XH] Added to Hand: {0}", CardLibrary.GetCard(c).Name); }
            }

            foreach (var c in movedFromField)
            {
                if (CardLibrary.GetCard(c).Type == "Unit")
                {
                    Log.WriteLine(MyLogType, "[FX] Removed from Battlefield: {0}", CardLibrary.GetCard(c).Name);
                }
            }

            foreach (var c in movedToField)
            {
                Card card = CardLibrary.GetCard(c);
                Log.WriteLine(MyLogType, "[XF] {0}: {1}", (card.Type == "Unit") ? "Summoned" : "Cast", card.Name);
            }

            if (!IsInitialDraw && MyLogType != LogType.Opponent)
            {
                foreach (var c in movedFromStage) { Log.WriteLine(LogType.Debug, "[SX] Mulliganed: {0}", CardLibrary.GetCard(c).Name); }
            }

            // Reporting disabled to make output less verbose
            foreach (var c in movedToStage) { Log.WriteLine(LogType.Debug, "[XS] {0}: {1}", (MyLogType != LogType.Player) ? "Playing" : "Drawing", CardLibrary.GetCard(c).Name); }

            if (MyLogType != LogType.Opponent)
            {
                foreach (var c in movedFromHandToEther) { Log.WriteLine(LogType.Debug, "[HE] Disappeared from hand: {0}", CardLibrary.GetCard(c).Name); }
                foreach (var c in movedFromEtherToHand) { Log.WriteLine(LogType.Debug, "[EH] Came back to hand: {0}", CardLibrary.GetCard(c).Name); }
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

            // Update hand based on card movement only
            Hand = GetDifference(Hand, movedFromHandToStage);
            Hand = GetDifference(Hand, movedFromHandToField);
            Hand = GetDifference(Hand, movedFromHandToEther);
            Hand.AddRange(movedFromStageToHand);
            Hand.AddRange(movedFromFieldToHand);
            Hand.AddRange(movedFromEtherToHand);
            Hand.AddRange(movedToHand);

            // Special case: champion card may change form, either by upgrading or by
            // switching to the spell type. We try to account for these here
            for (int i = 0; i < movedToHand.Count; i++)
            {
                string c = (string)movedToHand[i];
                Card card = CardLibrary.GetCard(c);
                foreach (var acc in card.AssociatedCardCodes)
                {
                    int index = movedFromHand.FindIndex(item => item.Equals(acc));
                    if (index >= 0)
                    {
                        Hand.Remove(c);
                        Hand[i] = acc;
                        break;
                    }
                }
            }

            // Update stage based on card movement only
            Stage = GetDifference(Stage, movedFromStageToHand);
            Stage = GetDifference(Stage, movedFromStageToField);
            Stage.AddRange(movedFromHandToStage);
            Stage.AddRange(movedFromFieldToStage);
            Stage.AddRange(movedToStage);
            if (!IsInitialDraw)
            {
                Stage = GetDifference(Stage, movedFromStage);
            }

            Ether = GetDifference(Ether, movedFromEtherToHand);
            Ether.AddRange(movedFromHandToEther);
            if (cardsDrawn != null && cardsDrawn.Count > 0)
            {
                Ether.AddRange(movedFromEther);
            }
            else
            {
                foreach (var c in movedFromHand) { Log.WriteLine(MyLogType, "[HX] Discarded: {0}", CardLibrary.GetCard(c).Name); }
            }

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

        public string PlayerName { get; private set; } = string.Empty;
        public List<OverlayElement> PlayerElements { get; private set; }

        public List<CardWithCount> PlayerDrawnCards { get; private set; }
        public List<CardWithCount> PlayerPlayedCards { get; private set; }
        public List<CardWithCount> PlayerTossedCards { get; private set; }
        public List<CardWithCount> OpponentPlayedCards { get; private set; }

        public string OpponentName { get; private set; } = string.Empty;
        public List<OverlayElement> OpponentElements { get; private set; }

        private PlayerOverlay PlayerTracker;
        private PlayerOverlay OpponentTracker;

        private bool GameWasAnnounced = false;

        private bool NotRespondingHasBeenReported = false;

        // Tossed cards cannot be tracked reliably yet
        private bool ShouldTrackTossedCards = false;

        OverlayUpdateCallback Callback;

        private AutoUpdatingWebString WebString;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callback"></param>
        public Overlay(OverlayUpdateCallback callback)
        {
            Callback = callback;
            PlayerElements = new List<OverlayElement>();
            OpponentElements = new List<OverlayElement>();
            PlayerDrawnCards = new List<CardWithCount>();
            PlayerPlayedCards = new List<CardWithCount>();
            PlayerTossedCards = new List<CardWithCount>();
            OpponentPlayedCards = new List<CardWithCount>();
            PlayerTracker = new PlayerOverlay(LogType.Player);
            OpponentTracker = new PlayerOverlay(LogType.Opponent);
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
                    if (cardCode != "face")
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

            if (oldGameState != GameState)
            {
                Log.WriteLine("Game state changed from {0} to {1}", oldGameState, GameState);
                PlayerDrawnCards.Clear();
                PlayerPlayedCards.Clear();
                PlayerTossedCards.Clear();
                OpponentPlayedCards.Clear();
                PlayerTracker.Reset();
                OpponentTracker.Reset();
                GameWasAnnounced = false;
                Callback.OnPlayerDrawnSetUpdated(PlayerDrawnCards);
                Callback.OnPlayerPlayedSetUpdated(PlayerPlayedCards);
                Callback.OnPlayerTossedSetUpdated(PlayerTossedCards);
                Callback.OnOpponentPlayedSetUpdated(OpponentPlayedCards);
                if (GameState == "InProgress")
                {
                    Log.Clear();
                }
                if (oldGameState == "InProgress")
                {
                    string json = Utilities.GetStringFromURL(Constants.GameResultURL());
                    if (json != null && Utilities.IsJsonStringValid(json))
                    {
                        Dictionary<string, JsonElement> gameResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                        Log.WriteLine("Game no. {0} Result: {1}", gameResult["GameID"].ToObject<int>(),
                            gameResult["LocalPlayerWon"].ToObject<bool>() ? "Win" : "Loss");
                    }
                }
            }

            if (!string.IsNullOrEmpty(PlayerName) && !string.IsNullOrEmpty(OpponentName))
            {
                if (!GameWasAnnounced && !string.IsNullOrEmpty(PlayerName))
                {
                    Log.WriteLine("New Game: {0} vs {1}", PlayerName, OpponentName);
                    GameWasAnnounced = true;
                }
                List<string> cardsDrawn = new List<string>();
                List<string> cardsPlayed = new List<string>();
                List<string> cardsTossed = new List<string>();
                List<string> opponentCardsPlayed = new List<string>();
                PlayerTracker.Update(PlayerElements, ScreenWidth, ScreenHeight, cardsDrawn, cardsPlayed, ShouldTrackTossedCards ? cardsTossed : null);
                OpponentTracker.Update(OpponentElements, ScreenWidth, ScreenHeight, null, opponentCardsPlayed, null);

                if (AddCardsToSet(PlayerDrawnCards, cardsDrawn))
                {
                    Callback.OnPlayerDrawnSetUpdated(Utilities.Clone(PlayerDrawnCards));
                }
                if (AddCardsToSet(PlayerPlayedCards, cardsPlayed))
                {
                    Callback.OnPlayerPlayedSetUpdated(Utilities.Clone(PlayerPlayedCards));
                }
                if (AddCardsToSet(PlayerTossedCards, cardsTossed))
                {
                    Callback.OnPlayerTossedSetUpdated(Utilities.Clone(PlayerTossedCards));
                }
                if (AddCardsToSet(OpponentPlayedCards, opponentCardsPlayed))
                {
                    Callback.OnOpponentPlayedSetUpdated(Utilities.Clone(OpponentPlayedCards));
                }
            }
        }

        private bool AddCardsToSet(List<CardWithCount> cardSet, List<string> cardCodes)
        {
            foreach (string cardCode in cardCodes)
            {
                int index = cardSet.FindIndex(item => item.Code.Equals(cardCode));
                if (index >= 0)
                {
                    cardSet[index].Count++;
                }
                else
                {
                    cardSet.Add(new CardWithCount(CardLibrary.GetCard(cardCode), 1));
                }
            }
            cardSet = cardSet.OrderBy(card => card.Cost).ThenBy(card => card.Name).ToList();

            return cardCodes.Count > 0;
        }
    }
}
