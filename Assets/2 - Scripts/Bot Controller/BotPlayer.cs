using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;

namespace KinematicCharacterController.Bot
{
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
        bool isActive;
        public bool canPing;
        public int zoneCount; 
        float rot;
        public LayerMask cameraLayer;
        public LayerMask worldLayer; 
        private void Start()
        {
            DisableMove();
            rot = lookTarget.transform.rotation.eulerAngles.y; 
        }

        private void Update()
        {
            if (isActive) { HandleCharacterInput(); }
        if (zoneCount > 0)
            {
                canPing = true; 
            } else
            {
                canPing = false;
            }
        }

        private void LateUpdate()
        {
            if (isActive) { HandleCameraInput(); } 
        }

        private void HandleCameraInput()
        {
           
           if (Input.GetKey(KeyCode.A))
           {
                rot -= 2; 
           }
            //
            if (Input.GetKey(KeyCode.D))
            {
                rot += 2;
            }
            lookTarget.transform.rotation = Quaternion.Euler(lookTarget.transform.rotation.eulerAngles.x, rot, lookTarget.transform.rotation.eulerAngles.z);
            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            
#if UNITY_WEBGL
        scrollInput = 0f;
#endif   
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = 0f; 
            characterInputs.CameraRotation = lookTarget.transform.rotation;
            characterInputs.JumpDown = false;
            characterInputs.CrouchDown = false;
            characterInputs.CrouchUp = false;

            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }
        public void EnableMove()
        {
            isActive = true;
        }
        public void DisableMove()
        {
            isActive = false;
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
                    if (Physics.Linecast(hit.transform.position,Character.transform.position,out lineHit, worldLayer))
                    {
                        print("Could not ping " + hit.name + ": The " + lineHit.transform.name + "was in the way"); 
                    } else
                    {
                        hit.GetComponent<CameraBrain>().ToggleTarget(Character.transform);
                        print("Camera: " + hit.gameObject.name + " has been found, and pingged"); 
                    }
                  
                }
            }
        }
    }
}