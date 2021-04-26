using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using KinematicCharacterController.Bot;
using Cinemachine;
using UnityEngine.SceneManagement; 

public class NameplateController : MonoBehaviour
{
    CinemachineVirtualCamera[] cameras;
    public GameObject nameplatePrefab;
    public CameraGroup cameraGroup; 
    //GameObject[] nameplates;
    public List<GameObject> nameplates = new List<GameObject>();
    public BotPlayer bot;
    public List<int> layers = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        // get all cameras
      //  cameras = FindObjectsOfType(typeof(CinemachineVirtualCamera)) as CinemachineVirtualCamera[];
        //create a nameplate per camera
        for (int i = 0; i < cameraGroup.cams.Length - 1; i++)
        {
            
            GameObject name = Instantiate(nameplatePrefab, transform);
            nameplates.Add(name);
            name.transform.GetChild(0).GetComponent<TMP_Text>().text = bot.name;
            name.layer = cameraGroup.cams[i].gameObject.layer; 
            name.transform.GetChild(0).gameObject.layer = cameraGroup.cams[i].gameObject.layer;
            print(layers[i]); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        //update position per camera 

        for (int i = 0; i < cameraGroup.cams.Length - 1; i++)
        {
            nameplates[i].transform.LookAt(cameraGroup.cams[i].transform); 
        }
    }
}
