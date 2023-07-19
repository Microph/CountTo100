using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using CountTo100.Utilities;
using System.Threading.Tasks;
using System;
using System.Linq;

public class GameplayServerStateManager : NetworkStateManager
{
    public class GameplayServerContext
    {
        public GameplaySceneManager GameplaySceneManager;
        public NetworkManager NetworkManager;
        public int TargetNumberOfPlayers;
        public Dictionary<ulong, PlayerData> ConnectedPlayerDataDict;
        public Player PlayerPrefab;
        public Transform[] PlayerPositionTransforms;

        public GameplayServerContext(GameplaySceneManager gameplaySceneManager, NetworkManager networkManager, int targetNumberOfPlayers, Dictionary<ulong, PlayerData> connectedPlayerDataDict, Player playerPrefab, Transform[] playerPositionTransforms)
        {
            GameplaySceneManager = gameplaySceneManager;
            NetworkManager = networkManager;
            TargetNumberOfPlayers = targetNumberOfPlayers;
            ConnectedPlayerDataDict = connectedPlayerDataDict;
            PlayerPrefab = playerPrefab;
            PlayerPositionTransforms = playerPositionTransforms;
        }
    }

    public event Action<int> OnPlayerReadySignal;
    public NetworkVariable<int> NVCurrentScore = new NetworkVariable<int>(k_defaultScore);
    
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Transform[] _playerPositionTransforms;

    private const int k_defaultScore = 0;

    private NetworkManager _networkManager;
    private UnityTransport _transport;
    private int _targetNumberOfPlayers;
    private Dictionary<ulong, PlayerData> _connectedPlayerDataDict = new Dictionary<ulong, PlayerData>();

    [ServerRpc(RequireOwnership = false)]
    public void PlayerReadySignalServerRpc(ulong clientId)
    {
        if(!_connectedPlayerDataDict.ContainsKey(clientId))
        {
            return;
        }
        else if (_connectedPlayerDataDict[clientId].ReadyStatus) 
        {
            return;
        }

        _connectedPlayerDataDict[clientId].ReadyStatus = true;
        OnPlayerReadySignal?.Invoke(_connectedPlayerDataDict.Sum(x => x.Value.ReadyStatus ? 1 : 0));
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnPlayerCountServerRpc(ulong clientId)
    {
        //TODO also limit 5 times/sec
        if (CurrentStateEnum != Enums.State.GameplayServer_AllowCounting)
        {
            return;
        }

        NVCurrentScore.Value++;
        //TODO fire event update latest clicker clientId
    }

    public async Task InitializeAndStart()
    {
        if(!GlobalServerConfigManager.IsServer)
        {
            Debug.LogWarning("Did not config as a server");
            return;
        }

        _networkManager = NetworkManager.Singleton;
        Debug.Assert(_networkManager != null);
        _transport = _networkManager.GetComponent<UnityTransport>();
        Debug.Assert(_transport != null);
        _targetNumberOfPlayers = GlobalServerConfigManager.LocalServerAllocationPayload.numberOfPlayers;
        _networkManager.ConnectionApprovalCallback = ConnectionApprovalCheck;
        _transport.SetConnectionData("127.0.0.1", 7777); //TODO: not hardcoded
        _networkManager.StartServer();
        await TaskHelper.When(() => IsServer && IsSpawned);
        Debug.Log("Server object spawned!");
        SetState(new GameplayServerServerStartedState(
                stateManager: this,
                gameplayServerContext: new GameplayServerContext(
                    gameplaySceneManager: GameplaySceneManager.Instance,
                    networkManager: _networkManager,
                    targetNumberOfPlayers: _targetNumberOfPlayers,
                    connectedPlayerDataDict: _connectedPlayerDataDict,
                    playerPrefab: _playerPrefab,
                    playerPositionTransforms: _playerPositionTransforms
                )
            )
        );
    }

    private void ConnectionApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var clientId = request.ClientNetworkId;
        if (_connectedPlayerDataDict.Count < _targetNumberOfPlayers)
        {
            _connectedPlayerDataDict.Add(clientId, new PlayerData(clientId, System.Text.Encoding.ASCII.GetString(request.Payload)));
            response.Approved = true;
        }
        else
        {
            response.Approved = false;
            response.Reason = "Reached maximum number of players";
        }
    }

}
