#if DEBUG
#define USE_DECK_LISTS
#define ALLOW_GAME_RECORDING
#endif
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoRSideTracker
{
    /// <summary>
    /// Overlay update callback interface
    /// </summary>
    public interface ICardsInPlayCallback
    {
        /// <summary>
        /// Callback for when player deck set has been changed
        /// </summary>
        /// <param name="cards">Cards in the set</param>
        void OnPlayerDeckChanged(List<CardWithCount> cards);

        /// <summary>
        /// Callback for when player drawn and played set has been changed
        /// </summary>
        /// <param name="cards">Cards in the set</param>
        void OnPlayerDrawnAndPlayedChanged(List<CardWithCount> cards);

        /// <summary>
        /// Callback for when player graveyard has been changed
        /// </summary>
        /// <param name="cards">Cards in the set</param>
        void OnPlayerGraveyardChanged(List<CardWithCount> cards);

        /// <summary>
        /// Callback for when opponent graveyard has been changed
        /// </summary>
        /// <param name="cards">Cards in the set</param>
        void OnOpponentGraveyardChanged(List<CardWithCount> cards);

        /// <summary>
        /// Callback for when opponent graveyard has been changed
        /// </summary>
        /// <param name="cards">Cards in the set</param>
        void OnOpponentDeckChanged(List<CardWithCount> cards);

        /// <summary>
        /// Callback for when game state changes
        /// </summary>
        /// <param name="oldGameState">Previous game state</param>
        /// <param name="newGameState">New game state</param>
        void OnGameStateChanged(string oldGameState, string newGameState);

        /// <summary>
        /// Callback for when elements have been updated
        /// </summary>
        /// <param name="playerCards"></param>
        /// <param name="opponentCards"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        void OnElementsUpdate(CardList<CardInPlay> playerCards, CardList<CardInPlay> opponentCards, int screenWidth, int screenHeight);
    }

    /// <summary>
    /// Tracks all cards in play
    /// </summary>
    public class CardsInPlayWorker : AutoUpdatingWebStringCallback
    {
        /// <summary>Current Game State</summary>
        public string GameState { get; private set; } = "Unknown";
        /// <summary>Current Game Screen Width</summary>
        public int ScreenWidth { get; private set; }
        /// <summary>Current Game Screen Height</summary>
        public int ScreenHeight { get; private set; }

        /// <summary>Current Player Name</summary>
        public string PlayerName { get; private set; } = string.Empty;
        /// <summary>Current Opponent Name</summary>
        public string OpponentName { get; private set; } = string.Empty;

        static int NumZones = Enum.GetValues(typeof(PlayZone)).Length;
        CardList<CardInPlay>[] PlayerCards = new CardList<CardInPlay>[NumZones];
        CardList<CardInPlay>[] OpponentCards = new CardList<CardInPlay>[NumZones];

        private CardList<CardInPlay> FullPlayerDeck = new CardList<CardInPlay>();

        private bool IsInitialDraw;
        private Point LocalPlayerFace;
        private PlayerType AttackingPlayer = PlayerType.None;

        private readonly ICardsInPlayCallback Callback;
        private AutoUpdatingWebString WebString;
        private bool NotRespondingHasBeenReported = false;

#if USE_DECK_LISTS
        private CardsInPlayDebugView DeckLists;
#endif

        private int TimeCounter = 0;
        private bool InGame = false;

        private CardInPlayMoveLogger MoveLogger = new CardInPlayMoveLogger();

        private bool TestMode = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public CardsInPlayWorker(ICardsInPlayCallback callback)
        {
            for (int i = 0; i < NumZones; i++)
            {
                PlayerCards[i] = new CardList<CardInPlay>();
                OpponentCards[i] = new CardList<CardInPlay>();
            }
            IsInitialDraw = true;
            Callback = callback;

#if USE_DECK_LISTS
            DeckLists = new CardsInPlayDebugView();
            DeckLists.Show();
#endif
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~CardsInPlayWorker()
        {
            if (WebString == null)
            {
                WebString.Stop();
            }
        }

        /// <summary>
        /// Process game has started event
        /// </summary>
        public void GameStarted()
        {
            if (!InGame)
            {
                for (int i = 0; i < NumZones; i++)
                {
                    PlayerCards[i].Clear();
                    OpponentCards[i].Clear();
                }
                PlayerCards[(int)PlayZone.Deck] = FullPlayerDeck.Clone();
                InGame = true;
                Log.Clear();
                Log.WriteLine("New Game: {0} vs {1}", PlayerName, OpponentName);

#if ALLOW_GAME_RECORDING
                WebString.StartLog();
#endif
            }
        }

        /// <summary>
        /// Process game has ended event
        /// </summary>
        public void GameEnded()
        {
            if (InGame)
            {
                InGame = false;
                for (int i = 0; i < NumZones; i++)
                {
                    PlayerCards[i].Clear();
                    OpponentCards[i].Clear();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveGameLog(string filePath)
        {
            string codes = "";
            for (int i = 0; i < FullPlayerDeck.Count; i++)
            {
                codes += FullPlayerDeck[i].CardCode + " ";
            }
            List<string> gameLog = WebString.StopLog();
            gameLog.Insert(0, codes);
            File.WriteAllBytes(filePath, Utilities.ZipFromStringList(gameLog));
        }

        /// <summary>
        /// Set the local player full deck
        /// </summary>
        /// <param name="deck"></param>
        public void SetDeck(List<CardWithCount> deck)
        {
            if (WebString == null)
            {
                WebString = new AutoUpdatingWebString(Constants.OverlayStateURL(), 10, this, 100, true);
            }
            if (!InGame || FullPlayerDeck.Count == 0)
            {
                FullPlayerDeck.Clear();
                foreach (var card in deck)
                {
                    for (int i = 0; i < card.Count; i++)
                    {
                        FullPlayerDeck.Add(card.Cost, card.Name, new CardInPlay(PlayerType.LocalPlayer, card.TheCard, PlayZone.Deck, true));
                    }
                }
                for (int i = 0; i < NumZones; i++)
                {
                    PlayerCards[i].Clear();
                    OpponentCards[i].Clear();
                }
                PlayerCards[(int)PlayZone.Deck] = FullPlayerDeck.Clone();
                IsInitialDraw = true;
            }
        }

        /// <summary>
        /// Set the local player full deck
        /// </summary>
        /// <param name="gameLogFilePath"></param>
        public void SetTestDeck(string gameLogFilePath)
        {
            List<string> gameLog = Utilities.UnzipToStringList(File.ReadAllBytes(gameLogFilePath));
            TestMode = true;
            FullPlayerDeck.Clear();
            for (int i = 0; i < NumZones; i++)
            {
                PlayerCards[i].Clear();
                OpponentCards[i].Clear();
            }

            // Process the deck
            char[] charSeparators = new char[] { ' ' };
            var codes = gameLog[0].Split(charSeparators);
            foreach (var code in codes)
            {
                if (code.Length > 0)
                {
                    var card = CardLibrary.GetCard(code);
                    FullPlayerDeck.Add(card.Cost, card.Name, new CardInPlay(PlayerType.LocalPlayer, card, PlayZone.Deck, true));
                }
            }
            PlayerCards[(int)PlayZone.Deck] = FullPlayerDeck.Clone();
            IsInitialDraw = true;
            gameLog.RemoveAt(0);

            if (WebString != null)
            {
                WebString.Stop();
            }
            WebString = new AutoUpdatingWebString(gameLog, 10, this, 100);
        }

        /// <summary>
        /// Process newly updated web string to generate new overlay state
        /// </summary>
        /// <param name="newValue">new web string</param>
        /// <param name="timestamp">associated timestamp</param>
        public void OnWebStringUpdated(string newValue, double timestamp)
        {
            if (Utilities.IsJsonStringValid(newValue))
            {
                NotRespondingHasBeenReported = false;
                var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(newValue);
                UpdateGameState(dict);
                if (GameState == "InProgress")
                {
                    ProcessNext(dict, timestamp);
                }
            }
            else
            {
                GameState = "Unknown";
                if (!NotRespondingHasBeenReported)
                {
                    Log.WriteLine("{0} is producing invalid data", Constants.OverlayStateURL());
                    NotRespondingHasBeenReported = true;
                }
                GameEnded();
            }
        }

        private void UpdateGameState(Dictionary<string, JsonElement> currentOverlay)
        {
            CardList<CardInPlay> cards = new CardList<CardInPlay>();
            string oldGameState = GameState;

            if (currentOverlay != null)
            {
                GameState = currentOverlay["GameState"].GetString();
                if (GameState == null) GameState = oldGameState;
                else if (GameState == "InProgress")
                {
                    PlayerName = currentOverlay["PlayerName"].GetString();
                    OpponentName = currentOverlay["OpponentName"].GetString();
                }
            }
            else
            {
                GameState = "Unknown";
            }

            if (oldGameState != GameState)
            {
                Callback.OnGameStateChanged(oldGameState, GameState);

                if (GameState == "InProgress")
                {
                    GameStarted();
                }
                else if (oldGameState == "InProgress")
                {
                    GameEnded();
                    NotifyCardSetUpdates();
                }
            }
        }

        /// <summary>
        /// Process next rectangle layout
        /// </summary>
        /// <param name="overlay"></param>
        /// <param name="timestamp"></param>
        public void ProcessNext(Dictionary<string, JsonElement> overlay, double timestamp)
        {
            TimeCounter++;

            if (!TestMode && PlayerCards[(int)PlayZone.Deck].Count == 0 && PlayerCards[(int)PlayZone.Graveyard].Count == 0)
            {
                // Have not received the deck yet
                return;
            }
            CardList<CardInPlay> cardsInPlay = new CardList<CardInPlay>();

            bool isChampionUpgrading = false;
            if (overlay != null)
            {
                var screen = overlay["Screen"].ToObject<Dictionary<string, JsonElement>>();
                ScreenWidth = screen["ScreenWidth"].GetInt32();
                ScreenHeight = screen["ScreenHeight"].GetInt32();

                // We normalize elements' bounding box based on screen height. However, if screen ratio becomes
                // too high, screen expands height-wise. To make sure we have same behavior as before,
                // We adjust the height accordingly.
                int normalizedScreenHeight = ScreenHeight;
                if (ScreenWidth * 0.658 < ScreenHeight)
                {
                    normalizedScreenHeight = (int)(0.5 + ScreenWidth * 0.658);
                }

                Point correctionOffset = new Point(0, 0);
                var rectangles = overlay["Rectangles"].ToObject<Dictionary<string, JsonElement>[]>();
                foreach (var dict in rectangles)
                {
                    string cardCode = dict["CardCode"].GetString();
                    if (cardCode == "face")
                    {
                        if (dict["LocalPlayer"].GetBoolean())
                        {
                            int x = dict["TopLeftX"].GetInt32();
                            int y = dict["TopLeftY"].GetInt32();
                            if (IsInitialDraw)
                            {
                                LocalPlayerFace = new Point(x, y);
                            }
                            else
                            {
                                correctionOffset.X = LocalPlayerFace.X - x;
                                correctionOffset.Y = LocalPlayerFace.Y - y;
                            }
                        }

                        // We don't process face
                        continue;
                    }
                    Card card = CardLibrary.GetCard(cardCode);

                    // Also ignore abilities
                    if (card.Type != "Ability")
                    {
                        CardInPlay c = new CardInPlay(dict, ScreenWidth, ScreenHeight, correctionOffset, normalizedScreenHeight);
                        cardsInPlay.Add(card.Cost, card.Name, c);
                        if (c.CurrentZone == PlayZone.Hand && c.NormalizedCenter.Y > 1.3f)
                        {
                            // Champion is upgrading
                            isChampionUpgrading = true;
                        }
                    }
                }
            }

            // Split next elements between owners. Also, disregard cards with unknown zone
            CardList<CardInPlay> nextPlayerCards = new CardList<CardInPlay>();
            CardList<CardInPlay> nextOpponentCards = new CardList<CardInPlay>();
            cardsInPlay.Split(ref nextPlayerCards, ref nextOpponentCards, x => x.Owner == PlayerType.LocalPlayer && x.CurrentZone != PlayZone.Unknown);
            Callback.OnElementsUpdate(nextPlayerCards, nextOpponentCards, ScreenWidth, ScreenHeight);
            if (isChampionUpgrading)
            {
                // Bail, champion is upgrading
                return;
            }

            // Mark all opponent cards as "from deck" for now
            // This is because we cannot reliably know which ones are not from deck yet
            for (int i = 0; i < nextOpponentCards.Count; i++)
            {
                nextOpponentCards[i].IsFromDeck = true;
            }

            MoveToNext(ref PlayerCards, nextPlayerCards, timestamp, IsInitialDraw);
            if (IsInitialDraw)
            {
                // Initial draw until we add some cards to hand
                IsInitialDraw = (PlayerCards[(int)PlayZone.Hand].Count() == 0);
            }

            MoveToNext(ref OpponentCards, nextOpponentCards, timestamp, false);

            // Purge Ether of spells that have been cast
            bool thoroughCleanUp = false;
            int handCardIndex = PlayerCards[(int)PlayZone.Hand].FindIndex(x => x.CurrentZone == PlayZone.Hand);
            if (PlayerCards[(int)PlayZone.Hand].Count() > 0 && PlayerCards[(int)PlayZone.Hand][0].NormalizedBoundingBox.Height < 0.235f)
            {
                thoroughCleanUp = true;
            }
            PlayerCards[(int)PlayZone.Graveyard].AddRange(CleanUpEther(ref PlayerCards[(int)PlayZone.Ether], timestamp, thoroughCleanUp));
            OpponentCards[(int)PlayZone.Graveyard].AddRange(CleanUpEther(ref OpponentCards[(int)PlayZone.Ether], timestamp, thoroughCleanUp));

            NotifyCardSetUpdates();

            // Update attacking player
            int numPlayerAttackers = PlayerCards[(int)PlayZone.Battle].Count + PlayerCards[(int)PlayZone.Windup].Count + PlayerCards[(int)PlayZone.Attack].Count;
            int numOpponentAttackers = OpponentCards[(int)PlayZone.Battle].Count + OpponentCards[(int)PlayZone.Windup].Count + OpponentCards[(int)PlayZone.Attack].Count;
            if (AttackingPlayer == PlayerType.None)
            {
                if (numPlayerAttackers != numOpponentAttackers)
                {
                    AttackingPlayer = numPlayerAttackers > numOpponentAttackers ? PlayerType.LocalPlayer : PlayerType.Opponent;
                }
            }
            else if (numPlayerAttackers == 0 && numOpponentAttackers == 0)
            {
                AttackingPlayer = PlayerType.None;
            }
        }

        private void NotifyCardSetUpdates()
        {
#if USE_DECK_LISTS
            // Broadcast all changes to deck lists
            DeckLists.SetCards(PlayerCards, OpponentCards);
#endif

            // Log all moves
            MoveLogger.LogMoves(PlayerCards, OpponentCards, AttackingPlayer);

            // Send deck updates
            Callback.OnPlayerDeckChanged(GetDeck(PlayerCards[(int)PlayZone.Deck]));
            Callback.OnPlayerDrawnAndPlayedChanged(GetDeck(PlayerCards, (int)PlayZone.Deck));
            Callback.OnPlayerGraveyardChanged(GetDeck(PlayerCards[(int)PlayZone.Graveyard]));
            Callback.OnOpponentGraveyardChanged(GetDeck(OpponentCards[(int)PlayZone.Graveyard]));
            Callback.OnOpponentDeckChanged(GetDeck(OpponentCards, (int)PlayZone.Deck));
        }

        private List<CardWithCount> GetDeck(CardList<CardInPlay> current)
        {
            List<CardWithCount> deck = new List<CardWithCount>();
            int countFromDeck = 0;
            int countNotFromDeck = 0;
            for (int i = 0; i < current.Count; i++)
            {
                if (current[i].IsFromDeck)
                {
                    countFromDeck++;
                }
                else
                {
                    countNotFromDeck++;
                }
                if (i + 1 == current.Count || current[i].CardCode != current[i + 1].CardCode)
                {
                    if (countFromDeck > 0)
                    {
                        deck.Add(new CardWithCount(current[i].TheCard, countFromDeck, true));
                    }
                    if (countNotFromDeck > 0)
                    {
                        deck.Add(new CardWithCount(current[i].TheCard, countNotFromDeck, false));
                    }
                    countFromDeck = 0;
                    countNotFromDeck = 0;
                }
            }
            return deck;
        }

        private List<CardWithCount> GetDeck(CardList<CardInPlay>[] current, int excludeZone = -1)
        {
            CardList<CardInPlay> allCards = new CardList<CardInPlay>();
            for (int i = 0; i < NumZones; i++)
            {
                if (i != excludeZone)
                {
                    allCards.AddRange(current[i]);
                }
            }
            return GetDeck(allCards);
        }

        void MoveToNext(ref CardList<CardInPlay>[] current, CardList<CardInPlay> next, double timestamp, bool isInitialDraw)
        {
            CardList<CardInPlay> stationaryResult = new CardList<CardInPlay>();
            CardList<CardInPlay> movedResult = new CardList<CardInPlay>();

            // For each card in 'next', look for a card in 'current' that's in the same zone
            // If found, remove from both and add to stationaryCards set
            for (int i = 0; i < NumZones; i++)
            {
                stationaryResult.AddRange(CardList<CardInPlay>.Extract(ref next, ref current[i], (x, y) =>
                {
                    // Skip values in next that are incorrect zone
                    if ((int)x.CurrentZone != i) return -1;
                    int z = x.TheCard.Cost - y.TheCard.Cost;
                    if (z == 0) z = x.TheCard.Name.CompareTo(y.TheCard.Name);
                    return z;
                }, (x, y) => 
                {
                    y.BoundingBox = x.BoundingBox;
                    y.NormalizedBoundingBox = x.NormalizedBoundingBox;
                    y.NormalizedCenter = x.NormalizedCenter;
                    return y;
                }));
            }

            // For each card in next, look for a card in 'current' Ether zone that may have returned to the same zone
            movedResult.AddRange(CardList<CardInPlay>.Extract(ref next, ref current[(int)PlayZone.Ether], (x, y) =>
            {
                // Skip values in next that are incorrect zone
                if (y.CurrentZone != PlayZone.Ether) return -1;
                int z = x.TheCard.Cost - y.TheCard.Cost;
                if (z == 0) z = x.TheCard.Name.CompareTo(y.TheCard.Name);
                // Skip values in current that are incorrect zone
                if (z == 0 && x.CurrentZone != y.LastNonEtherZone) z = 1;
                return z;
            }, (x, y) =>
            {
                y.BoundingBox = x.BoundingBox;
                y.NormalizedBoundingBox = x.NormalizedBoundingBox;
                y.NormalizedCenter = x.NormalizedCenter;
                y.MoveToZone(x.CurrentZone, timestamp);
                return y;
            }));

            // For each card in next, look for a card in 'current' Ether zone that may has an approved transition
            movedResult.AddRange(CardList<CardInPlay>.Extract(ref next, ref current[(int)PlayZone.Ether], (x, y) =>
            {
                // Skip values in next that are incorrect zone
                if (y.CurrentZone != PlayZone.Ether) return -1;
                int z = x.TheCard.Cost - y.TheCard.Cost;
                if (z == 0) z = x.TheCard.Name.CompareTo(y.TheCard.Name);
                // Skip values in current that are incorrect zone
                if (z == 0 && GameBoard.TransitionResult.Proceed != GameBoard.TransitionAllowed(y.LastNonEtherZone, x.CurrentZone, isInitialDraw)) z = -1;
                return z;
            }, (x, y) =>
            {
                x.LastNonEtherZone = y.LastNonEtherZone;
                x.LastZone = PlayZone.Ether;
                x.IsFromDeck = y.IsFromDeck;
                return x;
            }));

            // For each card in next, look for approved transitions, skipping current in deck
            for (int i = 0; i < NumZones; i++)
            {
                if (i == (int) PlayZone.Deck)
                {
                    continue;
                }

                movedResult.AddRange(CardList<CardInPlay>.Extract(ref next, ref current[i], (x, y) =>
                {
                    // Skip values in next that are incorrect zone
                    int z = x.TheCard.Cost - y.TheCard.Cost;
                    if (z == 0) z = x.TheCard.Name.CompareTo(y.TheCard.Name);
                    if (z == 0 && GameBoard.TransitionResult.Proceed != GameBoard.TransitionAllowed(y.CurrentZone, x.CurrentZone, isInitialDraw)) z = -1;
                    return z;
                }, (x, y) =>
                {
                    x.SetLastZone(y.CurrentZone);
                    x.IsFromDeck = y.IsFromDeck;
                    return x;
                }));
            }

            // For each card in next, look for declined transitions
            for (int i = 0; i < NumZones; i++)
            {
                stationaryResult.AddRange(CardList<CardInPlay>.Extract(ref next, ref current[i], (x, y) =>
                {
                    // Skip values in next that are incorrect zone
                    int z = x.TheCard.Cost - y.TheCard.Cost;
                    if (z == 0) z = x.TheCard.Name.CompareTo(y.TheCard.Name);
                    if (z == 0 && GameBoard.TransitionResult.Stay != GameBoard.TransitionAllowed(y.CurrentZone, x.CurrentZone, isInitialDraw)) z = -1;
                    return z;
                }, (x, y) =>
                {
                    y.BoundingBox = x.BoundingBox;
                    y.NormalizedBoundingBox = x.NormalizedBoundingBox;
                    y.NormalizedCenter = x.NormalizedCenter;
                    return y;
                }));
            }

            // For each card in next, look for approved transitions from deck
            movedResult.AddRange(CardList<CardInPlay>.Extract(ref next, ref current[(int)PlayZone.Deck], (x, y) =>
            {
                // Skip values in next that are incorrect zone
                int z = x.TheCard.Cost - y.TheCard.Cost;
                if (z == 0) z = x.TheCard.Name.CompareTo(y.TheCard.Name);
                if (z == 0 && GameBoard.TransitionResult.Proceed != GameBoard.TransitionAllowed(y.CurrentZone, x.CurrentZone, isInitialDraw)) z = -1;
                return z;
            }, (x, y) =>
            {
                x.SetLastZone(y.CurrentZone);
                x.IsFromDeck = y.IsFromDeck;
                return x;
            }));

            // Move not in deck to ether
            for (int i = 0; i < NumZones; i++)
            {
                PlayZone newZone = PlayZone.Ether;
                if (i == (int)PlayZone.Deck || i == (int)PlayZone.Graveyard || i == (int)PlayZone.Ether)
                {
                    continue;
                }
                if (i == (int) PlayZone.Stage && isInitialDraw)
                {
                    newZone = PlayZone.Deck;
                }

                for (int j = 0; j < current[i].Count; j++)
                {
                    current[i][j].MoveToZone(newZone, timestamp);
                }
                movedResult.AddRange(current[i]);
                current[i].Clear();
            }

            // Remove cards in next that are in a zone that does not accept from Unknown
            next = next.GetSubset(x => GameBoard.TransitionResult.Proceed == GameBoard.TransitionAllowed(x.LastNonEtherZone, x.CurrentZone, isInitialDraw));

            // Add remaining cards to the moved set
            movedResult.AddRange(next);

            // Log all moves
            foreach (var card in movedResult)
            {
                LogMove(card, card.CurrentZone != PlayZone.Ether);
            }

            // Re-add all the cars to the zones
            for (int i = 0; i < NumZones; i++)
            {
                current[i].AddRange(stationaryResult.GetSubset(x => (int)x.CurrentZone == i));
                current[i].AddRange(movedResult.GetSubset(x => (int)x.CurrentZone == i));
            }
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

        private CardList<CardInPlay> CleanUpEther(ref CardList<CardInPlay> currentEther, double timestamp, bool doThoroughCleanUp = false)
        {
            CardList<CardInPlay> result = new CardList<CardInPlay>();

            // First, clean up all the spells that were cast
            result.AddRange(currentEther.ExtractSubset(x => x.LastZone == PlayZone.Cast && timestamp - x.EtherStartTime > 100));

            // Next, clean up all the units from hand or tossing that have been here even longer
            // Only do this if we are asked to do a thorough cleanup
            if (doThoroughCleanUp)
            {
                result.AddRange(currentEther.ExtractSubset(x => timestamp - x.EtherStartTime > 7000));
            }

            for (int i = 0; i < result.Count; i++)
            {
                result[i].MoveToZone(PlayZone.Graveyard, timestamp);
                LogMove(result[i], false);
            }
            return result;
        }
    }
}
