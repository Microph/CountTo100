using CountTo100.Utilities;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameplayServerStartServerState : State
{
    private NetworkManager _networkManager;
    private int _targetNumberOfPlayers;
    private Dictionary<ulong, PlayerData> _connectedPlayerDataDict;
    private Player _playerPrefab;
    private Transform[] _playerPositionTransforms;

    public GameplayServerStartServerState(
        NetworkStateManager stateManager,
        NetworkManager networkManager,
        int targetNumberOfPlayers,
        Dictionary<ulong, PlayerData> connectedPlayerDataDict,
        Player playerPrefab,
        Transform[] playerPositionTransforms
    )
        : base(
            stateEnum: Enums.State.GameplayServer_StartServer,
            stateManager: stateManager,
            availableStateTransitions: new StateTransition[]
            {
                new BeginGameplayCountDownStateTransition()
            }
        )
    {
        _networkManager = networkManager;
        _targetNumberOfPlayers = targetNumberOfPlayers;
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
        _networkManager.OnClientConnectedCallback += OnClientConnected;
        _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        SpawnPlayer(clientId, _connectedPlayerDataDict[clientId].PlayerName, _playerPositionTransforms[_connectedPlayerDataDict.Count - 1].position);
        if(_connectedPlayerDataDict.Count == _targetNumberOfPlayers)
        {
            _stateManager.TransitTo(new GameplayServerBeginGameplayCountDownState(_stateManager));
        }
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
