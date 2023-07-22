using System;
using System.Net;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class ClientJoinLobbyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject EnterPlayerNameUIGroup, LobbyUIGroup;
    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private Button _joinALobbyButton;
    [SerializeField] private LobbyPlayerElement _lobbyPlayerElementPrefab;
    [SerializeField] private Transform _lobbyPlayerElementContentTransform;
    [SerializeField] private Button _readyButton;
    [SerializeField] private Button _startGameButton;

    private LobbyManager _lobbyManager;

    public void Setup(LobbyManager lobbyManager)
    {
        _lobbyManager = lobbyManager;
        _lobbyManager.OnJoinedLobby += OnJoinedLobby;
        _lobbyManager.OnJoinedLobbyUpdate += OnJoinedLobbyUpdate;
    }

    public void OnDestroy()
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
            newPlayerElement.Setup(playerNameDataObject?.Value, _lobbyManager.IsLobbyHost(player.Id), _lobbyManager.IsPlayerReady(playerReadyStatusDataObject?.Value));
        }

        bool isAllPlayersReady = false; //TODO: implement check logic
        if(AuthenticationService.Instance != null)
        {
            if(AuthenticationService.Instance.PlayerId == lobby.HostId)
            {
                ShowStartGameButton(isAllPlayersReady);
            }
            else
            {
                ShowReadyButton();
            }
        }
    }

    private void ShowStartGameButton(bool isAllPlayersReady)
    {
        _readyButton.gameObject.SetActive(false);
        _startGameButton.interactable = isAllPlayersReady;
        _startGameButton.gameObject.SetActive(true);
    }

    private void ShowReadyButton()
    {
        _readyButton.gameObject.SetActive(true);
        _startGameButton.gameObject.SetActive(false);
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
