using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class ProgressionUIController : MonoBehaviour
{
    float evalProgress;
    public Color inactiveColor;
    public Color activeColor;
    public Color testingColor; 
    public Image evalFillImage;
    public GameObject progressObj;
    public GameObject gradePanel;
    public TextMeshProUGUI gradeText;
    public GameObject difficultyText;
    public GameObject testingInProgress;
    public Button floor12Button;   
    public Button floor11Button;   
    public Button floorB2Button;
    public GameObject nextLevelText;
    public GameObject endGame; 
    int level;
    bool button12Active;
    bool button11Active;
    bool buttonB2Active;
    //
    public GameObject fadeToBlack; 
    // Start is called before the first frame update
    void Start()
    {
        floor12Button.onClick.AddListener(Floor12Button);
        floor11Button.onClick.AddListener(Floor11Button);
        floorB2Button.onClick.AddListener(FloorB2Button);
        gradeText.text = "";
        gradePanel.SetActive(false);
        level = 0;
        testingInProgress.SetActive(true);
        endGame.SetActive(false);
        //
        SetButton(floor12Button, true);
        SetButton(floor11Button, false);
        SetButton(floorB2Button, false);
        //
  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
          
            EvaluateSolve();
        } 
        evalFillImage.fillAmount = evalProgress; 
    }
    void Floor12Button()
    {
        print("BUTTON"); 
    }
    void Floor11Button()
    {
        if (button11Active)
        {
            PlayerManager.instance.levelManager.LoadNextScene();
            testingInProgress.SetActive(true);
            nextLevelText.SetActive(false);
            //
            SetButtonYellow(floor11Button); 
                //
            print("BUTTON");
            button11Active = false; 
        }
    }
    void FloorB2Button()
    {
        if (buttonB2Active)
        {
            PlayerManager.instance.levelManager.LoadNextScene();
            testingInProgress.SetActive(true);
            nextLevelText.SetActive(false);
            //
            SetButtonYellow(floorB2Button);
            //
            buttonB2Active = false;
            //
            print("BUTTON");
        }
    }
    void SetButtonYellow(Button button)
    {
        button.image.color = testingColor;
    }
   void SetButton(Button button, bool state)
    {
        if (state)
        {
            button.image.color = activeColor;
        } else
        {
            button.image.color = inactiveColor;
        }
       
        if (button == floor12Button)
        {
            button12Active = state; 
        }
        if (button == floor11Button)
        {
            button11Active = state;
        }
        if (button == floorB2Button)
        {
            buttonB2Active = state;
        }
    }

   public void EvaluateSolve()
    {
        level += 1;
        evalProgress = 0f;

        StartCoroutine(EvaluateSequence()); 
    }
    //
    IEnumerator EvaluateSequence()
    {
        nextLevelText.SetActive(false); 
        testingInProgress.SetActive(false);
        yield return new WaitForSeconds(.5f);
        progressObj.SetActive(true);
        gradePanel.SetActive(false);
        difficultyText.SetActive(false);
        gradeText.text = "";
        yield return new WaitForSeconds(0.5f);
        while (evalProgress < 1)
        {
            yield return new WaitForSeconds(0.02f);
            evalProgress += 0.01f;
        }
        yield return new WaitForSeconds(1f);
        progressObj.SetActive(false);
        yield return new WaitForSeconds(.5f);
        gradePanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (level == 1)
        {
            gradeText.text = "A+";
            yield return new WaitForSeconds(1f);
        } 
        if (level == 2)
        {
            gradeText.text = "A";
            yield return new WaitForSeconds(1f);
            gradeText.text += "+";
            yield return new WaitForSeconds(.5f);
            while (gradeText.text != "A++++++++++++++++++++")
            {
                yield return new WaitForSeconds(.1f);
                gradeText.text += "+"; 
            }
            yield return new WaitForSeconds(1.5f);
        }
        if (level == 3)
        {
            yield return new WaitForSeconds(2f);
            gradeText.text = "D";
            yield return new WaitForSeconds(3f);
            gradePanel.SetActive(false); 
            yield return new WaitForSeconds(1f);
            endGame.SetActive(true);
            yield return new WaitForSeconds(4f);
            // fade to black goes here; 
            fadeToBlack.SetActive(true); 
        } else
        {

            difficultyText.SetActive(true);
            yield return new WaitForSeconds(3f);
            gradePanel.SetActive(false);
            yield return new WaitForSeconds(1f);
            nextLevelText.SetActive(true);
            if (level == 1)
            {
                SetButton(floor11Button, true);
                SetButton(floor12Button, false);
                SetButton(floorB2Button, false);
            }
            if (level == 2)
            {
                SetButton(floor11Button, false);
                SetButton(floor12Button, false);
                SetButton(floorB2Button, true);
            }
        }
        //yield return new WaitForSecondsw(.75f);

    }
}
