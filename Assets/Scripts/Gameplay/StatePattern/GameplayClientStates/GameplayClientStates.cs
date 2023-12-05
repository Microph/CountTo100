public class GameplayClientStates
{
    public readonly GameplayClientAllowCountingState GameplayClientAllowCountingState;
    public readonly GameplayClientBeginGameplayCountDownState GameplayClientBeginGameplayCountDownState;
    public readonly GameplayClientClientStartedState GameplayClientClientStartedState;
    public readonly GameplayClientEndGameState GameplayClientEndGameState;

    public GameplayClientStates()
    {
        GameplayClientAllowCountingState = new();
        GameplayClientBeginGameplayCountDownState = new();
        GameplayClientClientStartedState = new();
        GameplayClientEndGameState = new();
    }
}