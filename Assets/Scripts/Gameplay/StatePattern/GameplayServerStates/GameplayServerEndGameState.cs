using CountTo100.Utilities;
using static GameplayServerStateManager;

public class GameplayServerEndGameState : State<GameplayServerContext>
{
    public GameplayServerEndGameState() 
        : base(
            stateEnum: Enums.State.GameplayServer_EndGame,
            availableStateTransitions: new StateTransition[]
            {
            }
        )
    {
    }

    public override void OnEnter()
    {
        string serverAddress = string.IsNullOrEmpty(_context.Transport.ConnectionData.Address) ? "localhost" : _context.Transport.ConnectionData.Address;
        _context.GameplaySceneManager.GameplayUIManager.ShowServerInfo($"Number of players: {GlobalServerConfig.LocalServerAllocationPayload.numberOfPlayers}\nBinding IP: {serverAddress}\nPort: {_context.Transport.ConnectionData.Port}\nGame is over. Please shutdown this server.");
    }
}