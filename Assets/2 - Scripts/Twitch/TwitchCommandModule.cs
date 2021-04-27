using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController.Bot
{
    public class TwitchCommandModule
    {

        #region --------------------    Public Properties

        public static List<CommandDefinition> commandDefinitions = new List<CommandDefinition>();
        public static Dictionary<string, System.Func<Command, BotPlayer, bool>> commandActions = new Dictionary<string, System.Func<Command, BotPlayer, bool>>();
        public static TwitchCommandModule instance { get; private set; } = null;

        #endregion

        #region --------------------    Public Methods

        public static void Initialize() => instance = instance ?? new TwitchCommandModule();

        static TwitchCommandModule()
        {
            commandDefinitions.Add(new CommandDefinition() { 
                name = "MOVE",
                valueFilter = (val) => val >= -9 && val != 0 && val <= 9 });
            commandActions.Add("MOVE", (def, bot) => bot.SetAIMove(def.value) );

            commandDefinitions.Add(new CommandDefinition() {
                name = "ROTATE",
                valueFilter = (val) => Mathf.Abs(val) == 180 || Mathf.Abs(val) == 90 || Mathf.Abs(val) == 45 || Mathf.Abs(val) == 30 || Mathf.Abs(val) == 15 || Mathf.Abs(val) == 10 });
            commandActions.Add("ROTATE", (def, bot) => bot.SetAIRotate(def.value) );

            commandDefinitions.Add(new CommandDefinition() { name = "PING" });
            commandActions.Add("PING", (def, bot) => bot.PingCameras() );

            commandDefinitions.Add(new CommandDefinition() { name = "ZOOM" });
            commandActions.Add("ZOOM", (def, bot) => bot.ZoomCamera() );

            commandDefinitions.Add(new CommandDefinition() { name = "AWAKE" });
            commandActions.Add("AWAKE", (def, bot) => bot.IncreaseAwake() );

            commandDefinitions.Add(new CommandDefinition() { name = "SLEEP" });
            commandActions.Add("SLEEP", (def, bot) => bot.IncreaseSleep() );
        }

        #endregion

        #region --------------------    Public Sub-Classes

        public class CommandDefinition
        {

            #region --------------------    Public Properties / Fields

            public string name { get; set; } = "";
            public System.Func<int, bool> valueFilter { get; set; } = null;

            public int value = 0;

            #endregion

            #region --------------------    Public Methods

            public bool MatchesPattern(string _pBotName, string _pMsg)
            {
                Debug.Log($"Checking pattern for {_pBotName} - {_pMsg}.");

                /// Split up the message and filter for a bot name
                if (!_pMsg.ToUpper().StartsWith($"!{_pBotName.Replace(" ", "").ToUpper()}")) return false;
                List<string> _parts = new List<string>(_pMsg.ToUpper().Split(' '));

                Debug.Log($"Bot name matches: {_pBotName}.");

                /// Parse out the command
                if (_parts[1] == null || _parts[1] != name) return false;

                Debug.Log($"Command matches: {name}");

                /// Parse out a value if there is a value filter
                if (valueFilter != null && !int.TryParse(_parts[2], out value)) return false;

                Debug.Log($"Parsed val: {value}.");

                /// Run value filter
                if (valueFilter != null && !valueFilter(value)) return false;

                Debug.Log($"Cleared filter.");

                return true;
            }

            #endregion

        }

        [System.Serializable]
        public struct Command
        {

            public string name;

            public int value;

        }

        #endregion

    }
}