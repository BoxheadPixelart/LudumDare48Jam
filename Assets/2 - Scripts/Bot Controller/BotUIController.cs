using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Bot;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro; 

public class BotUIController : MonoBehaviour
{
    public Vector3 startPos;
    public Quaternion startRot;
    public Vector3 startScale; 
    public BotPlayer bot;

    public Scrollbar stateOutput; 
    public Button controlButton;
    public Button aiButton;
    public Button sleepButton;
    public Button pingCamera;
    public Button toggleZoom;
    public TextMeshProUGUI title; 
    //
    float handleScaleOffset;
    float pingScaleOffset;
    float toggleZoomScaleOffset;
    bool canLook; 
    //

    // Start is called  the first frame update
    void Start()
    {
        canLook = false; 
        title.text = bot.name; 
        handleScaleOffset = 0f;
        pingScaleOffset = 0f;
        toggleZoomScaleOffset = 0f; 
        controlButton.onClick.AddListener(SetBotActive);
        aiButton.onClick.AddListener(SetBotAI);
        sleepButton.onClick.AddListener(SetBotSleep);
        pingCamera.onClick.AddListener(PingCamera);
        toggleZoom.onClick.AddListener(ToggleZoom);
        //
        bot.OnStateChange -= UpdateScrollBar;
        bot.OnStateChange += UpdateScrollBar;
        UpdateScrollBar(bot.state); 
        print(gameObject.name + " subscripted to Bot State Change");
    }
   
    // Update is called once per frame
    void Update()
    {
        handleScaleOffset = handleScaleOffset / 1.1f;
        pingScaleOffset = pingScaleOffset / 1.1f;
        toggleZoomScaleOffset = toggleZoomScaleOffset / 1.1f; 
        toggleZoom.image.rectTransform.localScale = new Vector3(1 + toggleZoomScaleOffset, 1 + toggleZoomScaleOffset, 1 + toggleZoomScaleOffset);
        pingCamera.image.rectTransform.localScale = new Vector3(1 + pingScaleOffset, 1 + pingScaleOffset, 1 + pingScaleOffset); 
        stateOutput.handleRect.localScale = new Vector3(1 + handleScaleOffset, 1 + handleScaleOffset, 1 + handleScaleOffset); 
    }
    public void SetOrigin()
    {
        canLook = true; 
        print(name + " Has set its origin"); 
        startPos = transform.position;
        startRot = transform.rotation;
        startScale = transform.localScale;
    }
    public void LookAtCamera()
    {
        if (canLook)
        {
            transform.SetAsLastSibling();
            transform.LookAt(Camera.main.transform);
            transform.localScale = new Vector3(2, 2, 2);
        }
       
    }
    public void ResetLook()
    {
        if (canLook)
        {
            print("RESET PANEL LOOK");
            transform.position = startPos;
            transform.rotation = startRot;
            transform.localScale = startScale;
        }
    }
    void ToggleZoom()
    {
        toggleZoomScaleOffset += 0.3f; 
        bot.ZoomCamera(); 
    }
    void PingCamera()
    {
        pingScaleOffset += 0.3f;
        bot.PingCameras();
        print("PING CAMERA BUTTON HAS BEEN CLICKED"); 
    }
    // These methods are subscribed to the buttons on the lower-thirds of the panel; 
    void SetBotActive()
    {
        print("ACTIVE");
        handleScaleOffset += 0.4f;
        bot.SetManual();

    }
    void SetBotAI()
    {
        print("AI");
        handleScaleOffset += 0.4f;
        bot.SetAuto();

    }
    void SetBotSleep()
    {
        print("Sleep");
        handleScaleOffset += 0.4f;
        bot.SetSleeping();
 
    }
    // These methods are subscribed to the appropiate bots state change method
    void SetScrollActive()
    {
        stateOutput.value = 0;
        stateOutput.handleRect.GetComponent<Image>().color = Color.blue; 
    }

    void SetScrollAI()
    {
        stateOutput.value = .5f;     
        stateOutput.handleRect.GetComponent<Image>().color = Color.green;
    }

    void SetScrollSleep()
    {
        stateOutput.value = 1; 
        stateOutput.handleRect.GetComponent<Image>().color = Color.red;
        print(name + " UI has been Updated to sleep ");
    }

    // this parces the bots state and updates ui accordinly 
    void UpdateScrollBar(BotState state)
    {
        print(name + " is in " + state.ToString()); 
        switch (state)
        {
            case BotState.Auto:
                SetScrollAI(); 
                break;
            case BotState.Manual:
                SetScrollActive();
                break;
            case BotState.Sleeping:
                print(name + " has been set to sleep ");
                SetScrollSleep();
                break;
            default:
                break;
        }
    }

}
