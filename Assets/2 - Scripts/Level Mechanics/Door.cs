using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening; 

public class Door : MonoBehaviour
{

    #region --------------------    Public Properties

    public bool isClosed { get; private set; } = true;
    public GameObject doorMesh;
    public bool doorMeshIsOpen; 
    #endregion

    #region --------------------    Private Fields

    [SerializeField] private List<PressurePlate> _requiredPlates = new List<PressurePlate>();
    [SerializeField] private List<Light> _lights = new List<Light>();
    [SerializeField] private NavMeshObstacle _obstacle = null;
    [SerializeField] private Collider _collider = null;

  
    #endregion

    #region --------------------    Private Methods

    private void LateUpdate() => isClosed = (_requiredPlates.Exists(p => p.state != PressurePlate.PressurePlateState.Pressed));

    private void Update()
    {
        /// Update Lights
        for (int i = 0; i < _requiredPlates.Count; i++)
        {
            if (_lights[i] == null) break;
            _lights[i].enabled = _requiredPlates[i].state == PressurePlate.PressurePlateState.Pressed;
        }
        _obstacle.enabled = isClosed;
        _collider.enabled = isClosed;
        /// Possibly update visuals here?
        /// 
        if (isClosed)
        {
            if (doorMeshIsOpen)
            {
                CloseDoor(); 
            }
        } else
        {
            if (!doorMeshIsOpen)
            {
                OpenDoor(); 
            }
        }
      
    }

    void OpenDoor()
    {
        doorMesh.transform.DOLocalMoveY(-7, .75f).SetEase(Ease.OutBounce).OnComplete(() => { doorMeshIsOpen = true; });
    }

    void CloseDoor()
    {
        doorMesh.transform.DOLocalMoveY(0, .75f).SetEase(Ease.OutBounce).OnComplete(() => { doorMeshIsOpen = false;  });
    }
    #endregion

}