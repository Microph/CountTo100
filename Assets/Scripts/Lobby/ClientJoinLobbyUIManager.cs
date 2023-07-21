using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class ClientJoinLobbyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject EnterPlayerNameUIGroup, LobbyUIGroup;
    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private Button _joinALobbyButton;

    private LobbyManager _lobbyManager;

    public void Setup(LobbyManager lobbyManager)
    {
        _lobbyManager = lobbyManager;
        _lobbyManager.OnJoinedLobby += OnJoinedLobby;
    }

    private void Awake()
    {
        _joinALobbyButton.onClick.AddListener(OnJoinALobbyButtonClicked);
        ShowEnterPlayerNameUIGroup();
    }

    private async void OnJoinALobbyButtonClicked()
    {
        _joinALobbyButton.interactable = false;
        try
        {
            await _lobbyManager.AuthenticateAndQuickJoinLobby(_playerNameInputField.text);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            _joinALobbyButton.interactable = true;
        }
    }

    private void OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        foreach(Player player in e.lobby.Players)
        {
            Debug.Log($"KEY_PLAYER_NAME: {player.Data[LobbyManager.KEY_PLAYER_NAME].Value}");
        }
        ShowLobbyUIGroup();
    }

    private void ShowEnterPlayerNameUIGroup()
    {
        LobbyUIGroup.SetActive(false);
        EnterPlayerNameUIGroup.SetActive(true);
    }

    private void ShowLobbyUIGroup()
    {
        LobbyUIGroup.SetActive(true);
        EnterPlayerNameUIGroup.SetActive(false);
    }
}
