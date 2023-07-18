using CountTo100.Utilities;

public class GameplayServerBeginGameplayCountDownState : State
{
    public GameplayServerBeginGameplayCountDownState(IStateManageable stateManager)
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

    public override void OnEnter()
    {
        //TODO
        //Show 3 2 1 countdown animation
        //then, transition to allow player input state
    }
}
