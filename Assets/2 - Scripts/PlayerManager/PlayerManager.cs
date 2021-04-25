using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Custom;
using KinematicCharacterController.Bot; 

public class PlayerManager : MonoBehaviour
{
    public CustomPlayer player;
    public BotPlayer[] bots;
    public BotPlayer manualBot { get; set; } = null;
    public TwitchHookup twitch;
    public static PlayerManager instance { get; private set; } = null;

    // Start is called before the first frame update
    void Start()
    {
        instance = instance ?? this;
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.lockState = CursorLockMode.Confined;
        player.EnableMove();
    }

    public void ButtonCheck()
    {
        print("A Button has been clicked"); 
    }
}