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
        UpdateLevel(); 
    }
    public void UpdateLevel()
    {
        PlayerManager.instance.currentLevel = gameObject; 
        bots.Clear();
        foreach (BotUIController panel in PlayerManager.instance.panels)
        {
            Destroy(panel.gameObject);
          
        }
        PlayerManager.instance.panels.Clear();
        PlayerManager.instance.worldCanvas.GetComponent<HorizontalLayoutGroup>().enabled = true; 
        for (int i = 0; i < botParent.transform.childCount; i++)
        {
            bots.Add(botParent.transform.GetChild(i).gameObject.GetComponent<BotPlayer>()); 

        }
        PlayerManager.instance.bots = bots.ToArray();
        PlayerManager.instance.SetupBots(); 

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
