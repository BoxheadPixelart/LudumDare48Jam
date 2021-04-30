using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; 

public class CameraGroup : MonoBehaviour
{
    // Start is called before the first frame update
    public CinemachineVirtualCamera[] cams;
    public CinemachineBrain[] brains;
    public Camera[] cameraComps; 
    int cameraIndex = 0;
    void Start()
    {
        for (int i = 0; i <= cameraComps.Length - 1; i++)
        {
            cameraComps[i].enabled = false; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i <= cameraComps.Length - 1; i++)
        {
            cameraComps[i].Render(); 
        }
    }
    public void TurnOffBrains()
    {
        foreach (CinemachineBrain brain in brains)
        {
            if (brain == null) { continue; }
            brain.enabled = false; 
        }
    }
}
