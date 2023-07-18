using CountTo100.Utilities;
using Unity.Netcode;
using UnityEngine;

public abstract class NetworkStateManager : NetworkBehaviour
{
    public NetworkVariable<Enums.State> NVCurrentStateEnum = new NetworkVariable<Enums.State>(k_defaultState);
    public Enums.State CurrentStateEnum => _currentState.StateEnum;
    
    private const Enums.State k_defaultState = Enums.State.None;

    private State _currentState = null;

    public virtual void TransitTo(State newState)
    {
        if(!_currentState.AvailableStateTransitions.ContainsKey((CurrentStateEnum, newState.StateEnum)))
        {
            Debug.LogError($"Invalid state transition from {CurrentStateEnum} to {newState.StateEnum}");
            return;
        }

        _currentState.OnExit();
        _currentState.AvailableStateTransitions[(CurrentStateEnum, newState.StateEnum)].OnTransit();
        _currentState = newState;
        NVCurrentStateEnum.Value = CurrentStateEnum;
        _currentState.OnEnter();
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
