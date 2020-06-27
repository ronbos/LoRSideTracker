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
    /// Debug view of all cards in all zones
    /// </summary>
    public partial class CardsInPlayDebugView : Form
    {
        Label PlayerTitle;
        Label PlayerList;
        Label OpponentTitle;
        Label OpponentList;

        /// <summary>
        /// Constructor
        /// </summary>
        public CardsInPlayDebugView()
        {
            InitializeComponent();

            int count = Enum.GetValues(typeof(PlayZone)).Length;
            Rectangle rect = new Rectangle(3, 3, 220, 1000);
            int titleHeight = 16;

            PlayerTitle = new Label();
            PlayerTitle.Parent = this;
            PlayerTitle.AutoSize = false;
            PlayerTitle.SetBounds(rect.X, rect.Y, rect.Width, titleHeight);
            PlayerTitle.Text = "Player Cards";
            PlayerTitle.Font = new Font(PlayerTitle.Font, FontStyle.Bold);
            PlayerTitle.Visible = true;

            PlayerList = new Label();
            PlayerList.Parent = this;
            PlayerList.AutoSize = false;
            PlayerList.SetBounds(rect.X, rect.Y + titleHeight, rect.Width, rect.Height - titleHeight);
            PlayerList.Text = "";
            PlayerList.Visible = true;

            rect.X += rect.Width + rect.Top;

            OpponentTitle = new Label();
            OpponentTitle.Parent = this;
            OpponentTitle.AutoSize = false;
            OpponentTitle.SetBounds(rect.X, rect.Y, rect.Width, titleHeight);
            OpponentTitle.Text = "Opponent Cards";
            OpponentTitle.Font = new Font(OpponentTitle.Font, FontStyle.Bold);
            OpponentTitle.Visible = true;

            OpponentList = new Label();
            OpponentList.Parent = this;
            OpponentList.AutoSize = false;
            OpponentList.SetBounds(rect.X, rect.Y + titleHeight, rect.Width, rect.Height - titleHeight);
            OpponentList.Text = "";
            OpponentList.Visible = true;
        }

        /// <summary>
        /// Set all the card zone contents
        /// </summary>
        /// <param name="playerZones"></param>
        /// <param name="opponentZones"></param>
        public void SetCards(CardList<CardInPlay>[] playerZones, CardList<CardInPlay>[] opponentZones)
        {
            int NumZones = Enum.GetValues(typeof(PlayZone)).Length;
            string playerFullList = "";
            string opponentFullList = "";
            for (int i = 0; i < NumZones; i++)
            {
                string playerList = DeckListToString(playerZones[i]);
                if (playerList.Length > 0)
                {
                    playerFullList += "### " + ((PlayZone)i).ToString().ToUpper() + " ###\r\n\r\n";
                    playerFullList += playerList + "\r\n";
                }

                string opponentList = DeckListToString(opponentZones[i]);
                if (opponentList.Length > 0)
                {
                    opponentFullList += "### " + ((PlayZone)i).ToString().ToUpper() + " ###\r\n\r\n";
                    opponentFullList += opponentList + "\r\n";
                }
            }
            Utilities.CallActionSafelyAndWait(PlayerList, new Action(() => { PlayerList.Text = "\r\n" + playerFullList; }));
            Utilities.CallActionSafelyAndWait(OpponentList, new Action(() => { OpponentList.Text = "\r\n" + opponentFullList; }));
        }

        private string DeckListToString(IEnumerable<CardInPlay> cards)
        {
            string list = "";
            int count = 0;
            for (int i = 0; i < cards.Count(); i++)
            {
                if (i == cards.Count() - 1 || cards.ElementAt(i).CardCode != cards.ElementAt(i + 1).CardCode)
                {
                    list += string.Format("({0}) {1} x{2}\r\n", cards.ElementAt(i).TheCard.Cost, cards.ElementAt(i).TheCard.Name, count + 1);
                    count = 0;
                }
                else
                {
                    count++;
                }
            }
            return list;
        }
    }
}
