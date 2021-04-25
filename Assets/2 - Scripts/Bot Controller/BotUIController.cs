using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Bot;
using UnityEngine.UI;
using UnityEngine.Events; 

public class BotUIController : MonoBehaviour
{
    public BotPlayer bot;

    public Scrollbar stateOutput; 
    public Button controlButton;
    public Button aiButton;
    public Button sleepButton;
    public Button pingCamera;
    public Button toggleZoom;

    //
    float handleScaleOffset;
    float pingScaleOffset;
    float toggleZoomScaleOffset; 
    //

    // Start is called  the first frame update
    void Start()
    {
        handleScaleOffset = 0f;
        pingScaleOffset = 0f;
        toggleZoomScaleOffset = 0f; 
        controlButton.onClick.AddListener(SetBotActive);
        aiButton.onClick.AddListener(SetBotAI);
        sleepButton.onClick.AddListener(SetBotSleep);
        pingCamera.onClick.AddListener(PingCamera);
        toggleZoom.onClick.AddListener(ToggleZoom); 
    }
   
    // Update is called once per frame
    void Update()
    {
        handleScaleOffset = handleScaleOffset / 1.1f;
        pingScaleOffset = pingScaleOffset / 1.1f; 
        pingCamera.image.rectTransform.localScale = new Vector3(1 + pingScaleOffset, 1 + pingScaleOffset, 1 + pingScaleOffset); 
        stateOutput.handleRect.localScale = new Vector3(1 + handleScaleOffset, 1 + handleScaleOffset, 1 + handleScaleOffset); 
    }

    void ToggleZoom()
    {

    }
    void PingCamera()
    {
        pingScaleOffset += 0.4f; 
    }
    // These methods are subscribed to the buttons on the lower-thirds of the panel; 
    void SetBotActive()
    {
        print("ACTIVE");
        handleScaleOffset += 0.4f;
        SetScrollActive(); 
    }
    void SetBotAI()
    {
        print("AI");
        handleScaleOffset += 0.4f;
        SetScrollAI(); 
    }

    void SetBotSleep()
    {
        print("Sleep");
        handleScaleOffset += 0.4f;
        SetScrollSleep(); 
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
    }

    // this parces the bots state and updates ui accordinly 
    void UpdateScrollBar()
    {

    }

}
