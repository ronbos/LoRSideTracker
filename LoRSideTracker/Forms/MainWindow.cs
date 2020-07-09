using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace LoRSideTracker
{
    /// <summary>
    /// Main app wondow
    /// </summary>
    public partial class MainWindow : Form, IExpeditionUpdateCallback, ICardsInPlayCallback, ISetDownloaderCallback
    {
        private Expedition CurrentExpedition;
        private CardsInPlayWorker CurrentPlayState;

        private DeckWindow PlayerActiveDeckWindow;
        private DeckWindow PlayerDrawnCardsWindow;
        private DeckWindow PlayerGraveyardWindow;
        private DeckWindow OpponentGraveyardWindow;

        private LogWindow ActiveLogWindow;
        private GameBoardWatchWindow OverlayWindow;

        private GameRecord CurrentGameRecord = new GameRecord();

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.ResizeRedraw = true;
            HighlightedDeckControl.CustomDeckScale = DeckControl.DeckScale.Small;

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
        /// Receives notification that expedition deck was updated
        /// </summary>
        /// <param name="cards">Updated set</param>
        public void OnExpeditionDeckUpdated(List<CardWithCount> cards)
        {
            if (CurrentExpedition.State == "Picking" || CurrentExpedition.State == "Swapping" || CurrentExpedition.State == "Other")
            {
                PlayerActiveDeckWindow.Title = string.Format("Expedition {0}-{1}{2}", CurrentExpedition.NumberOfWins,
                    CurrentExpedition.NumberOfLosses, CurrentExpedition.IsEliminationGame ? "*" : "");
                PlayerActiveDeckWindow.SetFullDeck(CurrentExpedition.Cards);
                PlayerActiveDeckWindow.SetCurrentDeck(CurrentExpedition.Cards);
            }
            else if (CurrentPlayState.GameState != "InProgress")
            {
                PlayerActiveDeckWindow.Title = string.Format("No Active Deck");
                PlayerActiveDeckWindow.SetFullDeck(new List<CardWithCount>());
            }
        }

        /// <summary>
        /// Receives notification that player deck was set
        /// </summary>
        /// <param name="cards">Deck contents</param>
        /// <param name="name">Deck name</param>
        public void OnPlayerDeckSet(List<CardWithCount> cards, string name)
        {
            if (cards.Count > 0)
            {
                PlayerActiveDeckWindow.Title = name;
                PlayerActiveDeckWindow.SetFullDeck(cards);
                PlayerActiveDeckWindow.SetCurrentDeck(cards);
                PlayerDrawnCardsWindow.SetFullDeck(new List<CardWithCount>());
                PlayerGraveyardWindow.SetFullDeck(new List<CardWithCount>());
                OpponentGraveyardWindow.SetFullDeck(new List<CardWithCount>());
            }
            else
            {
                string title = "No Active Deck";
                PlayerActiveDeckWindow.Title = title;
                PlayerActiveDeckWindow.SetFullDeck(new List<CardWithCount>());
                PlayerDrawnCardsWindow.SetFullDeck(new List<CardWithCount>());
                PlayerGraveyardWindow.SetFullDeck(new List<CardWithCount>());
                OpponentGraveyardWindow.SetFullDeck(new List<CardWithCount>());
            }
        }

        /// <summary>
        /// Callback for when player deck set has been changed
        /// </summary>
        /// <param name="cards">Cards in the set</param>
        public void OnPlayerDeckChanged(List<CardWithCount> cards)
        {
            PlayerActiveDeckWindow.SetCurrentDeck(Utilities.Clone(cards));
        }

        /// <summary>
        /// Callback for when player hand has been changed
        /// </summary>
        /// <param name="cards">Cards in the set</param>
        public void OnPlayerDrawnAndPlayedChanged(List<CardWithCount> cards)
        {
            PlayerDrawnCardsWindow.SetCurrentDeck(Utilities.Clone(cards));
        }

        /// <summary>
        /// Callback for when player graveyard has been changed
        /// </summary>
        /// <param name="cards">Cards in the set</param>
        public void OnPlayerGraveyardChanged(List<CardWithCount> cards)
        {
            PlayerGraveyardWindow.SetCurrentDeck(Utilities.Clone(cards));
        }

        /// <summary>
        /// Callback for when opponent graveyard has been changed
        /// </summary>
        /// <param name="cards">Cards in the set</param>
        public void OnOpponentGraveyardChanged(List<CardWithCount> cards)
        {
            OpponentGraveyardWindow.SetCurrentDeck(Utilities.Clone(cards));
        }

        /// <summary>
        /// Receives notification that game state was changed
        /// </summary>
        /// <param name="oldGameState"></param>
        /// <param name="newGameState"></param>
        public void OnGameStateChanged(string oldGameState, string newGameState)
        {
            Log.WriteLine("Game state changed from {0} to {1}", oldGameState, newGameState);
        }

        /// <summary>
        /// Receives notification that game state has ended
        /// </summary>
        /// <param name="gameNumber"></param>
        /// <param name="gameRecord"></param>
        public void OnGameEnded(int gameNumber, GameRecord gameRecord)
        {
            // Game ended. Grab game result
            Log.WriteLine("Game no. {0} Result: {1}", gameNumber, gameRecord.Result);

            if (string.IsNullOrEmpty(Constants.PlayBackDeckPath) && gameRecord.OpponentDeck.Count > 0)
            {
                var gameLog = CurrentPlayState.StopGameLog();
                GameHistory.AddGameRecord(gameRecord, true, gameLog);
                Utilities.CallActionSafelyAndWait(DecksListCtrl, new Action(() =>
                {
                    DecksListCtrl.AddToDeckList(gameRecord, true);
                }));
            }

            OnPlayerDeckSet(new List<CardWithCount>(), "");
        }

        /// <summary>
        /// Callback for when elements have been updated
        /// </summary>
        /// <param name="playerElements"></param>
        /// <param name="opponentElements"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        public void OnElementsUpdate(CardList<CardInPlay> playerElements, CardList<CardInPlay> opponentElements, int screenWidth, int screenHeight)
        {
            if (OverlayWindow != null)
            {
                OverlayWindow.Update(playerElements, opponentElements, screenWidth, screenHeight);
            }
        }

        /// <summary>
        /// Position progress display to the center of the window and initiate set download
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Shown(object sender, EventArgs e)
        {
            Rectangle progressRect = MyProgressDisplay.Bounds;
            progressRect.Offset(
                ClientRectangle.Width / 2 - (progressRect.Left + progressRect.Right) / 2,
                ClientRectangle.Height / 2 - (progressRect.Top + progressRect.Bottom) / 2);
            MyProgressDisplay.SetBounds(progressRect.X, progressRect.Y, progressRect.Width, progressRect.Height);
            MyProgressDisplay.Show();

            if (!(new SetDownloader(this)).DownloadAllSets(MyProgressDisplay))
            {
                Close();
            }
        }


        /// <summary>
        /// Callback for when a download was canceled or error occured
        /// </summary>
        /// <param name="errorOccured"></param>
        public void OnDownloadCanceled(bool errorOccured)
        {
            if (errorOccured)
            {
                MessageBox.Show("Download Error Occurred. Exiting...", "Error", MessageBoxButtons.OK);
                Close();
            }
        }

        private DeckWindow CreateDeckWindow(string title, DeckControl.DeckScale deckScale, double opacity,
            bool showWindow, int posX, int posY)
        {
            DeckWindow deckWindow = new DeckWindow();
            deckWindow.CreateControl();
            deckWindow.Title = title;
            deckWindow.CustomDeckScale = deckScale;
            deckWindow.ShouldShowDeckStats = Properties.Settings.Default.ShowDeckStats;
            deckWindow.Show();
            deckWindow.IsNonEmptyDeckVisible = showWindow;
            deckWindow.SetBounds(posX, posY, 0, 0, BoundsSpecified.Location);
            deckWindow.Opacity = opacity;
            deckWindow.UpdateDeck(null, null);
            return deckWindow;

        }

        /// <summary>
        /// Runs after all sets are downloaded and processed.
        /// Initializes the StaticDeck windows and deck tracking objects.
        /// </summary>
        public async void OnAllSetsDownloaded()
        {
            await Task.Run(() => CardLibrary.LoadAllCards(MyProgressDisplay));
            await Task.Run(() => GameHistory.LoadAllGames(MyProgressDisplay));
            DeckControl.DeckScale deckScale = DeckControl.DeckScale.Medium;
            if (Properties.Settings.Default.DeckDrawSize == 0)
            {
                deckScale = DeckControl.DeckScale.Small;
            }
            if (Properties.Settings.Default.DeckDrawSize == 2)
            {
                deckScale = DeckControl.DeckScale.Large;
            }

            double deckOpacity = Properties.Settings.Default.DeckTransparency / 100.0;
            PlayerActiveDeckWindow = CreateDeckWindow("No Active Deck", deckScale, deckOpacity,
                Properties.Settings.Default.ShowPlayerDeck,
                Properties.Settings.Default.PlayerDeckLocation.X, 
                Properties.Settings.Default.PlayerDeckLocation.Y);
            PlayerDrawnCardsWindow = CreateDeckWindow("Drawn Cards", deckScale, deckOpacity,
                Properties.Settings.Default.ShowPlayerDrawnCards,
                Properties.Settings.Default.PlayerDrawnCardsLocation.X, 
                Properties.Settings.Default.PlayerDrawnCardsLocation.Y);
            PlayerGraveyardWindow = CreateDeckWindow("Graveyard", deckScale, deckOpacity,
                Properties.Settings.Default.ShowPlayerGraveyard,
                Properties.Settings.Default.PlayerPlayedCardsLocation.X, 
                Properties.Settings.Default.PlayerPlayedCardsLocation.Y);
            OpponentGraveyardWindow = CreateDeckWindow("Opponent Graveyard", deckScale, deckOpacity,
                Properties.Settings.Default.ShowOpponentGraveyard,
                Properties.Settings.Default.OpponentPlayedCardsLocation.X, 
                Properties.Settings.Default.OpponentPlayedCardsLocation.Y);

            PlayerActiveDeckWindow.HideZeroCountCards = Properties.Settings.Default.HideZeroCountInDeck;

            ActiveLogWindow = new LogWindow();
            ActiveLogWindow.CreateControl();
            if (Properties.Settings.Default.ActiveLogWindowBounds.Width > 0
                && Properties.Settings.Default.ActiveLogWindowBounds.Left > 0)
            {
                ActiveLogWindow.StartPosition = FormStartPosition.Manual;
                ActiveLogWindow.SetBounds(
                    Properties.Settings.Default.ActiveLogWindowBounds.X,
                    Properties.Settings.Default.ActiveLogWindowBounds.Y,
                    Properties.Settings.Default.ActiveLogWindowBounds.Width,
                    Properties.Settings.Default.ActiveLogWindowBounds.Height,
                    BoundsSpecified.All);
            }

            {
                // Show and hide active log window to make sure it's loaded ahead of time
                ActiveLogWindow.Show();
                ActiveLogWindow.Hide();
            }
            Log.SetLogWindow(ActiveLogWindow);

            // Special debigging window is shown if D key is held during load
            if (!string.IsNullOrEmpty(Constants.PlayBackDeckPath) || Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                OverlayWindow = new GameBoardWatchWindow();
                OverlayWindow.Show();
            }

            CurrentPlayState = new CardsInPlayWorker(this);
            if (string.IsNullOrEmpty(Constants.PlayBackDeckPath))
            {
                CurrentExpedition = new Expedition(this);
            }

            // Hide the progress display and make all the other UI elements visible
            MyProgressDisplay.Visible = false;
            DecksListCtrl.Visible = true;
            HighlightedGameLogControl.Visible = true;
            TheMenuBar.Visible = true;

            // Load all games. We do this in reverse as AddDeckToList expects them
            // in chronological order
            for (int i = GameHistory.Games.Count - 1; i >= 0; i--)
            {
                DecksListCtrl.AddToDeckList(GameHistory.Games[i]);
            }

            Utilities.CallActionSafelyAndWait(DecksListCtrl, new Action(() => { DecksListCtrl.SwitchDeckView(false); }));

            CurrentPlayState.Start(string.IsNullOrEmpty(Constants.PlayBackDeckPath) 
                ? null : Constants.GetLocalGamesPath() + "\\" + Constants.PlayBackDeckPath);
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.MainWindowBounds.Width > 0 && Properties.Settings.Default.MainWindowBounds.Height > 0)
            {
                this.WindowState = Properties.Settings.Default.MainWindowState;
                this.Bounds = Properties.Settings.Default.MainWindowBounds;
            }
        }

        /// <summary>
        /// Save properties while window is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.MainWindowState = this.WindowState;
            Properties.Settings.Default.MainWindowBounds = (this.WindowState == FormWindowState.Normal) ? Bounds : RestoreBounds;
            if (ActiveLogWindow != null)
            {
                Properties.Settings.Default.ActiveLogWindowBounds = (ActiveLogWindow.WindowState == FormWindowState.Normal)
                ? ActiveLogWindow.Bounds : ActiveLogWindow.RestoreBounds;
            }

            if (PlayerActiveDeckWindow != null)
            {
                Properties.Settings.Default.PlayerDeckLocation = PlayerActiveDeckWindow.Location;
                Properties.Settings.Default.ShowPlayerDeck = PlayerActiveDeckWindow.IsNonEmptyDeckVisible;
                Properties.Settings.Default.HideZeroCountInDeck = PlayerActiveDeckWindow.HideZeroCountCards;
                Properties.Settings.Default.ShowDeckStats = PlayerActiveDeckWindow.ShouldShowDeckStats;
                Properties.Settings.Default.DeckDrawSize =
                    (PlayerActiveDeckWindow.CustomDeckScale.CardSize == DeckControl.DeckScale.Small.CardSize) ? 0 :
                    (PlayerActiveDeckWindow.CustomDeckScale.CardSize == DeckControl.DeckScale.Large.CardSize) ? 2 : 1;
            }
            if (PlayerDrawnCardsWindow != null)
            {
                Properties.Settings.Default.PlayerDrawnCardsLocation = PlayerDrawnCardsWindow.Location;
                Properties.Settings.Default.ShowPlayerDrawnCards = PlayerDrawnCardsWindow.IsNonEmptyDeckVisible;
            }
            if (PlayerGraveyardWindow != null)
            {
                Properties.Settings.Default.PlayerPlayedCardsLocation = PlayerGraveyardWindow.Location;
                Properties.Settings.Default.ShowPlayerGraveyard = PlayerGraveyardWindow.IsNonEmptyDeckVisible;
            }
            if (OpponentGraveyardWindow != null)
            {
                Properties.Settings.Default.OpponentPlayedCardsLocation = OpponentGraveyardWindow.Location;
                Properties.Settings.Default.ShowOpponentGraveyard = OpponentGraveyardWindow.IsNonEmptyDeckVisible;
            }


            // Save the settings
            Properties.Settings.Default.Save();
        }
        private void DecksListCtrl_SelectionChanged(object sender, EventArgs e)
        {
            GameRecord gr = DecksListCtrl.SelectedItem;
            if (gr == null)
            {
                HighlightedGameLogControl.Clear();
                HighlightedDeckControl.ClearDeck();
                HighlightedDeckPanel.Visible = false;
                return;
            }

            // Highlight the deck
            string deckName = null;
            if (!gr.IsExpedition() && gr.MyDeckName != GameRecord.DefaultConstructedDeckName)
            {
                deckName = gr.MyDeckName;
            }
            HighlightedGameLogControl.LoadGames(gr.GetDeckSignature(), deckName, Properties.Settings.Default.HideAIGames);
            HighlightedDeckControl.SetDeck(gr.MyDeck);
            HighlightedDeckControl.Title = gr.ToString();
            HighlightedDeckStatsDisplay.TheDeck = gr.MyDeck;

            // Fix the size
            Size bestDeckSize = HighlightedDeckControl.GetBestSize();
            HighlightedDeckControl.SetBounds(0, 0, bestDeckSize.Width, bestDeckSize.Height, BoundsSpecified.Size);
            int deckStatsHeight = HighlightedDeckStatsDisplay.GetBestHeight(bestDeckSize.Width);
            HighlightedDeckStatsDisplay.SetBounds(HighlightedDeckControl.Left, HighlightedDeckControl.Top + bestDeckSize.Height, bestDeckSize.Width, deckStatsHeight, BoundsSpecified.All);

            HighlightedGameLogControl.Invalidate();
            HighlightedDeckControl.Invalidate();
            HighlightedDeckStatsDisplay.Invalidate();
            HighlightedDeckPanel.Visible = true;
        }


        /// <summary>
        /// Code from here: https://nickstips.wordpress.com/2010/03/03/c-panel-resets-scroll-position-after-focus-is-lost-and-regained/
        /// To prevent game log scrollbar from snapping back to top
        /// </summary>
        /// <param name="activeControl"></param>
        /// <returns></returns>
        protected override System.Drawing.Point ScrollToControl(System.Windows.Forms.Control activeControl)
        {
            // Returning the current location prevents the panel from
            // scrolling to the active control when the panel loses and regains focus
            return this.DisplayRectangle.Location;
        }

        /// <summary>
        /// Added to fix some of the redraw issues when resizing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_SizeChanged(object sender, EventArgs e)
        {
            ((ListBox)sender).Invalidate();
        }

        private bool SnapWindow(Form window, bool isSnapping, int margin, ref Point nextTopLeft)
        {
            // Only snap if visible
            if (window.Visible)
            {
                if (!isSnapping)
                {
                    // Not snapping yet, initialize top left
                    nextTopLeft = PlayerActiveDeckWindow.Location;
                }
                else
                {
                    window.SetDesktopBounds(nextTopLeft.X, nextTopLeft.Y, PlayerActiveDeckWindow.DesktopBounds.Width, PlayerActiveDeckWindow.DesktopBounds.Height);
                }
                // Offset top-left to the new top right of this window
                nextTopLeft.X += window.DesktopBounds.Width + margin;
                return true;
            }

            // Window not visible, no change
            return isSnapping;
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsWindow myOptions = new OptionsWindow();
            myOptions.SetDeckWindows(PlayerActiveDeckWindow, PlayerDrawnCardsWindow, PlayerGraveyardWindow, OpponentGraveyardWindow);
            myOptions.ShowDialog();
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveLogWindow != null)
            {
                if (ActiveLogWindow.Visible)
                {
                    ActiveLogWindow.Hide();
                    logToolStripMenuItem.Checked = false;
                }
                else
                {
                    ActiveLogWindow.Show();
                    logToolStripMenuItem.Checked = true;
                }
            }
        }

        private void myDeckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerActiveDeckWindow.IsNonEmptyDeckVisible = !PlayerActiveDeckWindow.IsNonEmptyDeckVisible;
            PlayerActiveDeckWindow.Visible = PlayerActiveDeckWindow.IsNonEmptyDeckVisible;
            PlayerActiveDeckWindow.UpdateDeck(null, null);
        }

        private void drawnCardsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerDrawnCardsWindow.IsNonEmptyDeckVisible = !PlayerDrawnCardsWindow.IsNonEmptyDeckVisible;
            PlayerDrawnCardsWindow.Visible = PlayerDrawnCardsWindow.IsNonEmptyDeckVisible;
            PlayerDrawnCardsWindow.UpdateDeck(null, null);
        }

        private void playedCardsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerGraveyardWindow.IsNonEmptyDeckVisible = !PlayerGraveyardWindow.IsNonEmptyDeckVisible;
            PlayerDrawnCardsWindow.Visible = PlayerGraveyardWindow.IsNonEmptyDeckVisible;
            PlayerGraveyardWindow.UpdateDeck(null, null);
        }

        private void opponentPlayedCardsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpponentGraveyardWindow.IsNonEmptyDeckVisible = !OpponentGraveyardWindow.IsNonEmptyDeckVisible;
            OpponentGraveyardWindow.Visible = OpponentGraveyardWindow.IsNonEmptyDeckVisible;
            OpponentGraveyardWindow.UpdateDeck(null, null);
        }

        private void ChangeDeckOpacity(int opacity)
        {
            Properties.Settings.Default.DeckTransparency = opacity;
            if (PlayerActiveDeckWindow != null)
            {
                PlayerActiveDeckWindow.Opacity = opacity / 100.0;
            }
            if (PlayerDrawnCardsWindow != null)
            {
                PlayerDrawnCardsWindow.Opacity = opacity / 100.0;
            }
            if (PlayerGraveyardWindow != null)
            {
                PlayerGraveyardWindow.Opacity = opacity / 100.0;
            }
            if (OpponentGraveyardWindow != null)
            {
                OpponentGraveyardWindow.Opacity = opacity / 100.0;
            }
        }

        private void deckOpacity20ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDeckOpacity(20);
        }

        private void deckOpacity40ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDeckOpacity(40);
        }

        private void deckOpacity60ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDeckOpacity(60);
        }

        private void deckOpacity80ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDeckOpacity(80);
        }

        private void deckOpacity100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDeckOpacity(100);
        }

        private void snapDecksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool snapping = false;
            Point location = new Point();
            int margin = 2;
            snapping = SnapWindow(PlayerActiveDeckWindow, snapping, margin, ref location);
            snapping = SnapWindow(PlayerDrawnCardsWindow, snapping, margin, ref location);
            snapping = SnapWindow(PlayerGraveyardWindow, snapping, margin, ref location);
            snapping = SnapWindow(OpponentGraveyardWindow, snapping, margin, ref location);
        }


        private void fileMenu_DropDownOpened(object sender, EventArgs e)
        {
            logToolStripMenuItem.Checked = (ActiveLogWindow != null) && ActiveLogWindow.Visible;
        }

        private void optionsMenu_DropDownOpened(object sender, EventArgs e)
        {
            myDeckToolStripMenuItem.Checked = (PlayerActiveDeckWindow != null) && PlayerActiveDeckWindow.IsNonEmptyDeckVisible;
            drawnCardsToolStripMenuItem.Checked = (PlayerDrawnCardsWindow != null) && PlayerDrawnCardsWindow.IsNonEmptyDeckVisible;
            playedCardsToolStripMenuItem.Checked = (PlayerGraveyardWindow != null) && PlayerGraveyardWindow.IsNonEmptyDeckVisible;
            opponentPlayedCardsToolStripMenuItem.Checked = (OpponentGraveyardWindow != null) && OpponentGraveyardWindow.IsNonEmptyDeckVisible;
            hideZeroCountCardsToolStripMenuItem.Checked = (PlayerActiveDeckWindow != null) && PlayerActiveDeckWindow.HideZeroCountCards;

            deckOpacity20ToolStripMenuItem.Checked = (Properties.Settings.Default.DeckTransparency == 20);
            deckOpacity40ToolStripMenuItem.Checked = (Properties.Settings.Default.DeckTransparency == 40);
            deckOpacity60ToolStripMenuItem.Checked = (Properties.Settings.Default.DeckTransparency == 60);
            deckOpacity80ToolStripMenuItem.Checked = (Properties.Settings.Default.DeckTransparency == 80);
            deckOpacity100ToolStripMenuItem.Checked = (Properties.Settings.Default.DeckTransparency == 100);

            smallDeckSizeToolStripMenuItem.Checked = (PlayerActiveDeckWindow.CustomDeckScale.CardSize == DeckControl.DeckScale.Small.CardSize);
            mediumDeckSizeToolStripMenuItem.Checked = (PlayerActiveDeckWindow.CustomDeckScale.CardSize == DeckControl.DeckScale.Medium.CardSize);
            largeDeckSizeToolStripMenuItem.Checked = (PlayerActiveDeckWindow.CustomDeckScale.CardSize == DeckControl.DeckScale.Large.CardSize);
        }

        private void windowMenu_DropDownOpened(object sender, EventArgs e)
        {
            hideGamesVsAIToolStripMenuItem.Checked = Properties.Settings.Default.HideAIGames;
        }
        private void hideGamesVsAIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hideGamesVsAIToolStripMenuItem.Checked = !hideGamesVsAIToolStripMenuItem.Checked;
            Properties.Settings.Default.HideAIGames = hideGamesVsAIToolStripMenuItem.Checked;
            DecksListCtrl_SelectionChanged(this, null);
        }

        private void hideZeroCountCardsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerActiveDeckWindow.HideZeroCountCards = !PlayerActiveDeckWindow.HideZeroCountCards;
            PlayerActiveDeckWindow.UpdateDeck(null, null);
        }

        private void ChangeCustomDeckScale(DeckControl.DeckScale deckScale)
        {
            if (PlayerActiveDeckWindow != null)
            {
                PlayerActiveDeckWindow.CustomDeckScale = deckScale;
            }
            if (PlayerDrawnCardsWindow != null)
            {
                PlayerDrawnCardsWindow.CustomDeckScale = deckScale;
            }
            if (PlayerGraveyardWindow != null)
            {
                PlayerGraveyardWindow.CustomDeckScale = deckScale;
            }
            if (OpponentGraveyardWindow != null)
            {
                OpponentGraveyardWindow.CustomDeckScale = deckScale;
            }
        }

        private void smallDeckSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeCustomDeckScale(DeckControl.DeckScale.Small);
        }

        private void mediumDeckSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeCustomDeckScale(DeckControl.DeckScale.Medium);
        }

        private void largeDeckSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeCustomDeckScale(DeckControl.DeckScale.Large);
        }

        private void HighlightedDeckPanel_SizeChanged(object sender, EventArgs e)
        {
            bool sizeChanged = false;
            if (HighlightedDeckPanel.Width >= DeckControl.DeckScale.Medium.CardSize.Width + System.Windows.Forms.SystemInformation.VerticalScrollBarWidth)
            {
                if (DeckControl.DeckScale.Medium.CardSize.Width != HighlightedDeckControl.CustomDeckScale.CardSize.Width)
                {
                    HighlightedDeckControl.CustomDeckScale = DeckControl.DeckScale.Medium;
                    sizeChanged = true;
                }
            }
            else
            {
                if (DeckControl.DeckScale.Small.CardSize.Width != HighlightedDeckControl.CustomDeckScale.CardSize.Width)
                {
                    HighlightedDeckControl.CustomDeckScale = DeckControl.DeckScale.Small;
                    sizeChanged = true;
                }
            }

            if (sizeChanged)
            {
                // Fix the size
                Size bestDeckSize = HighlightedDeckControl.GetBestSize();
                HighlightedDeckControl.SetBounds(0, 0, bestDeckSize.Width, bestDeckSize.Height, BoundsSpecified.Size);
                HighlightedDeckControl.Invalidate();
                HighlightedDeckStatsDisplay.Invalidate();
                int deckStatsHeight = HighlightedDeckStatsDisplay.GetBestHeight(bestDeckSize.Width);
                HighlightedDeckStatsDisplay.SetBounds(HighlightedDeckControl.Left, HighlightedDeckControl.Top + bestDeckSize.Height, bestDeckSize.Width, deckStatsHeight, BoundsSpecified.All);
            }
        }
    }
}
