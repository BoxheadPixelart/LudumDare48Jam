using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using DG.Tweening;

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
        public int zoneCount; 
        float rot;
        float rotCmdValue = 0;
        public LayerMask cameraLayer;
        public LayerMask worldLayer;
        public BotAI ai;
        public BotState state = BotState.Auto;
        public delegate void StateChange(BotState _pState);
        public StateChange OnStateChange { get; set; } = null;
        private float _aiTimer = 0;
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();
        private float navmeshCheckInterval = 1f;

        public List<CameraBrain> cameras = new List<CameraBrain>();
        public bool canPing => zoneCount > 0;

        public void SetSleeping()
        {
            if (state == BotState.Sleeping) return;
            PlayerManager.instance.manualBot = (PlayerManager.instance.manualBot == this) ? null : PlayerManager.instance.manualBot;
            state = BotState.Sleeping;
            characterInputs = new PlayerCharacterInputs();
            Character.SetInputs(ref characterInputs);
            OnStateChange?.Invoke(state);
        }

        public void SetAuto()
        {
            if (state == BotState.Auto) return;
            PlayerManager.instance.manualBot = (PlayerManager.instance.manualBot == this) ? null : PlayerManager.instance.manualBot;
            state = BotState.Auto;
            characterInputs = new PlayerCharacterInputs();
            Character.SetInputs(ref characterInputs);
            OnStateChange?.Invoke(state);
        }

        public void SetManual()
        {
            if (state == BotState.Manual) return;
            PlayerManager.instance.manualBot?.SetSleeping();
            state = BotState.Manual;
            PlayerManager.instance.manualBot = this;
            characterInputs = new PlayerCharacterInputs();
            Character.SetInputs(ref characterInputs);
            OnStateChange?.Invoke(state);
        }

        public void PingCameras()
        {
            if (canPing)
            {
                foreach (CameraBrain cam in cameras)
                {
                    cam.ToggleTarget(Character.transform);
                }
            }
        }
        
        public void ZoomCamera()
        {
            foreach (CameraBrain cam in cameras)
            {
                cam.ToggleZoom();
            }
        }

        private void Start()
        {
            SetSleeping();
            rot = lookTarget.transform.rotation.eulerAngles.y;
            ai.OnGoToSleep += () => { if (state == BotState.Auto) SetSleeping(); };
            ai.OnWakeUp += () => { if (state == BotState.Sleeping) SetAuto(); };
        }

        private void Update()
        {
            switch (state)
            {
                case BotState.Sleeping:
                    ai.agent.transform.position = Character.transform.position;
                    break;
                case BotState.Auto:
                    _UpdateAITarget();
                    ai.isBusy = ai.agent.remainingDistance > ai.agent.stoppingDistance + 0.5f;
                    if (ai.isCommandMoving && !ai.isBusy)
                    {
                        ai.isCommandMoving = false;
                        ai.commands.Dequeue();
                    }
                    break;
                case BotState.Manual:
                    _HandleCharacterInput();
                    break;
            }
        }

        private void LateUpdate() => _Rotate();

        private void _Rotate()
        {
            if (state == BotState.Manual && Input.GetKey(KeyCode.A)) rot -= 2;
            if (state == BotState.Manual && Input.GetKey(KeyCode.D)) rot += 2;
            lookTarget.transform.rotation = Quaternion.Euler(lookTarget.transform.rotation.eulerAngles.x, rot, lookTarget.transform.rotation.eulerAngles.z);
        }

        private void _HandleCharacterInput()
        {
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
            ai.agent.transform.position = Character.transform.position;
            ai.agent.SetDestination(ai.agent.transform.position);
        }

        private void _UpdateAITarget()
        {
            if (ai.commands.Count == 0 && PlayerManager.instance.manualBot != null) ai.agent.SetDestination(PlayerManager.instance.manualBot.Character.transform.position);
            if (ai.commands.Count > 0 && !ai.isBusy)
            {
                ai.isBusy = true;
                BotAI.BotCommand _cmd = ai.commands.Peek();
                switch(_cmd.command)
                {
                    case BotAI.BotCommand.Command.Rotate:
                        if (ai.isCommandRotating) break;
                        rotCmdValue = _cmd.value;
                        ai.isCommandRotating = true;
                        DOTween.To(() => _aiTimer, x => _aiTimer = x, 1f, 1f)
                            .OnComplete(() => { ai.commands.Dequeue(); ai.isCommandRotating = false; ai.isBusy = false; });
                        break;
                    case BotAI.BotCommand.Command.Move:
                        if (ai.isCommandMoving) break;
                        ai.agent.SetDestination(Character.transform.position + (Character.transform.forward * _cmd.value));
                        ai.isCommandMoving = true;
                        break;
                    case BotAI.BotCommand.Command.Ping:
                        if (ai.isCommandPinging) break;
                        PingCameras();
                        ai.isCommandPinging = false;
                        ai.commands.Dequeue();
                        DOTween.To(() => _aiTimer, x => _aiTimer = x, 1f, 1f)
                            .OnComplete(() => { ai.isCommandPinging = false; ai.isBusy = false; });
                        break;
                    case BotAI.BotCommand.Command.Sleep:
                    case BotAI.BotCommand.Command.Awake:
                        ai.commands.Dequeue();
                        break;
                }
            }
            FollowAgent();

            /// Follow AI Agent
            void FollowAgent()
            {
                if (navmeshCheckInterval <= 0)
                {
                    navmeshCheckInterval = 1f;
                    if (Vector3.Distance(Character.transform.position, ai.agent.transform.position) > 3f) ai.agent.transform.position = Character.transform.position;
                }
                navmeshCheckInterval -= Time.deltaTime;
                Vector3 _cachedAngles = lookTarget.transform.rotation.eulerAngles;
                float _angles = (ai.isBusy) ? Quaternion.FromToRotation(lookTarget.transform.forward, (ai.agent.transform.position - Character.transform.position).normalized).eulerAngles.y - 180f : 0f;
                rot += (!ai.isCommandRotating) ? ((_angles > -10f && _angles < 10f) ? 0f : ((_angles >= 10f) ? -2f : 2f)) : rotCmdValue * Time.deltaTime;
                characterInputs = new PlayerCharacterInputs()
                {
                    MoveAxisForward = (Vector3.Distance(ai.agent.transform.position, Character.transform.position) > ai.agent.stoppingDistance) ? 0.95f : 0f,
                    CameraRotation = lookTarget.transform.rotation
                };
                Character.SetInputs(ref characterInputs);
            }
        }

    }
}