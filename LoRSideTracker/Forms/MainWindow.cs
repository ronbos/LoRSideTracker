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
            Log.SetTextBox(null);
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
        public void OnDeckUpdated(List<CardWithCount> cards)
        {
            if (cards.Count > 0 &&
                (CurrentExpedition == null || !AreDecksEqual(cards, CurrentExpedition.Cards)))
            {
                string title = "Constructed Deck";
                PlayerActiveDeckWindow.Title = string.Format(title);
                PlayerActiveDeckWindow.SetFullDeck(Utilities.Clone(cards));

                CurrentGameRecord.MyDeck = Utilities.Clone(cards);
                CurrentGameRecord.Notes = title;
                CurrentGameRecord.Result = "-";
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
                    CurrentGameRecord.Notes = title;
                    CurrentGameRecord.Result = "-";
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
                // Game ended. Grab game result
                string json = Utilities.GetStringFromURL(Constants.GameResultURL());
                if (json != null && Utilities.IsJsonStringValid(json))
                {
                    Dictionary<string, JsonElement> gameResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                    CurrentGameRecord.Result = gameResult["LocalPlayerWon"].ToObject<bool>() ? "Win" : "Loss";
                    Log.WriteLine("Game no. {0} Result: {1}", gameResult["GameID"].ToObject<int>(), CurrentGameRecord.Result);
                }
                else
                {
                    CurrentGameRecord.Result = "unknown";
                }

                // Save game record to file
                CurrentGameRecord.Timestamp = DateTime.Now;
                string filePath = string.Format(@"{0}\{1}_{2}_{3}_{4}_{5}_{6}.txt", 
                    Constants.GetLocalGamesPath(),
                    CurrentGameRecord.Timestamp.Year, 
                    CurrentGameRecord.Timestamp.Month, 
                    CurrentGameRecord.Timestamp.Day,
                    CurrentGameRecord.Timestamp.Hour, 
                    CurrentGameRecord.Timestamp.Minute, 
                    CurrentGameRecord.Timestamp.Second);
                CurrentGameRecord.SaveToFile(filePath);
                GameHistory.AddGameRecord(CurrentGameRecord);
            }

        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            Log.SetTextBox(LogTextBox);
            DebugLogsCheckBox.Checked = Log.ShowDebugLog;

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
            PlayerActiveDeckWindow.Title = "No Active Deck";
            PlayerActiveDeckWindow.ShouldShowDeckStats = DeckStatsCheckBox.Checked;
            PlayerActiveDeckWindow.Show();
            if (!PlayerDeckCheckBox.Checked) PlayerActiveDeckWindow.Hide();

            PlayerDrawnCardsWindow = new DeckWindow();
            PlayerDrawnCardsWindow.Title = "Cards Drawn";
            PlayerDrawnCardsWindow.ShouldShowDeckStats = DeckStatsCheckBox.Checked;
            PlayerDrawnCardsWindow.Show();
            if (!PlayerDrawnCheckBox.Checked) PlayerDrawnCardsWindow.Hide();

            PlayerPlayedCardsWindow = new DeckWindow();
            PlayerPlayedCardsWindow.Title = "You Played";
            PlayerPlayedCardsWindow.ShouldShowDeckStats = DeckStatsCheckBox.Checked;
            PlayerPlayedCardsWindow.Show();
            if (!PlayerPlayedCheckBox.Checked) PlayerPlayedCardsWindow.Hide();

            OpponentPlayedCardsWindow = new DeckWindow();
            OpponentPlayedCardsWindow.Title = "Opponent Played";
            OpponentPlayedCardsWindow.ShouldShowDeckStats = DeckStatsCheckBox.Checked;
            OpponentPlayedCardsWindow.Show();
            if (!OpponentPlayedCheckBox.Checked) OpponentPlayedCardsWindow.Hide();

            PlayerActiveDeckWindow.SetBounds(Properties.Settings.Default.PlayerDeckLocation.X, Properties.Settings.Default.PlayerDeckLocation.Y, 0, 0, BoundsSpecified.Location);
            PlayerDrawnCardsWindow.SetBounds(Properties.Settings.Default.PlayerDrawnCardsLocation.X, Properties.Settings.Default.PlayerDrawnCardsLocation.Y, 0, 0, BoundsSpecified.Location);
            PlayerPlayedCardsWindow.SetBounds(Properties.Settings.Default.PlayerPlayedCardsLocation.X, Properties.Settings.Default.PlayerPlayedCardsLocation.Y, 0, 0, BoundsSpecified.Location);
            OpponentPlayedCardsWindow.SetBounds(Properties.Settings.Default.OpponentPlayedCardsLocation.X, Properties.Settings.Default.OpponentPlayedCardsLocation.Y, 0, 0, BoundsSpecified.Location);

            PlayerActiveDeckWindow.Opacity = TransparencyTrackBar.Value / 100.0;
            PlayerDrawnCardsWindow.Opacity = TransparencyTrackBar.Value / 100.0;
            PlayerPlayedCardsWindow.Opacity = TransparencyTrackBar.Value / 100.0;
            OpponentPlayedCardsWindow.Opacity = TransparencyTrackBar.Value / 100.0;

            PlayerActiveDeckWindow.HideZeroCountCards = HideZeroCountCheckBox.Checked;

            GameHistory = new GameHistoryWindow();
            //GameHistory.Show();

            CurrentDeck = new StaticDeck(this);
            Thread.Sleep(500);
            CurrentExpedition = new Expedition(this);
            Thread.Sleep(500);
            CurrentOverlay = new Overlay(this);

            //SnapWindowsButton_Click(null, null);

            DeckOptionsGroupBox.Visible = true;
            SnapWindowsButton.Visible = true;
            ShowHistoryButton.Visible = true;
            DebugLogsCheckBox.Visible = true;
            LogTextBox.Visible = true;
        }

        private void DebugLogsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Log.ShowDebugLog = DebugLogsCheckBox.Checked;
        }

        private void SnapWindowsButton_Click(object sender, EventArgs e)
        {
            Point location;
            if (PlayerDeckCheckBox.Checked)
            {
                location = PlayerActiveDeckWindow.Location;
            }
            else if (PlayerDrawnCheckBox.Checked)
            {
                location = PlayerDrawnCardsWindow.Location;
            }
            else if (PlayerPlayedCheckBox.Checked)
            {
                location = PlayerPlayedCardsWindow.Location;
            }
            else if (OpponentPlayedCheckBox.Checked)
            {
                location = OpponentPlayedCardsWindow.Location;
            }
            else
            {
                return;
            }

            int margin = 2;
            if (PlayerDeckCheckBox.Checked)
            {
                PlayerActiveDeckWindow.SetDesktopBounds(location.X, location.Y, PlayerActiveDeckWindow.DesktopBounds.Width, PlayerActiveDeckWindow.DesktopBounds.Height);
                location.X += PlayerActiveDeckWindow.DesktopBounds.Width + margin;
            }
            if (PlayerDrawnCheckBox.Checked)
            {
                PlayerDrawnCardsWindow.SetDesktopBounds(location.X, location.Y, PlayerDrawnCardsWindow.DesktopBounds.Width, PlayerDrawnCardsWindow.DesktopBounds.Height);
                location.X += PlayerDrawnCardsWindow.DesktopBounds.Width + margin;
            }
            if (PlayerPlayedCheckBox.Checked)
            {
                PlayerPlayedCardsWindow.SetDesktopBounds(location.X, location.Y, PlayerPlayedCardsWindow.DesktopBounds.Width, PlayerPlayedCardsWindow.DesktopBounds.Height);
                location.X += PlayerPlayedCardsWindow.DesktopBounds.Width + margin;
            }
            if (OpponentPlayedCheckBox.Checked)
            {
                OpponentPlayedCardsWindow.SetDesktopBounds(location.X, location.Y, OpponentPlayedCardsWindow.DesktopBounds.Width, OpponentPlayedCardsWindow.DesktopBounds.Height);
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            DebugLogsCheckBox.Checked = Properties.Settings.Default.MainWindowShowDebugLog;
            if (Properties.Settings.Default.MainWindowBounds.Width > 0 && Properties.Settings.Default.MainWindowBounds.Height > 0)
            {
                this.WindowState = Properties.Settings.Default.MainWindowState;
                this.Bounds = Properties.Settings.Default.MainWindowBounds;
            }
            PlayerDeckCheckBox.Checked = Properties.Settings.Default.ShowPlayerDeck;
            HideZeroCountCheckBox.Checked = Properties.Settings.Default.HideZeroCountInDeck;
            PlayerDrawnCheckBox.Checked = Properties.Settings.Default.ShowPlayerDrawnCards;
            PlayerPlayedCheckBox.Checked = Properties.Settings.Default.ShowPlayerPlayedCards;
            OpponentPlayedCheckBox.Checked = Properties.Settings.Default.ShowOpponentPlayedCards;
            DeckStatsCheckBox.Checked = Properties.Settings.Default.ShowDeckStats;
            TransparencyTrackBar.Value = Properties.Settings.Default.DeckTransparency;
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.MainWindowShowDebugLog = DebugLogsCheckBox.Checked;
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
            Properties.Settings.Default.ShowPlayerDeck = PlayerDeckCheckBox.Checked;
            Properties.Settings.Default.HideZeroCountInDeck = HideZeroCountCheckBox.Checked;
            Properties.Settings.Default.ShowPlayerDrawnCards = PlayerDrawnCheckBox.Checked;
            Properties.Settings.Default.ShowPlayerPlayedCards = PlayerPlayedCheckBox.Checked;
            Properties.Settings.Default.ShowOpponentPlayedCards = OpponentPlayedCheckBox.Checked;
            Properties.Settings.Default.ShowDeckStats = DeckStatsCheckBox.Checked;
            Properties.Settings.Default.DeckTransparency = TransparencyTrackBar.Value;

            if (PlayerActiveDeckWindow != null) Properties.Settings.Default.PlayerDeckLocation = PlayerActiveDeckWindow.Location;
            if (PlayerDrawnCardsWindow != null) Properties.Settings.Default.PlayerDrawnCardsLocation = PlayerDrawnCardsWindow.Location;
            if (PlayerPlayedCardsWindow != null) Properties.Settings.Default.PlayerPlayedCardsLocation = PlayerPlayedCardsWindow.Location;
            if (OpponentPlayedCardsWindow != null) Properties.Settings.Default.OpponentPlayedCardsLocation = OpponentPlayedCardsWindow.Location;

            // Save the settings
            Properties.Settings.Default.Save();
        }

        private void PlayerDeckCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (PlayerActiveDeckWindow != null)
            {
                PlayerActiveDeckWindow.Visible = PlayerDeckCheckBox.Checked;
            }
        }

        private void PlayerDrawnCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (PlayerDrawnCardsWindow != null)
            {
                PlayerDrawnCardsWindow.Visible = PlayerDrawnCheckBox.Checked;
            }
        }

        private void PlayerPlayedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (PlayerPlayedCardsWindow != null)
            {
                PlayerPlayedCardsWindow.Visible = PlayerPlayedCheckBox.Checked;
            }
        }

        private void OpponentPlayedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (OpponentPlayedCardsWindow != null)
            {
                OpponentPlayedCardsWindow.Visible = OpponentPlayedCheckBox.Checked;
            }
        }

        private void DecksStatsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (PlayerActiveDeckWindow != null)
            {
                PlayerActiveDeckWindow.ShouldShowDeckStats = DeckStatsCheckBox.Checked;
                PlayerActiveDeckWindow.UpdateSize();
            }
            if (PlayerDrawnCardsWindow != null)
            {
                PlayerDrawnCardsWindow.ShouldShowDeckStats = DeckStatsCheckBox.Checked;
                PlayerDrawnCardsWindow.UpdateSize();
            }
            if (PlayerPlayedCardsWindow != null)
            {
                PlayerPlayedCardsWindow.ShouldShowDeckStats = DeckStatsCheckBox.Checked;
                PlayerPlayedCardsWindow.UpdateSize();
            }
            if (OpponentPlayedCardsWindow != null)
            {
                OpponentPlayedCardsWindow.ShouldShowDeckStats = DeckStatsCheckBox.Checked;
                OpponentPlayedCardsWindow.UpdateSize();
            }
        }

        private void TransparencyTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (PlayerActiveDeckWindow != null)
            {
                PlayerActiveDeckWindow.Opacity = TransparencyTrackBar.Value / 100.0;
            }
            if (PlayerDrawnCardsWindow != null)
            {
                PlayerDrawnCardsWindow.Opacity = TransparencyTrackBar.Value / 100.0;
            }
            if (PlayerPlayedCardsWindow != null)
            {
                PlayerPlayedCardsWindow.Opacity = TransparencyTrackBar.Value / 100.0;
            }
            if (OpponentPlayedCardsWindow != null)
            {
                OpponentPlayedCardsWindow.Opacity = TransparencyTrackBar.Value / 100.0;
            }

        }

        private void HideZeroCountCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (PlayerActiveDeckWindow != null)
            {
                PlayerActiveDeckWindow.HideZeroCountCards = HideZeroCountCheckBox.Checked;
                PlayerActiveDeckWindow.RefreshDeck();

            }
        }

        private void ShowHistoryButton_Click(object sender, EventArgs e)
        {
            if (GameHistory != null)
            {
                GameHistory.Show();
            }
        }
    }
}
