using Unity.Netcode;
using UnityEngine;

namespace CountTo100.Utilities
{
    public abstract class NetworkStateManager<T> : NetworkBehaviour, IStateManageable<T>
    {
        public NetworkVariable<Enums.State> NVCurrentStateEnum = new NetworkVariable<Enums.State>(k_defaultState);
        public Enums.State CurrentStateEnum => _currentState.StateEnum;

        private const Enums.State k_defaultState = Enums.State.None;

        private State<T> _currentState = null;

        public virtual void SetState(State<T> state, T context)
        {
            if (!IsSpawned)
            {
                Debug.LogError("Object is not spawned");
                return;
            }
            _currentState = state;
            _currentState.Initialize(this, context);
            NVCurrentStateEnum.Value = CurrentStateEnum;
            _currentState.OnEnter();
        }

        public virtual void TransitTo(State<T> newState, T context)
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
            SetState(newState, context);
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
}