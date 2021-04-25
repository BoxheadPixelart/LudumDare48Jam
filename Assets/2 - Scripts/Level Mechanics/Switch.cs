using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{

    #region --------------------    Public Properties

    public bool isOn => _isOn;

    #endregion

    #region --------------------    Public Methods

    public void Toggle() => _isOn = !_isOn;

    #endregion

    #region --------------------    Private Fields

    [SerializeField] private bool _isOn = false;
    [SerializeField] private MeshRenderer _rend = null;
    private List<BotInteraction> _knownInteractions = new List<BotInteraction>();
    private float _triggerRadius = 0f;

    #endregion

    #region --------------------    Private Methods

    private void Start() => _triggerRadius = GetComponent<MeshFilter>().mesh.bounds.extents.magnitude / 2f;

    private void FixedUpdate()
    {
        Collider[] _hits = Physics.OverlapSphere(transform.position + (transform.forward * _triggerRadius), _triggerRadius, LayerMask.GetMask("Bot"));
        List<BotInteraction> _remove = new List<BotInteraction>(_knownInteractions);
        foreach (Collider _hit in _hits)
        {
            BotInteraction _match = _knownInteractions.Find(i => i.transform == _hit.transform.parent);
            if (_match != null)
            {
                /// Bot stayed within range
                _remove.Remove(_match);
            }
            else
            {
                /// New bot entered range
                BotInteraction _new = _hit.GetComponentInParent<BotInteraction>();
                _new.OnInteractEvent -= Toggle;
                _new.OnInteractEvent += Toggle;
                _knownInteractions.Add(_new);
            }
        }
        if (_remove.Count > 0)
        {
            _remove.ForEach(i => {
                i.OnInteractEvent -= Toggle;
                _knownInteractions.Remove(i);
            });
            _remove.Clear();
        }

        /// Update switch visuals to match state
        _rend.material.color = (isOn) ? Color.green : Color.red;
    }

    #endregion

}
