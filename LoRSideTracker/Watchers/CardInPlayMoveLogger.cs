using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoRSideTracker
{
    /// <summary>
    /// Logs all the moves according to zone transitions
    /// </summary>
    public class CardInPlayMoveLogger
    {
        static private int NumZones = Enum.GetValues(typeof(PlayZone)).Length;
        CardList<CardInPlay>[] LastPlayerZones = new CardList<CardInPlay>[NumZones];
        CardList<CardInPlay>[] LastOppponentZones = new CardList<CardInPlay>[NumZones];

        /// <summary>
        /// Constructor
        /// </summary>
        public CardInPlayMoveLogger()
        {
            for (int i = 0; i < NumZones; i++)
            {
                LastPlayerZones[i] = new CardList<CardInPlay>();
                LastOppponentZones[i] = new CardList<CardInPlay>();
            }
        }

        /// <summary>
        /// Clear last zones
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < NumZones; i++)
            {
                LastPlayerZones[i].Clear();
                LastOppponentZones[i].Clear();
            }
        }

        /// <summary>
        /// Log moves for cards that appeared in a new zone
        /// </summary>
        /// <param name="playerZones">Array of player zone contents</param>
        /// <param name="opponentZones">Array of opponent zone contents</param>
        public void LogMoves(CardList<CardInPlay>[] playerZones, CardList<CardInPlay>[] opponentZones)
        {
            CardList<CardInPlay> playerBattlingUnits = new CardList<CardInPlay>();
            CardList<CardInPlay> opponentBattlingUnits = new CardList<CardInPlay>();
            playerBattlingUnits.AddRange(playerZones[(int)PlayZone.Attack]);
            playerBattlingUnits.AddRange(playerZones[(int)PlayZone.Windup]);
            playerBattlingUnits.AddRange(playerZones[(int)PlayZone.Battle]);
            opponentBattlingUnits.AddRange(opponentZones[(int)PlayZone.Attack]);
            opponentBattlingUnits.AddRange(opponentZones[(int)PlayZone.Windup]);
            opponentBattlingUnits.AddRange(opponentZones[(int)PlayZone.Battle]);

            for (int i = 0; i < NumZones; i++)
            {
                LogMovedInCards(LastPlayerZones[i], playerZones[i], opponentBattlingUnits);
                LogMovedInCards(LastOppponentZones[i], opponentZones[i], playerBattlingUnits);
                LastPlayerZones[i] = playerZones[i].Clone();
                LastOppponentZones[i] = opponentZones[i].Clone();
            }
        }

        /// <summary>
        /// Log moves for cards that appeared in a new zone, for a specific player
        /// </summary>
        /// <param name="last"></param>
        /// <param name="current"></param>
        /// <param name="opponentBattlingUnits"></param>
        private void LogMovedInCards(CardList<CardInPlay> last, CardList<CardInPlay> current, CardList<CardInPlay> opponentBattlingUnits)
        {
            foreach (var card in current)
            {
                int index = last.FindIndex(x => x.CardCode == card.CardCode);
                if (index >= 0)
                {
                    last.RemoveAt(index);
                }
                else if (card.CurrentZone != card.LastNonEtherZone)
                {
                    LogMove(card, opponentBattlingUnits);
                }
            }
        }

        /// <summary>
        /// Log a single moved card
        /// </summary>
        /// <param name="card">Card to log</param>
        /// <param name="opponentBattlingUnits">Opponent's battling units, used to find opposing unit in attack</param>
        private void LogMove(CardInPlay card, CardList<CardInPlay> opponentBattlingUnits)
        {
            //if (card.LastNonEtherZone == card.CurrentZone)
            //{
            //    continue;
            //}
            LogType logType = (card.Owner == PlayerType.LocalPlayer) ? LogType.Player : LogType.Opponent;
            string action = null;
            string target = "";
            switch (card.CurrentZone)
            {
                case PlayZone.Deck:
                    //if (card.LastNonEtherZone != PlayZone.Unknown) action = "Shuffled into Deck";
                    break;
                case PlayZone.Tossing:
                    break;
                case PlayZone.Zoom:
                    action = "Drawn from deck";
                    break;
                case PlayZone.Stage:
                    if (card.LastNonEtherZone == PlayZone.Deck) action = "Presented";
                    break;
                case PlayZone.Hand:
                    if (card.LastNonEtherZone == PlayZone.Field || card.LastNonEtherZone == PlayZone.Battle) action = "Recalled to Hand";
                    else if (card.LastNonEtherZone != PlayZone.Zoom) action = "Added to Hand";
                    break;
                case PlayZone.Cast:
                    action = "Cast";
                    break;
                case PlayZone.Field:
                    if (card.LastNonEtherZone == PlayZone.Stage || card.LastNonEtherZone == PlayZone.Hand)
                    {
                        action = "Played";
                    }
                    else if (card.LastNonEtherZone == PlayZone.Unknown || card.LastNonEtherZone == PlayZone.Tossing)
                    {
                        action = "Summoned";
                    }
                    break;
                case PlayZone.Battle:
                    //action = "Placed";
                    break;
                case PlayZone.Windup:
                    logType = LogType.Debug;
                    action = "Attacking";
                    break;
                case PlayZone.Attack:
                    // Look for the opposing card the opposing 
                    action = "Attacked";
                    int index = opponentBattlingUnits.FindIndex(x => Math.Abs(card.NormalizedCenter.X - x.NormalizedCenter.X) < 0.05);
                    if (index == -1)
                    {
                        target = " hits Face";
                    }
                    else
                    {
                        target = string.Format(" hits {0}", opponentBattlingUnits[index].TheCard.Name);
                    }
                    break;
                case PlayZone.Graveyard:
                    logType = LogType.Debug;
                    action = (card.TheCard.Type == "Spell") ? "Resolved" : "Removed";
                    break;
                case PlayZone.Ether:
                case PlayZone.Unknown:
                    break;
            }
            if (action != null)
            {
                Log.WriteLine(logType, "[{0}{1}] {2}: {3}{4}", card.LastNonEtherZone.ToString()[0], card.CurrentZone.ToString()[0],
                    action, card.TheCard.Name, target);
            }
        }
    }
}
