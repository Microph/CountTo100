using CountTo100.Utilities;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameplayServerStartServerState : State
{
    private NetworkManager _networkManager;
    private UnityTransport _transport;
    private Dictionary<ulong, PlayerData> _connectedPlayerDataDict;
    private Player _playerPrefab;
    private Transform[] _playerPositionTransforms;

    public GameplayServerStartServerState(
        NetworkManager networkManager, 
        UnityTransport transport, 
        Dictionary<ulong, PlayerData> connectedPlayerDataDict,
        Player playerPrefab,
        Transform[] playerPositionTransforms
    )
        : base(
            stateEnum: Enums.State.GameplayServer_StartServer,
            availableStateTransitions: new StateTransition[]
            {
                new BeginGameplayCountDownStateTransition()
            }
        )
    {
        _networkManager = networkManager;
        _transport = transport;
        _connectedPlayerDataDict = connectedPlayerDataDict;
        _playerPrefab = playerPrefab;
        _playerPositionTransforms = playerPositionTransforms;
    }

    public class BeginGameplayCountDownStateTransition : StateTransition
    {
        public BeginGameplayCountDownStateTransition()
            : base(Enums.State.GameplayServer_StartServer, Enums.State.GameplayServer_BeginGameplayCountDown)
        {
        }
    }

    public override void OnEnter()
    {
        _networkManager.ConnectionApprovalCallback = ConnectionApprovalCheck;
        _networkManager.OnClientConnectedCallback += OnClientConnected;
        _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        _transport.SetConnectionData("127.0.0.1", 7777); //TODO: not hardcoded
        _networkManager.StartServer();
    }

    private void ConnectionApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var clientId = request.ClientNetworkId;
        if (_connectedPlayerDataDict.Count < GlobalServerConfigManager.LocalServerAllocationPayload.numberOfPlayers)
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
        //TODO: BeginGameplayCountDownStateTransition if all clients are connected and signaled ready
    }

    private void OnClientDisconnected(ulong clientId)
    {
        _connectedPlayerDataDict.Remove(clientId);
        Debug.Log($"Disconnected client ID: {clientId}");
        Debug.Log($"Disconnect reason: {_networkManager.DisconnectReason}");
    }

    private void SpawnPlayer(ulong clientId, string playerName, Vector3 position)
    {
        var newPlayer = Object.Instantiate(original: _playerPrefab, position: position, rotation: Quaternion.identity);
        newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        newPlayer.Setup(clientId, playerName);
        newPlayer.SetupClientRpc(clientId, playerName);
    }
}
