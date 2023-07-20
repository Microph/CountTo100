using CountTo100.Utilities;
using UnityEngine.SceneManagement;
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
        _gameplayClientContext.GameplaySceneManager.GameplayUIManager.SetWinnerTextAndExitButtonAction(
            playerName: _gameplayClientContext.GameplaySceneManager.GameplayServerStateManager.NVWinnerClickerName.Value.ToString(),
            clientId: winnerId,
            onExitButtonClickedAction: OnExitButtonClickedAction 
        );
        _gameplayClientContext.GameplaySceneManager.GameplayUIManager.ShowWinnerTextAndExitButton();
    }

    private void OnExitButtonClickedAction()
    {
        //TODO: back to the same lobby instead
        _gameplayClientContext.NetworkManager.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }
}