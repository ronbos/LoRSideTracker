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
    public partial class MainWindow : Form, IExpeditionUpdateCallback, StaticDeckUpdateCallback, IOverlayUpdateCallback, ISetDownloaderCallback
    {
        private Expedition CurrentExpedition;
        private StaticDeck CurrentDeck;
        private Overlay CurrentOverlay;

        private DeckWindow PlayerActiveDeckWindow;
        private DeckWindow PlayerDrawnCardsWindow;
        private DeckWindow PlayerPlayedCardsWindow;
        private DeckWindow OpponentPlayedCardsWindow;

        private LogWindow ActiveLogWindow;
        private OverlayWatchWindow OverlayWindow;
        private int ExpeditionsCount = 0;

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
                (CurrentExpedition == null || !cards.SequenceEqual(CurrentExpedition.Cards)))
            {
                string title = "Constructed Deck";
                try { title = GameHistory.DeckNames[deckCode]; } catch { }
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
            if (CurrentDeck.Cards.Count == 0 || CurrentDeck.Cards.SequenceEqual(cards))
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
                    AddToDeckList(gameRecord);
                    SwitchDeckView(gameRecord.IsExpedition());
                }));
            }
        }

        /// <summary>
        /// Callback for when elements have been updated
        /// </summary>
        /// <param name="playerElements"></param>
        /// <param name="opponentElements"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        public void OnElementsUpdate(List<OverlayElement> playerElements, List<OverlayElement> opponentElements, int screenWidth, int screenHeight)
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
            if (showWindow)
            {
                deckWindow.Show();
            }
            deckWindow.SetBounds(posX, posY, 0, 0, BoundsSpecified.Location);
            deckWindow.Opacity = opacity;
            return deckWindow;

        }

        /// <summary>
        /// Runs after all sets are downloaded and processed.
        /// Initializes the StaticDeck windows and deck tracking objects.
        /// </summary>
        public async void OnAllSetsDownloaded()
        {
            await Task.Run(() => CardLibrary.LoadAllCards(MyProgressDisplay));
            GameHistory.LoadAllGames();

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
            PlayerDrawnCardsWindow = CreateDeckWindow("Cards Drawn", deckScale, deckOpacity,
                Properties.Settings.Default.ShowPlayerDrawnCards,
                Properties.Settings.Default.PlayerDrawnCardsLocation.X, 
                Properties.Settings.Default.PlayerDrawnCardsLocation.Y);
            PlayerPlayedCardsWindow = CreateDeckWindow("You Played", deckScale, deckOpacity,
                Properties.Settings.Default.ShowPlayerPlayedCards,
                Properties.Settings.Default.PlayerPlayedCardsLocation.X, 
                Properties.Settings.Default.PlayerPlayedCardsLocation.Y);
            OpponentPlayedCardsWindow = CreateDeckWindow("Opponent Played", deckScale, deckOpacity,
                Properties.Settings.Default.ShowOpponentPlayedCards,
                Properties.Settings.Default.OpponentPlayedCardsLocation.X, 
                Properties.Settings.Default.OpponentPlayedCardsLocation.Y);

            PlayerActiveDeckWindow.HideZeroCountCards = Properties.Settings.Default.HideZeroCountInDeck;

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

            if (Keyboard.IsKeyDown(Key.D))
            {
                OverlayWindow = new OverlayWatchWindow();
                OverlayWindow.Show();
            }

            CurrentDeck = new StaticDeck(this);
            Thread.Sleep(500);
            CurrentExpedition = new Expedition(this);
            Thread.Sleep(500);
            CurrentOverlay = new Overlay(this);

            // Hide the progress display and make all the other UI elements visible
            MyProgressDisplay.Visible = false;
            SnapWindowsButton.Visible = true;
            OptionsButton.Visible = true;
            LogButton.Visible = true;
            DecksListBox.Visible = true;
            DeckPanel.Visible = true;
            DecksButton.Visible = true;
            ExpeditionsButton.Visible = true;

            // Load all games. We do this in reverse as AddDeckToList expects them
            // in chronological order
            for (int i = GameHistory.Games.Count - 1; i >= 0; i--)
            {
                AddToDeckList(GameHistory.Games[i]);
            }

            // Set up constructed deck list to be visible
            SwitchDeckView(false);
        }

        /// <summary>
        /// Add a deck to the top of the list of either decks or expeditions
        /// If game record matches existing entry, old entry is removed from list
        /// </summary>
        /// <param name="gr">Game record to add</param>
        private void AddToDeckList(GameRecord gr)
        {
            ListBox listBox = gr.IsExpedition() ? ExpeditionsListBox : DecksListBox;
            string deckName = gr.IsExpedition() ? string.Format("Expedition #{0}", ExpeditionsCount + 1) : gr.MyDeckName;

            // Does the deck already exist in the list? If it does, remove it
            string grSig = gr.GetDeckSignature();
            int index = listBox.Items.Cast<GameRecord>().ToList().FindIndex(x => grSig == x.GetDeckSignature());
            if (index != -1)
            {
                // Keep the deck name
                deckName = ((GameRecord)listBox.Items[index]).ToString();

                // Remove old item
                listBox.Items.RemoveAt(index);
            }
            else
            {
                if (gr.IsExpedition())
                {
                    // This is guaranteed to be a new expedition, increase expeditions count
                    ExpeditionsCount++;

                    // Default expedition name if it is not customized
                    deckName = string.Format("Expedition #{0}", ExpeditionsCount);
                }
                else
                {
                    deckName = gr.MyDeckName;
                }

                // Map the name if it has been customized (if not, default name is kept)
                try { deckName = GameHistory.DeckNames[gr.GetDeckSignature()]; } catch { }
            }

            // Add the new item
            gr.DisplayString = deckName;
            listBox.Items.Insert(0, gr);
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

        private void SnapWindowsButton_Click(object sender, EventArgs e)
        {
            bool snapping = false;
            Point location = new Point();
            int margin = 2;
            snapping = SnapWindow(PlayerActiveDeckWindow, snapping, margin, ref location);
            snapping = SnapWindow(PlayerDrawnCardsWindow, snapping, margin, ref location);
            snapping = SnapWindow(PlayerPlayedCardsWindow, snapping, margin, ref location);
            snapping = SnapWindow(OpponentPlayedCardsWindow, snapping, margin, ref location);
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
            Properties.Settings.Default.ActiveLogWindowBounds = (ActiveLogWindow.WindowState == FormWindowState.Normal) 
                ? ActiveLogWindow.Bounds : ActiveLogWindow.RestoreBounds;

            if (PlayerActiveDeckWindow != null) Properties.Settings.Default.PlayerDeckLocation = PlayerActiveDeckWindow.Location;
            if (PlayerDrawnCardsWindow != null) Properties.Settings.Default.PlayerDrawnCardsLocation = PlayerDrawnCardsWindow.Location;
            if (PlayerPlayedCardsWindow != null) Properties.Settings.Default.PlayerPlayedCardsLocation = PlayerPlayedCardsWindow.Location;
            if (OpponentPlayedCardsWindow != null) Properties.Settings.Default.OpponentPlayedCardsLocation = OpponentPlayedCardsWindow.Location;

            // Save the settings
            Properties.Settings.Default.Save();
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            OptionsWindow myOptions = new OptionsWindow();
            myOptions.SetDeckWindows(PlayerActiveDeckWindow, PlayerDrawnCardsWindow, PlayerPlayedCardsWindow, OpponentPlayedCardsWindow);
            myOptions.ShowDialog();
        }

        /// <summary>
        /// Show or hide log window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Switch to viewing decks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecksButton_Click(object sender, EventArgs e)
        {
            SwitchDeckView(false);
        }

        /// <summary>
        /// Switch to viewing expeditions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpeditionsButton_Click(object sender, EventArgs e)
        {
            SwitchDeckView(true);
        }

        private void SwitchDeckView(bool showExpeditions)
        {
            ListBox fromListBox, toListBox;
            Button fromButton, toButton;
            if (showExpeditions)
            {
                fromListBox = DecksListBox;
                fromButton = DecksButton;
                toListBox = ExpeditionsListBox;
                toButton = ExpeditionsButton;
            }
            else
            {
                fromListBox = ExpeditionsListBox;
                fromButton = ExpeditionsButton;
                toListBox = DecksListBox;
                toButton = DecksButton;
            }
            if (fromListBox.Visible)
            {
                fromButton.BackColor = BackColor;
                fromButton.FlatAppearance.MouseOverBackColor = toButton.FlatAppearance.MouseOverBackColor;
                fromButton.FlatAppearance.MouseDownBackColor = toButton.FlatAppearance.MouseDownBackColor;
                toButton.BackColor = Color.FromArgb(BackColor.R * 2, BackColor.G * 2, BackColor.B * 2);
                toButton.FlatAppearance.MouseOverBackColor = toButton.BackColor;
                toButton.FlatAppearance.MouseDownBackColor = toButton.BackColor;
                fromListBox.Visible = false;
                toListBox.Visible = true;
                fromListBox.SelectedIndex = -1;
            }

            if (toListBox.Items.Count > 0)
            {
                toListBox.SelectedIndex = 0;
            }
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            GameRecord gr = (GameRecord)listBox.SelectedItem;
            if (gr == null)
            {
                if (listBox.Items.Count > 0)
                {
                    gr = (GameRecord)listBox.Items[0];
                }
                else
                {
                    HighlightedGameLogControl.Clear();
                    HighlightedDeckControl.ClearDeck();
                    HighlightedDeckStatsDisplay.Visible = false;
                    return;
                }
            }

            // Highlight the deck
            HighlightedGameLogControl.LoadGames(gr.GetDeckSignature());

            // Update deck
            HighlightedDeckControl.ClearDeck();
            for (int i = 0; i < gr.MyDeck.Count; i++)
            {
                HighlightedDeckControl.SetCard(i, gr.MyDeck[i]);
            }

            HighlightedDeckControl.Title = gr.ToString();
            Size bestDeckSize = HighlightedDeckControl.GetBestSize();
            HighlightedDeckControl.SetBounds(0, 0, bestDeckSize.Width, bestDeckSize.Height, BoundsSpecified.Size);
            HighlightedDeckStatsDisplay.TheDeck = gr.MyDeck;
            HighlightedDeckStatsDisplay.Invalidate();
            int deckStatsHeight = HighlightedDeckStatsDisplay.GetBestHeight(bestDeckSize.Width);
            HighlightedDeckStatsDisplay.SetBounds(HighlightedDeckControl.Left, HighlightedDeckControl.Top + bestDeckSize.Height, bestDeckSize.Width, deckStatsHeight, BoundsSpecified.All);
            HighlightedDeckPanel.Visible = true;
        }

        private void ListBox_DoubleClick(object sender, EventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            int index = listBox.SelectedIndex;
            if (index >= 0)
            {
                GameRecord gr = (GameRecord)((GameRecord)listBox.Items[index]).Clone();
                string result = Microsoft.VisualBasic.Interaction.InputBox("Name:", "Change Deck Name", gr.DisplayString);
                if (!string.IsNullOrEmpty(result))
                {
                    gr.DisplayString = result;
                    listBox.Items[index] = gr;
                    listBox.Refresh();

                    GameHistory.SetDeckName(gr.GetDeckSignature(), result);
                }
            }
        }

        private void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            e.DrawBackground() ;
            Rectangle rect = e.Bounds;
            rect.Height--;
            if (e.Index >= 0)
            {
                Color borderColor = e.ForeColor;
                if (!e.State.HasFlag(DrawItemState.Selected))
                {
                    borderColor = Color.FromArgb(
                        (e.ForeColor.R + e.BackColor.R) / 2,
                        (e.ForeColor.G + e.BackColor.G) / 2,
                        (e.ForeColor.B + e.BackColor.B) / 2);
                }
                e.Graphics.FillRectangle(new SolidBrush(borderColor), rect);
                rect.Inflate(-1, -1);

                GameRecord gr = (GameRecord)listBox.Items[e.Index];
                // Find the card to use for art
                int drawIndex = -1;
                int championCount = 0;
                for (int i = gr.MyDeck.Count - 1; i >= 0; i--)
                {
                    if (gr.MyDeck[i].TheCard.SuperType == "Champion" && gr.MyDeck[i].Count > championCount)
                    {
                        drawIndex = i;
                        championCount = gr.MyDeck[i].Count;
                    }
                    else if (gr.MyDeck[i].TheCard.Type == "Unit" && drawIndex == -1)
                    {
                        drawIndex = i;
                    }
                }
                if (drawIndex >= 0)
                {
                    gr.MyDeck[drawIndex].TheCard.DrawCardArt(e.Graphics, rect);
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(
                        e.State.HasFlag(DrawItemState.Selected) ? 128 : 192, Color.Black)), rect);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(e.BackColor), rect);
                }

                // Determine deck regions
                Dictionary<string, int> regions = new Dictionary<string, int>();
                foreach (var c in gr.MyDeck)
                {
                    regions.TryGetValue(c.TheCard.Region, out int currentCount);
                    regions[c.TheCard.Region] = currentCount + 1;
                }

                // Sort the regions from lowest to highest
                var regionsInReverseOrder = regions.OrderBy(i => i.Value).ToList();

                // Draw all regions from right to left
                int right = rect.Right;
                for (int i = 0; i < regionsInReverseOrder.Count; i++)
                {
                    Image img = CardLibrary.GetRegion(regionsInReverseOrder[i].Key).Banner;
                    int width = img.Width * rect.Height / img.Height * 7 / 8;
                    int height = width * img.Height / img.Height;
                    Rectangle imgRect = new Rectangle(right - width, rect.Top, width, height);
                    e.Graphics.DrawImage(img, imgRect, new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                    right -= width * 7 / 8;
                }

                TextRenderer.DrawText(e.Graphics, gr.ToString(), e.Font, rect, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
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
    }
}
