using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml.XPath;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace LoRSideTracker
{
    /// <summary>
    /// Overlay update callback interface
    /// </summary>
    public interface IOverlayUpdateCallback
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

        /// <summary>
        /// Callback for when game state changes
        /// </summary>
        /// <param name="oldGameState">Previous game state</param>
        /// <param name="newGameState">New game state</param>
        void OnGameStateChanged(string oldGameState, string newGameState);

        /// <summary>
        /// Callback for when elements have been updated
        /// </summary>
        /// <param name="playerElements"></param>
        /// <param name="opponentElements"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        void OnElementsUpdate(List<OverlayElement> playerElements, List<OverlayElement> opponentElements, int screenWidth, int screenHeight);
    }

    /// <summary>
    /// Overlay element
    /// </summary>
    public class OverlayElement : ICloneable
    {
        /// <summary></summary>
        public Card TheCard { get; private set; }
        /// <summary></summary>
        public string CardCode { get; private set; }
        /// <summary></summary>
        public Rectangle BoundingBox { get; private set; }
        /// <summary></summary>
        public RectangleF NormalizedBoundingBox { get; private set; }
        /// <summary></summary>
        public PointF NormalizedCenter { get; private set; }

        private OverlayElement()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        /// <param name="screenHeightForNormalized"></param>
        public OverlayElement(Dictionary<string, JsonElement> dict, int screenWidth, int screenHeight, int screenHeightForNormalized)
        {
            CardCode = dict["CardCode"].GetString();
            TheCard = CardLibrary.GetCard(CardCode);
            BoundingBox = new Rectangle(
                dict["TopLeftX"].GetInt32(),
                screenHeight - dict["TopLeftY"].GetInt32(), // Elements are reported upside down for some reason
                dict["Width"].GetInt32(),
                dict["Height"].GetInt32());

            float left = (float)(BoundingBox.X - screenWidth / 2) / (float)screenHeightForNormalized;
            float right = (float)(BoundingBox.Right - screenWidth / 2) / (float)screenHeightForNormalized;
            float top = (float)(BoundingBox.Y - (screenHeight - screenHeightForNormalized) / 2) / (float)screenHeightForNormalized;
            float bottom = (float)(BoundingBox.Bottom - (screenHeight - screenHeightForNormalized) / 2) / (float)screenHeightForNormalized;
            NormalizedBoundingBox = new RectangleF(left, top, right - left, bottom - top);
            NormalizedCenter = new PointF((left + right) / 2, (top + bottom) / 2);
        }

        /// <summary>
        /// ICloneable interface
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            OverlayElement result = new OverlayElement();
            result.TheCard = TheCard;
            result.CardCode = CardCode;
            result.BoundingBox = BoundingBox;
            result.NormalizedBoundingBox = NormalizedBoundingBox;
            result.NormalizedCenter = NormalizedCenter;
            return result;
        }
    }

    class OverlayZone
    {
        public List<string> CurrentSet;
        private List<string>[] OutgoingSets;
        private List<string> IncomingSet;

        public OverlayZone(int numExtraOutBuffers = 0)
        {
            CurrentSet = new List<string>();
            OutgoingSets = new List<string>[numExtraOutBuffers + 1];
            for (int i = 0; i < OutgoingSets.Length; i++)
            {
                OutgoingSets[i] = new List<string>();
            }
            IncomingSet = new List<string>();
        }

        public void Clear()
        {
            CurrentSet.Clear();
            foreach (var set in OutgoingSets) { set.Clear(); }
            IncomingSet.Clear();
        }

        public void SetNextState(List<string> nextSet, LogType logType = LogType.Debug, string logFormatUnit = null, string logFormatSpell = null)
        {
            // Incoming set is all the cards that are in next set but not in current set
            IncomingSet = GetDifference(nextSet, CurrentSet);

            // Some cards in the incoming set may be in one of the outgoing sets,
            // either due to a glitch or due to a transformation
            for (int i = IncomingSet.Count - 1; i >= 0; i--)
            {
                Card cardA = CardLibrary.GetCard(IncomingSet[i]);
                List<string> cardAndAssociated = new List<string>();
                cardAndAssociated.Add(cardA.Code);
                cardAndAssociated.AddRange(cardA.AssociatedCardCodes);
                for (int j = OutgoingSets.Length - 1; j >= 0; j--)
                {
                    int index = OutgoingSets[j].FindIndex(x => cardAndAssociated.Contains(x));
                    if (index >= 0)
                    {
                        CurrentSet.Add(IncomingSet[i]);
                        OutgoingSets[j].RemoveAt(index);
                        IncomingSet.RemoveAt(i);
                        break;
                    }
                }
            }

            // Roll all the outgoing set out by 1, and finalize tossing of the oldset set
            List<string> removed = OutgoingSets[OutgoingSets.Length - 1];
            for (int i = OutgoingSets.Length - 1; i > 0; i--)
            {
                // We toss the intersection
                OutgoingSets[i] = OutgoingSets[i - 1];
            }
            OutgoingSets[0] = GetDifference(CurrentSet, nextSet);
            CurrentSet = GetDifference(CurrentSet, OutgoingSets[0]);

            LogResult(removed, logType, logFormatUnit, logFormatSpell);
        }

        public List<string> AcceptFrom(OverlayZone otherZone, LogType logType, string logFormatUnit, string logFormatSpell = null)
        {
            return AcceptTransfer(otherZone, this, logType, logFormatUnit, logFormatSpell);
        }

        public List<string> ReleaseTo(OverlayZone otherZone, LogType logType, string logFormatUnit, string logFormatSpell = null)
        {
            return AcceptTransfer(this, otherZone, logType, logFormatUnit, logFormatSpell);
        }

        public List<string> AcceptFromAnywhere(LogType logType, string logFormatUnit, string logFormatSpell = null)
        {
            return AcceptTransfer(null, this, logType, logFormatUnit, logFormatSpell);
        }

        public List<string> ReleaseToAnywhere(LogType logType, string logFormatUnit, string logFormatSpell = null)
        {
            return AcceptTransfer(this, null, logType, logFormatUnit, logFormatSpell);
        }

        public void CancelOutgoing()
        {
            for (int i = 0; i < OutgoingSets.Length; i++) 
            {
                CurrentSet.AddRange(OutgoingSets[i]);
                OutgoingSets[i].Clear();
            }
        }

        public void CancelIncoming()
        {
            IncomingSet.Clear();
        }

        private static List<string> GetDifference(List<string> l1, List<string> l2)
        {
            List<string> l1copy = Utilities.Clone(l1);
            List<string> l2copy = Utilities.Clone(l2);
            List<string> intersection = ExtractIntersection(l1copy, l2copy);

            List<string> result = Utilities.Clone(l1);
            ExtractIntersection(result, intersection);
            return result;
        }

        private static List<string> ExtractIntersection(List<string> l1, List<string> l2)
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

        private static List<string> AcceptTransfer(OverlayZone fromZone, OverlayZone toZone, LogType logType, string logFormatUnit, string logFormatSpell)
        {
            List<string> result = new List<string>();
            if (fromZone == null)
            {
                result = toZone.IncomingSet.Clone();
                toZone.IncomingSet.Clear();
                toZone.CurrentSet.AddRange(result);
            }
            else if (toZone == null)
            {
                // Only release the oldest outgoing set
                result.AddRange(fromZone.OutgoingSets[fromZone.OutgoingSets.Length - 1]);
                fromZone.OutgoingSets[fromZone.OutgoingSets.Length - 1].Clear();
                fromZone.CurrentSet = GetDifference(fromZone.CurrentSet, result);
            }
            else
            {
                foreach (var set in fromZone.OutgoingSets)
                {
                    result.AddRange(ExtractIntersection(set, toZone.IncomingSet));
                }
                fromZone.CurrentSet = GetDifference(fromZone.CurrentSet, result);
                toZone.CurrentSet.AddRange(result);
            }

            LogResult(result, logType, logFormatUnit, logFormatSpell);
            return result;
        }

        private static void LogResult(List<string> result, LogType logType, string logFormatUnit, string logFormatSpell)
        {
            if (!string.IsNullOrEmpty(logFormatUnit))
            {
                foreach (var c in result)
                {
                    var card = CardLibrary.GetCard(c);
                    if (string.IsNullOrEmpty(logFormatSpell) || CardLibrary.GetCard(c).Type == "Unit")
                    {
                        Log.WriteLine(logType, logFormatUnit, card.Name);
                    }
                    else
                    {
                        Log.WriteLine(logType, logFormatSpell, card.Name);
                    }
                }
            }
        }
    }


    class PlayerOverlay
    {
        public OverlayZone HandZone;
        public OverlayZone FieldZone;
        public OverlayZone StageZone;
        public OverlayZone ZoomZone;
        public OverlayZone TossingZone;
        public OverlayZone CastZone;

        private bool IsInitialDraw = true;

        private LogType MyLogType;
        public PlayerOverlay(LogType logType)
        {
            MyLogType = logType;
            HandZone = new OverlayZone();

            // When dragging a unit, it may momentarily disappear from overlay
            // To account for this, we add a single frame of delay to Field zone
            FieldZone = new OverlayZone(3);
            StageZone = new OverlayZone();
            ZoomZone = new OverlayZone();
            TossingZone = new OverlayZone();
            CastZone = new OverlayZone(2);
        }

        public void Reset()
        {
            HandZone.Clear();
            FieldZone.Clear();
            StageZone.Clear();
            ZoomZone.Clear();
            TossingZone.Clear();
            CastZone.Clear();
            IsInitialDraw = true;
        }

        public void Update(List<OverlayElement> elements, 
            List<string> cardsDrawn,
            List<string> cardsPlayed,
            List<string> cardsTossed)
        {
            List<string> newHand = new List<string>();
            List<string> newStage = new List<string>();
            List<string> newZoom = new List<string>();
            List<string> newField = new List<string>();
            List<string> newTossing = new List<string>();
            List<string> newCast = new List<string>();
            int numProcessedElements = 0;

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
                //Log.WriteLine(LogType.DebugVerbose, "[--] {0} ({1},{2},{3},{4})", element.TheCard.Name, element.NormalizedBoundingBox.X, element.NormalizedBoundingBox.Y, element.NormalizedBoundingBox.Width, element.NormalizedBoundingBox.Height);

                if (element.NormalizedBoundingBox.Height == 0)
                {
                    newTossing.Add(element.CardCode);
                }
                else if (element.NormalizedBoundingBox.Height > 0.4f)
                {
                    newZoom.Add(element.CardCode);
                }
                else if (element.NormalizedBoundingBox.Height > 0.32f)
                {
                    newStage.Add(element.CardCode);
                }
                else if (element.NormalizedBoundingBox.Height > 0.20f && element.NormalizedBoundingBox.Bottom > 1.0f)
                {
                    newHand.Add(element.CardCode);
                }
                else if (element.NormalizedBoundingBox.Height < 0.125f)
                {
                    newCast.Add(element.CardCode);
                }
                else if (element.NormalizedBoundingBox.Height < 0.18f)
                {
                    newField.Add(element.CardCode);
                }
                else
                {
                    // Ignored. This may mean card is being dragged or highlighted off-screen
                }
                numProcessedElements++;
            }

            if (numProcessedElements == 0)
            {
                return;
            }


            HandZone.SetNextState(newHand);
            FieldZone.SetNextState(newField, MyLogType, "[FX] Removed from Battlefield: {0}", "[FX] Resolved: {0}");
            StageZone.SetNextState(newStage);
            ZoomZone.SetNextState(newZoom);
            TossingZone.SetNextState(newTossing);
            CastZone.SetNextState(newCast);

            ZoomZone.AcceptFromAnywhere(LogType.Debug, "[XZ] Drawing: {0}");
            var movedFromZoomToHand = ZoomZone.ReleaseTo(HandZone, MyLogType, "[ZH] Drawn: {0}");
            var movedFromCastToHand = CastZone.ReleaseTo(HandZone, MyLogType, "[CH] UNEXPECTED: {0}", "[CH] Spell cancelled: {0}");
            List<string> movedFromStageToHand = null;
            if (IsInitialDraw)
            {
                movedFromStageToHand = StageZone.ReleaseTo(HandZone, MyLogType, "[SH] Initial Draw: {0}");
            }
            else
            {
                movedFromStageToHand = new List<string>();
            }

            var movedToHand = HandZone.AcceptFromAnywhere(MyLogType, "[XH] Added to Hand: {0}");

            var movedFromStageToCast = CastZone.AcceptFrom(StageZone, MyLogType, "[SC] UNEXPECTED: {0}", "[SC] Casting: {0}");
            var movedFromHandToCast = CastZone.AcceptFrom(HandZone, MyLogType, "[HC] UNEXPECTED: {0}", "[HC] Casting: {0}");

            var movedFromCastToField = FieldZone.AcceptFrom(CastZone, MyLogType, "[CF] UNEXPECTED: {0}", "[CF] UNEXPECTED: {0}");
            var movedFromHandToField = FieldZone.AcceptFrom(HandZone, MyLogType, "[HF] Played: {0}", "[HF] Cast: {0}");
            var movedFromStageToField = FieldZone.AcceptFrom(StageZone, MyLogType, "[SF] Played: {0}", "[SF] Cast: {0}");
            var movedFromTossingToField = FieldZone.AcceptFrom(TossingZone, MyLogType, "[TF] Summoned: {0}");
            var movedToField = FieldZone.AcceptFromAnywhere(MyLogType, "[XF] Summoned: {0}", "[XF] Invoked: {0}");

            var movedFromHandToStage = StageZone.AcceptFrom(HandZone, MyLogType, "[HS] Playing: {0}", "[HS] Casting: {0}");
            var movedToStage = StageZone.AcceptFromAnywhere(LogType.Debug, "[XS] Presented: {0}");
            if (StageZone.CurrentSet.Count > 4)
            {
                // This is for the mulligan stage. Since there is a delay between cards
                // disappearing from stage and showing in hand, we only discard when we know
                // we have more than 4 cards
                StageZone.ReleaseToAnywhere(LogType.Debug, "[SX] Returned: {0}");
            }
            StageZone.CancelOutgoing();

            // We have a bug where cards disappear from hand when mouse is hovered over.
            // We work around this by preventing cards to leave hand except when we know where.
            // This breaks logic for cards that are discarded, but that's preferable to other problems
            HandZone.CancelOutgoing();

            var movedToCast = CastZone.AcceptFromAnywhere(LogType.Debug, "[XC] UNEXPECTED: {0}", "[XC] Casting: {0}");
            var removedFromCast = CastZone.ReleaseToAnywhere(LogType.Debug, "[CX] UNEXPECTED: {0}", "[CX] Resolved: {0}");

            if (cardsDrawn != null)
            {
                cardsDrawn.AddRange(movedFromZoomToHand);

                // Cancelled spells
                cardsDrawn.AddRange(movedFromCastToHand);

                if (IsInitialDraw)
                {
                    cardsDrawn.AddRange(movedFromStageToHand);
                    // Initial draw may occur when we are in the middle of the game.
                    // Therefore we one-time accept all card coming to hand
                    cardsDrawn.AddRange(movedToHand);
                }
            }
            if (cardsPlayed != null)
            {
                if (MyLogType == LogType.Player)
                {
                    // Played units go hand to stage
                    cardsPlayed.AddRange(movedFromHandToStage);

                    // Played spells go from hand to cast
                    cardsPlayed.AddRange(movedFromHandToCast);
                }
                else
                {
                    // Played units go from stage to field
                    cardsPlayed.AddRange(movedFromStageToField);
                    // Played spells go from cast to cast
                    cardsPlayed.AddRange(removedFromCast);
                }
            }

            TossingZone.ReleaseTo(ZoomZone, LogType.Debug, "[TZ] Not Tossed, Drawing: {0}");
            var movedFromTossing = TossingZone.ReleaseToAnywhere(MyLogType, "[TX] Tossed: {0}");
            TossingZone.AcceptFromAnywhere(LogType.Debug, "[XT] Being Tossed: {0}");

            // Currently not tracking tossed cards since reporting is buggy
            //if (cardsTossed != null)
            //{
            //    cardsTossed.AddRange(movedFromTossing);
            //}

            if (HandZone.CurrentSet.Count > 0 && IsInitialDraw)
            {
                IsInitialDraw = false;
                Log.WriteLine("==============");
            }

            //Log.WriteLine(LogType.DebugVerbose, "------------");

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
        private readonly bool ShouldTrackTossedCards = false;

        private readonly IOverlayUpdateCallback Callback;
        private readonly AutoUpdatingWebString WebString;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callback"></param>
        public Overlay(IOverlayUpdateCallback callback)
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

            WebString = new AutoUpdatingWebString(Constants.OverlayStateURL(), 66, this, 100);
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
                if (GameState == null) GameState = oldGameState;
                PlayerElements = new List<OverlayElement>();
                OpponentElements = new List<OverlayElement>();

                var screen = currentOverlay["Screen"].ToObject<Dictionary<string, JsonElement>>();
                ScreenWidth = screen["ScreenWidth"].GetInt32();
                ScreenHeight = screen["ScreenHeight"].GetInt32();

                // We normalize elements' bounding box based on screen height. However, if screen ratio becomes
                // too high, screen expands height-wise. To make sure we have same behavior as before,
                // We adjust the height accordingly.
                int normalizedScreenHeight = ScreenHeight;
                if (ScreenWidth * 0.66 < ScreenHeight)
                {
                    normalizedScreenHeight = (int)(0.5 + ScreenWidth * 0.66);
                }


                var rectangles = currentOverlay["Rectangles"].ToObject<Dictionary<string, JsonElement>[]>();
                foreach (var dict in rectangles)
                {
                    string cardCode = dict["CardCode"].GetString();
                    if (cardCode != "face")
                    {
                        if (dict["LocalPlayer"].GetBoolean())
                        {
                            PlayerElements.Add(new OverlayElement(dict, ScreenWidth, ScreenHeight, normalizedScreenHeight));
                        }
                        else
                        {
                            OpponentElements.Add(new OverlayElement(dict, ScreenWidth, ScreenHeight, normalizedScreenHeight));
                        }
                    }
                    int TopLeftX = dict["TopLeftX"].GetInt32();
                    int TopLeftY = dict["TopLeftY"].GetInt32();
                    int Width = dict["Width"].GetInt32();
                    int Height = dict["Height"].GetInt32();
                }

                Callback.OnElementsUpdate(PlayerElements, OpponentElements, ScreenWidth, ScreenHeight);
            }
            else
            {
                GameState = "Unknown";
            }

            if (oldGameState != GameState)
            {
                Callback.OnGameStateChanged(oldGameState, GameState);
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
                PlayerTracker.Update(PlayerElements, cardsDrawn, cardsPlayed, ShouldTrackTossedCards ? cardsTossed : null);
                OpponentTracker.Update(OpponentElements, null, opponentCardsPlayed, null);

                if (AddCardsToSet(PlayerDrawnCards, cardsDrawn))
                {
                    PlayerDrawnCards = PlayerDrawnCards.OrderBy(card => card.Cost).ThenBy(card => card.Name).ToList();
                    Callback.OnPlayerDrawnSetUpdated(Utilities.Clone(PlayerDrawnCards));
                }
                if (AddCardsToSet(PlayerPlayedCards, cardsPlayed))
                {
                    PlayerPlayedCards = PlayerPlayedCards.OrderBy(card => card.Cost).ThenBy(card => card.Name).ToList();
                    Callback.OnPlayerPlayedSetUpdated(Utilities.Clone(PlayerPlayedCards));
                }
                if (AddCardsToSet(PlayerTossedCards, cardsTossed))
                {
                    PlayerTossedCards = PlayerTossedCards.OrderBy(card => card.Cost).ThenBy(card => card.Name).ToList();
                    Callback.OnPlayerTossedSetUpdated(Utilities.Clone(PlayerTossedCards));
                }
                if (AddCardsToSet(OpponentPlayedCards, opponentCardsPlayed))
                {
                    OpponentPlayedCards = OpponentPlayedCards.OrderBy(card => card.Cost).ThenBy(card => card.Name).ToList();
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

            return cardCodes.Count > 0;
        }
    }
}
