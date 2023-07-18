using CountTo100.Utilities;
using static GameplayServerStateManager;

public class GameplayServerBeginGameplayCountDownState : State
{
    private GameplayServerContext _gameplayServerContext;

    public GameplayServerBeginGameplayCountDownState(IStateManageable stateManager, GameplayServerContext gameplayServerContext)
        : base(
            stateEnum: Enums.State.GameplayServer_BeginGameplayCountDown,
            stateManager: stateManager,
            availableStateTransitions: new StateTransition[]
            {
                new AllowCountingStateTransition()
            }
        )
    {
        _gameplayServerContext = gameplayServerContext;
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
