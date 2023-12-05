using CountTo100.Utilities;
using static GameplayClientStateManager;

public class GameplayClientClientStartedState : State<GameplayClientContext>
{
    public GameplayClientClientStartedState()
        : base(
            stateEnum: Enums.State.GameplayClient_ClientStarted,
            availableStateTransitions: new StateTransition[]
            {
                new BeginGameplayCountDownStateTransition()
            },
            stateManager: null
        )
    {
    }

    public class BeginGameplayCountDownStateTransition : StateTransition
    {
        public BeginGameplayCountDownStateTransition()
            : base(Enums.State.GameplayClient_ClientStarted, Enums.State.GameplayClient_BeginGameplayCountDown)
        {
        }
    }

    public override void OnEnter()
    {
        _context.GameplaySceneManager.GameplayServerStateManager.NVCurrentStateEnum.OnValueChanged += OnGameplayServerStateChanged;
        _context.GameplaySceneManager.GameplayUIManager.ShowWaitingForPlayerText();
        _context.PlayerObject.PlayerReadyServerRpc();
    }

    public override void OnExit()
    {
        _context.GameplaySceneManager.GameplayServerStateManager.NVCurrentStateEnum.OnValueChanged -= OnGameplayServerStateChanged;
    }

    private void OnGameplayServerStateChanged(Enums.State previousValue, Enums.State newValue)
    {
        if(newValue == Enums.State.GameplayServer_BeginGameplayCountDown)
        {
            _stateManager.TransitTo(_context.GameplayClientStates.GameplayClientBeginGameplayCountDownState, _context);
        }
    }
}
