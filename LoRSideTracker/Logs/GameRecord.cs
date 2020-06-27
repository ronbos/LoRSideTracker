using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoRSideTracker
{
    /// <summary>
    /// Game record that can be stored or loaded from file
    /// </summary>
    public class GameRecord : ICloneable
    {
        /// <summary>Player Deck Name</summary>
        public string MyDeckName { get; set; }
        /// <summary>Player Deck Code</summary>
        public string MyDeckCode { get; set; }
        /// <summary>Player Deck</summary>
        public List<CardWithCount> MyDeck = new List<CardWithCount>();
        /// <summary>Opponent Name</summary>
        public string OpponentName { get; set; }
        /// <summary>Opponent Deck</summary>
        public List<CardWithCount> OpponentDeck = new List<CardWithCount>();
        /// <summary>Game Result</summary>
        public string Result { get; set; }
        /// <summary>Game Notes</summary>
        public string Notes { get; set; }
        /// <summary>Game End Time</summary>
        public DateTime Timestamp { get; set; }
        /// <summary>Game Log</summary>
        public string Log { get; set; }

        /// <summary>Expedition Signature, if expedition</summary>
        public string ExpeditionSignature { get; set; }

        /// <summary>Game Record display string</summary>
        public string DisplayString { get; set; } = "???";

        /// <summary>Game Playback File</summary>
        public string GamePlaybackFile { get; set; }

        /// <summary>
        ///  Constructor
        /// </summary>
        public GameRecord()
        {
        }

        /// <summary>
        /// Returns unique deck signature
        /// </summary>
        /// <returns></returns>
        public string GetDeckSignature()
        {
            return (!string.IsNullOrEmpty(MyDeckCode)) ? MyDeckCode : ExpeditionSignature;
        }

        /// <summary>
        /// Check if a game record is an expedition
        /// </summary>
        /// <returns>true if it is an expedition</returns>
        public bool IsExpedition()
        {
            return (string.IsNullOrEmpty(MyDeckCode));
        }


        /// <summary>
        /// Return string to display in the list of decks
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DisplayString;
        }

        /// <summary>
        /// Load game record from file
        /// </summary>
        /// <param name="path">file path</param>
        /// <param name="matchingDeckSignature">deck signature to match, if specified</param>
        /// <returns>Game record, or null if signature did not match</returns>
        public static GameRecord LoadFromFile(string path, string matchingDeckSignature = null)
        {
            var json = Utilities.ReadLocalFile(path);
            GameRecord result = new GameRecord();
            Dictionary<string, JsonElement> gameRecord = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            result.MyDeckCode = gameRecord["MyDeckCode"].ToString();
            result.ExpeditionSignature = gameRecord["ExpeditionState"].ToString();

            // Check if deck signature matches
            if (matchingDeckSignature != null && result.GetDeckSignature() != matchingDeckSignature)
            {
                return null;
            }

            result.MyDeckName = gameRecord["MyDeckName"].ToString();
            if (!string.IsNullOrEmpty(result.MyDeckCode))
            {
                // Map the deck name for constructed decks only
                try { result.MyDeckName = GameHistory.DeckNames[result.MyDeckCode]; } catch { }
            }

            result.MyDeck = Utilities.DeckFromStringCodeList(gameRecord["MyDeck"].ToObject<string[]>());
            result.OpponentName = gameRecord["OpponentName"].ToString();
            result.OpponentDeck = Utilities.DeckFromStringCodeList(gameRecord["OpponentDeck"].ToObject<string[]>());
            result.Result = gameRecord["Result"].ToString();
            result.Notes = gameRecord["Notes"].ToString();
            result.Timestamp = gameRecord["Timestamp"].ToObject<DateTime>();
            result.Log = gameRecord["Log"].ToString();
            result.GamePlaybackFile = path.Substring(0, path.Length - 4) + ".playback";
            if (!File.Exists(result.GamePlaybackFile))
            {
                result.GamePlaybackFile = "";
            }
            return result;
        }

        /// <summary>
        /// Save game record to file
        /// </summary>
        /// <param name="path">file path</param>
        /// <param name="myDeckName"></param>
        /// <param name="myDeckCode"></param>
        /// <param name="myDeck"></param>
        /// <param name="opponentName"></param>
        /// <param name="opponentDeck"></param>
        /// <param name="result"></param>
        /// <param name="notes"></param>
        /// <param name="timestamp"></param>
        /// <param name="log"></param>
        /// <param name="expeditionState"></param>
        public static void SaveToFile(
            string path,
            string myDeckName,
            string myDeckCode,
            List<CardWithCount> myDeck,
            string opponentName,
            List<CardWithCount> opponentDeck,
            string result,
            string notes,
            DateTime timestamp,
            string log,
            string expeditionState)
        {
            string[] myDeckList = Utilities.DeckToStringCodeList(myDeck);
            string[] opponentDeckList = Utilities.DeckToStringCodeList(opponentDeck);

            var json = JsonSerializer.Serialize(new
            {
                MyDeckName = myDeckName,
                MyDeckCode = myDeckCode,
                MyDeck = myDeckList,
                OpponentName = opponentName,
                OpponentDeck = opponentDeckList,
                Result = result,
                Notes = notes,
                Timestamp = timestamp,
                Log = log,
                ExpeditionState = expeditionState
            });
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// ICloneable interface support
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Save game record to file
        /// </summary>
        /// <param name="path">file path</param>
        public void SaveToFile(string path)
        {
            GameRecord.SaveToFile(path, MyDeckName, MyDeckCode, MyDeck, OpponentName, OpponentDeck, Result, Notes, Timestamp, Log, ExpeditionSignature);
        }

        /// <summary>
        /// Read property display string
        /// </summary>
        /// <param name="propertyName">Propert name</param>
        /// <returns></returns>
        public string ReadPropertyAsString(string propertyName)
        {
            if (propertyName == "MyDeckName")
            {
                return MyDeckName;
            }
            if (propertyName == "TimeMyDeckCodestamp")
            {
                return MyDeckCode;
            }
            if (propertyName == "OpponentName")
            {
                return OpponentName;
            }
            if (propertyName == "Result")
            {
                return Result;
            }
            if (propertyName == "Notes")
            {
                return Notes;
            }
            if (propertyName == "Timestamp")
            {
                return Timestamp.ToLocalTime().ToString();
            }
            if (propertyName == "Log")
            {
                return Log;
            }
            if (propertyName == "ExpeditionState")
            {
                return ExpeditionSignature;
            }

            return string.Empty;
        }

    }
}
