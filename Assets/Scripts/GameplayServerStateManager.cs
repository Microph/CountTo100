using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class GameplayServerStateManager : MonoBehaviour
{
    private NetworkManager _networkManager;
    private UnityTransport _transport;
    private HashSet<ulong> _connectedClientIds = new HashSet<ulong>();

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
        _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        _transport.SetConnectionData("127.0.0.1", 7777); //TODO: not hardcoded
        _networkManager.StartServer();
    }

    private void ConnectionApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var clientId = request.ClientNetworkId;
        //TODO var connectionData = request.Payload;
        if(_connectedClientIds.Count < GlobalServerConfigManager.LocalServerAllocationPayload.numberOfPlayers)
        {
            response.Approved = true;
            response.CreatePlayerObject = true;
            response.PlayerPrefabHash = null;
            response.Position = Vector3.zero;
            response.Rotation = Quaternion.identity;
            _connectedClientIds.Add(clientId);
        }
        else
        {
            response.Approved = false;
            response.Reason = "Reached maximum number of players";
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        _connectedClientIds.Remove(clientId);
        Debug.Log($"Disconnected client ID: {clientId}");
        Debug.Log($"Disconnect reason: {_networkManager.DisconnectReason}");
    }
}
