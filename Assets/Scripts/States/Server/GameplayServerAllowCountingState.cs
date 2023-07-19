using CountTo100.Utilities;
using static GameplayServerStateManager;

public class GameplayServerAllowCountingState : State
{
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
    }

    public class EndGameStateTransition : StateTransition
    {
        public EndGameStateTransition()
            : base(Enums.State.GameplayServer_AllowCounting, Enums.State.GameplayServer_EndGame)
        {
        }
    }
}