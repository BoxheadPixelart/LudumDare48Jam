using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndwPlate : MonoBehaviour
{
    public PressurePlate plate;
    bool hasEnded; 
    // Start is called before the first frame update
    void Start()
    {
        hasEnded = false; 
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasEnded){
            if (plate.state == PressurePlate.PressurePlateState.Pressed)
            {
                PlayerManager.instance.progressionUIController.EvaluateSolve();
                hasEnded = true; 
            }
        }
    }
}
