using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using CountTo100.Utilities;
using System.Threading.Tasks;
using System;
using System.Linq;
using Unity.Collections;
using static GameplayServerStateManager;

public class GameplayServerStateManager : NetworkStateManager<GameplayServerContext>
{
    public class GameplayServerContext
    {
        public GameplayServerStates GameplayServerStates;
        public GameplaySceneManager GameplaySceneManager;
        public NetworkManager NetworkManager;
        public UnityTransport Transport;
        public int TargetNumberOfPlayers;
        public Dictionary<ulong, PlayerData> ConnectedPlayerDataDict;
        public PlayerObject PlayerPrefab;
        public Transform[] PlayerPositionTransforms;

        public GameplayServerContext(GameplayServerStates gameplayServerStates, GameplaySceneManager gameplaySceneManager, NetworkManager networkManager, UnityTransport transport, int targetNumberOfPlayers, Dictionary<ulong, PlayerData> connectedPlayerDataDict, PlayerObject playerPrefab, Transform[] playerPositionTransforms)
        {
            GameplayServerStates = gameplayServerStates;
            GameplaySceneManager = gameplaySceneManager;
            NetworkManager = networkManager;
            Transport = transport;
            TargetNumberOfPlayers = targetNumberOfPlayers;
            ConnectedPlayerDataDict = connectedPlayerDataDict;
            PlayerPrefab = playerPrefab;
            PlayerPositionTransforms = playerPositionTransforms;
        }
    }

    public event Action<int> OnPlayerReady;
    public event Action<ulong> OnPlayerCount;
    public NetworkVariable<int> NVCurrentScore = new NetworkVariable<int>();
    public NetworkVariable<ulong> NVLatestClickerId = new NetworkVariable<ulong>();
    public NetworkVariable<FixedString64Bytes> NVWinnerClickerName = new NetworkVariable<FixedString64Bytes>();
    
    [SerializeField] private PlayerObject _playerPrefab;
    [SerializeField] private Transform[] _playerPositionTransforms;
    [SerializeField] private Color[] _playerColorByOrder;

    private NetworkManager _networkManager;
    private UnityTransport _transport;
    private int _targetNumberOfPlayers;
    private Dictionary<ulong, PlayerData> _connectedPlayerDataDict = new Dictionary<ulong, PlayerData>();

    public async Task InitializeAndStart(GameplaySceneManager gameplaySceneManager)
    {
        if(!GlobalServerConfig.IsServer)
        {
            Debug.LogWarning("Did not config as a server");
            return;
        }

        _networkManager = NetworkManager.Singleton;
        Debug.Assert(_networkManager != null);
        _transport = _networkManager.GetComponent<UnityTransport>();
        Debug.Assert(_transport != null);
        _targetNumberOfPlayers = GlobalServerConfig.LocalServerAllocationPayload.numberOfPlayers;
        _networkManager.ConnectionApprovalCallback = ConnectionApprovalCheck;
        _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        _transport.SetConnectionData(GlobalServerConfig.LocalServerAllocationPayload.serverBindingIP, GlobalServerConfig.LocalServerAllocationPayload.serverPort);
        _networkManager.StartServer();
        await TaskHelper.When(() => IsServer && IsSpawned);
        Debug.Log("Server object spawned!");
        GameplayServerStates gameplayServerStates = new();
        SetState(
            state: gameplayServerStates.GameplayServerServerStartedState,
            context: new GameplayServerContext(
                gameplayServerStates: gameplayServerStates,
                gameplaySceneManager: gameplaySceneManager,
                networkManager: _networkManager,
                transport: _transport,
                targetNumberOfPlayers: _targetNumberOfPlayers,
                connectedPlayerDataDict: _connectedPlayerDataDict,
                playerPrefab: _playerPrefab,
                playerPositionTransforms: _playerPositionTransforms
            )
        );
    }

    public override void OnDestroy()
    {
        if (_networkManager != null)
        {
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        }
        base.OnDestroy();
    }

    public string GetPlayerName(ulong clientId)
    {
        _connectedPlayerDataDict.TryGetValue(clientId, out PlayerData playerData);
        if(playerData != null)
        {
            return playerData.PlayerName;
        }
        else
        {
            return string.Empty;
        }
    }

    public void PlayerReady(ulong clientId)
    {
        if (!_connectedPlayerDataDict.ContainsKey(clientId))
        {
            return;
        }
        else if (_connectedPlayerDataDict[clientId].ReadyStatus)
        {
            return;
        }

        _connectedPlayerDataDict[clientId].ReadyStatus = true;
        OnPlayerReady?.Invoke(_connectedPlayerDataDict.Sum(x => x.Value.ReadyStatus ? 1 : 0));
    }

    public void PlayerCount(ulong clientId)
    {
        OnPlayerCount?.Invoke(clientId);
    }

    private void ConnectionApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        ulong clientId = request.ClientNetworkId;
        int connectedPlayerCount = _connectedPlayerDataDict.Count;
        if (_connectedPlayerDataDict.Count < _targetNumberOfPlayers)
        {
            _connectedPlayerDataDict.Add(clientId, new PlayerData(clientId, System.Text.Encoding.ASCII.GetString(request.Payload), _playerColorByOrder[connectedPlayerCount]));
            response.Approved = true;
        }
        else
        {
            response.Approved = false;
            response.Reason = "Reached maximum number of players";
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"ClientId: {clientId} disconnect reason: {_networkManager.DisconnectReason}");
    }
}
