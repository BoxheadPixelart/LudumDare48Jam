using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; 

public class CameraGroup : MonoBehaviour
{
    // Start is called before the first frame update
    public CinemachineVirtualCamera[] cams;
    public CinemachineBrain[] brains; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
