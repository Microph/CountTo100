using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using CountTo100.Utilities;
using System;

public class GameplayServerStateManager : NetworkStateManager
{
    public NetworkVariable<int> NVCurrentScore = new NetworkVariable<int>(k_defaultScore);
    
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Transform[] _playerPositionTransforms;

    private const int k_defaultScore = 0;

    private NetworkManager _networkManager;
    private UnityTransport _transport;
    private Dictionary<ulong, PlayerData> _connectedPlayerDataDict = new Dictionary<ulong, PlayerData>();

    public void InitializeAndStart()
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
        SetState(new GameplayServerStartServerState(
                networkManager: _networkManager,
                transport: _transport,
                connectedPlayerDataDict: _connectedPlayerDataDict,
                playerPrefab: _playerPrefab,
                playerPositionTransforms: _playerPositionTransforms
            )
        );
    }

    [ServerRpc]
    public void PlayerReadySignal(ulong clientId)
    {
        //TODO: BeginGameplayCountDownStateTransition if all clients are connected and signaled ready
        throw new NotImplementedException();
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
}
