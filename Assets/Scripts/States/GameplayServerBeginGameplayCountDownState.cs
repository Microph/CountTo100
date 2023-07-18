using CountTo100.Utilities;

public class GameplayServerBeginGameplayCountDownState : State
{
    public GameplayServerBeginGameplayCountDownState(NetworkStateManager stateManager)
        : base(
            stateEnum: Enums.State.GameplayServer_BeginGameplayCountDown,
            stateManager: stateManager,
            availableStateTransitions: new StateTransition[]
            {
                new AllowCountingStateTransition()
            }
        )
    {
    }

    public class AllowCountingStateTransition : StateTransition
    {
        public AllowCountingStateTransition() 
            : base(Enums.State.GameplayServer_BeginGameplayCountDown, Enums.State.GameplayServer_AllowCounting)
        {
        }
    }
}
