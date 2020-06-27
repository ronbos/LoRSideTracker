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
    /// 
    /// </summary>
    public class CardInPlay : ICloneable
    {
        // Card info

        /// <summary></summary>
        public Card TheCard { get; private set; }
        /// <summary></summary>
        public string CardCode { get => TheCard.Code; }
        /// <summary></summary>
        public PlayerType Owner { get; private set; }
        /// <summary></summary>
        public bool IsFromDeck { get; set; }

        // Card position on the game board

        /// <summary></summary>
        public Rectangle BoundingBox { get; set; }
        /// <summary></summary>
        public RectangleF NormalizedBoundingBox { get; set; }
        /// <summary></summary>
        public PointF NormalizedCenter { get; set; }

        // Card zone

        /// <summary></summary>
        public PlayZone CurrentZone { get; set; }
        /// <summary></summary>
        public PlayZone LastZone { get; set; }
        /// <summary></summary>
        public PlayZone LastNonEtherZone { get; set; }
        /// <summary></summary>
        public double EtherStartTime { get; set; }


        /// <summary>
        /// Constructor for a card belonging to the local player and in deck
        /// </summary>
        /// <param name="card"></param>
        public CardInPlay(Card card)
        {
            TheCard = card;
            BoundingBox = new Rectangle();
            NormalizedBoundingBox = new RectangleF();
            NormalizedCenter = new PointF();
            Owner = PlayerType.LocalPlayer;
            CurrentZone = PlayZone.Deck;
            LastZone = PlayZone.Unknown;
            LastNonEtherZone = PlayZone.Unknown;
            IsFromDeck = true;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="card"></param>
        /// <param name="currentZone"></param>
        /// <param name="isFromDeck"></param>
        public CardInPlay(PlayerType owner, Card card, PlayZone currentZone, bool isFromDeck)
        {
            TheCard = card;
            BoundingBox = new Rectangle();
            NormalizedBoundingBox = new RectangleF();
            NormalizedCenter = new PointF();
            Owner = owner;
            CurrentZone = currentZone;
            LastZone = PlayZone.Unknown;
            LastNonEtherZone = PlayZone.Unknown;
            IsFromDeck = isFromDeck;
        }

        /// <summary>
        /// Constructor that parses a json element dictionary
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        /// <param name="correctionOffset"></param>
        /// <param name="screenHeightForNormalized"></param>
        public CardInPlay(Dictionary<string, JsonElement> dict,
            int screenWidth,
            int screenHeight,
            Point correctionOffset,
            float screenHeightForNormalized)
        {
            TheCard = CardLibrary.GetCard(dict["CardCode"].GetString());
            Owner = dict["LocalPlayer"].GetBoolean() ? PlayerType.LocalPlayer : PlayerType.Opponent;
            BoundingBox = new Rectangle(
                dict["TopLeftX"].GetInt32() + correctionOffset.X,
                screenHeight - dict["TopLeftY"].GetInt32() - correctionOffset.Y, // Elements are reported upside down for some reason
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
            IsFromDeck = false;
        }

        /// <summary>
        /// Move card to a new zone and propagate last zone parameters
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="timestamp"></param>
        public void MoveToZone(PlayZone zone, double timestamp)
        {
            if (CurrentZone != zone)
            {
                SetLastZone(CurrentZone);
                if (zone == PlayZone.Ether)
                {
                    EtherStartTime = timestamp;
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
            CardInPlay result = new CardInPlay(Owner, TheCard, CurrentZone, IsFromDeck);
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
