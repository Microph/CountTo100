using System;
using TMPro;
using UnityEngine;

public class LobbyPlayerElement : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private GameObject _isHostSpriteObject;
    [SerializeField] private GameObject _isReadyObject;

    public void Setup(string playerName, bool isHost, bool isReady)
    {
        _playerNameText.text = playerName;
        _isHostSpriteObject.SetActive(isHost);
        _isReadyObject.SetActive(isReady);
    }
}