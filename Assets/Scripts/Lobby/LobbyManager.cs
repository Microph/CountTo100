using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviour
{
    public const string KEY_PLAYER_NAME = "PlayerName";

    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }
    public event EventHandler<LobbyEventArgs> OnJoinedLobby;

    private string _playerName;
    private Player _player;
    private Lobby _joinedLobby;
    private bool _isHandlingLobbyHeartbeat = false;
    private float _heartbeatTimer = 0;

    public async Task AuthenticateAndQuickJoinLobby(string playerName)
    {
        _playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);
        await UnityServices.InitializeAsync(initializationOptions);
        AuthenticationService.Instance.SignedIn += OnSignedIn;
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public bool IsLobbyHost()
    {
        return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private void Update()
    {
        if (IsLobbyHost() && !_isHandlingLobbyHeartbeat)
        {
            HandleLobbyHeartbeat();
        }
    }

    private async void OnSignedIn()
    {
        AuthenticationService.Instance.SignedIn -= OnSignedIn;
        bool getNoOpenLobbyError = false;
        try
        {
            await QuickJoinLobby();
        }
        catch (LobbyServiceException ex)
        {
            getNoOpenLobbyError = ex.ErrorCode == 16006;
            Debug.LogException(ex);
        }

        if(getNoOpenLobbyError)
        {
            try
            {
                await CreateAndJoinLobby(3);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    private async Task QuickJoinLobby()
    {
        QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();
        options.Player = GetPlayer();
        Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
        _joinedLobby = lobby;
        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        Debug.Log("Quick joined a lobby");
    }

    private async Task CreateAndJoinLobby(int maxPlayers)
    {
        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Player = GetPlayer(),
            IsPrivate = false
        };

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(_playerName, maxPlayers, options);
        _joinedLobby = lobby;
        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        Debug.Log("Created and joined a lobby");
    }

    private Player GetPlayer()
    {
        _player ??= new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _playerName) },
        });
        return _player;
    }

    private async void HandleLobbyHeartbeat()
    {
        _isHandlingLobbyHeartbeat = true;
        _heartbeatTimer -= Time.deltaTime;
        if (_heartbeatTimer <= 0f)
        {
            _heartbeatTimer = 15f;
            Debug.Log("Heartbeat lobby");
            try
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        _isHandlingLobbyHeartbeat = false;
    }
}