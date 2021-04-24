using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Custom;
using KinematicCharacterController.Bot; 

public class PlayerManager : MonoBehaviour
{
    public CustomPlayer player;
    public BotPlayer[] bots;
    bool playerIsActive = true; 
    // Start is called before the first frame update
    void Start()
    {
        EnablePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnablePlayer()
    {
        playerIsActive = true;
        player.EnableMove();
        foreach (BotPlayer bot in bots)
        {
            bot.DisableMove(); 
        }
    }
    public void EnableBot(BotPlayer selectBot)
    {
        playerIsActive = false;
        player.DisableMove();
        foreach (BotPlayer bot in bots)
        {
            
            if (bot == selectBot)
            {
                bot.EnableMove(); 
            } else
            {
                bot.DisableMove();
            }
        }
        //
    }

    public void ToggleBot(BotPlayer selectBot)
    {
        if (playerIsActive)
        {
            EnableBot(selectBot); 
        } else
        {
            EnablePlayer(); 
        }
    }
    public void ButtonCheck()
    {
        print("A Button has been clicked"); 
    }

}
