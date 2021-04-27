using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace KinematicCharacterController.Bot
{
    public class BotAI : MonoBehaviour
    {

        #region --------------------    Public Properties

        public BotPlayer player { get; set; } = null;
        public NavMeshAgent agent => _agent;
        public Queue<BotCommand> commands => _commands;
        public bool isBusy { get; set; } = false;
        public bool isCommandMoving { get; set; } = false;
        public bool isCommandRotating { get; set; } = false;
        public bool isCommandPinging { get; set; } = false;


        public delegate void StateEvent();
        public StateEvent OnWakeUp { get; set; } = null;
        public StateEvent OnGoToSleep { get; set; } = null;

        #endregion

        #region --------------------    Private Fields

        [SerializeField] private NavMeshAgent _agent = null;
        [SerializeField] private int _commandLimit = 10;
        private Queue<BotCommand> _commands = new Queue<BotCommand>();
        private List<BotCommand.Command> _allCommandTypes;

        #endregion

        #region --------------------    Private Methods

        private void Start()
        {
            if (TwitchHookup.instance == null) return;
            TwitchHookup.instance.OnMessageReceived -= _FilterMessage;
            TwitchHookup.instance.OnMessageReceived += _FilterMessage;
            _allCommandTypes = new List<BotCommand.Command>();
            foreach (int i in System.Enum.GetValues(typeof(BotCommand.Command)))
            {
                _allCommandTypes.Add((BotCommand.Command)i);
            }
        }

        private void OnDestroy()
        {
            TwitchHookup.instance.OnMessageReceived -= _FilterMessage;
        }

        /// <summary>
        /// Filters the message and creates a command for it in the bot's command queue
        /// </summary>
        /// <param name="_pMsg"></param>
        private void _FilterMessage(string _pMsg)
        {
            /// Split up the message and filter for a bot name
            if (!_pMsg.ToUpper().StartsWith($"!{name.Replace(" ","").ToUpper()}")) return;
            List<string> _parts = new List<string>(_pMsg.Split(' '));

            /// Parse out the command
            if (_parts[1] == null) return;
            BotCommand _cmd = new BotCommand();
            _allCommandTypes.ForEach(c => { if (_parts[1].ToUpper() == c.ToString().ToUpper()) _cmd.command = c; });

            /// Parse out a value 
            if ((int)_cmd.command < 100 && _parts[2] == null) return;
            if ((int)_cmd.command < 100 && !int.TryParse(_parts[2], out _cmd.value)) return;

            /// Hard-coded specific cases
            if (_cmd.command == BotCommand.Command.Rotate &&
                (Mathf.Abs(_cmd.value) != 10 && Mathf.Abs(_cmd.value) != 15 && Mathf.Abs(_cmd.value) != 30 &&
                Mathf.Abs(_cmd.value) != 45 && Mathf.Abs(_cmd.value) != 90 && Mathf.Abs(_cmd.value) != 180)) return;    //  Limit rotations
            if (_cmd.command == BotCommand.Command.Move) _cmd.value = (_cmd.value / Mathf.Abs(_cmd.value)) * Mathf.Min(Mathf.Abs(_cmd.value + 2), 11);   //  Limit movement from -9 to 9

            /// Queue-up the command
            if (_commands.Count < _commandLimit) _commands.Enqueue(_cmd);
            else TwitchHookup.instance.irc.SendChatMessage($"{name} ignored command, queue full.");
        }

        #endregion

        #region --------------------    Public Sub-Classes

        public struct BotCommand
        {

            public enum Command { Rotate = 0, Move = 50, Ping = 100, Awake = 101, Sleep = 102 };

            public Command command;

            public int value;

        }

        #endregion

    }

}
