using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoRSideTracker
{
    /// <summary>
    /// Main app wondow
    /// </summary>
    public partial class MainWindow : Form, ExpeditionUpdateCallback, StaticDeckUpdateCallback, OverlayUpdateCallback
    {
        private int CurrentDownloadIndex = 0;
        private List<ValueTuple<int,long>> MissingSets;

        private Expedition CurrentExpedition;
        private StaticDeck CurrentDeck;
        private Overlay CurrentOverlay;

        private DeckWindow PlayerActiveDeckWindow;
        private DeckWindow PlayerDrawnCardsWindow;
        private DeckWindow PlayerPlayedCardsWindow;
        private DeckWindow OpponentPlayedCardsWindow;

        private GameHistoryWindow GameHistory;
        private LogWindow ActiveLogWindow;
        private int ExpeditionsCount = 0;

        private GameRecord CurrentGameRecord = new GameRecord();
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            if (!Directory.Exists(Constants.GetLocalGamesPath()))
            {
                Directory.CreateDirectory(Constants.GetLocalGamesPath());
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MainWindow()
        {
            Log.SetLogWindow(null);
        }

        /// <summary>
        /// Check if two decks are equal
        /// </summary>
        /// <param name="cardsA">First deck</param>
        /// <param name="cardsB">Second deck</param>
        /// <returns>true if decks are exactly the same</returns>
        private bool AreDecksEqual(List<CardWithCount> cardsA, List<CardWithCount> cardsB)
        {
            if (cardsA.Count != cardsB.Count) return false;
            for (int i = 0; i < cardsA.Count; i++)
            {
                if (cardsA[i].Code != cardsB[i].Code || cardsA[i].Count != cardsB[i].Count)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Receives notification that static deck was updated
        /// </summary>
        /// <param name="cards">Updated set</param>
        /// <param name="deckCode">Associated Deck code</param>
        public void OnDeckUpdated(List<CardWithCount> cards, string deckCode)
        {
            if (cards.Count > 0 &&
                (CurrentExpedition == null || !AreDecksEqual(cards, CurrentExpedition.Cards)))
            {
                string title = "Constructed Deck";
                PlayerActiveDeckWindow.Title = string.Format(title);
                PlayerActiveDeckWindow.SetFullDeck(cards);

                CurrentGameRecord.MyDeck = Utilities.Clone(cards);
                CurrentGameRecord.MyDeckName = title;
                CurrentGameRecord.MyDeckCode = deckCode;
                CurrentGameRecord.Notes = "";
                CurrentGameRecord.Result = "-";
                CurrentGameRecord.ExpeditionSignature = "";
            }
            else
            {
                // No active static deck -- show expedition deck or no deck
                OnExpeditionDeckUpdated((CurrentExpedition != null) ? CurrentExpedition.Cards : new List<CardWithCount>());
            }
        }

        /// <summary>
        /// Receives notification that expedition deck was updated
        /// </summary>
        /// <param name="cards">Updated set</param>
        public void OnExpeditionDeckUpdated(List<CardWithCount> cards)
        {
            if (CurrentDeck.Cards.Count == 0 || AreDecksEqual(CurrentDeck.Cards, cards))
            {
                if (cards.Count > 0)
                {
                    bool isEliminationGame = (Array.FindLastIndex(CurrentExpedition.Record, item => item.Equals("win")) < Array.FindLastIndex(CurrentExpedition.Record, item => item.Equals("loss")))
                        || (CurrentExpedition.NumberOfWins == 6);
                    string title = string.Format("Expedition {0}-{1}{2}", CurrentExpedition.NumberOfWins, CurrentExpedition.NumberOfLosses, isEliminationGame ? "*" : "");
                    PlayerActiveDeckWindow.Title = title;
                    PlayerActiveDeckWindow.SetFullDeck(Utilities.Clone(cards));

                    CurrentGameRecord.MyDeck = Utilities.Clone(cards);
                    CurrentGameRecord.MyDeckName = title;
                    CurrentGameRecord.MyDeckCode = "";
                    CurrentGameRecord.Notes = "";
                    CurrentGameRecord.Result = "-";
                    CurrentGameRecord.ExpeditionSignature = CurrentExpedition.GetSignature();
                }
                else
                {
                    string title = "No Active Deck";
                    PlayerActiveDeckWindow.Title = title;
                    PlayerActiveDeckWindow.SetFullDeck(new List<CardWithCount>());

                    // Don't set game record here due to a race condition with saving
                    //CurrentGameRecord.MyDeck = new List<CardWithCount>();
                    //CurrentGameRecord.Notes = title;
                    //CurrentGameRecord.Result = "-";
                }
            }
        }

        /// <summary>
        /// Receives notification that player drawn set was changed
        /// </summary>
        /// <param name="cards">Updated set</param>
        public void OnPlayerDrawnSetUpdated(List<CardWithCount> cards)
        {
            PlayerActiveDeckWindow.SetDrawnCards(Utilities.Clone(cards));
            PlayerDrawnCardsWindow.SetFullDeck(Utilities.Clone(cards));
        }

        /// <summary>
        /// Receives notification that player played set was changed
        /// </summary>
        /// <param name="cards">Updated set</param>
        public void OnPlayerPlayedSetUpdated(List<CardWithCount> cards)
        {
            PlayerPlayedCardsWindow.SetFullDeck(Utilities.Clone(cards));
        }

        /// <summary>
        /// Receives notification that player played set was changed
        /// </summary>
        /// <param name="cards">Updated set</param>
        public void OnPlayerTossedSetUpdated(List<CardWithCount> cards)
        {
            PlayerActiveDeckWindow.SetTossedCards(Utilities.Clone(cards));
        }

        /// <summary>
        /// Receives notification that opponent played set was changed
        /// </summary>
        /// <param name="cards">Updated set</param>
        public void OnOpponentPlayedSetUpdated(List<CardWithCount> cards)
        {
            OpponentPlayedCardsWindow.SetFullDeck(Utilities.Clone(cards));

            CurrentGameRecord.OpponentDeck = Utilities.Clone(cards);
            CurrentGameRecord.OpponentName = CurrentOverlay.OpponentName;
        }

        /// <summary>
        /// Receives notification that game state was changed
        /// </summary>
        /// <param name="oldGameState"></param>
        /// <param name="newGameState"></param>
        public void OnGameStateChanged(string oldGameState, string newGameState)
        {
            Log.WriteLine("Game state changed from {0} to {1}", oldGameState, newGameState);
            if (oldGameState == "InProgress")
            {
                GameRecord gameRecord = (GameRecord)CurrentGameRecord.Clone();
                // Game ended. Grab game result
                string json = Utilities.GetStringFromURL(Constants.GameResultURL());
                if (json != null && Utilities.IsJsonStringValid(json))
                {
                    Dictionary<string, JsonElement> gameResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                    bool localPlayerWon = gameResult["LocalPlayerWon"].ToObject<bool>();
                    gameRecord.Result = localPlayerWon ? "Win" : "Loss";
                    Log.WriteLine("Game no. {0} Result: {1}", gameResult["GameID"].ToObject<int>(), gameRecord.Result);
                    if (gameRecord.MyDeckName.StartsWith("Expedition"))
                    {
                        // Extract record before the game
                        string record = gameRecord.MyDeckName.Substring(11);
                        bool isEliminationGame = false;
                        if (record.EndsWith("*"))
                        {
                            // Elimination game
                            isEliminationGame = true;
                            record = record.Remove(record.Length - 1);
                        }
                        string[] counts = record.Split('-');
                        int wins = int.Parse(counts[0]) + (localPlayerWon ? 1 : 0);
                        int losses = int.Parse(counts[1]) + (localPlayerWon ? 0 : 1);
                        gameRecord.MyDeckName = string.Format("Expedition {0}-{1}{2}", wins, losses,
                            (isEliminationGame && !localPlayerWon) ? "*" : "");
                    }
                }
                else
                {
                    gameRecord.Result = "unknown";
                }

                // Save game record to file
                gameRecord.Timestamp = DateTime.Now;
                gameRecord.Log = Log.CurrentLogRtf;
                string filePath = string.Format(@"{0}\{1}_{2}_{3}_{4}_{5}_{6}.txt", 
                    Constants.GetLocalGamesPath(),
                    gameRecord.Timestamp.Year,
                    gameRecord.Timestamp.Month,
                    gameRecord.Timestamp.Day,
                    gameRecord.Timestamp.Hour,
                    gameRecord.Timestamp.Minute,
                    gameRecord.Timestamp.Second);
                gameRecord.SaveToFile(filePath);
                GameHistory.AddGameRecord(gameRecord);
                Utilities.CallActionSafelyAndWait(DecksListBox, new Action (() => 
                {
                    int index = AddToDeckList(gameRecord);
                    DecksListBox.SetSelected(index, true);
                }));
            }
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            Rectangle progressRect = MyProgressDisplay.Bounds;
            progressRect.Offset(
                ClientRectangle.Width / 2 - (progressRect.Left + progressRect.Right) / 2,
                ClientRectangle.Height / 2 - (progressRect.Top + progressRect.Bottom) / 2);
            MyProgressDisplay.SetBounds(progressRect.X, progressRect.Y, progressRect.Width, progressRect.Height);
            MyProgressDisplay.Show();

            MissingSets = CardLibrary.FindMissingSets();
            if (MissingSets.Count > 0)
            {
                long totalDownloadSize = MissingSets.Sum(x => x.Item2);
                var result = MessageBox.Show(
                    string.Format("Card sets have been updated. Download size is {0} MB. Download new sets?", totalDownloadSize / 1024 / 1024), 
                    "Sets Out of Date", 
                    MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes)
                {
                    Close();
                    return;
                }

                // Delete existing outdated sets
                foreach (var set in MissingSets)
                {
                    if (Directory.Exists(Constants.GetSetPath(set.Item1)))
                    {
                        Directory.Delete(Constants.GetSetPath(set.Item1), true);
                    }
                }

                CurrentDownloadIndex = 0;
                CardLibrary.DownloadSet(MissingSets[CurrentDownloadIndex].Item1, 
                    new DownloadProgressChangedEventHandler(OnDownloadProgressChanged),
                    new AsyncCompletedEventHandler(OnDownloadFileCompleted));
            }
            else
            {
                OnAllSetsDownloaded();
            }
        }

        /// <summary>
        /// Receives notification that set download progress changed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            string message = String.Format("Downloading card set {0} ({1}/{2})", MissingSets[CurrentDownloadIndex].Item1, CurrentDownloadIndex + 1, MissingSets.Count);
            MyProgressDisplay.Update(message, percentage);
        }

        /// <summary>
        /// Receives notification that set download was completed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e != null && e.Error != null && e.Error.HResult != 0)
            {
                string localFile = Constants.GetSetZipPath(MissingSets[CurrentDownloadIndex].Item1);
                if (File.Exists(localFile))
                {
                    File.Delete(localFile);
                }

                // Error occured, finish up
                OnAllSetsDownloaded();
            }
            else
            {
                // Success, finish up and queue up the next one

                MyProgressDisplay.Update("Download completed. Processing...", 100);
                CardLibrary.ProcessDownloadedSet(MissingSets[CurrentDownloadIndex].Item1, MissingSets[CurrentDownloadIndex].Item2);
                CurrentDownloadIndex++;
                if (CurrentDownloadIndex < MissingSets.Count)
                {
                    CardLibrary.DownloadSet(MissingSets[CurrentDownloadIndex].Item1,
                        new DownloadProgressChangedEventHandler(OnDownloadProgressChanged),
                        new AsyncCompletedEventHandler(OnDownloadFileCompleted));
                }
                else
                {
                    // Finished
                    OnAllSetsDownloaded();
                }
            }
        }

        /// <summary>
        /// Runs after all sets are downloaded and processed.
        /// Initializes the StaticDeck windows and deck tracking objects.
        /// </summary>
        private async void OnAllSetsDownloaded()
        {
            await Task.Run(() => CardLibrary.LoadAllCards(MyProgressDisplay));

            MyProgressDisplay.Hide();

            PlayerActiveDeckWindow = new DeckWindow();
            PlayerActiveDeckWindow.CreateControl();
            PlayerActiveDeckWindow.Title = "No Active Deck";
            PlayerActiveDeckWindow.ShouldShowDeckStats = Properties.Settings.Default.ShowDeckStats;
            if (Properties.Settings.Default.ShowPlayerDeck) PlayerActiveDeckWindow.Show();

            PlayerDrawnCardsWindow = new DeckWindow();
            PlayerDrawnCardsWindow.CreateControl();
            PlayerDrawnCardsWindow.Title = "Cards Drawn";
            PlayerDrawnCardsWindow.ShouldShowDeckStats = Properties.Settings.Default.ShowDeckStats;
            if (Properties.Settings.Default.ShowPlayerDrawnCards) PlayerDrawnCardsWindow.Show();

            PlayerPlayedCardsWindow = new DeckWindow();
            PlayerPlayedCardsWindow.CreateControl();
            PlayerPlayedCardsWindow.Title = "You Played";
            PlayerPlayedCardsWindow.ShouldShowDeckStats = Properties.Settings.Default.ShowDeckStats;
            if (Properties.Settings.Default.ShowPlayerPlayedCards) PlayerPlayedCardsWindow.Show();

            OpponentPlayedCardsWindow = new DeckWindow();
            OpponentPlayedCardsWindow.CreateControl();
            OpponentPlayedCardsWindow.Title = "Opponent Played";
            OpponentPlayedCardsWindow.ShouldShowDeckStats = Properties.Settings.Default.ShowDeckStats;
            if (Properties.Settings.Default.ShowOpponentPlayedCards) OpponentPlayedCardsWindow.Show();

            PlayerActiveDeckWindow.SetBounds(Properties.Settings.Default.PlayerDeckLocation.X, Properties.Settings.Default.PlayerDeckLocation.Y, 0, 0, BoundsSpecified.Location);
            PlayerDrawnCardsWindow.SetBounds(Properties.Settings.Default.PlayerDrawnCardsLocation.X, Properties.Settings.Default.PlayerDrawnCardsLocation.Y, 0, 0, BoundsSpecified.Location);
            PlayerPlayedCardsWindow.SetBounds(Properties.Settings.Default.PlayerPlayedCardsLocation.X, Properties.Settings.Default.PlayerPlayedCardsLocation.Y, 0, 0, BoundsSpecified.Location);
            OpponentPlayedCardsWindow.SetBounds(Properties.Settings.Default.OpponentPlayedCardsLocation.X, Properties.Settings.Default.OpponentPlayedCardsLocation.Y, 0, 0, BoundsSpecified.Location);

            double deckOpacity = Properties.Settings.Default.DeckTransparency / 100.0;
            PlayerActiveDeckWindow.Opacity = deckOpacity;
            PlayerDrawnCardsWindow.Opacity = deckOpacity;
            PlayerPlayedCardsWindow.Opacity = deckOpacity;
            OpponentPlayedCardsWindow.Opacity = deckOpacity;

            PlayerActiveDeckWindow.HideZeroCountCards = Properties.Settings.Default.HideZeroCountInDeck;

            //Utilities.CallActionSafelyAndWait(this, new Action(() => { GameHistory.CreateControl(); }));
            GameHistory = new GameHistoryWindow();
            GameHistory.CreateControl();
            if (Properties.Settings.Default.GameHistoryWindowBounds.Width > 0)
            {
                GameHistory.StartPosition = FormStartPosition.Manual;
                GameHistory.SetBounds(
                    Properties.Settings.Default.GameHistoryWindowBounds.X,
                    Properties.Settings.Default.GameHistoryWindowBounds.Y,
                    Properties.Settings.Default.GameHistoryWindowBounds.Width,
                    Properties.Settings.Default.GameHistoryWindowBounds.Height,
                    BoundsSpecified.All);
            }

            ActiveLogWindow = new LogWindow();
            ActiveLogWindow.CreateControl();
            if (Properties.Settings.Default.ActiveLogWindowBounds.Width > 0)
            {
                ActiveLogWindow.StartPosition = FormStartPosition.Manual;
                ActiveLogWindow.SetBounds(
                    Properties.Settings.Default.ActiveLogWindowBounds.X,
                    Properties.Settings.Default.ActiveLogWindowBounds.Y,
                    Properties.Settings.Default.ActiveLogWindowBounds.Width,
                    Properties.Settings.Default.ActiveLogWindowBounds.Height,
                    BoundsSpecified.All);
            }
            ActiveLogWindow.Show();
            ActiveLogWindow.Hide();
            Log.SetLogWindow(ActiveLogWindow);

            CurrentDeck = new StaticDeck(this);
            Thread.Sleep(500);
            CurrentExpedition = new Expedition(this);
            Thread.Sleep(500);
            CurrentOverlay = new Overlay(this);

            SnapWindowsButton.Visible = true;
            ShowHistoryButton.Visible = true;
            OptionsButton.Visible = true;
            LogButton.Visible = true;
            DecksListBox.Visible = true;
            DeckPanel.Visible = true;
            DecksLabel.Visible = true;

            // Load all games
            if (Directory.Exists(Constants.GetLocalGamesPath()))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Constants.GetLocalGamesPath());
                FileInfo[] files = dirInfo.GetFiles();

                foreach (FileInfo fi in files.OrderBy(x => x.CreationTime))
                {
                    try
                    {
                        GameRecord gr = GameRecord.LoadFromFile(fi.FullName);
                        AddToDeckList(gr);
                    }
                    catch
                    {
                        // Skip bad records
                    }
                }

                if (DecksListBox.Items.Count > 0)
                {
                    DecksListBox.SelectedIndex = 0;
                }
            }
        }

        private int AddToDeckList(GameRecord gr)
        {
            string grSig = gr.GetDeckSignature();
            int index = -1;
            string deckName = gr.IsExpedition() ? string.Format("Expedition #{0}", ExpeditionsCount + 1) : gr.MyDeckName;
            for (int i = 0; i < DecksListBox.Items.Count; i++)
            {
                GameRecord gr2 = (GameRecord)DecksListBox.Items[i];
                if (grSig == gr2.GetDeckSignature())
                {
                    deckName = gr2.DisplayString;
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                DecksListBox.Items.RemoveAt(index);
            }
            else if (gr.IsExpedition())
            {
                ExpeditionsCount++;
            }

            gr.DisplayString = deckName;
            DecksListBox.Items.Insert(0, gr);
            index = 0;

            return index;
        }

        private void SnapWindowsButton_Click(object sender, EventArgs e)
        {
            Point location;
            if (PlayerActiveDeckWindow.Visible)
            {
                location = PlayerActiveDeckWindow.Location;
            }
            else if (PlayerDrawnCardsWindow.Visible)
            {
                location = PlayerDrawnCardsWindow.Location;
            }
            else if (PlayerPlayedCardsWindow.Visible)
            {
                location = PlayerPlayedCardsWindow.Location;
            }
            else if (OpponentPlayedCardsWindow.Visible)
            {
                location = OpponentPlayedCardsWindow.Location;
            }
            else
            {
                return;
            }

            int margin = 2;
            if (PlayerActiveDeckWindow.Visible)
            {
                PlayerActiveDeckWindow.SetDesktopBounds(location.X, location.Y, PlayerActiveDeckWindow.DesktopBounds.Width, PlayerActiveDeckWindow.DesktopBounds.Height);
                location.X += PlayerActiveDeckWindow.DesktopBounds.Width + margin;
            }
            if (PlayerDrawnCardsWindow.Visible)
            {
                PlayerDrawnCardsWindow.SetDesktopBounds(location.X, location.Y, PlayerDrawnCardsWindow.DesktopBounds.Width, PlayerDrawnCardsWindow.DesktopBounds.Height);
                location.X += PlayerDrawnCardsWindow.DesktopBounds.Width + margin;
            }
            if (PlayerPlayedCardsWindow.Visible)
            {
                PlayerPlayedCardsWindow.SetDesktopBounds(location.X, location.Y, PlayerPlayedCardsWindow.DesktopBounds.Width, PlayerPlayedCardsWindow.DesktopBounds.Height);
                location.X += PlayerPlayedCardsWindow.DesktopBounds.Width + margin;
            }
            if (OpponentPlayedCardsWindow.Visible)
            {
                OpponentPlayedCardsWindow.SetDesktopBounds(location.X, location.Y, OpponentPlayedCardsWindow.DesktopBounds.Width, OpponentPlayedCardsWindow.DesktopBounds.Height);
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.MainWindowBounds.Width > 0 && Properties.Settings.Default.MainWindowBounds.Height > 0)
            {
                this.WindowState = Properties.Settings.Default.MainWindowState;
                this.Bounds = Properties.Settings.Default.MainWindowBounds;
            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.MainWindowState = this.WindowState;
            if (this.WindowState == FormWindowState.Normal)
            {
                // save location and size if the state is normal
                Properties.Settings.Default.MainWindowBounds = Bounds;
            }
            else
            {
                // save the RestoreBounds if the form is minimized or maximized!
                Properties.Settings.Default.MainWindowBounds = this.RestoreBounds;
            }

            if (GameHistory.WindowState == FormWindowState.Normal)
            {
                // save location and size if the state is normal
                Properties.Settings.Default.GameHistoryWindowBounds = GameHistory.Bounds;
            }
            else
            {
                // save the RestoreBounds if the form is minimized or maximized!
                Properties.Settings.Default.GameHistoryWindowBounds = GameHistory.RestoreBounds;
            }

            if (ActiveLogWindow.WindowState == FormWindowState.Normal)
            {
                // save location and size if the state is normal
                Properties.Settings.Default.ActiveLogWindowBounds = ActiveLogWindow.Bounds;
            }
            else
            {
                // save the RestoreBounds if the form is minimized or maximized!
                Properties.Settings.Default.ActiveLogWindowBounds = ActiveLogWindow.RestoreBounds;
            }
            if (PlayerActiveDeckWindow != null) Properties.Settings.Default.PlayerDeckLocation = PlayerActiveDeckWindow.Location;
            if (PlayerDrawnCardsWindow != null) Properties.Settings.Default.PlayerDrawnCardsLocation = PlayerDrawnCardsWindow.Location;
            if (PlayerPlayedCardsWindow != null) Properties.Settings.Default.PlayerPlayedCardsLocation = PlayerPlayedCardsWindow.Location;
            if (OpponentPlayedCardsWindow != null) Properties.Settings.Default.OpponentPlayedCardsLocation = OpponentPlayedCardsWindow.Location;

            // Save the settings
            Properties.Settings.Default.Save();
        }

        private void ShowHistoryButton_Click(object sender, EventArgs e)
        {
            if (GameHistory != null)
            {
                if (GameHistory.Visible)
                {
                    GameHistory.Hide();
                }
                else
                {
                    GameHistory.Show();
                }
            }
        }

        private void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            Rectangle progressRect = MyProgressDisplay.Bounds;
            progressRect.Offset(
                ClientRectangle.Width / 2 - (progressRect.Left + progressRect.Right) / 2,
                ClientRectangle.Height / 2 - (progressRect.Top + progressRect.Bottom) / 2);
            MyProgressDisplay.SetBounds(progressRect.X, progressRect.Y, progressRect.Width, progressRect.Height);
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            OptionsWindow myOptions = new OptionsWindow();
            myOptions.SetDeckWindows(PlayerActiveDeckWindow, PlayerDrawnCardsWindow, PlayerPlayedCardsWindow, OpponentPlayedCardsWindow);
            myOptions.ShowDialog();
        }

        private void LogButton_Click(object sender, EventArgs e)
        {
            if (ActiveLogWindow != null)
            {
                if (ActiveLogWindow.Visible)
                {
                    ActiveLogWindow.Hide();
                }
                else
                {
                    ActiveLogWindow.Show();
                }
            }
        }

        private void DecksListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            GameRecord gr = (GameRecord)DecksListBox.SelectedItem;
            if (gr == null) gr = (GameRecord)DecksListBox.Items[0];
            HighlightedGameLogControl.LoadGames(gr.GetDeckSignature());

            // Update deck
            HighlightedDeckControl.ClearDeck();
            for (int i = 0; i < gr.MyDeck.Count; i++)
            {
                HighlightedDeckControl.SetCard(i, gr.MyDeck[i]);
            }

            //HighlightedDeckControl.Title = "Deck";
            Size bestDeckSize = HighlightedDeckControl.GetBestSize();
            HighlightedDeckControl.SetBounds(0, 0, bestDeckSize.Width, bestDeckSize.Height, BoundsSpecified.Size);
            HighlightedDeckStatsDisplay.TheDeck = gr.MyDeck;
            HighlightedDeckStatsDisplay.Invalidate();
            int deckStatsHeight = HighlightedDeckStatsDisplay.GetBestHeight(bestDeckSize.Width);
            HighlightedDeckStatsDisplay.SetBounds(HighlightedDeckControl.Left, HighlightedDeckControl.Top + bestDeckSize.Height, bestDeckSize.Width, deckStatsHeight, BoundsSpecified.All);
            HighlightedDeckPanel.Visible = true;
        }

        /// <summary>
        /// Code from here: https://nickstips.wordpress.com/2010/03/03/c-panel-resets-scroll-position-after-focus-is-lost-and-regained/
        /// To prevent scrollbar from snapping back to position zero
        /// </summary>
        /// <param name="activeControl"></param>
        /// <returns></returns>
        protected override System.Drawing.Point ScrollToControl(System.Windows.Forms.Control activeControl)
        {
            // Returning the current location prevents the panel from
            // scrolling to the active control when the panel loses and regains focus
            return this.DisplayRectangle.Location;
        }
    }
}
