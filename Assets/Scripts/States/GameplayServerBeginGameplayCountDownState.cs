using CountTo100.Utilities;

public class GameplayServerBeginGameplayCountDownState : State
{
    public GameplayServerBeginGameplayCountDownState()
        : base(
            stateEnum: Enums.State.GameplayServer_BeginGameplayCountDown, 
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
