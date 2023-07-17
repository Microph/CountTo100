using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using CountTo100.Utilities;

public class GameplayServerStateManager : NetworkBehaviour
{
    public NetworkVariable<Enums.GameplayServerState> CurrentGameplayServerState = new NetworkVariable<Enums.GameplayServerState>(k_defaultGameplayServerState);
    public NetworkVariable<int> CurrentScore = new NetworkVariable<int>(k_defaultScore);
    
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Transform[] _playerPositionTransforms;

    private const Enums.GameplayServerState k_defaultGameplayServerState = Enums.GameplayServerState.Standby;
    private const int k_defaultScore = 0;

    private NetworkManager _networkManager;
    private UnityTransport _transport;
    private Dictionary<ulong, PlayerData> _connectedPlayerDataDict = new Dictionary<ulong, PlayerData>();

    public void InitializeAndStartServer()
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
        _networkManager.ConnectionApprovalCallback = ConnectionApprovalCheck;
        _networkManager.OnClientConnectedCallback += OnClientConnected;
        _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        _transport.SetConnectionData("127.0.0.1", 7777); //TODO: not hardcoded
        _networkManager.StartServer();
    }

    [ServerRpc]
    public void TestAddCurrentScoreServerRpc()
    {
        CurrentScore.Value ++;
    }

    [ServerRpc]
    public void OnPlayerCountServerRpc(ulong clientId)
    {
        //TODO also check 5 times/sec
        if (CurrentGameplayServerState.Value != Enums.GameplayServerState.AllowCounting)
        {
            return;
        }

        CurrentScore.Value ++;
    }

    private void ConnectionApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var clientId = request.ClientNetworkId;
        if(_connectedPlayerDataDict.Count < GlobalServerConfigManager.LocalServerAllocationPayload.numberOfPlayers)
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

    private void OnClientConnected(ulong clientId)
    {
        SpawnPlayer(clientId, _connectedPlayerDataDict[clientId].PlayerName, _playerPositionTransforms[_connectedPlayerDataDict.Count - 1].position);
    }

    private void SpawnPlayer(ulong clientId, string playerName, Vector3 position)
    {
        var newPlayer = Instantiate(original: _playerPrefab, position: position, rotation: Quaternion.identity);
        newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        newPlayer.Setup(clientId, playerName);
        newPlayer.SetupClientRpc(clientId, playerName);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        _connectedPlayerDataDict.Remove(clientId);
        Debug.Log($"Disconnected client ID: {clientId}");
        Debug.Log($"Disconnect reason: {_networkManager.DisconnectReason}");
    }
}
