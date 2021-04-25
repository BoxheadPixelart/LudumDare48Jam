using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotInteraction : MonoBehaviour
{

    #region --------------------    Public Properties

    public delegate void InteractEvent();
    public InteractEvent OnInteractEvent { get; set; } = null;

    #endregion

    #region --------------------    Public Methods

    public void Interact() => OnInteractEvent?.Invoke();

    #endregion

    #region --------------------    Private Methods

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Interact();
        }
    }

    #endregion

}