using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class MovingWall : MonoBehaviour
{

    #region --------------------    Public Properties



    #endregion

    #region --------------------    Private Fields

    [SerializeField] private Switch _requiredSwitch = null;
    [SerializeField] private DOTweenAnimation _anim = null;

    #endregion

    #region --------------------    Private Methods

    private void Update()
    {
        if (!_requiredSwitch.isOn)
        {
            _anim.DOPause();
        }
        else
        {
            _anim.DOPlay();
        }
    }

    #endregion

}