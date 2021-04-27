using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using DG.Tweening;
using UnityEngine.AI;

namespace KinematicCharacterController.Bot
{
    public enum BotState { Auto, Manual, Sleeping };

    public class BotPlayer : MonoBehaviour
    {
        public BotCharacterController Character;
        // public BotCharacterCamera CharacterCamera;
        public GameObject lookTarget;
        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();
        float rot;
        public List<CameraBrain> cameras = new List<CameraBrain>();
        public LayerMask cameraLayer;
        public LayerMask worldLayer;
        public int zoneCount;
        public bool canPing => zoneCount > 0;

        public BotState state = BotState.Auto;
        public delegate void StateChange(BotState _pState);
        public StateChange OnStateChange { get; set; } = null;

        public Queue<TwitchCommandModule.Command> queuedCommands = new Queue<TwitchCommandModule.Command>();
        public int queuedCommandLimit => 10;
        public bool isProcessingCommand = false;
        public float cmdCooldown => 1f;
        public TwitchCommandModule.Command currentCommand;
        public NavMeshAgent agent { get; private set; } = null;
        public bool isMoving = false;
        public bool isRotating = false;
        float rotCmdValue = 0;
        int sleepCount = 0;
        int wakeCount = 0;
        int stateChangeCommandCount => 1;
        private float _cmdTimer = 0;
        private float navmeshCheckInterval = 1f;


        public void SetSleeping()
        {
            if (state == BotState.Sleeping) return;
            PlayerManager.instance.manualBot = (PlayerManager.instance.manualBot == this) ? null : PlayerManager.instance.manualBot;
            state = BotState.Sleeping;
            _ResetAgent();
            sleepCount = 0;
            wakeCount = 0;
            characterInputs = new PlayerCharacterInputs();
            Character.SetInputs(ref characterInputs);
            OnStateChange?.Invoke(state);
        }

        public void SetAuto()
        {
            if (state == BotState.Auto) return;
            PlayerManager.instance.manualBot = (PlayerManager.instance.manualBot == this) ? null : PlayerManager.instance.manualBot;
            state = BotState.Auto;
            _ResetAgent();
            sleepCount = 0;
            wakeCount = 0;
            characterInputs = new PlayerCharacterInputs();
            Character.SetInputs(ref characterInputs);
            OnStateChange?.Invoke(state);
        }

        public void SetManual()
        {
            if (state == BotState.Manual) return;
            PlayerManager.instance.manualBot?.SetSleeping();
            state = BotState.Manual;
            wakeCount = 0;
            PlayerManager.instance.manualBot = this;
            characterInputs = new PlayerCharacterInputs();
            Character.SetInputs(ref characterInputs);
            OnStateChange?.Invoke(state);
        }

        /// <summary>
        /// Sets the ai's destination to the distance forwards or backwards
        /// </summary>
        /// <param name="_pDistance"></param>
        /// <returns></returns>
        public bool SetAIMove(int _pDistance)
        {
            if (state != BotState.Auto) return false;
            agent.transform.position = Character.transform.position;
            agent.SetDestination(Character.transform.position + (Character.transform.forward * _pDistance));
            isMoving = true;
            return true;
        }

        /// <summary>
        /// Sets the bot to rotating
        /// </summary>
        /// <param name="_pValue"></param>
        /// <returns></returns>
        public bool SetAIRotate(int _pValue)
        {
            if (state != BotState.Auto) return false;
            _ResetAgent();
            rotCmdValue = currentCommand.value;
            isRotating = true;
            return true;
        }

        public bool PingCameras()
        {
            if (state == BotState.Sleeping) return false;
            if (canPing)
            {
                foreach (CameraBrain cam in cameras)
                {
                    cam.ToggleTarget(Character.transform);
                }
            }
            return true;
        }

        public bool ZoomCamera()
        {
            if (state == BotState.Sleeping) return false;
            foreach (CameraBrain cam in cameras)
            {
                cam.ToggleZoom();
            }
            return true;
        }

        /// <summary>
        /// Increases the sleep command count and puts the bot to sleep if the threshold is reached
        /// </summary>
        /// <returns></returns>
        public bool IncreaseSleep()
        {
            if (state == BotState.Sleeping) return false;
            sleepCount++;
            if (sleepCount >= stateChangeCommandCount)
            {
                SetSleeping();
            }
            return true;
        }

        /// <summary>
        /// Increases the awake command count and wakes the bot up if the threshold is reached
        /// </summary>
        /// <returns></returns>
        public bool IncreaseAwake()
        {
            if (state != BotState.Sleeping) return false;
            wakeCount++;
            if (wakeCount >= stateChangeCommandCount)
            {
                SetAuto();
            }
            return true;
        }

