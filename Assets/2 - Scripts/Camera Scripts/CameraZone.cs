using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Bot; 

public class CameraZone : MonoBehaviour
{
    public CameraBrain brain; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerExit(Collider other)
    {
        print("Something Left a Camera Trigger Zone"); 
        brain.DeselectTarget();
        BotPlayer bot = other.transform.parent.GetComponent<BotPlayer>(); 
        if (bot)
        {
            bot.canPing = false; 
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        print("Something Entered a Camera Trigger Zone");
        BotPlayer bot = other.transform.parent.GetComponent<BotPlayer>();
        if (bot)
        {
            bot.canPing = true;
        }
    }
}
