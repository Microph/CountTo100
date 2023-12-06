using CountTo100.Utilities;
using UnityEngine;
using static GameplayClientStateManager;

public class GameplayClientBeginGameplayCountDownState : State<GameplayClientContext>
{
    private const float k_defaultCountDownTime = 3;

    private float _currentCountDownTime = 0;

    public GameplayClientBeginGameplayCountDownState()
        : base(
            stateEnum: Enums.State.GameplayClient_BeginGameplayCountDown,
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
            : base(Enums.State.GameplayClient_BeginGameplayCountDown, Enums.State.GameplayClient_AllowCounting)
        {
        }
    }

    public override void OnEnter()
    {
        _currentCountDownTime = k_defaultCountDownTime;
        _context.GameplaySceneManager.GameplayServerStateManager.NVCurrentStateEnum.OnValueChanged += OnGameplayServerStateChanged;
        _context.GameplaySceneManager.GameplayUIManager.UpdateCountDownStartGameplayNumber(Mathf.CeilToInt(_currentCountDownTime));
        _context.GameplaySceneManager.GameplayUIManager.ShowCountDownStartGameplayText();
    }

    public override void OnExit()
    {
        _context.GameplaySceneManager.GameplayServerStateManager.NVCurrentStateEnum.OnValueChanged -= OnGameplayServerStateChanged;
    }

    public override void OnUpdate()
    {
        _currentCountDownTime -= Time.deltaTime;
        _context.GameplaySceneManager.GameplayUIManager.UpdateCountDownStartGameplayNumber(Mathf.CeilToInt(_currentCountDownTime));
        if (_currentCountDownTime <= 0)
        {
            _currentCountDownTime = 0;
            _context.GameplaySceneManager.GameplayUIManager.UpdateCountDownStartGameplayNumber(Mathf.CeilToInt(_currentCountDownTime));
            _stateManager.TransitTo(_context.GameplayClientStates.GameplayClientAllowCountingState, _context);
        }
    }

    private void OnGameplayServerStateChanged(Enums.State _, Enums.State newState)
    {
        //in case client countdown slower than server does -> skip right to counting state
        if(newState == Enums.State.GameplayServer_AllowCounting)
        {
            _stateManager.TransitTo(_context.GameplayClientStates.GameplayClientAllowCountingState, _context);
        }
    }
}
