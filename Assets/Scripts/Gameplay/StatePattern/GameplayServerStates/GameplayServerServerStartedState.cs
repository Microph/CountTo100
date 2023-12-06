using CountTo100.Utilities;
using Unity.Netcode;
using UnityEngine;
using static GameplayServerStateManager;

public class GameplayServerServerStartedState : State<GameplayServerContext>
{
    public GameplayServerServerStartedState()
        : base(
            stateEnum: Enums.State.GameplayServer_ServerStarted,
            availableStateTransitions: new StateTransition[]
            {
                new BeginGameplayCountDownStateTransition()
            }
        )
    {
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
        _context.NetworkManager.OnClientConnectedCallback += OnClientConnected;
        _context.NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;
        _context.GameplaySceneManager.GameplayServerStateManager.OnPlayerReady += OnPlayerReady;
        string serverAddress = string.IsNullOrEmpty(_context.Transport.ConnectionData.Address) ? "localhost" : _context.Transport.ConnectionData.Address;
        _context.GameplaySceneManager.GameplayUIManager.ShowServerInfo($"Number of players: {GlobalServerConfig.LocalServerAllocationPayload.numberOfPlayers}\nBinding IP: {serverAddress}\nPort: {_context.Transport.ConnectionData.Port}");
    }

    public override void OnExit()
    {
        _context.NetworkManager.OnClientConnectedCallback -= OnClientConnected;
        _context.NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        _context.GameplaySceneManager.GameplayServerStateManager.OnPlayerReady -= OnPlayerReady;
    }

    private void OnClientConnected(ulong clientId)
    {
        SpawnPlayer(
            gameplayServerStateManager: _context.GameplaySceneManager.GameplayServerStateManager,
            clientId: clientId,
            playerName: _context.ConnectedPlayerDataDict[clientId].PlayerName,
            playerColor: _context.ConnectedPlayerDataDict[clientId].PlayerColor,
            position: _context.PlayerPositionTransforms[_context.ConnectedPlayerDataDict.Count - 1].position
        );
    }

    private void OnClientDisconnected(ulong clientId)
    {
        _context.ConnectedPlayerDataDict.Remove(clientId);
    }

    private void SpawnPlayer(
        GameplayServerStateManager gameplayServerStateManager, 
        ulong clientId, 
        string playerName, 
        Color playerColor, 
        Vector3 position
    )
    {
        PlayerObject newPlayer = Object.Instantiate(original: _context.PlayerPrefab, position: position, rotation: Quaternion.identity);
        newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        newPlayer.Setup(
            gameplayServerStateManager: gameplayServerStateManager, 
            clientId: clientId,
            playerName: playerName,
            playerColor: playerColor
        );
        _context.ConnectedPlayerDataDict[clientId].PlayerObject = newPlayer;
    }

    private void OnPlayerReady(int readyPlayers)
    {
        Debug.Log($"readyPlayers: {readyPlayers}");
        if (readyPlayers == _context.TargetNumberOfPlayers)
        {
            _stateManager.TransitTo(_context.GameplayServerStates.GameplayServerBeginGameplayCountDownState, _context);
        }
    }
}
