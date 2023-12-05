using UnityEngine;

namespace CountTo100.Utilities
{
    public abstract class StateManager<T> : MonoBehaviour, IStateManageable<T>
    {
        public event Delegates.ValueChangedAction<Enums.State> OnGameplayClientStateChanged;

        public Enums.State CurrentStateEnum => _currentState.StateEnum;

        private const Enums.State k_defaultState = Enums.State.None;

        private State<T> _currentState = null;

        public virtual void SetState(State<T> state, T context)
        {
            Enums.State prevStateEnum = _currentState == null ? Enums.State.None : _currentState.StateEnum;
            _currentState = state;
            _currentState.Initialize(this, context);
            OnGameplayClientStateChanged?.Invoke(prevStateEnum, _currentState.StateEnum);
            _currentState.OnEnter();
        }

        public virtual void TransitTo(State<T> newState, T context)
        {
            if (!_currentState.AvailableStateTransitions.ContainsKey((CurrentStateEnum, newState.StateEnum)))
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