using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoRSideTracker
{
    /// <summary>
    /// Options Window
    /// </summary>
    public partial class OptionsWindow : Form
    {
        DeckWindow PlayerActiveDeckWindow;
        DeckWindow PlayerDrawnCardsWindow;
        DeckWindow PlayerPlayedCardsWindow;
        DeckWindow OpponentPlayedCardsWindow;

        /// <summary>
        /// Constructor
        /// </summary>
        public OptionsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set associated deck windows
        /// </summary>
        /// <param name="playerActiveDeckWindow"></param>
        /// <param name="playerDrawnCardsWindow"></param>
        /// <param name="playerPlayedCardsWindow"></param>
        /// <param name="opponentPlayedCardsWindow"></param>
        public void SetDeckWindows(
            DeckWindow playerActiveDeckWindow,
            DeckWindow playerDrawnCardsWindow,
            DeckWindow playerPlayedCardsWindow,
            DeckWindow opponentPlayedCardsWindow)
        {
            PlayerActiveDeckWindow = playerActiveDeckWindow;
            PlayerDrawnCardsWindow = playerDrawnCardsWindow;
            PlayerPlayedCardsWindow = playerPlayedCardsWindow;
            OpponentPlayedCardsWindow = opponentPlayedCardsWindow;
        }

        private void OptionsWindow_Load(object sender, EventArgs e)
        {
            this.Location = Properties.Settings.Default.OptionsWindowLocation;

            PlayerDeckCheckBox.Checked = Properties.Settings.Default.ShowPlayerDeck;
            HideZeroCountCheckBox.Checked = Properties.Settings.Default.HideZeroCountInDeck;
            PlayerDrawnCheckBox.Checked = Properties.Settings.Default.ShowPlayerDrawnCards;
            PlayerPlayedCheckBox.Checked = Properties.Settings.Default.ShowPlayerPlayedCards;
            OpponentPlayedCheckBox.Checked = Properties.Settings.Default.ShowOpponentPlayedCards;
            DeckStatsCheckBox.Checked = Properties.Settings.Default.ShowDeckStats;
            TransparencyTrackBar.Value = Properties.Settings.Default.DeckTransparency;
        }

        private void OptionsWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.OptionsWindowLocation = this.Location;

            Properties.Settings.Default.ShowPlayerDeck = PlayerActiveDeckWindow.Visible;
            Properties.Settings.Default.HideZeroCountInDeck = HideZeroCountCheckBox.Checked;
            Properties.Settings.Default.ShowPlayerDrawnCards = PlayerDrawnCheckBox.Checked;
            Properties.Settings.Default.ShowPlayerPlayedCards = PlayerPlayedCheckBox.Checked;
            Properties.Settings.Default.ShowOpponentPlayedCards = OpponentPlayedCheckBox.Checked;
            Properties.Settings.Default.ShowDeckStats = DeckStatsCheckBox.Checked;
            Properties.Settings.Default.DeckTransparency = TransparencyTrackBar.Value;
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

        private void DeckStatsCheckBox_CheckedChanged(object sender, EventArgs e)
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

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
