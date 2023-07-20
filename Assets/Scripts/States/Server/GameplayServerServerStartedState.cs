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
        _gameplayServerContext.GameplaySceneManager.GameplayServerStateManager.OnPlayerReady += OnPlayerReady;
    }

    public override void OnExit()
    {
        _gameplayServerContext.NetworkManager.OnClientConnectedCallback -= OnClientConnected;
        _gameplayServerContext.NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        _gameplayServerContext.GameplaySceneManager.GameplayServerStateManager.OnPlayerReady -= OnPlayerReady;
    }

    private void OnClientConnected(ulong clientId)
    {
        SpawnPlayer(
            gameplayServerStateManager: _gameplayServerContext.GameplaySceneManager.GameplayServerStateManager,
            clientId: clientId,
            playerName: _gameplayServerContext.ConnectedPlayerDataDict[clientId].PlayerName,
            playerColor: _gameplayServerContext.ConnectedPlayerDataDict[clientId].PlayerColor,
            position: _gameplayServerContext.PlayerPositionTransforms[_gameplayServerContext.ConnectedPlayerDataDict.Count - 1].position
        );
    }

    private void OnClientDisconnected(ulong clientId)
    {
        _gameplayServerContext.ConnectedPlayerDataDict.Remove(clientId);
    }

    private void SpawnPlayer(
        GameplayServerStateManager gameplayServerStateManager, 
        ulong clientId, 
        string playerName, 
        Color playerColor, 
        Vector3 position
    )
    {
        PlayerObject newPlayer = Object.Instantiate(original: _gameplayServerContext.PlayerPrefab, position: position, rotation: Quaternion.identity);
        newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        newPlayer.Setup(
            gameplayServerStateManager: gameplayServerStateManager, 
            clientId: clientId,
            playerName: playerName,
            playerColor: playerColor
        );
        _gameplayServerContext.ConnectedPlayerDataDict[clientId].PlayerObject = newPlayer;
    }

    private void OnPlayerReady(int readyPlayers)
    {
        Debug.Log($"readyPlayers: {readyPlayers}");
        if (readyPlayers == _gameplayServerContext.TargetNumberOfPlayers)
        {
            _stateManager.TransitTo(new GameplayServerBeginGameplayCountDownState(_stateManager, _gameplayServerContext));
        }
    }
}
