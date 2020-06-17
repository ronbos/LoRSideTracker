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
        /// Constructor
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
        /// Constructor that parses a json element dictionary
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
            CurrentZone = GameBoard.FindPlayZone(NormalizedCenter, NormalizedBoundingBox);
        }

        /// <summary>
        /// Move card to a new zone and propagate last zone parameters
        /// </summary>
        /// <param name="zone"></param>
        public void MoveToZone(PlayZone zone)
        {
            if (CurrentZone != zone)
            {
                SetLastZone(CurrentZone);
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
