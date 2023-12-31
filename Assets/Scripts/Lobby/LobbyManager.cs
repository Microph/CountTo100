using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using CountTo100.Utilities;

public class LobbyManager : MonoSingleton<LobbyManager>
{
    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_PLAYER_READY_STATUS = "PlayerReadyStatus";
    public const string KEY_HOST_START_GAMEPLAY_TIMES = "HostStartGameplayTimes";
    public const string KEY_GAMEPLAY_SERVER_IP = "GameplayServerIP";
    public const string KEY_GAMEPLAY_SERVER_PORT = "GameplayServerPort";
    public const int DEFAULT_MAX_PLAYERS_IN_LOBBY = 3;
    public const int NO_OPEN_LOBBIES_ERROR_CODE = 16006;

    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }
    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;

    public Lobby JoinedLobby => _joinedLobby;
    public int CurrentLocalStartGameplayTimes => _currentLocalStartGameplayTimes;
    public string PlayerName => _playerName;
    
    private const float k_defaultLobbyHeartBeatTime = 15f;
    private const float k_defaultLobbyPollTime = 1.5f;

    private string _playerName;
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

    public bool IsPlayerReady(Player player)
    {
        if(_joinedLobby == null)
        {
            return false;
        }    

        player.Data.TryGetValue(KEY_PLAYER_READY_STATUS, out PlayerDataObject playerReadyStatusDataObject);
        return _joinedLobby.HostId == player.Id || (playerReadyStatusDataObject != null && playerReadyStatusDataObject.Value == "1");
    }

    public bool AreAllPlayersReadyIgnoreHost(List<Player> players)
    {
        if (players == null || players.Count <= 1)
        {
            return false;
        }

        foreach (Player player in players)
        {
            if (!IsPlayerReady(player))
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
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        if (AuthenticationService.Instance.IsSignedIn)
        {
            await QuickJoinLobby();
        }
    }

    public async Task QuickJoinLobby()
    {
        QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();
        options.Player = GenerateNewPlayerDataModel(_playerName);
        Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
        _joinedLobby = lobby;
        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }

    public async Task CreateAndJoinLobby(int maxPlayers)
    {
        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Player = GenerateNewPlayerDataModel(_playerName),
            IsPrivate = false
        };

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(_playerName, maxPlayers, options);
        _joinedLobby = lobby;
        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        Debug.Log("Created and joined a lobby");
    }

    public Player GenerateNewPlayerDataModel(string playerName)
    {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
        });
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
        Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, playerId, options);
        _joinedLobby = lobby;
        OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
    }

    public async Task UpdateHostLobbyData(bool lockLobby, bool increaseHostStartGameplayTime, string serverIP, string serverPort)
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

        UpdateLobbyOptions options = new UpdateLobbyOptions
        {
            IsLocked = lockLobby,
            Data = new Dictionary<string, DataObject>()
            {
                {
                    KEY_HOST_START_GAMEPLAY_TIMES, new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: (currentHostStartGameplayTimes + (increaseHostStartGameplayTime ? 1 : 0)).ToString())
                },
                {
                    KEY_GAMEPLAY_SERVER_IP, new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: serverIP)
                },
                {
                    KEY_GAMEPLAY_SERVER_PORT, new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: serverPort)
                }
            }
        };

        var lobby = await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, options);
        _joinedLobby = lobby;
        OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
    }

    public async Task LeaveCurrentLobby()
    {
        if (AuthenticationService.Instance == null || _joinedLobby == null)
        {
            return;
        }

        await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
    }

    protected override void Awake()
    {
        base.Awake();
        OnJoinedLobbyUpdate += OnJoinedLobbyUpdateCallback;
    }

    protected override void OnDestroy()
    {
        OnJoinedLobbyUpdate -= OnJoinedLobbyUpdateCallback;
        base.OnDestroy();
    }

    //does not work if the application process is killed (need Unity's Relay to handle this instead)
    private async void OnApplicationQuit()
    {
        try
        {
            await LeaveCurrentLobby();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
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

    private void OnJoinedLobbyUpdateCallback(object sender, LobbyEventArgs e)
    {
        CheckStartGameplay(e);
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

    private void CheckStartGameplay(LobbyEventArgs e)
    {
        if (e.lobby.Data != null)
        {
            e.lobby.Data.TryGetValue(KEY_HOST_START_GAMEPLAY_TIMES, out DataObject currentHostStartGameplayTimesDataObject);
            int currentHostStartGameplayTimes = currentHostStartGameplayTimesDataObject == null ? 0 : int.Parse(currentHostStartGameplayTimesDataObject.Value);
            if (_currentLocalStartGameplayTimes < currentHostStartGameplayTimes)
            {
                _currentLocalStartGameplayTimes++;
                SceneManager.LoadScene("Gameplay");
            }
        }
    }
}