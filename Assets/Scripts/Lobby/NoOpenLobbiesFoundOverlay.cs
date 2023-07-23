using System;
using TMPro;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class NoOpenLobbiesFoundOverlay : MonoBehaviour
{
    [SerializeField] private TMP_Text _mainText;
    [SerializeField] private Button _createMyLobbyButton;
    [SerializeField] private Button _retryButton;
    
    private LobbyManager _lobbyManager;
    
    public void Setup(LobbyManager lobbyManager)
    {
        _lobbyManager = lobbyManager;
    }

    public void SetMainText(string mainText)
    {
        _mainText.text = mainText;
    }

    private void Awake()
    {
        _createMyLobbyButton.onClick.AddListener(OnCreateMyLobbyButtonClicked);
        _retryButton.onClick.AddListener(OnRetryButtonClicked);
    }

    private async void OnCreateMyLobbyButtonClicked()
    {
        gameObject.SetActive(false);
        try
        {
            await _lobbyManager.CreateAndJoinLobby(LobbyManager.DEFAULT_MAX_PLAYERS_IN_LOBBY);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            _mainText.text = "An error occured.";
            gameObject.SetActive(true);
        }
    }

    private async void OnRetryButtonClicked()
    {
        gameObject.SetActive(false);
        try
        {
            await _lobbyManager.QuickJoinLobby();
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
            if (ex.ErrorCode == 16006)
            {
                _mainText.text = "No open lobbies found.";
            }
            else
            {
                _mainText.text = "An error occured.";
            }
            gameObject.SetActive(true);
        }
    }
}