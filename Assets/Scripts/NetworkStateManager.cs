using CountTo100.Utilities;
using Unity.Netcode;
using UnityEngine;

public abstract class NetworkStateManager : NetworkBehaviour //TODO: IStateManageable -> SetState & TransitTo
{
    public NetworkVariable<Enums.State> NVCurrentStateEnum = new NetworkVariable<Enums.State>(k_defaultState);
    public Enums.State CurrentStateEnum => _currentState.StateEnum;
    
    private const Enums.State k_defaultState = Enums.State.None;

    private State _currentState = null;

    public virtual void SetState(State state)
    {
        if (!IsSpawned)
        {
            Debug.LogError("Object is not spawned");
            return;
        }
        _currentState = state;
        NVCurrentStateEnum.Value = CurrentStateEnum;
        _currentState.OnEnter();
    }

    public virtual void TransitTo(State newState)
    {
        if (!IsSpawned)
        {
            Debug.LogError("Object is not spawned");
            return;
        }
        else if (!_currentState.AvailableStateTransitions.ContainsKey((CurrentStateEnum, newState.StateEnum)))
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
