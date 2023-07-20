using CountTo100.Utilities;
using UnityEngine;
using static GameplayServerStateManager;

public class GameplayServerAllowCountingState : State
{
    private GameplayServerContext _gameplayServerContext;

    public GameplayServerAllowCountingState(IStateManageable stateManager, GameplayServerContext gameplayServerContext) 
        : base(
            stateEnum: Enums.State.GameplayServer_AllowCounting,
            stateManager: stateManager,
            availableStateTransitions: new StateTransition[]
            {
                new EndGameStateTransition()
            }
        )
    {
        _gameplayServerContext = gameplayServerContext;
    }

    public class EndGameStateTransition : StateTransition
    {
        public EndGameStateTransition()
            : base(Enums.State.GameplayServer_AllowCounting, Enums.State.GameplayServer_EndGame)
        {
        }
    }

    public override void OnEnter()
    {
        _gameplayServerContext.GameplaySceneManager.GameplayServerStateManager.OnPlayerCount += OnPlayerCount;
    }

    public override void OnExit()
    {
        _gameplayServerContext.GameplaySceneManager.GameplayServerStateManager.OnPlayerCount -= OnPlayerCount;
    }

    public override void OnUpdate()
    {
        foreach((_, PlayerData playerData) in _gameplayServerContext.ConnectedPlayerDataDict)
        {
            playerData.CumulativeClicksResetTimer += Time.deltaTime;
            if(playerData.CumulativeClicksResetTimer >= 1)
            {
                playerData.CumulativeClicksResetTimer = 0;
                playerData.CumulativeClicks = 0;
            }
        }
    }

    private void OnPlayerCount(ulong clientId)
    {
        if (_gameplayServerContext.ConnectedPlayerDataDict[clientId].CumulativeClicks > 5) 
        {
            Debug.Log($"clientId {clientId} reached 5 clicks/sec limit");
            return;
        }

        _gameplayServerContext.ConnectedPlayerDataDict[clientId].CumulativeClicks++;
        _gameplayServerContext.GameplaySceneManager.GameplayServerStateManager.NVCurrentScore.Value++;
        _gameplayServerContext.GameplaySceneManager.GameplayServerStateManager.NVLatestClickerId.Value = clientId;
        //TODO: spawn +1 particle and enlarge player object
        if (_gameplayServerContext.GameplaySceneManager.GameplayServerStateManager.NVCurrentScore.Value == 100)
        {
            _gameplayServerContext.GameplaySceneManager.GameplayServerStateManager.NVWinnerClickerName.Value = _gameplayServerContext.GameplaySceneManager.GameplayServerStateManager.GetPlayerName(clientId);
            _stateManager.TransitTo(new GameplayServerEndGameState(_stateManager, _gameplayServerContext));
        }
    }
}