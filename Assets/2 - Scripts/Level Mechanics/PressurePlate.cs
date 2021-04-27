using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{

    #region --------------------    Public Enumerations

    public enum PressurePlateState { Inactive, Idle, Pressed };
    public Color activeColor;
    public Color inactiveColor;
    public bool isEndPlate; 
    #endregion

    #region --------------------    Public Properties

    public PressurePlateState state { get; private set; } = PressurePlateState.Idle;

    #endregion

    #region --------------------    Public Methods



    #endregion

    #region --------------------    Private Fields

    [SerializeField] private Switch _requiredSwitch = null;
    [SerializeField] private MeshRenderer _rend = null;
    private float _triggerRadius = 0;

    #endregion

    #region --------------------    Private Methods

    private void Start() => _triggerRadius = GetComponent<MeshFilter>().mesh.bounds.extents.magnitude / 2f;

    private void FixedUpdate()
    {
        state = ((_requiredSwitch != null && _requiredSwitch.isOn) || _requiredSwitch == null) ? 
            ((state != PressurePlateState.Inactive) ? state : PressurePlateState.Idle) : PressurePlateState.Inactive;
        if (state != PressurePlateState.Inactive)
        {
            Collider[] _hits = Physics.OverlapSphere(transform.position + new Vector3(0f, _triggerRadius, 0f), _triggerRadius, LayerMask.GetMask( "Bot" ));
            state = (_hits.Length > 0) ? PressurePlateState.Pressed : PressurePlateState.Idle;
        }
        Debug.DrawLine(transform.position + new Vector3(0f, _triggerRadius, 0f), transform.position + new Vector3(0f, _triggerRadius * 2f, 0f), Color.red, 10f);
        _rend.material.color = (state == PressurePlateState.Pressed) ? activeColor : ((state == PressurePlateState.Idle) ? inactiveColor : Color.grey);

        if (isEndPlate)
        {
            if (state == PressurePlateState.Pressed)
            {
                foreach (var bot in PlayerManager.instance.bots)
                {
                    bot.SetSleeping(); 
                }
                //PlayerManager.instance.bots 
            }
        }
    }

    #endregion

}