using CountTo100.Utilities;
using UnityEngine;
using static GameplayClientStateManager;

public class GameplayClientAllowCountingState : State<GameplayClientContext>
{
    private float _cumulativeClicksResetTimer;
    private int _cumulativeClicks;

    public GameplayClientAllowCountingState() 
        : base(
            stateEnum: Enums.State.GameplayClient_AllowCounting,
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
            : base(Enums.State.GameplayClient_AllowCounting, Enums.State.GameplayClient_EndGame)
        {
        }
    }

    public override void OnEnter()
    {
        _context.GameplaySceneManager.GameplayUIManager.UpdateGameplayScoreText(0);
        _context.GameplaySceneManager.GameplayUIManager.ShowCurrentGameplayScoreText();
        _context.GameplaySceneManager.GameplayServerStateManager.NVCurrentScore.OnValueChanged += OnCurrentScoreValueChanged;
        _context.GameplaySceneManager.GameplayServerStateManager.NVCurrentStateEnum.OnValueChanged += OnGameplayServerStateChanged;
        _context.GameplaySceneManager.InputManager.PlayerClickAction = PlayerClickAction;
        _cumulativeClicksResetTimer = 0;
        _cumulativeClicks = 0;
    }

    public override void OnExit()
    {
        _context.GameplaySceneManager.GameplayServerStateManager.NVCurrentScore.OnValueChanged -= OnCurrentScoreValueChanged;
        _context.GameplaySceneManager.GameplayServerStateManager.NVCurrentStateEnum.OnValueChanged -= OnGameplayServerStateChanged;
        _context.GameplaySceneManager.InputManager.PlayerClickAction = null;
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
        _context.GameplaySceneManager.GameplayUIManager.UpdateGameplayScoreText(newValue);
    }

    private void OnGameplayServerStateChanged(Enums.State _, Enums.State newState)
    {
        if(newState == Enums.State.GameplayServer_EndGame)
        {
            _stateManager.TransitTo(_context.GameplayClientStates.GameplayClientEndGameState, _context);
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
        _context.PlayerObject.PlayerCountServerRpc();
    }
}