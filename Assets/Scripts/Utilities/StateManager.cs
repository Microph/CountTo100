using CountTo100.Utilities;
using UnityEngine;

public abstract class StateManager : MonoBehaviour, IStateManageable
{
    public event Delegates.ValueChangedAction<Enums.State> OnGameplayClientStateChanged;
    
    public Enums.State CurrentStateEnum => _currentState.StateEnum;
    
    private const Enums.State k_defaultState = Enums.State.None;

    private State _currentState = null;

    public virtual void SetState(State state)
    {
        Enums.State prevStateEnum = _currentState == null ? Enums.State.None : _currentState.StateEnum;
        _currentState = state;
        OnGameplayClientStateChanged?.Invoke(prevStateEnum, _currentState.StateEnum);
        _currentState.OnEnter();
    }

    public virtual void TransitTo(State newState)
    {
        if (!_currentState.AvailableStateTransitions.ContainsKey((CurrentStateEnum, newState.StateEnum)))
        {
            Debug.LogError($"Invalid state transition from {CurrentStateEnum} to {newState.StateEnum}");
            return;
        }

        _currentState.OnExit();
        _currentState.AvailableStateTransitions[(CurrentStateEnum, newState.StateEnum)].OnTransit();
        SetState(newState);
    }

    protected virtual void Update()
    {
        _currentState?.OnUpdate();
    }

    protected virtual void FixedUpdate()
    {
        _currentState?.OnFixedUpdate();
    }
}
