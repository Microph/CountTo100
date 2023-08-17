using CountTo100.Utilities;
using UnityEngine;
using static GameplayClientStateManager;

public class GameplayClientAllowCountingState : State
{
    private GameplayClientContext _gameplayClientContext;
    private float _cumulativeClicksResetTimer;
    private int _cumulativeClicks;

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
        _gameplayClientContext = gameplayClientContext;
    }

    public class EndGameStateTransition : StateTransition
    {
        public EndGameStateTransition()
            : base(Enums.State.GameplayClient_AllowCounting, Enums.State.GameplayClient_EndGame)
        {
        }
    }

    public override void OnEnter()
    {
        _gameplayClientContext.GameplaySceneManager.GameplayUIManager.UpdateGameplayScoreText(0);
        _gameplayClientContext.GameplaySceneManager.GameplayUIManager.ShowCurrentGameplayScoreText();
        _gameplayClientContext.GameplaySceneManager.GameplayServerStateManager.NVCurrentScore.OnValueChanged += OnCurrentScoreValueChanged;
        _gameplayClientContext.GameplaySceneManager.GameplayServerStateManager.NVCurrentStateEnum.OnValueChanged += OnGameplayServerStateChanged;
        _gameplayClientContext.GameplaySceneManager.InputManager.PlayerClickAction = PlayerClickAction;
        _cumulativeClicksResetTimer = 0;
        _cumulativeClicks = 0;
    }

    public override void OnExit()
    {
        _gameplayClientContext.GameplaySceneManager.GameplayServerStateManager.NVCurrentScore.OnValueChanged -= OnCurrentScoreValueChanged;
        _gameplayClientContext.GameplaySceneManager.GameplayServerStateManager.NVCurrentStateEnum.OnValueChanged -= OnGameplayServerStateChanged;
        _gameplayClientContext.GameplaySceneManager.InputManager.PlayerClickAction = null;
    }

    public override void OnUpdate()
    {
        _cumulativeClicksResetTimer += Time.deltaTime;
        if (_cumulativeClicksResetTimer >= 1)
        {
            _cumulativeClicksResetTimer -= 1;
            _cumulativeClicks = 0;
        }
    }

    private void OnCurrentScoreValueChanged(int _, int newValue)
    {
        _gameplayClientContext.GameplaySceneManager.GameplayUIManager.UpdateGameplayScoreText(newValue);
    }

    private void OnGameplayServerStateChanged(Enums.State _, Enums.State newState)
    {
        if(newState == Enums.State.GameplayServer_EndGame)
        {
            _stateManager.TransitTo(new GameplayClientEndGameState(_stateManager, _gameplayClientContext));
        }
    }

    private void PlayerClickAction()
    {
        if(_cumulativeClicks >= 5)
        {
            Debug.Log($"reached 5 clicks/sec limit");
            return;
        }

        _cumulativeClicks++;
        _gameplayClientContext.PlayerObject.PlayerCountServerRpc();
    }
}