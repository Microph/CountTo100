using CountTo100.Utilities;
using static GameplayServerStateManager;

public class GameplayServerEndGameState : State
{
    private GameplayServerContext _gameplayServerContext;

    public GameplayServerEndGameState(IStateManageable stateManager, GameplayServerContext gameplayServerContext) 
        : base(
            stateEnum: Enums.State.GameplayServer_EndGame,
            stateManager: stateManager,
            availableStateTransitions: new StateTransition[]
            {
            }
        )
    {
        _gameplayServerContext = gameplayServerContext;
    }

    public override void OnEnter()
    {
        _gameplayServerContext.GameplaySceneManager.GameplayUIManager.ShowServerInfo($"Number of players: {GlobalServerConfigManager.LocalServerAllocationPayload.numberOfPlayers}\nServer port: {_gameplayServerContext.Transport.ConnectionData.Port}\nGame is over. Please shutdown this server.");
    }
}