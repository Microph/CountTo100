using CountTo100.Utilities;
using static GameplayClientStateManager;

public class GameplayClientEndGameState : State
{
    private GameplayClientContext _gameplayClientContext;

    public GameplayClientEndGameState(IStateManageable stateManager, GameplayClientContext gameplayClientContext) 
        : base(
            stateEnum: Enums.State.GameplayClient_EndGame,
            stateManager: stateManager,
            availableStateTransitions: new StateTransition[]
            {
            }
        )
    {
        _gameplayClientContext = gameplayClientContext;
    }

    public override void OnEnter()
    {
        ulong winnerId = _gameplayClientContext.GameplaySceneManager.GameplayServerStateManager.NVLatestClickerId.Value;
        _gameplayClientContext.GameplaySceneManager.GameplayUIManager.SetWinnerText(_gameplayClientContext.GameplaySceneManager.GameplayServerStateManager.NVWinnerClickerName.Value.ToString(), winnerId);
        _gameplayClientContext.GameplaySceneManager.GameplayUIManager.ShowWinnerText();
    }
}