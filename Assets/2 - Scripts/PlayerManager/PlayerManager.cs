using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Custom;
using KinematicCharacterController.Bot; 

public class PlayerManager : MonoBehaviour
{
    public CustomPlayer player;
    public BotPlayer[] bots;
    public BotPlayer manualBot = null;
    public TwitchHookup twitch;
    public static PlayerManager instance { get; private set; } = null;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.lockState = CursorLockMode.Confined;
        player.EnableMove();
    }
    private void Awake()
    {
        instance = instance ?? this;
    }
    public void ButtonCheck()
    {
        print("A Button has been clicked"); 
    }
    private void Update()
    {
       if (manualBot)
       {
            if (player.isActive)
            {
                player.DisableMove(); 
            }
       } 
        else
        {
            if(player.isActive == false)
            {
                player.EnableMove(); 
            }
        }
    }
}