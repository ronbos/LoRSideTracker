using System;
using System.Linq;

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
        /// <param name="attackingPlayer">Attacking player, for battle logging</param>
        public void LogMoves(CardList<CardInPlay>[] playerZones, CardList<CardInPlay>[] opponentZones, PlayerType attackingPlayer)
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
                // Log attacking player moves first
                if (attackingPlayer == PlayerType.Opponent)
                {
                    LogMovedInCards(LastOppponentZones[i], opponentZones[i], playerBattlingUnits, attackingPlayer == PlayerType.Opponent);
                    LogMovedInCards(LastPlayerZones[i], playerZones[i], opponentBattlingUnits, attackingPlayer == PlayerType.LocalPlayer);
                }
                else
                {
                    LogMovedInCards(LastPlayerZones[i], playerZones[i], opponentBattlingUnits, attackingPlayer == PlayerType.LocalPlayer);
                    LogMovedInCards(LastOppponentZones[i], opponentZones[i], playerBattlingUnits, attackingPlayer == PlayerType.Opponent);
                }
                LastPlayerZones[i] = playerZones[i].Clone();
                LastOppponentZones[i] = opponentZones[i].Clone();
            }
        }

        /// <summary>
        /// Log moves for cards that appeared in a new zone, for a specific player
        /// </summary>
        /// <param name="last"></param>
        /// <param name="current"></param>
        /// <param name="opponentBattlingUnits">Opponent's battling units, used to find opposing unit in attack</param>
        /// <param name="isPlayerAttacking">If true, this player is attacking</param>
        private void LogMovedInCards(CardList<CardInPlay> last, CardList<CardInPlay> current, CardList<CardInPlay> opponentBattlingUnits, bool isPlayerAttacking)
        {
            bool[] logged = Enumerable.Repeat(false, current.Count).ToArray();
            // First remove exact mathces. This is important when multiple of the same card are in the same zone
            // Order of matching in that case does matter
            for (int i = 0; i < current.Count; i++)
            {
                CardInPlay card = current[i];
                int index = last.FindIndex(x => x.CardCode == card.CardCode && x.LastNonEtherZone == card.LastNonEtherZone && x.LastZone == card.LastZone);
                if (index >= 0)
                {
                    last.RemoveAt(index);
                    logged[i] = true;
                }
            }

            // Now look for moved cards
            for (int i = 0; i < current.Count; i++)
            {
                if (logged[i]) continue;

                CardInPlay card = current[i];
                int index = last.FindIndex(x => x.CardCode == card.CardCode);
                if (index >= 0)
                {
                    last.RemoveAt(index);
                }
                else if (card.CurrentZone != card.LastNonEtherZone)
                {
                    LogMove(card, opponentBattlingUnits, isPlayerAttacking);
                }
            }
        }

        /// <summary>
        /// Log a single moved card
        /// </summary>
        /// <param name="card">Card to log</param>
        /// <param name="opponentBattlingUnits">Opponent's battling units, used to find opposing unit in attack</param>
        /// <param name="isPlayerAttacking">If true, this player is attacking</param>
        private void LogMove(CardInPlay card, CardList<CardInPlay> opponentBattlingUnits, bool isPlayerAttacking)
        {
            int index;
            LogType logType = (card.Owner == PlayerType.LocalPlayer) ? LogType.Player : LogType.Opponent;
            string action = null;
            string target = "";
            switch (card.CurrentZone)
            {
                case PlayZone.Deck:
                    if (card.LastNonEtherZone == PlayZone.Zoom) action = "Shuffled into Deck";
                    break;
                case PlayZone.Tossing:
                    if (card.LastNonEtherZone == PlayZone.Deck) action = "Drawing from deck";
                    break;
                case PlayZone.Zoom:
                    if (card.LastNonEtherZone == PlayZone.Deck) action = "Drawn from deck";
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
                    if (card.LastNonEtherZone != PlayZone.Attack)
                    {
                        action = "Attacking";
                        logType = LogType.Debug;
                    }
                    /*index = opponentBattlingUnits.FindIndex(x => Math.Abs(card.NormalizedCenter.X - x.NormalizedCenter.X) < 0.05);
                    if (index == -1)
                    {
                        target = " against Face";
                    }
                    else
                    {
                        target = string.Format(" against {0}", opponentBattlingUnits[index].TheCard.Name);
                    }*/
                    break;
                case PlayZone.Attack:
                    // Look for the opposing card the opposing 
                    action = isPlayerAttacking ? "Attacked" : "Defended";
                    //if (card.LastNonEtherZone != PlayZone.Windup)
                    {
                        index = opponentBattlingUnits.FindIndex(x => Math.Abs(card.NormalizedCenter.X - x.NormalizedCenter.X) < 0.05);
                        if (index == -1)
                        {
                            target = " hits Face";
                        }
                        else
                        {
                            target = string.Format(" hits {0}", opponentBattlingUnits[index].TheCard.Name);
                        }
                    }
                    //else
                    //{
                    //    logType = LogType.Debug;
                    //}
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
