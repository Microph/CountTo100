public class GameplayServerStates
{
    public readonly GameplayServerAllowCountingState GameplayServerAllowCountingState;
    public readonly GameplayServerBeginGameplayCountDownState GameplayServerBeginGameplayCountDownState;
    public readonly GameplayServerEndGameState GameplayServerEndGameState;
    public readonly GameplayServerServerStartedState GameplayServerServerStartedState;

    public GameplayServerStates()
    {
        GameplayServerAllowCountingState = new();
        GameplayServerBeginGameplayCountDownState = new();
        GameplayServerEndGameState = new();
        GameplayServerServerStartedState = new();
    }
}