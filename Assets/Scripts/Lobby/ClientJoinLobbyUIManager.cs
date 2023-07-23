using CountTo100.Utilities;
using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class ClientJoinLobbyUIManager : MonoBehaviour
{
    [Header("EnterPlayerNameUIGroup")]
    [SerializeField] private GameObject _enterPlayerNameUIGroup;
    [SerializeField] private NoOpenLobbiesFoundOverlay _noOpenLobbiesFoundOverlay;
    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private Button _joinALobbyButton;

    [Header("LobbyUIGroup")]
    [SerializeField] private GameObject _lobbyUIGroup;
    [SerializeField] private LobbyPlayerElement _lobbyPlayerElementPrefab;
    [SerializeField] private SimpleObjectPool _lobbyPlayerElementObjectPool;
    [SerializeField] private Transform _lobbyPlayerElementContentTransform;
    [SerializeField] private Button _readyButton;
    [SerializeField] private Button _startGameButton;

    [Header("StartGameHostConfigUI")]
    [SerializeField] private GameObject _startGameHostConfigUI;
    [SerializeField] private TMP_InputField _serverIPInputField;
    [SerializeField] private TMP_InputField _serverPortInputField;

    private LobbyManager _lobbyManager;

    public void Setup(LobbyManager lobbyManager)
    {
        _lobbyManager = lobbyManager;
        _lobbyManager.OnJoinedLobby += OnJoinedLobby;
        _lobbyManager.OnJoinedLobbyUpdate += OnJoinedLobbyUpdate;
        _noOpenLobbiesFoundOverlay.Setup(_lobbyManager);
        if (_lobbyManager.JoinedLobby == null)
        {
            ShowEnterPlayerNameUIGroup();
        }
        else
        {
            HideAllUIGroup();
        }
    }

    public void HideAllUIGroup()
    {
        _enterPlayerNameUIGroup.SetActive(false);
        _lobbyUIGroup.SetActive(false);
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
    }

    private async void OnJoinALobbyButtonClicked()
    {
        _enterPlayerNameUIGroup.SetActive(false);
        try
        {
            await _lobbyManager.AuthenticateAndQuickJoinLobby(_playerNameInputField.text);
        }
        catch (LobbyServiceException ex)
        {
            if (ex.ErrorCode == LobbyManager.NO_OPEN_LOBBIES_ERROR_CODE)
            {
                _noOpenLobbiesFoundOverlay.SetMainText("No open lobbies found.");
                _noOpenLobbiesFoundOverlay.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogException(ex);
                _enterPlayerNameUIGroup.SetActive(true);
            }
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
            _enterPlayerNameUIGroup.SetActive(true);
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
        _startGameButton.interactable = false;
        try
        {
            await _lobbyManager.UpdateHostLobbyData(true, _serverIPInputField.text, _serverPortInputField.text);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
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
        ShowLobbyUIGroup();
    }

    private void UpdateLobbyUI(Lobby lobby)
    {
        foreach (var obj in _lobbyPlayerElementContentTransform.GetComponentsInChildren<LobbyPlayerElement>())
        {
            _lobbyPlayerElementObjectPool.ReturnObject(obj.gameObject);
        }

        foreach (Player player in lobby.Players)
        {
            player.Data.TryGetValue(LobbyManager.KEY_PLAYER_NAME, out PlayerDataObject playerNameDataObject);
            player.Data.TryGetValue(LobbyManager.KEY_PLAYER_READY_STATUS, out PlayerDataObject playerReadyStatusDataObject);
            LobbyPlayerElement newPlayerElement = _lobbyPlayerElementObjectPool.GetObjectInstance().GetComponent<LobbyPlayerElement>();
            newPlayerElement.transform.SetParent(_lobbyPlayerElementContentTransform);
            newPlayerElement.Setup(playerNameDataObject?.Value, _lobbyManager.IsLobbyHost(player.Id), _lobbyManager.IsPlayerReady(player.Id, playerReadyStatusDataObject));
        }

        if (AuthenticationService.Instance != null)
        {
            if(AuthenticationService.Instance.PlayerId == lobby.HostId)
            {
                ShowStartGameButtonAndHostConfigUI(_lobbyManager.AreAllPlayersReadyIgnoreHost(lobby.Players));
            }
            else
            {
                ShowReadyButton(!_lobbyManager.IsPlayerReady(lobby.Players.Find(player => player.Id == AuthenticationService.Instance.PlayerId)));
            }
        }
    }

    private void ShowEnterPlayerNameUIGroup()
    {
        HideAllUIGroup();
        _enterPlayerNameUIGroup.SetActive(true);
    }

    private void ShowLobbyUIGroup()
    {
        HideAllUIGroup();
        _lobbyUIGroup.SetActive(true);
    }

    private void ShowStartGameButtonAndHostConfigUI(bool isStartGameButtonInteractible)
    {
        _readyButton.gameObject.SetActive(false);
        _startGameButton.interactable = isStartGameButtonInteractible;
        _startGameButton.gameObject.SetActive(true);
        _startGameHostConfigUI.SetActive(true);
    }

    private void ShowReadyButton(bool isReadyButtonInteractible)
    {
        _startGameButton.gameObject.SetActive(false);
        _startGameHostConfigUI.SetActive(false);
        _readyButton.interactable = isReadyButtonInteractible;
        _readyButton.gameObject.SetActive(true);
    }
}
