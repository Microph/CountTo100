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
}