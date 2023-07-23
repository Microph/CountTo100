using CountTo100.Utilities;
using UnityEngine;
using static GameplayClientStateManager;

public class GameplayClientBeginGameplayCountDownState : State
{
    private const float k_defaultCountDownTime = 3;

    private GameplayClientContext _gameplayClientContext;
    private float _currentCountDownTime = 0;

    public GameplayClientBeginGameplayCountDownState(
        IStateManageable stateManager,
        GameplayClientContext gameplayClientContext
    )
        : base(
            stateEnum: Enums.State.GameplayClient_BeginGameplayCountDown,
            stateManager: stateManager,
            availableStateTransitions: new StateTransition[]
            {
                new AllowCountingStateTransition()
            }
        )
    {
        _gameplayClientContext = gameplayClientContext;
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
        _gameplayClientContext.GameplaySceneManager.GameplayServerStateManager.NVCurrentStateEnum.OnValueChanged += OnGameplayServerStateChanged;
        _gameplayClientContext.GameplaySceneManager.GameplayUIManager.UpdateCountDownStartGameplayNumber(Mathf.CeilToInt(_currentCountDownTime));
        _gameplayClientContext.GameplaySceneManager.GameplayUIManager.ShowCountDownStartGameplayText();
    }

    public override void OnExit()
    {
        _gameplayClientContext.GameplaySceneManager.GameplayServerStateManager.NVCurrentStateEnum.OnValueChanged -= OnGameplayServerStateChanged;
    }

    public override void OnUpdate()
    {
        _currentCountDownTime -= Time.deltaTime;
        _gameplayClientContext.GameplaySceneManager.GameplayUIManager.UpdateCountDownStartGameplayNumber(Mathf.CeilToInt(_currentCountDownTime));
        if (_currentCountDownTime <= 0)
        {
            _currentCountDownTime = 0;
            _gameplayClientContext.GameplaySceneManager.GameplayUIManager.UpdateCountDownStartGameplayNumber(Mathf.CeilToInt(_currentCountDownTime));
            _stateManager.TransitTo(new GameplayClientAllowCountingState(_stateManager, _gameplayClientContext));
        }
    }

    private void OnGameplayServerStateChanged(Enums.State _, Enums.State newState)
    {
        //in case client countdown slower than server does -> skip right to counting state
        if(newState == Enums.State.GameplayServer_AllowCounting)
        {
            _stateManager.TransitTo(new GameplayClientAllowCountingState(_stateManager, _gameplayClientContext));
        }
    }
}
