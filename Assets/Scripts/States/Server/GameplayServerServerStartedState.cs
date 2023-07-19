using CountTo100.Utilities;
using Unity.Netcode;
using UnityEngine;
using static GameplayServerStateManager;

public class GameplayServerServerStartedState : State
{
    private GameplayServerContext _gameplayServerContext;

    public GameplayServerServerStartedState(
        IStateManageable stateManager,
        GameplayServerContext gameplayServerContext
    )
        : base(
            stateEnum: Enums.State.GameplayServer_ServerStarted,
            stateManager: stateManager,
            availableStateTransitions: new StateTransition[]
            {
                new BeginGameplayCountDownStateTransition()
            }
        )
    {
        _gameplayServerContext = gameplayServerContext;
    }

    public class BeginGameplayCountDownStateTransition : StateTransition
    {
        public BeginGameplayCountDownStateTransition()
            : base(Enums.State.GameplayServer_ServerStarted, Enums.State.GameplayServer_BeginGameplayCountDown)
        {
        }
    }

    public override void OnEnter()
    {
        _gameplayServerContext.NetworkManager.OnClientConnectedCallback += OnClientConnected;
        _gameplayServerContext.NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        SpawnPlayer(clientId, _gameplayServerContext.ConnectedPlayerDataDict[clientId].PlayerName, _gameplayServerContext.PlayerPositionTransforms[_gameplayServerContext.ConnectedPlayerDataDict.Count - 1].position);
        //TODO: change to check if all clients has sent ready signals
        if(_gameplayServerContext.ConnectedPlayerDataDict.Count == _gameplayServerContext.TargetNumberOfPlayers)
        {
            _stateManager.TransitTo(new GameplayServerBeginGameplayCountDownState(_stateManager, _gameplayServerContext));
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        _gameplayServerContext.ConnectedPlayerDataDict.Remove(clientId);
        Debug.Log($"Disconnected client ID: {clientId}");
        Debug.Log($"Disconnect reason: {_gameplayServerContext.NetworkManager.DisconnectReason}");
    }

    private void SpawnPlayer(ulong clientId, string playerName, Vector3 position)
    {
        var newPlayer = Object.Instantiate(original: _gameplayServerContext.PlayerPrefab, position: position, rotation: Quaternion.identity);
        newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        newPlayer.Setup(clientId, playerName);
        newPlayer.SetupClientRpc(clientId, playerName);
    }
}
