using CountTo100.Utilities;
using System.Collections.Generic;

public abstract class State
{
    public readonly Enums.State StateEnum;
    public readonly Dictionary<(Enums.State, Enums.State), StateTransition> AvailableStateTransitions;

    protected State(Enums.State stateEnum, IEnumerable<StateTransition> availableStateTransitions)
    {
        StateEnum = stateEnum;
        AvailableStateTransitions = new Dictionary<(Enums.State, Enums.State), StateTransition> ();
        foreach (var transition in availableStateTransitions)
        {
            AvailableStateTransitions.Add((transition.FromState, transition.ToState), transition);
        }
    }

    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnFixedUpdate()
    {
    }
}