using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using KinematicCharacterController.Custom;
using KinematicCharacterController.Bot; 

public class PlayerManager : MonoBehaviour
{
    public CustomPlayer player;
    public BotPlayer[] bots;
    public BotPlayer manualBot = null;
    public GameObject worldCanvas;
    public GameObject panelPrefab;
    public GameObject currentLevel; 
    public static PlayerManager instance { get; private set; } = null;
    public List<BotUIController> panels = new List<BotUIController>();
    public LevelManager levelManager;
    public ProgressionUIController progressionUIController; 
    // Start is called before the first frame update
    void Start()
    {
        //panels.Clear(); 
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

    public void SetupBots()
    {
        for (int i = 0; i < bots.Length; i++)
        {
         GameObject panel =   Instantiate(panelPrefab, worldCanvas.transform);
            print("Panel Game obj created"); 
            BotUIController ui = panel.GetComponent<BotUIController>();
            print("Panel script was found");
            panels.Add(ui);
            print("Panel script was added to a list");
            ui.bot = bots[i]; 
        }
        StartCoroutine(SetPanelPostions()); 
    }
    
    IEnumerator SetPanelPostions()
    {
        yield return new WaitForSeconds(1);
        foreach (BotUIController panel in panels)
        {
            panel.SetOrigin(); 
        }
        worldCanvas.GetComponent<HorizontalLayoutGroup>().enabled = false; 
        yield return null; 
    }
    public void SummonLevel()
    {
        currentLevel.GetComponent<LevelMover>().SetLevel(); 
    }
}