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
        private List<int> MissingSets;

        private Expedition CurrentExpedition;
        private StaticDeck CurrentDeck;
        private Overlay CurrentOverlay;

        private DeckWindow PlayerActiveDeckWindow;
        private DeckWindow PlayerDrawnCardsWindow;
        private DeckWindow PlayerPlayedCardsWindow;
        private DeckWindow OpponentPlayedCardsWindow;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MainWindow()
        {
            Log.SetTextBox(null);
        }

        /// <summary>
        /// Receives notification that static deck was updated
        /// </summary>
        /// <param name="cards">Updated set</param>
        public void OnDeckUpdated(List<CardWithCount> cards)
        {
            if (cards.Count > 0)
            {
                PlayerActiveDeckWindow.Title = string.Format("My Deck");
                PlayerActiveDeckWindow.SetFullDeck(cards);
            }
            else
            {
                // No active static deck -- show expedition deck or no deck
                OnExpeditionDeckUpdated(CurrentExpedition.Cards);
            }
        }

        /// <summary>
        /// Receives notification that expedition deck was updated
        /// </summary>
        /// <param name="cards">Updated set</param>
        public void OnExpeditionDeckUpdated(List<CardWithCount> cards)
        {
            if (CurrentDeck.Cards.Count == 0)
            {
                if (cards.Count > 0)
                {
                    bool isEliminationGame = (Array.FindLastIndex(CurrentExpedition.Record, item => item.Equals("win")) < Array.FindLastIndex(CurrentExpedition.Record, item => item.Equals("loss")))
                        || (CurrentExpedition.NumberOfWins == 6);
                    PlayerActiveDeckWindow.Title = string.Format("Expedition {0}-{1}{2}", CurrentExpedition.NumberOfWins, CurrentExpedition.NumberOfLosses, isEliminationGame ? "*" : "");
                    PlayerActiveDeckWindow.SetFullDeck(cards);
                }
                else
                {
                    PlayerActiveDeckWindow.Title = "No Active Deck";
                    PlayerActiveDeckWindow.SetFullDeck(new List<CardWithCount>());
                }
            }
        }

        /// <summary>
        /// Receives notification that player drawn set was changed
        /// </summary>
        /// <param name="cards">Updated set</param>
        public void OnPlayerDrawnSetUpdated(List<CardWithCount> cards)
        {
            PlayerActiveDeckWindow.SetDrawnCards(cards);
            PlayerDrawnCardsWindow.SetFullDeck(cards);
        }

        /// <summary>
        /// Receives notification that player played set was changed
        /// </summary>
        /// <param name="cards">Updated set</param>
        public void OnPlayerPlayedSetUpdated(List<CardWithCount> cards)
        {
            PlayerPlayedCardsWindow.SetFullDeck(cards);
        }

        /// <summary>
        /// Receives notification that opponent played set was changed
        /// </summary>
        /// <param name="cards">Updated set</param>
        public void OnOpponentPlayedSetUpdated(List<CardWithCount> cards)
        {
            OpponentPlayedCardsWindow.SetFullDeck(cards);
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
                CurrentDownloadIndex = 0;
                CardLibrary.DownloadSet(MissingSets[CurrentDownloadIndex], 
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
            string message = String.Format("Downloading card set {0} ({1}/{2})", MissingSets[CurrentDownloadIndex], CurrentDownloadIndex + 1, MissingSets.Count);
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
                string localFile = Constants.GetSetZip(MissingSets[CurrentDownloadIndex]);
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
                CardLibrary.ProcessDownloadedSet(MissingSets[CurrentDownloadIndex]);
                CurrentDownloadIndex++;
                if (CurrentDownloadIndex < MissingSets.Count)
                {
                    CardLibrary.DownloadSet(MissingSets[CurrentDownloadIndex],
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
            PlayerActiveDeckWindow.Title = "Your Deck";
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

            CurrentDeck = new StaticDeck(this);
            Thread.Sleep(500);
            CurrentExpedition = new Expedition(this);
            Thread.Sleep(500);
            CurrentOverlay = new Overlay(this);

            SnapWindowsButton_Click(null, null);
        }

        private void DebugLogsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Log.ShowDebugLog = DebugLogsCheckBox.Checked;
        }

        private void SnapWindowsButton_Click(object sender, EventArgs e)
        {
            int margin = 2;
            int x = Bounds.Left + 6;
            int y = Bounds.Bottom + 2;
            if (PlayerDeckCheckBox.Checked)
            {
                PlayerActiveDeckWindow.SetDesktopBounds(x, y, PlayerActiveDeckWindow.DesktopBounds.Width, PlayerActiveDeckWindow.DesktopBounds.Height);
                x += PlayerActiveDeckWindow.DesktopBounds.Width + margin;
            }
            if (PlayerDrawnCheckBox.Checked)
            {
                PlayerDrawnCardsWindow.SetDesktopBounds(x, y, PlayerDrawnCardsWindow.DesktopBounds.Width, PlayerDrawnCardsWindow.DesktopBounds.Height);
                x += PlayerDrawnCardsWindow.DesktopBounds.Width + margin;
            }
            if (PlayerPlayedCheckBox.Checked)
            {
                PlayerPlayedCardsWindow.SetDesktopBounds(x, y, PlayerPlayedCardsWindow.DesktopBounds.Width, PlayerPlayedCardsWindow.DesktopBounds.Height);
                x += PlayerPlayedCardsWindow.DesktopBounds.Width + margin;
            }
            if (OpponentPlayedCheckBox.Checked)
            {
                OpponentPlayedCardsWindow.SetDesktopBounds(x, y, OpponentPlayedCardsWindow.DesktopBounds.Width, OpponentPlayedCardsWindow.DesktopBounds.Height);
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
            PlayerDrawnCheckBox.Checked = Properties.Settings.Default.ShowPlayerDrawnCards;
            PlayerPlayedCheckBox.Checked = Properties.Settings.Default.ShowPlayerPlayedCards;
            OpponentPlayedCheckBox.Checked = Properties.Settings.Default.ShowOpponentPlayedCards;
            DeckStatsCheckBox.Checked = Properties.Settings.Default.ShowDeckStats;
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
            Properties.Settings.Default.ShowPlayerDrawnCards = PlayerDrawnCheckBox.Checked;
            Properties.Settings.Default.ShowPlayerPlayedCards = PlayerPlayedCheckBox.Checked;
            Properties.Settings.Default.ShowOpponentPlayedCards = OpponentPlayedCheckBox.Checked;
            Properties.Settings.Default.ShowDeckStats = DeckStatsCheckBox.Checked;

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
    }
}
