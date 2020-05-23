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
    public partial class MainWindow : Form, ExpeditionUpdateCallback, DeckUpdateCallback, OverlayUpdateCallback
    {
        private ProgressWindow MyProgressWindow;

        private int CurrentDownloadIndex = 0;
        private List<int> MissingSets;

        private Expedition CurrentExpedition;
        private Deck CurrentDeck;
        private Overlay CurrentOverlay;

        private DeckWindow ActiveDeckWindow;
        private DeckWindow PlayedCardsWindow;
        private DeckWindow OpponentCardsWindow;

        public MainWindow()
        {
            InitializeComponent();

            MyProgressWindow = new ProgressWindow();

            //string deckJson = Utilities.ReadLocalFile("..\\..\\example-static-decklist.json");
            //Dictionary<string, JsonElement> deck = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(deckJson);
            //CurrentDeck.LoadDeckFromJson(deck);
            //ActiveDeckWindow.SetFullDeck(CurrentDeck);
            //ActiveDeckWindow.Show();
            //ActiveDeckWindow.Show();

        }

        public void OnDeckUpdated(List<CardWithCount> cards)
        {
            if (CurrentDeck.Cards.Count > 0)
            {
                ActiveDeckWindow.SetFullDeck(CurrentDeck.Cards);
            }
            else if (CurrentExpedition != null)
            {
                ActiveDeckWindow.SetFullDeck(CurrentExpedition.Cards);
            }
        }

        public void OnExpeditionDeckUpdated(List<CardWithCount> cards)
        {
            if (CurrentDeck.Cards.Count == 0)
            {
                ActiveDeckWindow.SetFullDeck(CurrentExpedition.Cards);
            }
        }
        public void OnPlayerDrawnSetUpdated(List<CardWithCount> cards)
        {
            ActiveDeckWindow.SetDrawnCards(cards);
            PlayedCardsWindow.SetFullDeck(cards);
        }

        public void OnOpponentPlayedSetUpdated(List<CardWithCount> cards)
        {
            OpponentCardsWindow.SetFullDeck(cards);
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            Rectangle progressRect = MyProgressWindow.RectangleToScreen(MyProgressWindow.DesktopBounds);
            progressRect.Offset(
                (Bounds.Left + Bounds.Right) / 2 - (progressRect.Left + progressRect.Right) / 2,
                (Bounds.Top + Bounds.Bottom) / 2 - (progressRect.Top + progressRect.Bottom) / 2);
            MyProgressWindow.SetBounds(progressRect.X, progressRect.Y, progressRect.Width, progressRect.Height);
            MyProgressWindow.Show();

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

        void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            string message = String.Format("Downloading card set {0} ({1}/{2})", MissingSets[CurrentDownloadIndex], CurrentDownloadIndex + 1, MissingSets.Count);
            MyProgressWindow.Update(message, percentage);
        }

        void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
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

                MyProgressWindow.Update("Download completed. Processing...", 100);
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
        private async void OnAllSetsDownloaded()
        {
            await Task.Run(() => CardLibrary.LoadAllCards(MyProgressWindow));
            MyProgressWindow.Close();

            ActiveDeckWindow = new DeckWindow();
            ActiveDeckWindow.Title = "Active Deck";
            ActiveDeckWindow.Show();

            PlayedCardsWindow = new DeckWindow();
            PlayedCardsWindow.Title = "Cards Drawn";
            PlayedCardsWindow.Show();

            OpponentCardsWindow = new DeckWindow();
            OpponentCardsWindow.Title = "Opponent Deck";
            OpponentCardsWindow.Show();

            CurrentDeck = new Deck(this);
            Thread.Sleep(500);
            CurrentExpedition = new Expedition(this);
            Thread.Sleep(500);
            CurrentOverlay = new Overlay(this);
        }

    }
}
