using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class ClientJoinLobbyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _startingGameplayUIOverlay;

    [Header("EnterPlayerNameUIGroup")]
    [SerializeField] private GameObject EnterPlayerNameUIGroup;
    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private Button _joinALobbyButton;

    [Header("LobbyUIGroup")]
    [SerializeField] private GameObject LobbyUIGroup;
    [SerializeField] private LobbyPlayerElement _lobbyPlayerElementPrefab;
    [SerializeField] private Transform _lobbyPlayerElementContentTransform;
    [SerializeField] private Button _readyButton;
    [SerializeField] private Button _startGameButton;

    [Header("StartGameHostConfigUIGroup")]
    [SerializeField] private GameObject _startGameHostConfigUIGroup;
    [SerializeField] private TMP_InputField _serverIPInputField;
    [SerializeField] private TMP_InputField _serverPortInputField;

    private LobbyManager _lobbyManager;

    public void Setup(LobbyManager lobbyManager)
    {
        _lobbyManager = lobbyManager;
        _lobbyManager.OnJoinedLobby += OnJoinedLobby;
        _lobbyManager.OnJoinedLobbyUpdate += OnJoinedLobbyUpdate;
    }

    private void OnDestroy()
    {
        if(_lobbyManager != null)
        {
            _lobbyManager.OnJoinedLobby -= OnJoinedLobby;
            _lobbyManager.OnJoinedLobbyUpdate -= OnJoinedLobbyUpdate;
        }
    }

    private void Awake()
    {
        _joinALobbyButton.onClick.AddListener(OnJoinALobbyButtonClicked);
        _readyButton.onClick.AddListener(OnReadyButtonClicked);
        _startGameButton.onClick.AddListener(OnStartGameButtonClicked);
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

    private async void OnReadyButtonClicked()
    {
        _readyButton.interactable = false;
        try
        {
            await _lobbyManager.UpdatePlayerReadyStatus(true);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            _readyButton.interactable = true;
        }
    }

    private async void OnStartGameButtonClicked()
    {
        _startingGameplayUIOverlay.SetActive(true);
        _startGameButton.interactable = false;
        try
        {
            await _lobbyManager.UpdateHostStartGameplayTimes(_serverIPInputField.text, _serverPortInputField.text);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            _startingGameplayUIOverlay.SetActive(false);
            _startGameButton.interactable = true;
        }
    }

    private void OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        UpdateLobbyUI(e.lobby);
        ShowLobbyUIGroup();
    }

    private void OnJoinedLobbyUpdate(object sender, LobbyManager.LobbyEventArgs e)
    {
        UpdateLobbyUI(e.lobby);
    }

    private void UpdateLobbyUI(Lobby lobby)
    {
        //TODO: Use object pooling
        foreach (var obj in _lobbyPlayerElementContentTransform.GetComponentsInChildren<LobbyPlayerElement>())
        {
            Destroy(obj.gameObject);
        }

        foreach (Player player in lobby.Players)
        {
            player.Data.TryGetValue(LobbyManager.KEY_PLAYER_NAME, out PlayerDataObject playerNameDataObject);
            player.Data.TryGetValue(LobbyManager.KEY_PLAYER_READY_STATUS, out PlayerDataObject playerReadyStatusDataObject);
            var newPlayerElement = Instantiate(_lobbyPlayerElementPrefab, _lobbyPlayerElementContentTransform);
            newPlayerElement.Setup(playerNameDataObject?.Value, _lobbyManager.IsLobbyHost(player.Id), _lobbyManager.IsPlayerReady(player.Id, playerReadyStatusDataObject));
        }

        if (AuthenticationService.Instance != null)
        {
            if(AuthenticationService.Instance.PlayerId == lobby.HostId)
            {
                ShowStartGameUIGroup(_lobbyManager.AreAllPlayersReadyIgnoreHost(lobby.Players));
            }
            else
            {
                ShowReadyButton();
            }
        }
    }

    private void ShowStartGameUIGroup(bool areAllPlayersReadyIgnoreHost)
    {
        _readyButton.gameObject.SetActive(false);
        _startGameButton.interactable = areAllPlayersReadyIgnoreHost;
        _startGameButton.gameObject.SetActive(true);
        _startGameHostConfigUIGroup.SetActive(true);
    }

    private void ShowReadyButton()
    {
        _readyButton.gameObject.SetActive(true);
        _startGameButton.gameObject.SetActive(false);
        _startGameHostConfigUIGroup.SetActive(false);
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
