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

        private DeckWindow ActiveDeckWindow;
        private DeckWindow PlayedCardsWindow;
        private DeckWindow OpponentCardsWindow;

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
            if (CurrentDeck.Cards.Count > 0)
            {
                ActiveDeckWindow.Title = string.Format("My Deck");
                ActiveDeckWindow.SetFullDeck(CurrentDeck.Cards);
            }
            else if (CurrentExpedition != null && CurrentExpedition.Cards.Count > 0)
            {
                bool isEliminationGame = (Array.FindLastIndex(CurrentExpedition.Record, item => item.Equals("win")) < Array.FindLastIndex(CurrentExpedition.Record, item => item.Equals("loss")));
                ActiveDeckWindow.Title = string.Format("Expedition {0}-{1}({2})", CurrentExpedition.NumberOfWins, isEliminationGame ? 1 : 0, CurrentExpedition.NumberOfLosses);
                ActiveDeckWindow.SetFullDeck(CurrentExpedition.Cards);
            }
            else
            {
                ActiveDeckWindow.Title = "No Active Deck";
                ActiveDeckWindow.SetFullDeck(new List<CardWithCount>());
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
                if (CurrentExpedition.Cards.Count > 0)
                {
                    bool isEliminationGame = (Array.FindLastIndex(CurrentExpedition.Record, item => item.Equals("win")) < Array.FindLastIndex(CurrentExpedition.Record, item => item.Equals("loss")));
                    ActiveDeckWindow.Title = string.Format("Expedition {0}-{1}({2})", CurrentExpedition.NumberOfWins, isEliminationGame ? 1 : 0, CurrentExpedition.NumberOfLosses);
                    ActiveDeckWindow.SetFullDeck(CurrentExpedition.Cards);
                }
                else
                {
                    ActiveDeckWindow.Title = "No Active Deck";
                    ActiveDeckWindow.SetFullDeck(new List<CardWithCount>());
                }
            }
        }

        /// <summary>
        /// Receives notification that player drawn set was changed
        /// </summary>
        /// <param name="cards">Updated set</param>
        public void OnPlayerDrawnSetUpdated(List<CardWithCount> cards)
        {
            ActiveDeckWindow.SetDrawnCards(cards);
            PlayedCardsWindow.SetFullDeck(cards);
        }

        /// <summary>
        /// Receives notification that opponent played set was changed
        /// </summary>
        /// <param name="cards">Updated set</param>
        public void OnOpponentPlayedSetUpdated(List<CardWithCount> cards)
        {
            OpponentCardsWindow.SetFullDeck(cards);
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            Log.SetTextBox(LogTextBox);
            DebugLogsCheckBox.Checked = Log.ShowDebugLog;

            Rectangle progressRect = MyProgressDisplay.Bounds;
            progressRect.Offset(
                Bounds.Width / 2 - (progressRect.Left + progressRect.Right) / 2,
                Bounds.Height / 2 - (progressRect.Top + progressRect.Bottom) / 2);
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

            ActiveDeckWindow = new DeckWindow();
            ActiveDeckWindow.Title = "Your Deck";
            ActiveDeckWindow.Show();

            PlayedCardsWindow = new DeckWindow();
            PlayedCardsWindow.Title = "Cards Drawn";
            PlayedCardsWindow.Show();

            OpponentCardsWindow = new DeckWindow();
            OpponentCardsWindow.Title = "Opponent Deck";
            OpponentCardsWindow.Show();

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
            int totalWidth = ActiveDeckWindow.DesktopBounds.Width + PlayedCardsWindow.DesktopBounds.Width 
                + OpponentCardsWindow.DesktopBounds.Width + 2 * margin;
            int x = (Bounds.Left + Bounds.Right) / 2 - totalWidth / 2;
            int y = Bounds.Bottom + margin;
            ActiveDeckWindow.SetDesktopBounds(x, y, ActiveDeckWindow.DesktopBounds.Width, ActiveDeckWindow.DesktopBounds.Height);
            x += ActiveDeckWindow.DesktopBounds.Width + margin;
            PlayedCardsWindow.SetDesktopBounds(x, y, PlayedCardsWindow.DesktopBounds.Width, PlayedCardsWindow.DesktopBounds.Height);
            x += PlayedCardsWindow.DesktopBounds.Width + margin;
            OpponentCardsWindow.SetDesktopBounds(x, y, OpponentCardsWindow.DesktopBounds.Width, OpponentCardsWindow.DesktopBounds.Height);
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

            // Save the settings
            Properties.Settings.Default.Save();
        }
    }
}
