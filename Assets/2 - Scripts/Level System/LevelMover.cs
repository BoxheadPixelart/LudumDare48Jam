using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class LevelMover : MonoBehaviour
{
    public NavMeshSurface mesh;
    public bool skip;
    public CameraGroup cameras;
    public LevelData levelData; 

    // Start is called before the first frame update
    void Start()
    {
        levelData = GetComponent<LevelData>(); 
        SetLevelLower();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetLevelLower()
    {
        transform.position = new Vector3(0, -7, 0);
        SetLevel();
    }
    public void SetLevel()
    {
   
        if (skip)
        {
            transform.DOMoveY(0, 2).SetEase(Ease.Linear).OnComplete(() => { SetLevelUpper(); PlayerManager.instance.levelManager?.LoadNextScene(); }); ;
        } else
        {
            transform.DOMoveY(0, 2).SetEase(Ease.Linear).OnComplete(() => {levelData?.TurnOnMotors(); });
        }
      
    }
    public void SetLevelUpper()
    {

        cameras?.TurnOffBrains();
        levelData?.TurnOffMotors(); 
        if (skip)
        {
            transform.DOMoveY(7, 2).SetEase(Ease.Linear).OnComplete(() => { DoNext(); });
        } else
        {
            transform.DOMoveY(7, 2).SetEase(Ease.Linear).OnComplete(() => { DoNext(); });
        }
       
    }

    public void DoNext()
    {
        if (skip)
        {
            
            SceneManager.UnloadSceneAsync(gameObject.scene.buildIndex);
           
        } 
        else
        {
            SceneManager.UnloadSceneAsync(gameObject.scene.buildIndex);
        }
    }
}
