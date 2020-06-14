#define USE_DECK_LISTS
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoRSideTracker
{
    /// <summary>
    /// 
    /// </summary>
    public class CardsInPlayWorker
    {
        List<CardInPlay> PlayerDeck = new List<CardInPlay>();
        List<CardInPlay> PlayerCards = new List<CardInPlay>();
        List<CardInPlay> OpponentCards = new List<CardInPlay>();

        private List<CardInPlay> FullPlayerDeck = new List<CardInPlay>();

        private bool IsInitialDraw;

#if USE_DECK_LISTS
        private CardsInPlayDebugView DeckLists;
#endif

        private int TimeCounter = 0;
        private bool InGame = false;

        private CardInPlayMoveLogger MoveLogger = new CardInPlayMoveLogger();


        /// <summary>
        /// Constructor
        /// </summary>
        public CardsInPlayWorker()
        {
            IsInitialDraw = true;

#if USE_DECK_LISTS
            DeckLists = new CardsInPlayDebugView();
            DeckLists.Show();
#endif
        }

        /// <summary>
        /// Process game has started event
        /// </summary>
        public void GameStarted()
        {
            PlayerDeck = FullPlayerDeck.Clone();
            PlayerCards.Clear();
            OpponentCards.Clear();
            InGame = true;
        }

        /// <summary>
        /// Process game has ended event
        /// </summary>
        public void GameEnded()
        {
            InGame = false;
            PlayerDeck.Clear();
            PlayerCards.Clear();
            OpponentCards.Clear();
        }

        /// <summary>
        /// Set the local player full deck
        /// </summary>
        /// <param name="deck"></param>
        public void SetDeck(List<CardWithCount> deck)
        {
            if (!InGame || FullPlayerDeck.Count == 0)
            {
                FullPlayerDeck.Clear();
                foreach (var card in deck)
                {
                    for (int i = 0; i < card.Count; i++)
                    {
                        FullPlayerDeck.Add(new CardInPlay(PlayerType.LocalPlayer, card.TheCard, PlayZone.Deck));
                    }
                }
                PlayerDeck = FullPlayerDeck.Clone();
                PlayerCards.Clear();
                OpponentCards.Clear();
                IsInitialDraw = true;
            }
        }

        /// <summary>
        /// Process next rectangle layout
        /// </summary>
        /// <param name="overlay"></param>
        public void ProcessNext(Dictionary<string, JsonElement> overlay)
        {
            TimeCounter++;

            if (PlayerDeck.Count == 0 && PlayerCards.Count == 0)
            {
                // Have not received the deck yet
                return;
            }
            List<CardInPlay> cardsInPlay = new List<CardInPlay>();

            if (overlay != null)
            {
                var screen = overlay["Screen"].ToObject<Dictionary<string, JsonElement>>();
                int screenWidth = screen["ScreenWidth"].GetInt32();
                int screenHeight = screen["ScreenHeight"].GetInt32();

                // We normalize elements' bounding box based on screen height. However, if screen ratio becomes
                // too high, screen expands height-wise. To make sure we have same behavior as before,
                // We adjust the height accordingly.
                int normalizedScreenHeight = screenHeight;
                if (screenWidth * 0.658 < screenHeight)
                {
                    normalizedScreenHeight = (int)(0.5 + screenWidth * 0.658);
                }

                var rectangles = overlay["Rectangles"].ToObject<Dictionary<string, JsonElement>[]>();
                foreach (var dict in rectangles)
                {
                    string cardCode = dict["CardCode"].GetString();
                    if (cardCode == "face")
                    {
                        // ignore face
                        continue;
                    }
                    Card card = CardLibrary.GetCard(cardCode);

                    // Also ignore abilities
                    if (card.Type != "Ability")
                    {
                        cardsInPlay.Add(new CardInPlay(dict, screenWidth, screenHeight, normalizedScreenHeight));
                    }
                }
            }

            // Split next elements between owners. Also, disregard cards with unknown zone
            var nextPlayerCards = cardsInPlay.FindAll(x => x.Owner == PlayerType.LocalPlayer && x.CurrentZone != PlayZone.Unknown).ToList();
            var nextOpponentCards = cardsInPlay.FindAll(x => x.Owner == PlayerType.Opponent && x.CurrentZone != PlayZone.Unknown).ToList();

            var playerMovedCards = MoveToNext(ref PlayerCards, nextPlayerCards, ref PlayerDeck, IsInitialDraw);
            if (IsInitialDraw)
            {
                // Initial draw until we add some cards to hand
                IsInitialDraw = (PlayerCards.FindIndex(x => x.CurrentZone == PlayZone.Hand) == -1);
            }

            var emptyDeck = new List<CardInPlay>();
            var opponentMovedCards = MoveToNext(ref OpponentCards, nextOpponentCards, ref emptyDeck);

            // Purge Ether of spells that have been cast
            bool thoroughCleanUp = false;
            int handCardIndex = PlayerCards.FindIndex(x => x.CurrentZone == PlayZone.Hand);
            if (handCardIndex >= 0 && PlayerCards[handCardIndex].NormalizedBoundingBox.Height < 0.235f)
            {
                thoroughCleanUp = true;
            }
            playerMovedCards.AddRange(CleanUpEther(ref PlayerCards, thoroughCleanUp));
            opponentMovedCards.AddRange(CleanUpEther(ref OpponentCards, thoroughCleanUp));

            int numZones = Enum.GetValues(typeof(PlayZone)).Length;
            List<CardInPlay>[] playerZones = new List<CardInPlay>[numZones];
            List<CardInPlay>[] opponentZones = new List<CardInPlay>[numZones];
            for (int i = 0; i < numZones; i++)
            {
                playerZones[i] = PlayerCards.FindAll(x => x.CurrentZone == (PlayZone)i).ToList();
                opponentZones[i] = OpponentCards.FindAll(x => x.CurrentZone == (PlayZone)i).ToList();
            }
            playerZones[0] = PlayerDeck.Clone();

            // Broadcast all changes to deck windows
#if USE_DECK_LISTS
            DeckLists.SetCards(playerZones, opponentZones);
            /*int count = Enum.GetValues(typeof(PlayZone)).Length;
            DeckLists.SetCards(PlayerType.LocalPlayer, PlayZone.Deck, PlayerDeck);
            for (int i = 0; i < count; i++)
            {
                if (i != (int)PlayZone.Deck)
                {
                    var deck = PlayerCards.FindAll(x => x.CurrentZone == (PlayZone)i).OrderBy(card => card.TheCard.Cost).ThenBy(card => card.TheCard.Name);
                    DeckLists.SetCards(PlayerType.LocalPlayer, (PlayZone)i, deck);
                }
            }
            for (int i = 0; i < count; i++)
            {
                var deck = OpponentCards.FindAll(x => x.CurrentZone == (PlayZone)i).OrderBy(card => card.TheCard.Cost).ThenBy(card => card.TheCard.Name);
                DeckLists.SetCards(PlayerType.Opponent, (PlayZone)i, deck);
            }*/
#endif

            MoveLogger.LogMoves(playerZones, opponentZones);
        }

        private List<CardWithCount> GetDeck(List<CardInPlay> current, PlayZone playZone)
        {
            var cards = current.FindAll(x => x.CurrentZone == playZone).OrderBy(card => card.TheCard.Cost).ThenBy(card => card.TheCard.Name);
            List<CardWithCount> deck = new List<CardWithCount>();
            foreach (CardInPlay card in cards)
            {
                int index = deck.FindIndex(x => card.MatchesCode(x.Code));
                if (index >= 0)
                {
                    deck[index].Count++;
                }
                else
                {
                    deck.Add(new CardWithCount(card.TheCard, 1));
                }
            }
            return deck.OrderBy(card => card.Cost).ThenBy(card => card.Name).ToList();
        }

        List<CardInPlay> MoveToNext(ref List<CardInPlay> current, List<CardInPlay> next, ref List<CardInPlay> deck, bool isInitialDraw = false)
        {
            List<CardInPlay> result = new List<CardInPlay>();

            // For each card in 'next', look for a card in 'current' that's in the same zone
            // If found, remove from both and add to stationaryCards set
            var stationaryCards = RemoveStationary(ref current, ref next);

            // Another type of stationary card is one that went to ether and came back to the same zone
            var etherStationaryCards = RemoveEtherStationary(ref current, ref next);

            // We then check for cards that went to ether and came back in another zone with approved transition
            var etherTransitions = RemoveEtherTransitions(ref current, ref next, isInitialDraw);

            var movedCards = RemoveApprovedTransitions(ref current, ref next, isInitialDraw);
            var declinedCards = RemoveDeclinedTransitions(ref current, ref next, isInitialDraw);

            var addedFromDeck = RemoveApprovedTransitions(ref deck, ref next, isInitialDraw);

            // Move not in deck to ether
            for (int i = 0; i < current.Count; i++)
            {
                switch (current[i].CurrentZone)
                {
                    case PlayZone.Ether:
                    case PlayZone.Deck:
                    case PlayZone.Graveyard:
                        // Leave as is
                        continue;
                    case PlayZone.Stage:
                        if (!isInitialDraw)
                        {
                            goto case default;
                        }
                        // Put back to deck
                        current[i].MoveToZone(PlayZone.Deck);
                        result.Add(current[i]);
                        break;
                    default:
                        current[i].MoveToZone(PlayZone.Ether);
                        result.Add(current[i]);
                        break;
                }
                LogMove(current[i], false);
            }

            current.AddRange(stationaryCards);
            current.AddRange(declinedCards);

            foreach (var card in etherTransitions) LogMove(card, true);
            current.AddRange(etherStationaryCards);

            foreach (var card in addedFromDeck)
            {
                LogMove(card, true);
            }
            current.AddRange(addedFromDeck);

            foreach (var card in etherTransitions)
            {
                LogMove(card, true);
            }
            current.AddRange(etherTransitions);

            foreach (var card in movedCards)
            {
                if (card.LastZone != card.CurrentZone)
                {
                    LogMove(card, true);
                }
            }
            current.AddRange(movedCards);

            // Remove cards in next that are in a zone that does not accept from Unknown
            next = next.FindAll(x => TransitionResult.Proceed == TransitionAllowed(x.LastNonEtherZone, x.CurrentZone, isInitialDraw)).ToList();
            foreach (var card in next)
            {
                LogMove(card, true);
            }
            current.AddRange(next);

            result.AddRange(etherTransitions);
            result.AddRange(movedCards);
            result.AddRange(next);
            int num = current.FindAll(x => x.LastNonEtherZone == PlayZone.Unknown && x.CurrentZone == PlayZone.Attack).Count();
            return result;
        }

        private void LogMove(CardInPlay card, bool logPosition = false)
        {
            if (card.LastZone != card.CurrentZone)
            {
                if (logPosition)
                {
                    Log.WriteLine(LogType.DebugVerbose, "{0},{1},{2},{3},{4},{5},{6},{7:0.000},{8:0.000},{9:0.000},{10:0.000}", card.Owner.ToString()[0],
                        card.LastNonEtherZone.ToString()[0], card.LastZone.ToString()[0], card.CurrentZone.ToString()[0], card.TheCard.Name, card.CardCode, TimeCounter,
                         card.NormalizedCenter.X, card.NormalizedCenter.Y, card.NormalizedBoundingBox.Width, card.NormalizedBoundingBox.Height);
                }
                else
                {
                    Log.WriteLine(LogType.DebugVerbose, "{0},{1},{2},{3},{4},{5},{6}", card.Owner.ToString()[0],
                        card.LastNonEtherZone.ToString()[0], card.LastZone.ToString()[0], card.CurrentZone.ToString()[0], card.TheCard.Name, card.CardCode, TimeCounter);
                }
            }
        }

        private List<CardInPlay> CleanUpEther(ref List<CardInPlay> current, bool doThoroughCleanUp = false)
        {
            List<CardInPlay> result = new List<CardInPlay>();

            // First, clean up all the spells that were cast
            int index = -1;
            while (current.Count > 0 && index < current.Count)
            {
                index = current.FindIndex(index + 1, x => x.CurrentZone == PlayZone.Ether && x.LastZone == PlayZone.Cast);
                if (index == -1)
                {
                    break;
                }
                result.Add(current[index]);
                current.RemoveAt(index);
            }

            // Next, clean up all the units from hand or tossing that have been here even longer
            // Only do this if we are asked to do a thorough cleanup
            if (doThoroughCleanUp)
            {
                DateTime now = DateTime.Now;
                index = -1;
                while (current.Count > 0 && index < current.Count)
                {
                    index = current.FindIndex(index + 1, x => x.CurrentZone == PlayZone.Ether
                        && (now - x.EtherStartTime).TotalMilliseconds > 2000);
                    if (index == -1)
                    {
                        break;
                    }
                    result.Add(current[index]);
                    current.RemoveAt(index);
                }
            }

            for (int i = 0; i < result.Count; i++)
            {
                result[i].MoveToZone(PlayZone.Graveyard);
                LogMove(result[i], false);
            }
            current.AddRange(result);
            return result;
        }


        private List<CardInPlay> RemoveStationary(ref List<CardInPlay> current, ref List<CardInPlay> next)
        {
            List<CardInPlay> result = new List<CardInPlay>();
            for (int i = next.Count - 1; i >= 0; i--)
            {
                var code = next[i].CardCode;
                var zone = next[i].CurrentZone;
                int j = current.FindIndex(x => x.CurrentZone == zone && x.MatchesCode(code));
                if (j >= 0)
                {
                    current[j].BoundingBox = next[i].BoundingBox;
                    current[j].NormalizedBoundingBox = next[i].NormalizedBoundingBox;
                    current[j].NormalizedCenter = next[i].NormalizedCenter;

                    result.Add(current[j]);
                    current.RemoveAt(j);
                    next.RemoveAt(i);
                }
            }

            return result;
        }

        private List<CardInPlay> RemoveEtherStationary(ref List<CardInPlay> current, ref List<CardInPlay> next)
        {
            List<CardInPlay> result = new List<CardInPlay>();
            for (int i = next.Count - 1; i >= 0; i--)
            {
                var code = next[i].CardCode;
                var zone = next[i].CurrentZone;
                int j = current.FindIndex(x => x.CurrentZone == PlayZone.Ether && x.LastNonEtherZone == zone && x.MatchesCode(code));
                if (j >= 0)
                {
                    current[j].BoundingBox = next[i].BoundingBox;
                    current[j].NormalizedBoundingBox = next[i].NormalizedBoundingBox;
                    current[j].NormalizedCenter = next[i].NormalizedCenter;
                    current[j].MoveToZone(next[i].CurrentZone);
                    result.Add(current[j]);
                    current.RemoveAt(j);
                    next.RemoveAt(i);
                }
            }

            return result;
        }

        private List<CardInPlay> RemoveEtherTransitions(ref List<CardInPlay> current, ref List<CardInPlay> next, bool initialDraw)
        {
            List<CardInPlay> result = new List<CardInPlay>();
            for (int i = current.Count - 1; i >= 0; i--)
            {
                if (current[i].CurrentZone == PlayZone.Ether)
                {
                    var code = current[i].CardCode;
                    var associatedCardCodes = current[i].TheCard.AssociatedCardCodes;
                    var lastNonEtherZone = current[i].LastNonEtherZone;

                    // Firs find the exact cyclical match
                    int j = next.FindIndex(x => TransitionResult.Proceed == TransitionAllowed(lastNonEtherZone, x.CurrentZone, initialDraw)
                        && x.MatchesCode(code));

                    // Register the transition, if found
                    if (j >= 0)
                    {
                        next[j].LastNonEtherZone = current[i].LastNonEtherZone;
                        next[j].LastZone = PlayZone.Ether;
                        result.Add(next[j]);
                        current.RemoveAt(i);
                        next.RemoveAt(j);
                    }
                }
            }

            return result;
        }
        private List<CardInPlay> RemoveApprovedTransitions(ref List<CardInPlay> current, ref List<CardInPlay> next, bool isInitialDraw)
        {
            List<CardInPlay> result = new List<CardInPlay>();

            for (int i = next.Count - 1; i >= 0; i--)
            {
                var card = next[i];

                // Look for transitions
                int j = current.FindIndex(x => x.MatchesCode(card.CardCode)
                    && TransitionResult.Proceed == TransitionAllowed(x.CurrentZone, card.CurrentZone, isInitialDraw));
                if (j >= 0)
                {
                    next[i].SetLastZone(current[j].CurrentZone);
                    result.Add(next[i]);
                    current.RemoveAt(j);
                    next.RemoveAt(i);
                }
            }
            return result;
        }

        private List<CardInPlay> RemoveDeclinedTransitions(ref List<CardInPlay> current, ref List<CardInPlay> next, bool isInitialDraw)
        {
            List<CardInPlay> result = new List<CardInPlay>();

            for (int i = next.Count - 1; i >= 0; i--)
            {
                var card = next[i];

                // Look for transitions
                int j = current.FindIndex(x => x.MatchesCode(card.CardCode)
                    && TransitionResult.Stay == TransitionAllowed(x.CurrentZone, card.CurrentZone, isInitialDraw));
                if (j >= 0)
                {
                    current[j].BoundingBox = next[i].BoundingBox;
                    current[j].NormalizedBoundingBox = next[i].NormalizedBoundingBox;
                    current[j].NormalizedCenter = next[i].NormalizedCenter;
                    result.Add(current[j]);
                    current.RemoveAt(j);
                    next.RemoveAt(i);
                }
            }
            return result;
        }

        enum TransitionResult
        {
            Disallow,
            Proceed,
            Stay,
        }

        private TransitionResult TransitionAllowed(PlayZone from, PlayZone to, bool initialDraw)
        {
            // Only allow additions to deck from stage during initial draw
            if (to == PlayZone.Deck)
            {
                return (IsInitialDraw && from == PlayZone.Stage) ? TransitionResult.Proceed : TransitionResult.Disallow;
            }

            switch (from)
            {
                case PlayZone.Deck:
                    if (initialDraw)
                    {
                        return (to == PlayZone.Hand || to == PlayZone.Stage) ? TransitionResult.Proceed : TransitionResult.Disallow;
                    }
                    else
                    {
                        return (to == PlayZone.Zoom) ? TransitionResult.Proceed : TransitionResult.Disallow;
                    }
                case PlayZone.Tossing:
                    switch (to)
                    {
                        case PlayZone.Stage: // This is due to a bug
                        case PlayZone.Hand: // This is due to a bug
                        case PlayZone.Field: // This is due to a bug
                            return TransitionResult.Proceed;
                        default:
                            return TransitionResult.Disallow;
                    }
                case PlayZone.Zoom:
                    return (to == PlayZone.Hand) ? TransitionResult.Proceed : TransitionResult.Disallow;
                case PlayZone.Stage:
                    if (initialDraw)
                    {
                        // We only ever return to deck from initial draw
                        switch (to)
                        {
                            case PlayZone.Deck: return TransitionResult.Proceed;
                            default: return TransitionResult.Disallow;
                        }
                    }
                    else
                    {
                        switch (to)
                        {
                            case PlayZone.Field: return TransitionResult.Proceed;
                            case PlayZone.Hand: return TransitionResult.Proceed;
                            default: return TransitionResult.Disallow;
                        }
                    }
                case PlayZone.Hand:
                    switch (to)
                    {
                        case PlayZone.Field: return TransitionResult.Proceed;
                        case PlayZone.Cast: return TransitionResult.Proceed;
                        case PlayZone.Stage: return TransitionResult.Proceed;
                        default: return TransitionResult.Stay;
                    }
                case PlayZone.Cast:
                    return (to == PlayZone.Hand) ? TransitionResult.Proceed : TransitionResult.Disallow;
                case PlayZone.Field:
                    switch (to)
                    {
                        case PlayZone.Hand: return TransitionResult.Proceed; // For recall mechanics
                        case PlayZone.Battle: return TransitionResult.Proceed;
                        case PlayZone.Attack: return TransitionResult.Stay;
                        case PlayZone.Windup: return TransitionResult.Stay;
                        case PlayZone.Tossing: return TransitionResult.Proceed; // For spell summoning, likely a bug
                        default: return TransitionResult.Disallow;
                    }
                case PlayZone.Battle:
                    switch (to)
                    {
                        case PlayZone.Hand: return TransitionResult.Proceed; // For recall mechanics
                        case PlayZone.Field: return TransitionResult.Proceed;
                        case PlayZone.Windup: return TransitionResult.Proceed;
                        case PlayZone.Attack: return TransitionResult.Stay;
                        default: return TransitionResult.Disallow;
                    }
                case PlayZone.Windup:
                    switch (to)
                    {
                        case PlayZone.Field: return TransitionResult.Proceed;
                        case PlayZone.Battle: return TransitionResult.Stay;
                        case PlayZone.Attack: return TransitionResult.Proceed;
                        default: return TransitionResult.Disallow;
                    }
                case PlayZone.Attack:
                    switch (to)
                    {
                        case PlayZone.Field: return TransitionResult.Proceed;
                        case PlayZone.Battle: return TransitionResult.Stay;
                        case PlayZone.Windup: return TransitionResult.Stay;
                        default: return TransitionResult.Disallow;
                    }
                case PlayZone.Graveyard:
                    return TransitionResult.Disallow;
                case PlayZone.Unknown:
                    // We only allow cetain zones to accept cards from unknown
                    switch (to)
                    {
                        case PlayZone.Field:
                        case PlayZone.Battle:
                        case PlayZone.Stage:
                        case PlayZone.Cast:
                        case PlayZone.Tossing:
                        case PlayZone.Hand:
                            return TransitionResult.Proceed;
                        default: return TransitionResult.Disallow;
                    }
                case PlayZone.Ether:
                    return TransitionResult.Disallow;
            }
            return TransitionResult.Disallow;
        }
    }
}
