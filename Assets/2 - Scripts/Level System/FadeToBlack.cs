using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class FadeToBlack : MonoBehaviour
{
    // Start is called before the first frame update
    public Image fade;
    float alpha;
    public Scene endgameScene; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Color fadeColor = new Color();
        fadeColor.a = alpha;
        fade.color = fadeColor; 
    }
    private void Awake()
    {
        StartCoroutine(FadeOverTime()); 
    }
    IEnumerator FadeOverTime()
    {
       
        while (alpha < 1)
        {
            yield return new WaitForSeconds(0.01f);
            alpha += 0.005f; 
        }
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(16); 
    }
}