        private void Awake()
        {
            agent = GetComponentInChildren<NavMeshAgent>();
        }

        private void Start()
        {
            SetSleeping();
            rot = lookTarget.transform.rotation.eulerAngles.y;
            TwitchHookup.instance.irc.newChatMessageEvent.RemoveListener(_ScanMessage);
            TwitchHookup.instance.irc.newChatMessageEvent.AddListener(_ScanMessage);
        }

        private void OnDestroy() => TwitchHookup.instance.irc.newChatMessageEvent.RemoveListener(_ScanMessage);

        private void Update()
        {
            _ProcessCommandQueue();
            _FollowAgent();
            _HandleCharacterInput();
        }

        private void LateUpdate() => _Rotate();

        private void _Rotate()
        {
            if (state == BotState.Manual && Input.GetKey(KeyCode.A)) rot -= 3;
            if (state == BotState.Manual && Input.GetKey(KeyCode.D)) rot += 3;
            lookTarget.transform.rotation = Quaternion.Euler(lookTarget.transform.rotation.eulerAngles.x, rot, lookTarget.transform.rotation.eulerAngles.z);
        }

        private void _HandleCharacterInput()
        {
            if (state != BotState.Manual) return;
            characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = 0f;
            characterInputs.CameraRotation = lookTarget.transform.rotation;
            characterInputs.JumpDown = false;
            characterInputs.CrouchDown = false;
            characterInputs.CrouchUp = false;

            // Apply inputs to character
            Character.SetInputs(ref characterInputs); 
            _ResetAgent();
        }

        /// <summary>
        /// Scans a message and creates a command to queue if possible
        /// </summary>
        /// <param name="_pChatter"></param>
        private void _ScanMessage(Chatter _pChatter)
        {
            TwitchCommandModule.CommandDefinition _def = TwitchCommandModule.commandDefinitions.Find(d => d.MatchesPattern(name, _pChatter.message));
            if (_def == null || queuedCommands.Count >= queuedCommandLimit) return;
            /// Create a command and queue it
            queuedCommands.Enqueue(new TwitchCommandModule.Command() { name = _def.name, value = _def.value });
            Debug.Log($"Queued command name: {_def.name}, value: {_def.value}... {queuedCommands.Count} commands remain...");
        }

        /// <summary>
        /// Processes the top command in the queue
        /// </summary>
        private void _ProcessCommandQueue()
        {
            if (queuedCommands.Count == 0 || isProcessingCommand || isMoving || isRotating) return;
            isProcessingCommand = true;
            currentCommand = queuedCommands.Dequeue();
            Debug.Log($"Processing command: name - {currentCommand.name}, value - {currentCommand.value}");
            if (TwitchCommandModule.commandActions[currentCommand.name] != null && TwitchCommandModule.commandActions[currentCommand.name].Invoke(currentCommand, this))
            {
                DOTween.To(() => _cmdTimer, x => _cmdTimer = x, cmdCooldown, cmdCooldown)
                    .OnComplete(() => { 
                        isProcessingCommand = false;
                        isRotating = false;
                        Debug.Log($"Command Done: name - {currentCommand.name}, value - {currentCommand.value}... {queuedCommands.Count} commands remain...");
                    });
            }
            else
            {
                isProcessingCommand = false;
            }
        }

        private void _ResetAgent() { agent.transform.position = Character.transform.position; agent.SetDestination(Character.transform.position);}

        /// Follow AI Agent
        private void _FollowAgent()
        {
            if (state != BotState.Auto) return;
            if (navmeshCheckInterval <= 0)
            {
                navmeshCheckInterval = 1f;
                if (Vector3.Distance(Character.transform.position, agent.transform.position) > 3f) agent.transform.position = Character.transform.position;
            }
            navmeshCheckInterval -= Time.deltaTime;
            Vector3 _cachedAngles = lookTarget.transform.rotation.eulerAngles;
            float _angles = (isProcessingCommand) ?
                Quaternion.FromToRotation(lookTarget.transform.forward, (agent.transform.position - Character.transform.position).normalized).eulerAngles.y - 180f
                : 0f;
            rot += (!isRotating) ? ((_angles > -10f && _angles < 10f) ? 0f : ((_angles >= 10f) ? -2f : 2f)) : rotCmdValue * Time.deltaTime;
            characterInputs = new PlayerCharacterInputs()
            {
                MoveAxisForward = (Vector3.Distance(agent.transform.position, Character.transform.position) > agent.stoppingDistance) ? 0.95f : 0f,
                CameraRotation = lookTarget.transform.rotation
            };
            Character.SetInputs(ref characterInputs);
            isMoving = (isMoving) ? agent.remainingDistance > agent.stoppingDistance + 1f : false;
        }
    }
}