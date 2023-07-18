using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using CountTo100.Utilities;
using System.Threading.Tasks;

public class GameplayServerStateManager : NetworkStateManager
{
    public NetworkVariable<int> NVCurrentScore = new NetworkVariable<int>(k_defaultScore);
    
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Transform[] _playerPositionTransforms;

    private const int k_defaultScore = 0;

    private NetworkManager _networkManager;
    private UnityTransport _transport;
    private int _targetNumberOfPlayers;
    private Dictionary<ulong, PlayerData> _connectedPlayerDataDict = new Dictionary<ulong, PlayerData>();

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
        await TaskHelper.When(() => IsSpawned);
        Debug.Log("Server object spawned!");
        SetState(new GameplayServerStartServerState(
                stateManager: this,
                networkManager: _networkManager,
                targetNumberOfPlayers: _targetNumberOfPlayers,
                connectedPlayerDataDict: _connectedPlayerDataDict,
                playerPrefab: _playerPrefab,
                playerPositionTransforms: _playerPositionTransforms
            )
        );
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
