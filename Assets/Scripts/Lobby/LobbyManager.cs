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
    public const string KEY_PLAYER_READY_STATUS = "PlayerReadyStatus";

    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }
    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;

    private const float k_defaultLobbyHeartBeatTime = 15f;
    private const float k_defaultLobbyPollTime = 3f;

    private string _playerName;
    private Player _player;
    private Lobby _joinedLobby;
    private bool _isHandlingLobbyHeartbeat = false;
    private bool _isHandlingLobbyPoll = false;
    private float _heartbeatTimer = 0;
    public float _lobbyPollTimer = 0;

    public bool IsLobbyHost(string playerId)
    {
        return _joinedLobby != null && _joinedLobby.HostId == playerId;
    }

    public bool IsPlayerReady(string playerReadyStatusValue)
    {
        return playerReadyStatusValue != null && playerReadyStatusValue.Equals("1");
    }

    public async Task AuthenticateAndQuickJoinLobby(string playerName)
    {
        _playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);
        await UnityServices.InitializeAsync(initializationOptions);
        AuthenticationService.Instance.SignedIn += OnSignedIn;
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void UpdatePlayerReadyStatus(bool isPlayerReady)
    {
        if (_joinedLobby == null)
        {
            return;
        }
        
        UpdatePlayerOptions options = new UpdatePlayerOptions();
        options.Data = new Dictionary<string, PlayerDataObject>() {
            {
                KEY_PLAYER_READY_STATUS, new PlayerDataObject(
                    visibility: PlayerDataObject.VisibilityOptions.Public,
                    value: isPlayerReady ? "1" : null)
            }
        };
        string playerId = AuthenticationService.Instance.PlayerId;
        try
        {
            Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, playerId, options);
            _joinedLobby = lobby;
            OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void Update()
    {
        if(_joinedLobby != null)
        {
            if (!_isHandlingLobbyPoll)
            {
                HandleLobbyPoll();
            }
            if (AuthenticationService.Instance != null && IsLobbyHost(AuthenticationService.Instance.PlayerId) && !_isHandlingLobbyHeartbeat)
            {
                HandleLobbyHeartbeat();
            }
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
        _heartbeatTimer -= Time.deltaTime;
        if (_heartbeatTimer <= 0f)
        {
            _heartbeatTimer = k_defaultLobbyHeartBeatTime;
            Debug.Log("Heartbeat lobby");
            try
            {
                _isHandlingLobbyHeartbeat = true;
                await LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                _isHandlingLobbyHeartbeat = false;
            }
        }
    }

    private async void HandleLobbyPoll()
    {
        if (_joinedLobby == null)
        {
            return;
        }

        _lobbyPollTimer -= Time.deltaTime;
        if (_lobbyPollTimer <= 0f)
        {
            _lobbyPollTimer = k_defaultLobbyPollTime;
            try
            {
                _isHandlingLobbyPoll = true;
                _joinedLobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);
                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                _isHandlingLobbyPoll = false;
            }

            if (_joinedLobby != null && AreAllPlayersReadyExceptHost(_joinedLobby.Players))
            {
                //TODO If all players are ready except host, enable start gameplay button for host
            }
        }
    }

    private bool AreAllPlayersReadyExceptHost(List<Player> players)
    {
        if (players == null || players.Count <= 1)
        {
            return false;
        }

        foreach (Player player in players)
        {
            if (player.Id != _joinedLobby.HostId && !player.Data.ContainsKey(KEY_PLAYER_READY_STATUS))
            {
                return false;
            }
        }
        return true;
    }
}