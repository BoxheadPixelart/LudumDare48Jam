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
        public LayerMask cameraLayer;
        public LayerMask worldLayer;
        public BotAI ai;
        public BotState state = BotState.Auto;
        public delegate void StateChange(BotState _pState);
        public StateChange OnStateChange { get; set; } = null;
        private float _aiTimer = 0;
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

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
                Collider[] hits = Physics.OverlapSphere(Character.transform.position, 10, cameraLayer);
                foreach (Collider hit in hits)
                {
                    print("We have Detected " + hit.name);
                    RaycastHit lineHit;
                    if (Physics.Linecast(hit.transform.position, Character.transform.position, out lineHit, worldLayer))
                    {
                        print("Could not ping " + hit.name + ": The " + lineHit.transform.name + "was in the way");
                    }
                    else
                    {
                        hit.GetComponent<CameraBrain>().ToggleTarget(Character.transform);
                        print("Camera: " + hit.gameObject.name + " has been found, and pingged");
                    }

                }
            }
        }
        
        public void ZoomCamera()
        {

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
                BotAI.BotCommand _cmd = ai.commands.Dequeue();
                switch(_cmd.command)
                {
                    case BotAI.BotCommand.Command.Rotate:
                        rot += _cmd.value;
                        DOTween.To(() => _aiTimer, x => _aiTimer = x, 1f, 1f)
                            .OnComplete(() => { ai.isBusy = false; });
                        break;
                    case BotAI.BotCommand.Command.Move:
                        ai.agent.SetDestination(Character.transform.position + Character.transform.forward);
                        break;
                    case BotAI.BotCommand.Command.Ping:
                        PingCameras();
                        DOTween.To(() => _aiTimer, x => _aiTimer = x, 1f, 1f)
                            .OnComplete(() => { ai.isBusy = false; });
                        break;
                    case BotAI.BotCommand.Command.Sleep:
                        DOTween.To(() => _aiTimer, x => _aiTimer = x, 1f, 1f)
                            .OnComplete(() => { ai.isBusy = false; });
                        break;
                    case BotAI.BotCommand.Command.Awake:
                        DOTween.To(() => _aiTimer, x => _aiTimer = x, 1f, 1f)
                            .OnComplete(() => { ai.isBusy = false; });
                        break;
                }
            }
            Vector3 _cachedAngles = lookTarget.transform.rotation.eulerAngles;
            float _angles = Quaternion.FromToRotation(lookTarget.transform.forward, (ai.agent.transform.position - Character.transform.position).normalized).eulerAngles.y - 180f;
            rot += (_angles > -10f && _angles < 10f) ? 0f : ((_angles >= 10f) ? -2f : 2f);
            characterInputs = new PlayerCharacterInputs()
            {
                MoveAxisForward = (Vector3.Distance(ai.agent.transform.position, Character.transform.position) > ai.agent.stoppingDistance) ? 0.95f : 0f,
                CameraRotation = lookTarget.transform.rotation
            };
            Character.SetInputs(ref characterInputs);
        }

    }
}