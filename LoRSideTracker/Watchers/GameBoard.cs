//#define TRACK_TOSSING_ZONE
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
        /// <summary>Neither local or remote player</summary>
        None,
    }

    /// <summary>
    /// Play zones on the screen
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

    static class GameBoard
    {
        /// <summary>
        /// Map a card position to a play zone
        /// </summary>
        /// <param name="normalizedCenter"></param>
        /// <param name="normalizedBoundingBox"></param>
        /// <returns></returns>
        public static PlayZone FindPlayZone(PointF normalizedCenter, RectangleF normalizedBoundingBox)
        {
            // Find the zone for each card
#if TRACK_TOSSING_ZONE
            if (normalizedBoundingBox.Height < 0)
#else
            if (normalizedBoundingBox.Height <= 0)
#endif
            {
                // This looks like a definite bug, ignore cards with negative dimensions
                return PlayZone.Unknown;
            }
            else if (normalizedCenter.Y > 1.0f)
            {
                // Card in hand is at 0.22-0.235 height when in hand
                // When hand is moused over, it grows to ~0.28-0.295
                // When dragged around, size varies between 0.20-0.24, being smallest at the center (y == 0.5)
                return PlayZone.Hand;
            }
            else if (normalizedBoundingBox.Height <= 0)
            {
                // Current theory is that cards of size zero are being tossed, but that almost never works
                return PlayZone.Tossing;
            }
            else if (normalizedBoundingBox.Height > 0.4f)
            {
                // Cards are presented very large (~0.49 height) when being drawn
                return PlayZone.Zoom;
            }
            else if (normalizedBoundingBox.Height > 0.31f && normalizedCenter.Y < 0.7f)
            {
                // Cards are presented fairly large (~0.345 height) when being presented for mulligan
                // This size is also used when playing a unit
                return PlayZone.Stage;
            }
            else if (normalizedBoundingBox.Height > 0.20f)
            {
                // Card in hand is at 0.22-0.235 height when in hand
                // When hand is moused over, it grows to ~0.28-0.295
                // When dragged around, size varies between 0.20-0.24, being smallest at the center (y == 0.5)
                return PlayZone.Hand;
            }
            else if (normalizedBoundingBox.Height > 0.13f)
            {
                //return PlayZone.Field;

                // Camp, Battle, Attack zones all use nominal size of 0.145-0.1485
                //For camp, normalized center Y is ~ 0.83
                if (normalizedBoundingBox.Height > 0.155f || normalizedCenter.Y > 0.82f)
                {
                    return PlayZone.Field;
                }
                else if (normalizedCenter.Y > 0.71f)
                {
                    return PlayZone.Windup;
                }
                else if (normalizedCenter.Y > 0.62f)
                {
                    return PlayZone.Battle;
                }
                else
                {
                    return PlayZone.Attack;
                }
            }
            else if (normalizedBoundingBox.Height < 0.125f && Math.Abs(normalizedCenter.Y - 0.5f) < 0.05f)
            {
                return PlayZone.Cast;
            }
            else
            {
                return PlayZone.Unknown;
            }
        }

        public enum TransitionResult
        {
            /// <summary>Transition not recognized/allowed</summary>
            Disallow,
            /// <summary>Transition recognized/allowed</summary>
            Proceed,
            /// <summary>Transition recognized, but card should stay in original zone</summary>
            Stay,
        }

        public static TransitionResult TransitionAllowed(PlayZone from, PlayZone to, bool isInitialDraw)
        {
            // Only allow additions to deck from stage during initial draw
            if (to == PlayZone.Deck)
            {
                return (isInitialDraw && from == PlayZone.Stage) ? TransitionResult.Proceed : TransitionResult.Disallow;
            }

            switch (from)
            {
                case PlayZone.Deck:
                    if (isInitialDraw)
                    {
                        return (to == PlayZone.Hand || to == PlayZone.Stage) ? TransitionResult.Proceed : TransitionResult.Disallow;
                    }
                    else
                    {
                        return (to == PlayZone.Zoom) ? TransitionResult.Proceed : TransitionResult.Disallow;
                    }
                case PlayZone.Tossing:
                    switch (to)
                    {
                        case PlayZone.Stage: // This is due to a bug
                        case PlayZone.Hand: // This is due to a bug
                        case PlayZone.Field: // This is due to a bug
                            return TransitionResult.Proceed;
                        default:
                            return TransitionResult.Disallow;
                    }
                case PlayZone.Zoom:
                    return (to == PlayZone.Hand) ? TransitionResult.Proceed : TransitionResult.Disallow;
                case PlayZone.Stage:
                    if (isInitialDraw)
                    {
                        // We only ever return to deck from initial draw
                        switch (to)
                        {
                            case PlayZone.Deck: return TransitionResult.Proceed;
                            default: return TransitionResult.Disallow;
                        }
                    }
                    else
                    {
                        switch (to)
                        {
                            case PlayZone.Field: return TransitionResult.Proceed;
                            case PlayZone.Hand: return TransitionResult.Proceed;
                            default: return TransitionResult.Disallow;
                        }
                    }
                case PlayZone.Hand:
                    switch (to)
                    {
                        case PlayZone.Field: return TransitionResult.Proceed;
                        case PlayZone.Cast: return TransitionResult.Proceed;
                        case PlayZone.Stage: return TransitionResult.Proceed;
                        default: return TransitionResult.Stay;
                    }
                case PlayZone.Cast:
                    return (to == PlayZone.Hand) ? TransitionResult.Proceed : TransitionResult.Disallow;
                case PlayZone.Field:
                    switch (to)
                    {
                        case PlayZone.Hand: return TransitionResult.Proceed; // For recall mechanics
                        case PlayZone.Battle: return TransitionResult.Proceed;
                        case PlayZone.Attack: return TransitionResult.Stay;
                        case PlayZone.Windup: return TransitionResult.Stay;
                        case PlayZone.Tossing: return TransitionResult.Proceed; // For spell summoning, likely a bug
                        default: return TransitionResult.Disallow;
                    }
                case PlayZone.Battle:
                    switch (to)
                    {
                        case PlayZone.Hand: return TransitionResult.Proceed; // For recall mechanics
                        case PlayZone.Field: return TransitionResult.Proceed;
                        case PlayZone.Windup: return TransitionResult.Proceed;
                        case PlayZone.Attack: return TransitionResult.Stay;
                        default: return TransitionResult.Disallow;
                    }
                case PlayZone.Windup:
                    switch (to)
                    {
                        case PlayZone.Field: return TransitionResult.Proceed;
                        case PlayZone.Battle: return TransitionResult.Stay;
                        case PlayZone.Attack: return TransitionResult.Proceed;
                        default: return TransitionResult.Disallow;
                    }
                case PlayZone.Attack:
                    switch (to)
                    {
                        case PlayZone.Field: return TransitionResult.Proceed;
                        case PlayZone.Battle: return TransitionResult.Stay;
                        case PlayZone.Windup: return TransitionResult.Proceed;
                        case PlayZone.Hand: return TransitionResult.Proceed; // Katarina-style recall mechanics
                        default: return TransitionResult.Disallow;
                    }
                case PlayZone.Graveyard:
                    return TransitionResult.Disallow;
                case PlayZone.Unknown:
                    // We only allow cetain zones to accept cards from unknown
                    switch (to)
                    {
                        case PlayZone.Field:
                        case PlayZone.Battle:
                        case PlayZone.Stage:
                        case PlayZone.Cast:
                        case PlayZone.Tossing:
                        case PlayZone.Hand:
                            return TransitionResult.Proceed;
                        default: return TransitionResult.Disallow;
                    }
                case PlayZone.Ether:
                    return TransitionResult.Disallow;
            }
            return TransitionResult.Disallow;
        }
    }
}
