using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class LevelManager : MonoBehaviour
{
    int numberOfScenes;
    public int currentLevel; 

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = 1; 
        numberOfScenes = SceneManager.sceneCountInBuildSettings;
        LoadNextScene();
    }

    // Update is called once per frame
    void Update()
    {
      
    }
   public void LoadNextScene()
    {
        currentLevel += 1; 
        SceneManager.LoadScene(currentLevel, LoadSceneMode.Additive);
        if (PlayerManager.instance.currentLevel)
        {
            PlayerManager.instance.currentLevel.GetComponent<LevelMover>().SetLevelUpper(); 
        }
    }
    void MoveLoadedScenes()
    {

    }
    void UnloadOldScene()
    {

    }
    //Load The Next Scene
    //Place it UNDER the current secen
    //Tween Both Scenes Upwards
    //Unload Last Scene; 
}
