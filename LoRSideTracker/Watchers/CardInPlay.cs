using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace LoRSideTracker
{
    /// <summary>
    /// Play zone
    /// </summary>
    public enum PlayZone
    {
        /// <summary>Card is in the deck</summary>
        Deck,

        /// <summary>Card has zero size, which sometimes means it's being tossed (very buggy)</summary>
        Tossing,
        /// <summary>Card is in the process of being drawn from deck, very large on screen</summary>
        Zoom,
        /// <summary>Card is on the "stage", either being played or during initial mulligan stage</summary>
        Stage,
        /// <summary>Card in in hand</summary>
        Hand,
        /// <summary>Spell is being cast</summary>
        Cast,

        /// <summary>Unit in on the field, but not in the battle zone</summary>
        Field,
        /// <summary>Unit is placed for battle</summary>
        Battle,
        /// <summary>Wind up before attack</summary>
        Windup,
        /// <summary>Attack happens after windup</summary>
        Attack,

        /// <summary>Units that have been destroyed and spells that have been cast</summary>
        Graveyard,
        /// <summary>Cards that have disappeared</summary>
        Ether,
        /// <summary>Used as default when new cards appear in any zone</summary>
        Unknown
    }


    /// <summary>
    /// Player type
    /// </summary>
    public enum PlayerType
    {
        /// <summary>Local Player</summary>
        LocalPlayer,
        /// <summary>Remote Opponent</summary>
        Opponent,
    }

    /// <summary>
    /// 
    /// </summary>
    public class CardInPlay : ICloneable
    {
        /// <summary></summary>
        public Card TheCard { get; private set; }
        /// <summary></summary>
        public string CardCode { get; private set; }
        /// <summary></summary>
        public Rectangle BoundingBox { get; set; }
        /// <summary></summary>
        public RectangleF NormalizedBoundingBox { get; set; }
        /// <summary></summary>
        public PointF NormalizedCenter { get; set; }
        /// <summary></summary>
        public PlayerType Owner { get; private set; }
        /// <summary></summary>
        public PlayZone CurrentZone { get; set; }
        /// <summary></summary>
        public PlayZone LastZone { get; set; }
        /// <summary></summary>
        public PlayZone LastNonEtherZone { get; set; }
        /// <summary></summary>
        public DateTime EtherStartTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="card"></param>
        /// <param name="currentZone"></param>
        public CardInPlay(PlayerType owner, Card card, PlayZone currentZone)
        {
            TheCard = card;
            CardCode = card.Code;
            BoundingBox = new Rectangle();
            NormalizedBoundingBox = new RectangleF();
            NormalizedCenter = new PointF();
            Owner = owner;
            CurrentZone = currentZone;
            LastZone = PlayZone.Unknown;
            LastNonEtherZone = PlayZone.Unknown;
            EtherStartTime = DateTime.Now;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        /// <param name="screenHeightForNormalized"></param>
        public CardInPlay(Dictionary<string, JsonElement> dict,
            int screenWidth,
            int screenHeight,
            float screenHeightForNormalized)
        {
            CardCode = dict["CardCode"].GetString();
            TheCard = CardLibrary.GetCard(CardCode);
            Owner = dict["LocalPlayer"].GetBoolean() ? PlayerType.LocalPlayer : PlayerType.Opponent;
            BoundingBox = new Rectangle(
                dict["TopLeftX"].GetInt32(),
                screenHeight - dict["TopLeftY"].GetInt32(), // Elements are reported upside down for some reason
                dict["Width"].GetInt32(),
                dict["Height"].GetInt32());

            float left = (float)(BoundingBox.X - screenWidth / 2) / (float)screenHeightForNormalized;
            float right = (float)(BoundingBox.Right - screenWidth / 2) / (float)screenHeightForNormalized;
            float top = (float)(BoundingBox.Y - (screenHeight - screenHeightForNormalized) / 2) / (float)screenHeightForNormalized;
            float bottom = (float)(BoundingBox.Bottom - (screenHeight - screenHeightForNormalized) / 2) / (float)screenHeightForNormalized;

            if (Owner == PlayerType.LocalPlayer)
            {
                NormalizedBoundingBox = new RectangleF(left, top, right - left, bottom - top);
                NormalizedCenter = new PointF((left + right) / 2, (top + bottom) / 2);
            }
            else
            {
                // If this is opponent, flip the board so coordinate system is the same
                NormalizedBoundingBox = new RectangleF(left, 1.0f - bottom, right - left, bottom - top);
                NormalizedCenter = new PointF((left + right) / 2, 1.0f - (top + bottom) / 2);
            }

            LastZone = PlayZone.Unknown;
            LastNonEtherZone = PlayZone.Unknown;

            // Find the zone for each card
            if (NormalizedCenter.Y > 1.0f)
            {
                // Card in hand is at 0.22-0.235 height when in hand
                // When hand is moused over, it grows to ~0.28-0.295
                // When dragged around, size varies between 0.20-0.24, being smallest at the center (y == 0.5)
                CurrentZone = PlayZone.Hand;
            }
            else if (NormalizedBoundingBox.Height == 0)
            {
                // Current theory is that cards of size zero are being tossed, but that almost never works
                CurrentZone = PlayZone.Tossing;
            }
            else if (NormalizedBoundingBox.Height > 0.4f)
            {
                // Cards are presented very large (~0.49 height) when being drawn
                CurrentZone = PlayZone.Zoom;
            }
            else if (NormalizedBoundingBox.Height > 0.31f)
            {
                // Cards are presented fairly large (~0.345 height) when being presented for mulligan
                // This size is also used when playing a unit
                CurrentZone = PlayZone.Stage;
            }
            else if (NormalizedBoundingBox.Height > 0.20f)
            {
                // Card in hand is at 0.22-0.235 height when in hand
                // When hand is moused over, it grows to ~0.28-0.295
                // When dragged around, size varies between 0.20-0.24, being smallest at the center (y == 0.5)
                CurrentZone = PlayZone.Hand;
            }
            else if (NormalizedBoundingBox.Height > 0.13f)
            {
                //CurrentZone = PlayZone.Field;

                // Camp, Battle, Attack zones all use nominal size of 0.145-0.1485
                //For camp, normalized center Y is ~ 0.83
                if (NormalizedBoundingBox.Height > 0.155f || NormalizedCenter.Y > 0.8f)
                {
                    CurrentZone = PlayZone.Field;
                }
                else if (NormalizedCenter.Y > 0.71f)
                {
                    CurrentZone = PlayZone.Windup;
                }
                else if (NormalizedCenter.Y > 0.62f)
                {
                    CurrentZone = PlayZone.Battle;
                }
                else
                {
                    CurrentZone = PlayZone.Attack;
                }
            }
            else if (NormalizedBoundingBox.Height < 0.125f && Math.Abs(NormalizedCenter.Y - 0.5f) < 0.05f)
            {
                CurrentZone = PlayZone.Cast;
            }
            else
            {
                CurrentZone = PlayZone.Unknown;
            }
        }

        /// <summary>
        /// Move card to a new zone and propagate last zone parameters
        /// </summary>
        /// <param name="zone"></param>
        public void MoveToZone(PlayZone zone)
        {
            SetLastZone(CurrentZone);
            if (CurrentZone != zone)
            {
                if (zone == PlayZone.Ether)
                {
                    EtherStartTime = DateTime.Now;
                }
                CurrentZone = zone;
            }
        }

        /// <summary>
        /// Set last zone
        /// </summary>
        /// <param name="zone"></param>
        public void SetLastZone(PlayZone zone)
        {
            if (zone != LastZone && zone != PlayZone.Ether)
            {
                LastNonEtherZone = zone;
            }
            LastZone = zone;
        }

        /// <summary>
        /// Check if card code matches
        /// </summary>
        /// <param name="cardCode"></param>
        /// <returns></returns>
        public bool MatchesCode(string cardCode)
        {
            return (cardCode == CardCode);
        }

        /// <summary>
        /// ICloneable interface
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            CardInPlay result = new CardInPlay(Owner, TheCard, CurrentZone);
            result.BoundingBox = BoundingBox;
            result.NormalizedBoundingBox = NormalizedBoundingBox;
            result.NormalizedCenter = NormalizedCenter;
            result.LastZone = LastZone;
            result.LastNonEtherZone = LastNonEtherZone;
            result.EtherStartTime = EtherStartTime;
            return result;
        }
    }
}
