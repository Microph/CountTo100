using CountTo100.Utilities;
using static GameplayClientStateManager;

public class GameplayClientAllowCountingState : State
{
    public GameplayClientAllowCountingState(IStateManageable stateManager, GameplayClientContext gameplayClientContext) 
        : base(
            stateEnum: Enums.State.GameplayClient_AllowCounting,
            stateManager: stateManager,
            availableStateTransitions: new StateTransition[]
            {
                new EndGameStateTransition()
            }
        )
    {
    }

    public class EndGameStateTransition : StateTransition
    {
        public EndGameStateTransition()
            : base(Enums.State.GameplayClient_AllowCounting, Enums.State.GameplayClient_EndGame)
        {
        }
    }
}