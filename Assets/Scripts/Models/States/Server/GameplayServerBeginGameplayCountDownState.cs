using CountTo100.Utilities;
using UnityEngine;
using static GameplayServerStateManager;

public class GameplayServerBeginGameplayCountDownState : State<GameplayServerContext>
{
    private const float k_defaultCountDownTime = 3;

    private float _currentCountDownTime = 0;

    public GameplayServerBeginGameplayCountDownState()
        : base(
            stateEnum: Enums.State.GameplayServer_BeginGameplayCountDown,
            availableStateTransitions: new StateTransition[]
            {
                new AllowCountingStateTransition()
            },
            stateManager: null
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
        _currentCountDownTime = k_defaultCountDownTime;
    }

    public override void OnUpdate()
    {
        _currentCountDownTime -= Time.deltaTime;
        if(_currentCountDownTime <= 0 ) 
        {
            _currentCountDownTime = 0;
            _stateManager.TransitTo(_context.GameplayServerStates.GameplayServerAllowCountingState, _context);
        }
    }
}
