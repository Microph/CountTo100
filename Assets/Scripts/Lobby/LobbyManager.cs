using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_PLAYER_READY_STATUS = "PlayerReadyStatus";
    public const string KEY_HOST_START_GAMEPLAY_TIMES = "HostStartGameplayTimes";

    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }
    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;

    public int CurrentLocalStartGameplayTimes => _currentLocalStartGameplayTimes;
    
    private const float k_defaultLobbyHeartBeatTime = 15f;
    private const float k_defaultLobbyPollTime = 1.5f;
    private const int k_defaultMaxPlayersInLobby = 3;

    private string _playerName;
    private Player _cachedPlayerModel;
    private Lobby _joinedLobby;
    private bool _isHandlingLobbyHeartbeat = false;
    private bool _isHandlingLobbyPoll = false;
    private float _heartbeatTimer = 0;
    private float _lobbyPollTimer = 0;
    private int _currentLocalStartGameplayTimes = 0;

    public bool IsLobbyHost(string playerId)
    {
        return _joinedLobby != null && _joinedLobby.HostId == playerId;
    }

    public bool IsPlayerReady(string playerId, PlayerDataObject playerReadyStatusDataObject)
    {
        //host is always ready
        if(AuthenticationService.Instance != null && IsLobbyHost(playerId))
        {
            return true;
        }

        string playerReadyStatusValue = playerReadyStatusDataObject?.Value;
        return playerReadyStatusValue != null && playerReadyStatusValue.Equals("1");
    }

    public bool AreAllPlayersReadyIgnoreHost(List<Player> players)
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

    public async Task AuthenticateAndQuickJoinLobby(string playerName)
    {
        _playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(_playerName);
        await UnityServices.InitializeAsync(initializationOptions);
        AuthenticationService.Instance.SignedIn += OnSignedIn;
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async Task UpdatePlayerReadyStatus(bool isPlayerReady)
    {
        if (_joinedLobby == null)
        {
            return;
        }
        
        UpdatePlayerOptions options = new UpdatePlayerOptions();
        options.Data = new Dictionary<string, PlayerDataObject>() {
            {
                KEY_PLAYER_READY_STATUS, new PlayerDataObject(
                    visibility: PlayerDataObject.VisibilityOptions.Member,
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
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
        }
    }

    public async Task UpdateHostStartGameplayTimes(string serverIP, string serverPort)
    {
        if (_joinedLobby == null)
        {
            return;
        }

        int currentHostStartGameplayTimes = 0;
        if (_joinedLobby.Data != null)
        {
            _joinedLobby.Data.TryGetValue(KEY_HOST_START_GAMEPLAY_TIMES, out DataObject currentHostStartGameplayTimesDataObject);
            currentHostStartGameplayTimes = currentHostStartGameplayTimesDataObject == null ? 0 : int.Parse(currentHostStartGameplayTimesDataObject.Value);
        }

        UpdateLobbyOptions options = new UpdateLobbyOptions();
        options.Data = new Dictionary<string, DataObject>()
        {
            {
                KEY_HOST_START_GAMEPLAY_TIMES, new DataObject(
                    visibility: DataObject.VisibilityOptions.Member,
                    value: (currentHostStartGameplayTimes + 1).ToString())
            }
        };
        //TODO: set server ip and port and use to connect in gameplay scene

        try
        {
            var lobby = await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, options);
            _joinedLobby = lobby;
            OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
        }
    }

    private void Awake()
    {
        OnJoinedLobbyUpdate += OnJoinedLobbyUpdateCallback;
    }

    private void OnDestroy()
    {
        OnJoinedLobbyUpdate -= OnJoinedLobbyUpdateCallback;
    }

    private async void OnApplicationQuit()
    {
        await LeaveCurrentLobby();
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
                await CreateAndJoinLobby(k_defaultMaxPlayersInLobby);
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
        _cachedPlayerModel ??= new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _playerName) },
        });
        return _cachedPlayerModel;
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
        }
    }

    private async Task LeaveCurrentLobby()
    {
        if (AuthenticationService.Instance == null || _joinedLobby == null)
        {
            return;
        }

        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
        }
    }

    private void OnJoinedLobbyUpdateCallback(object sender, LobbyEventArgs e)
    {
        if (e.lobby.Data != null)
        {
            e.lobby.Data.TryGetValue(KEY_HOST_START_GAMEPLAY_TIMES, out DataObject currentHostStartGameplayTimesDataObject);
            int currentHostStartGameplayTimes = currentHostStartGameplayTimesDataObject == null ? 0 : int.Parse(currentHostStartGameplayTimesDataObject.Value);
            if (_currentLocalStartGameplayTimes < currentHostStartGameplayTimes)
            {
                _currentLocalStartGameplayTimes++;
                //TODO: get server ip and port and use to connect in gameplay scene
                //TODO: keep showing starting gameplay overlay until successfully connected to gameplay server
                SceneManager.LoadScene("Gameplay");
            }
        }
    }
}