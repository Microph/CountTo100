using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using CountTo100.Utilities;

public class GameplayServerStateManager : NetworkStateManager
{
    public NetworkVariable<int> NVCurrentScore = new NetworkVariable<int>(k_defaultScore);
    
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Transform[] _playerPositionTransforms;

    private const int k_defaultScore = 0;

    private NetworkManager _networkManager;
    private UnityTransport _transport;
    private Dictionary<ulong, PlayerData> _connectedPlayerDataDict = new Dictionary<ulong, PlayerData>();

    //States
    private readonly GameplayServerStartServerState _startServerState = new();
    private readonly GameplayServerBeginGameplayCountDownState _beginGameplayCountDownState = new();

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
        BeginFirstState(_startServerState);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TestAddCurrentScoreServerRpc()
    {
        NVCurrentScore.Value ++;
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnPlayerCountServerRpc(ulong clientId)
    {
        //TODO also check 5 times/sec
        if (CurrentStateEnum != Enums.State.GameplayServer_AllowCounting)
        {
            return;
        }

        NVCurrentScore.Value ++;
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
