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
        public string MyDeckName { get; set; }
        public string MyDeckCode { get; set; }
        public List<CardWithCount> MyDeck = new List<CardWithCount>();
        public string OpponentName { get; set; }
        public List<CardWithCount> OpponentDeck = new List<CardWithCount>();
        public string Result { get; set; }
        public string Notes { get; set; }
        public DateTime Timestamp { get; set; }

        public GameRecord()
        {

        }

        GameRecord(List<CardWithCount> myDeck, List<CardWithCount> opponentDeck, string result)
        {
            int myDeckSize = myDeck.Sum(x => x.Count);

        }

        /// <summary>
        /// Load game record from file
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns></returns>
        public static GameRecord LoadFromFile(string path)
        {
            var json = Utilities.ReadLocalFile(path);
            GameRecord result = new GameRecord();
            Dictionary<string, JsonElement> gameRecord = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            result.MyDeckName = gameRecord["MyDeckName"].ToString();
            result.MyDeckCode = gameRecord["MyDeckCode"].ToString();
            result.MyDeck = Utilities.LoadDeckFromStringCodeList(gameRecord["MyDeck"].ToObject<string[]>());
            result.OpponentName = gameRecord["OpponentName"].ToString();
            result.OpponentDeck = Utilities.LoadDeckFromStringCodeList(gameRecord["OpponentDeck"].ToObject<string[]>());
            result.Result = gameRecord["Result"].ToString();
            result.Notes = gameRecord["Notes"].ToString();
            result.Timestamp = gameRecord["Timestamp"].ToObject<DateTime>();
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
        public static void SaveToFile(
            string path,
            string myDeckName,
            string myDeckCode,
            List<CardWithCount> myDeck, 
            string opponentName, 
            List<CardWithCount> opponentDeck, 
            string result, 
            string notes,
            DateTime timestamp)
        {
            int myDeckSize = myDeck.Sum(x => x.Count);
            int opponentDeckSize = opponentDeck.Sum(x => x.Count);

            string [] myDeckList = new string[myDeckSize];
            int i = 0;
            foreach (var card in myDeck)
            {
                for (int j = 0; j < card.Count; j++)
                {
                    myDeckList[i++] = card.Code;
                }
            }

            string[] opponentDeckList = new string[opponentDeckSize];
            i = 0;
            foreach (var card in opponentDeck)
            {
                for (int j = 0; j < card.Count; j++)
                {
                    opponentDeckList[i++] = card.Code;
                }
            }

            var json = JsonSerializer.Serialize(new 
            {
                MyDeckName = myDeckName,
                MyDeckCode = myDeckCode,
                MyDeck = myDeckList,
                OpponentName = opponentName,
                OpponentDeck = opponentDeckList,
                Result = result,
                Notes = notes,
                Timestamp = timestamp
            });
            File.WriteAllText(path, json);
        }

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
            GameRecord.SaveToFile(path, MyDeckName, MyDeckCode, MyDeck, OpponentName, OpponentDeck, Result, Notes, Timestamp);
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

            return string.Empty;
        }
    }
}
