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
        _gameplayServerContext.GameplaySceneManager.GameplayServerStateManager.OnPlayerReadySignal += OnPlayerReadySignal;
    }

    public override void OnExit()
    {
        _gameplayServerContext.NetworkManager.OnClientConnectedCallback -= OnClientConnected;
        _gameplayServerContext.NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        _gameplayServerContext.GameplaySceneManager.GameplayServerStateManager.OnPlayerReadySignal -= OnPlayerReadySignal;
    }

    private void OnClientConnected(ulong clientId)
    {
        SpawnPlayer(clientId, _gameplayServerContext.ConnectedPlayerDataDict[clientId].PlayerName, _gameplayServerContext.PlayerPositionTransforms[_gameplayServerContext.ConnectedPlayerDataDict.Count - 1].position);
        //TODO assign Player ref to client
    }

    private void OnClientDisconnected(ulong clientId)
    {
        _gameplayServerContext.ConnectedPlayerDataDict.Remove(clientId);
    }

    private void SpawnPlayer(ulong clientId, string playerName, Vector3 position)
    {
        var newPlayer = Object.Instantiate(original: _gameplayServerContext.PlayerPrefab, position: position, rotation: Quaternion.identity);
        newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        newPlayer.Setup(_gameplayServerContext.GameplaySceneManager.GameplayServerStateManager, clientId, playerName);
    }

    private void OnPlayerReadySignal(int readyPlayers)
    {
        Debug.Log($"readyPlayers: {readyPlayers}");
        if (readyPlayers == _gameplayServerContext.TargetNumberOfPlayers)
        {
            _stateManager.TransitTo(new GameplayServerBeginGameplayCountDownState(_stateManager, _gameplayServerContext));
        }
    }
}
