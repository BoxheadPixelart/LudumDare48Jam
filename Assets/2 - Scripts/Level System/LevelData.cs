using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Bot;
using UnityEngine.UI;

public class LevelData : MonoBehaviour
{
    public GameObject botParent;
    public List<BotPlayer> bots = new List<BotPlayer>();
    // Start is called before the first frame update
    void Start()
    {
        CollectBots();
        TurnOffMotors(); 
        UpdateLevel(); 
    }
    public void UpdateLevel()
    {
        PlayerManager.instance.currentLevel = gameObject; 
        foreach (BotUIController panel in PlayerManager.instance.panels)
        {
            Destroy(panel.gameObject);
          
        }
        PlayerManager.instance.panels.Clear();
        PlayerManager.instance.worldCanvas.GetComponent<HorizontalLayoutGroup>().enabled = true; 
      
        PlayerManager.instance.bots = bots.ToArray();
        PlayerManager.instance.SetupBots(); 

    }
    void CollectBots()
    {
        bots.Clear();
        for (int i = 0; i < botParent.transform.childCount; i++)
        {
            bots.Add(botParent.transform.GetChild(i).gameObject.GetComponent<BotPlayer>());

        }
    }
    public void TurnOnMotors()
    {
        foreach (BotPlayer bot in bots)
        {
            bot.Character.Motor.SetTransientPosition(bot.Character.transform.position); 
            bot.Character.Motor.enabled = true; 
        }
    }
    //``
    public void TurnOffMotors()
    {
        foreach (BotPlayer bot in bots)
        {
            if (bot == null) continue; 
            bot.Character.Motor.enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
