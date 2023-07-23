using CountTo100.Utilities;
using UnityEngine;
using static GameplayServerStateManager;

public class GameplayServerBeginGameplayCountDownState : State
{
    private const float k_defaultCountDownTime = 3;

    private GameplayServerContext _gameplayServerContext;
    private float _currentCountDownTime = 0;

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
        _currentCountDownTime = k_defaultCountDownTime;
    }

    public override void OnUpdate()
    {
        _currentCountDownTime -= Time.deltaTime;
        if(_currentCountDownTime <= 0 ) 
        {
            _currentCountDownTime = 0;
            _stateManager.TransitTo(new GameplayServerAllowCountingState(_stateManager, _gameplayServerContext));
        }
    }
}
