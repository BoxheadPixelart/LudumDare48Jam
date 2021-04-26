using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TwitchButtonUpdate : MonoBehaviour
{

    [SerializeField] private Image _back;
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] private Color _disconnected;
    [SerializeField] private Color _connected;
    public void Update()
    {
        _back.color = (TwitchHookup.instance.isConnected) ? _connected : _disconnected;
        _buttonText.text = (TwitchHookup.instance.isConnected) ? "<sprite=0> Connected" : "Connect <sprite=0>";
    }

}