using CountTo100.Utilities;
using UnityEngine.SceneManagement;
using static GameplayClientStateManager;

public class GameplayClientEndGameState : State<GameplayClientContext>
{
    public GameplayClientEndGameState() 
        : base(
            stateEnum: Enums.State.GameplayClient_EndGame,
            availableStateTransitions: new StateTransition[]
            {
            },
            stateManager: null
        )
    {
    }

    public override void OnEnter()
    {
        ulong winnerId = _context.GameplaySceneManager.GameplayServerStateManager.NVLatestClickerId.Value;
        _context.GameplaySceneManager.GameplayUIManager.SetWinnerTextAndExitButtonAction(
            playerName: _context.GameplaySceneManager.GameplayServerStateManager.NVWinnerClickerName.Value.ToString(),
            clientId: winnerId,
            onExitButtonClickedAction: OnExitButtonClickedAction 
        );
        _context.GameplaySceneManager.GameplayUIManager.ShowWinnerTextAndExitButton();
    }

    private void OnExitButtonClickedAction()
    {
        _context.NetworkManager.Shutdown();
        SceneManager.LoadScene("ClientJoinLobbyScene");
    }
}