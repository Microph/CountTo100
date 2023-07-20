using CountTo100.Utilities;

public abstract class StateTransition
{
    public readonly Enums.State FromState, ToState;

    protected StateTransition(Enums.State fromState, Enums.State toState)
    {
        FromState = fromState;
        ToState = toState;
    }

    public virtual void OnTransit()
    {
    }
}